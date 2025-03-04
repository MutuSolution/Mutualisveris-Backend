using Application.AppConfigs;
using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses;
using Common.Responses.Wrappers;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfiguration _appConfiguration;

    public TokenService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfiguration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfiguration = appConfiguration.Value;
    }

    public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
    {
        if (tokenRequest.Email is null)
        {
            return await ResponseWrapper<TokenResponse>
                .FailAsync("[ML95] Email boş olamaz");
        }

        var user = await _userManager.FindByEmailAsync(tokenRequest.Email);

        if (user is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML42] Mail yada şifreniz yanlış.");
        }

        if (!user.LockoutEnabled)
        {
            return await ResponseWrapper<TokenResponse>
                .FailAsync("[ML43] Hesap beklemeye alınmış, lütfen daha sonra tekrar deneyiniz.");
        }
        if (!user.IsActive)
        {
            return await ResponseWrapper<TokenResponse>
                .FailAsync("[ML104] Hesap aktif değil lütfen, iletişime geçiniz.");
        }
        if (!user.EmailConfirmed)
        {
            return await ResponseWrapper<TokenResponse>
                .FailAsync("[ML105] Email adresinizi onaylayınız.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML44] Hesap beklemeye alındı, lütfen daha sonra tekrar deneyiniz.");
        }

        if (!await _userManager.CheckPasswordAsync(user, tokenRequest.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML46] Mail yada şifreniz yanlış.");
        }
        await _userManager.ResetAccessFailedCountAsync(user);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        var token = await GenerateJWTAsync(user);
        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };

        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
    }

    public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {

        if (refreshTokenRequest is null)
        {
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML47] Bilinmeyen token.");
        }
        var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);

        if (user is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML48] Hesap bulunamadı.");
        if (user.RefreshToken != refreshTokenRequest.RefreshToken ||
            user.RefreshTokenExpiryDate <= DateTime.Now)
            return await ResponseWrapper<TokenResponse>.FailAsync("[ML49] Bilinmeyen token.");

        var token = GenerateEncrytedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        user.RefreshToken = GenerateRefreshToken();
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };
        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateJWTAsync(ApplicationUser user)
    {
        var token = GenerateEncrytedToken(GetSigningCredentials(), await GetClaimsAsync(user));
        return token;
    }

    private string GenerateEncrytedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
            signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secret = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("role", role));
            var currentRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
            permissionClaims.AddRange(allPermissionsForCurrentRole);
        }

        var claims = new List<Claim>
        {
        new ("id", user.Id), // ClaimTypes.NameIdentifier yerine kısa "id"
        new("email", user.Email), // ClaimTypes.Email yerine kısa "email"
        new("firstName", user.FirstName), // ClaimTypes.Name yerine kısa "name"
        new("lastName", user.LastName), // ClaimTypes.Surname yerine kısa "surname"
        new("userName", user.UserName), // ClaimTypes.Surname yerine kısa "surname"
        new("phone", user.PhoneNumber ?? string.Empty) // ClaimTypes.MobilePhone yerine kısa "phone"
        }
        .Union(userClaims)
        .Union(roleClaims)
        .Union(permissionClaims);

        return claims;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,//YG! bu satırı kendim ekledim
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg
            .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("[ML50] Bilinmeyen token.");
        }

        return principal;
    }


}
