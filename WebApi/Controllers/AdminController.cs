using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Admin;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : MyBaseController<AdminController>
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICurrentUserService currentUserService, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    // POST api/<Admin>
    public class UserEmailStateRequest
    {
        public string UserName { get; set; }
        public bool IsActive { get; set; }
    }

    [HttpPost("user-email-state")]
    [MustHavePermission(AppFeature.UserRoles, AppAction.Update)]
    public async Task<IResponseWrapper> UserEmailActive([FromBody] UserEmailStateRequest request)
    {
        var currentLoggedInUser = await _userManager.FindByNameAsync(request.UserName);
        if (currentLoggedInUser is null)
            return await ResponseWrapper.FailAsync("[ML102] User does not exist");

        currentLoggedInUser.EmailConfirmed = request.IsActive;

        var identityResult = await _userManager.UpdateAsync(currentLoggedInUser);
        if (identityResult.Succeeded)
            return await ResponseWrapper<string>
                .SuccessAsync("[ML103] User email state successfully updated.");
        return await ResponseWrapper
            .FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    [HttpPut("user-update")]
    [MustHavePermission(AppFeature.UserRoles, AppAction.Update)]
    public async Task<IResponseWrapper> UpdateUserDetails([FromBody] AdminUpdateUserRequest userRequest)
    {
        var userInDb = await _userManager.FindByIdAsync(userRequest.UserId);
        if (userInDb is null)
            return await ResponseWrapper.FailAsync("[ML106] User does not exist.");

        if (userInDb.UserName == "yunus" || userRequest.Email == "yunus@mail.com")
            return await ResponseWrapper.FailAsync("[ML107] Not permitted.");

        userInDb.FirstName = userRequest.FirstName;
        userInDb.LastName = userRequest.LastName;
        userInDb.IsActive = userRequest.IsActive;

        if (userRequest.IsEmailChange)
        {
            var userWithEmailInDb = await _userManager.FindByEmailAsync(userRequest.Email);
            if (userWithEmailInDb is not null) await ResponseWrapper.FailAsync("[ML108] Email already taken.");
            userInDb.Email = userRequest.Email;
            userInDb.EmailConfirmed = userRequest.EmailConfirmed ?? true;
        }
        if (userRequest.IsUserNameChange)
        {
            var userNameCheck = await _userManager.FindByNameAsync(userRequest.UserName);
            if (userNameCheck is not null)
                return await ResponseWrapper.FailAsync("[ML109] Username is already taken.");
            userInDb.UserName = userRequest.UserName;
        }

        if (userRequest.IsPasswordChange)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(userInDb);
            var result = await _userManager.ResetPasswordAsync(userInDb, token, userRequest.Password);
            if (!result.Succeeded)
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(result));
        }


        var identityResult = await _userManager.UpdateAsync(userInDb);
        if (identityResult.Succeeded)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (userRequest.IsUserNameChange)
                {
                    var currentUserName = userInDb.UserName; // Mevcut kullanıcı adı
                    var newUserName = userRequest.UserName; // Yeni kullanıcı adı

                    // Salt SQL sorgusu
                    var sql = "UPDATE PRODUCT.Products SET UserName = {0} WHERE UserName = {1}";

                    // SQL'i çalıştır
                    await _context.Database.ExecuteSqlRawAsync(sql, newUserName, currentUserName);
                    await transaction.CommitAsync();
                }
                return await ResponseWrapper<string>.SuccessAsync("[ML110] User details successfully updated.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await ResponseWrapper.FailAsync(ex.Message);
            }

        }
        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }
    private List<string> GetIdentityResultErrorDescriptions(IdentityResult identityResult)
    {
        var errorDescriptions = new List<string>();
        foreach (var error in identityResult.Errors)
        {
            errorDescriptions.Add(error.Description);
        }
        return errorDescriptions;
    }
}