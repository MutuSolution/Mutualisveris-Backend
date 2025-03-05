using Application.Features.Identity.Token.Queries;
using Application.Features.Identity.Users.Commands;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers.Identity;

[Route("[controller]")]
public class AuthController : MyBaseController<AuthController>
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
    {

        var response = await MediatorSender.Send(new GetTokenQuery { TokenRequest = tokenRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }


    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        var response = await MediatorSender.Send(
            new GetRefreshTokenQuery { RefreshTokenRequest = refreshTokenRequest });
        if (response.IsSuccessful)
        {
            return Ok(response);
        }
        return BadRequest(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest userRegistration)
    {
        var response = await MediatorSender
            .Send(new UserRegistrationCommand { UserRegistration = userRegistration });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("admin-add-user")]
    [MustHavePermission(AppFeature.Users, AppAction.Create)]
    public async Task<IActionResult> RegisterUserByAdmin([FromBody] UserRegistrationRequest userRegistration)
    {
        var response = await MediatorSender
            .Send(new UserAddingByAdminCommand { UserRegistration = userRegistration });
        if (response.IsSuccessful) return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> LogoutAsync()
    {
        await Task.CompletedTask;
        return Ok();
    }
}
