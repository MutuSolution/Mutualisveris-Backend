using Application.Features.Identity.Users.Commands;
using Application.Features.Identity.Users.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity;

[Route("[controller]")]
public class UsersController : MyBaseController<UsersController>
{

    [HttpGet("id/{userId}")]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var response = await MediatorSender
            .Send(new GetUserByIdQuery { UserId = userId });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpGet("username/{userName}")]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetUserByuserName(string userName)
    {
        var response = await MediatorSender
            .Send(new GetUserByUserNameQuery { UserName = userName });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpGet("email/{email}")]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var response = await MediatorSender
            .Send(new GetUserByEmailQuery { Email = email });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpGet]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await MediatorSender.Send(new GetAllUsersQuery());
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpGet("all")]
    [MustHavePermission(AppFeature.Users, AppAction.Read)]
    public async Task<IActionResult> GetUsers([FromQuery] UserParameters parameters)
    {
        var query = new GetPagedUsersQuery { Parameters = parameters };
        var result = await MediatorSender.Send(query);
        if (result.IsSuccessful) return Ok(result);
        return NotFound(result);
    }

    [HttpPut]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserRequest userRequest)
    {
        var response = await MediatorSender.Send(new UpdateUserCommand { UpdateUser = userRequest });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpPut("change-email")]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> UpdateUserEmailAsync([FromBody] ChangeEmailRequest request)
    {
        var response = await MediatorSender.Send(new UpdateUserEMailCommand { request = request });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpPut("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangeUserPassword([FromBody] ChangePasswordRequest request)
    {
        var response = await MediatorSender.Send(new ChangeUserPasswordCommand { ChangePassword = request });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpPut("change-status")]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest request)
    {
        var response = await MediatorSender.Send(new ChangeUserStatusCommand { ChangeUserStatus = request });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpGet("roles/{userId}")]
    [MustHavePermission(AppFeature.Roles, AppAction.Read)]
    public async Task<IActionResult> GetRoles(string userId)
    {
        var response = await MediatorSender.Send(new GetRolesQuery { UserId = userId });
        if (response.IsSuccessful) return Ok(response);
        return NotFound(response);
    }

    [HttpPut("user-roles")]
    [MustHavePermission(AppFeature.Users, AppAction.Update)]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest updateUserRoles)
    {
        var response = await MediatorSender.Send(new UpdateUserRolesCommand
        {
            UpdateUserRoles = updateUserRoles
        });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpDelete("hard")]
    [MustHavePermission(AppFeature.Users, AppAction.Delete)]
    public async Task<IActionResult> DeleteUser([FromBody] DeleteUserByUsernameRequest deleteUser)
    {
        var response = await MediatorSender.Send(new DeleteUserByUsernameCommand
        {
            DeleteUserByUsername = deleteUser
        });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }
}
