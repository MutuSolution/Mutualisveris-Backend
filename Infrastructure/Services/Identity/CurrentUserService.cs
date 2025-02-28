using Application.Services.Identity;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirst("id")?.Value ?? "0";
        }
    }
    public string UserName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirst("userName")?.Value ?? "user";
        }
    }

    public string Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?
                .FindFirst("email")?.Value ?? "user@localhost";
        }
    }
}
