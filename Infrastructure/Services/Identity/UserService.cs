using Application.Extensions;
using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Pagination;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public UserService(UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IMapper mapper,
        ICurrentUserService currentUserService,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _context = context;
    }

    public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmedNewPassword)
            return ResponseWrapper.Fail("[ML51] New passwords must be equal.");

        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb == null) return await ResponseWrapper.FailAsync("[ML52] User does not exist.");

        var identityResult = await _userManager
            .ChangePasswordAsync(userInDb, request.CurrentPassword, request.NewPassword);
        if (!identityResult.Succeeded)
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        return await ResponseWrapper<string>.SuccessAsync("[ML53] User password changed.");
    }
    public async Task<IResponseWrapper> ChangeUserEmailAsync(ChangeEmailRequest request)
    {
        var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
        if (currentLoggedInUser is null)
            return await ResponseWrapper.FailAsync("[ML99] User does not exist.");

        var userInDb = await _userManager.FindByEmailAsync(request.Email);
        if (userInDb is not null)
            return await ResponseWrapper.FailAsync("[ML100] Email is already taken.");

        currentLoggedInUser.Email = request.Email;
        currentLoggedInUser.EmailConfirmed = false;

        var identityResult = await _userManager.UpdateAsync(currentLoggedInUser);
        if (identityResult.Succeeded)
            return await ResponseWrapper<string>
                .SuccessAsync("[ML101] User email successfully updated.");
        return await ResponseWrapper
            .FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb == null) return await ResponseWrapper.FailAsync("[ML54] User does not exist.");

        userInDb.IsActive = request.Activate;
        var identityResult = await _userManager.UpdateAsync(userInDb);
        if (identityResult.Succeeded)
            return await ResponseWrapper<string>
                .SuccessAsync(request.Activate ?
                "[ML55] User activated successfully." : "[ML56] User de-activated successfully");

        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> GetAllUsersAsync()
    {
        var userInDb = await _userManager.Users.ToListAsync();
        if (userInDb.Count <= 0) return await ResponseWrapper.FailAsync("[ML57] No users were found.");
        var mappedUsers = _mapper.Map<List<UserResponse>>(userInDb);
        return await ResponseWrapper<List<UserResponse>>.SuccessAsync(mappedUsers);
    }

    public async Task<PaginationResult<ApplicationUser>> GetPagedUsersAsync(UserParameters parameters)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(x =>
              x.FirstName.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
              x.LastName.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
              x.UserName.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
              x.Id.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
              x.Email.ToLower().Contains(parameters.SearchTerm.ToLower()) ||
              string
              .Concat(x.FirstName, " ", x.LastName).ToLower()
                .Contains(parameters.SearchTerm.ToLower())
  );

        }

        query = (IQueryable<ApplicationUser>)query.Where(x => x.IsActive == parameters.IsActive)
                     .SortUser(parameters.OrderBy);

        var totalCount = await query.CountAsync();
        var totalPage = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / parameters.ItemsPerPage) : 0;

        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.ItemsPerPage)
            .ToListAsync();

        return new PaginationResult<ApplicationUser>(items, totalCount, totalPage, parameters.Page, parameters.ItemsPerPage);

    }

    public async Task<IResponseWrapper> GetRolesAsync(string userId)
    {
        var userRolesVM = new List<UserRoleViewModel>();

        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb == null) return await ResponseWrapper.FailAsync("[ML58] User does not exist.");

        var allRoles = await _roleManager.Roles.ToListAsync();
        if (allRoles == null) return await ResponseWrapper.FailAsync("[ML59] Role not found.");

        foreach (var role in allRoles)
        {
            var userRoleVM = new UserRoleViewModel
            {
                RoleName = role.Name
            };

            if (await _userManager.IsInRoleAsync(userInDb, role.Name))
            {
                userRoleVM.IsAssignedToUser = true;
            }
            else
            {
                userRoleVM.IsAssignedToUser = false;
            }
            userRolesVM.Add(userRoleVM);
        }
        return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRolesVM);
    }

    public async Task<IResponseWrapper<UserResponse>> GetUserByEmailAsync(string email)
    {
        var userInDb = await _userManager.FindByEmailAsync(email);
        if (userInDb == null)
            return await ResponseWrapper<UserResponse>.FailAsync("[ML60] User not found.");

        var mappedUser = _mapper.Map<UserResponse>(userInDb);
        return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
    }

    public async Task<IResponseWrapper<UserResponse>> GetUserByUserNameAsync(string username)
    {
        var userInDb = await _userManager.FindByNameAsync(username);
        if (userInDb == null)
            return await ResponseWrapper<UserResponse>.FailAsync("[ML61] User not found.");

        var mappedUser = _mapper.Map<UserResponse>(userInDb);
        return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
    }

    public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb == null) return await ResponseWrapper.FailAsync("[ML62] User not found.");

        var mappedUser = _mapper.Map<UserResponse>(userInDb);
        return await ResponseWrapper<UserResponse>.SuccessAsync(mappedUser);
    }

    public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request)
    {
        var userWithEmailInDb = await _userManager.FindByEmailAsync(request.Email);
        if (userWithEmailInDb is not null)
            await ResponseWrapper.FailAsync("[ML63] Email already taken.");

        if (request.Password != request.ConfirmPassword)
            await ResponseWrapper.FailAsync("[ML200] password must be equal.");

        var username = request.Email.Split('@')[0];

        var newUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IsActive = true,
            UserName = username,
            Role = "Basic",
            EmailConfirmed = false
        };
        ;
        var password = new PasswordHasher<ApplicationUser>();
        newUser.PasswordHash = password.HashPassword(newUser, request.Password);

        var identityResult = await _userManager.CreateAsync(newUser);

        if (identityResult.Succeeded)
        {
            //Assing user to basic role
            await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

            var message = "[ML65] User registered successfully, please confirm your email with the code that you have received.";

            return await ResponseWrapper<string>
                .SuccessAsync(message);
        }
        else
        {
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        }
    }

    public async Task<IResponseWrapper> RegisterUserByAdminAsync(UserRegistrationRequest request)
    {
        var userWithEmailInDb = await _userManager.FindByEmailAsync(request.Email);
        if (userWithEmailInDb is not null) await ResponseWrapper.FailAsync("[ML63] Email already taken.");

        var newUser = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IsActive = true,
            EmailConfirmed = true
        };
        var password = new PasswordHasher<ApplicationUser>();
        newUser.PasswordHash = password.HashPassword(newUser, request.Password);

        var identityResult = await _userManager.CreateAsync(newUser);

        if (identityResult.Succeeded)
        {
            //Assing user to basic role
            await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

            var message = "[ML102] User registered successfully, also email confirmed.";

            return await ResponseWrapper<string>
                .SuccessAsync(message);
        }
        else
        {
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        }
    }

    public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb is null) return await ResponseWrapper.FailAsync("[ML66] User does not exist.");

        userInDb.FirstName = request.FirstName;
        userInDb.LastName = request.LastName;

        var userNameCheck = await _userManager.FindByNameAsync(request.UserName);
        var currentUserUserName = _currentUserService.UserName;

        if (userNameCheck is not null && currentUserUserName != request.UserName)
            return await ResponseWrapper.FailAsync("[ML98] Username is already taken.");

        userInDb.UserName = request.UserName;

        var identityResult = await _userManager.UpdateAsync(userInDb);
        if (identityResult.Succeeded)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var currentUserName = _currentUserService.UserName; // Mevcut kullanıcı adı
                var newUserName = request.UserName; // Yeni kullanıcı adı

                // Salt SQL sorgusu
                var sql = "UPDATE PRODUCT.Products SET UserName = {0} WHERE UserName = {1}";

                // SQL'i çalıştır
                await _context.Database.ExecuteSqlRawAsync(sql, newUserName, currentUserName);
                await transaction.CommitAsync();
                return await ResponseWrapper<string>.SuccessAsync("[ML67] User details successfully updated.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return await ResponseWrapper.FailAsync(ex.Message);
            }

        }
        return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
    }

    public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRolesRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb is not null)
        {
            if (userInDb.Email == AppCredentials.Email)
            {
                return await ResponseWrapper.FailAsync("[ML68] User Roles update not permitted.");
            }
            var currentAssignedRoles = await _userManager.GetRolesAsync(userInDb);
            var rolesToBeAssigned = request.Roles
                .Where(role => role.IsAssignedToUser == true)
                .ToList();

            var currentLoggedInUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (currentLoggedInUser is null)
            {
                return await ResponseWrapper.FailAsync("User does not exist.");
            }

            if (await _userManager.IsInRoleAsync(currentLoggedInUser, AppRoles.Admin))
            {
                var identityResult1 = await _userManager.RemoveFromRolesAsync(userInDb, currentAssignedRoles);
                if (identityResult1.Succeeded)
                {
                    var identityResult2 = await _userManager
                        .AddToRolesAsync(userInDb, rolesToBeAssigned.Select(role => role.RoleName));
                    if (identityResult2.Succeeded)
                    {
                        return await ResponseWrapper<string>.SuccessAsync("[ML69] User Roles Updated Successfully.");
                    }
                    return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult2));
                }
                return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult1));
            }
            return await ResponseWrapper.FailAsync("[ML70] User Roles update not permitted.");
        }
        return await ResponseWrapper.FailAsync("[ML71] User does not exist.");
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

    public async Task<IResponseWrapper> DeleteAsync(DeleteUserByUsernameRequest request)
    {

        var userInDb = await _userManager.FindByNameAsync(request.UserName);
        if (userInDb == null) return await ResponseWrapper.FailAsync("[ML72] User does not exist.");

        var identityResult = await _userManager.DeleteAsync(userInDb);
        if (!identityResult.Succeeded)
            return await ResponseWrapper.FailAsync(GetIdentityResultErrorDescriptions(identityResult));
        return await ResponseWrapper<string>.SuccessAsync("[ML73] User successfully deleted.");
    }

}
