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

namespace Infrastructure.Services.Identity
{
    public sealed class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppConfiguration _appConfiguration;

        public TokenService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<AppConfiguration> appConfiguration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appConfiguration = appConfiguration.Value;
        }

        public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest)
        {
            if (string.IsNullOrWhiteSpace(tokenRequest?.Email))
            {
                return await ResponseWrapper<TokenResponse>
                    .FailAsync("[ML95] Email boş olamaz.");
            }

            var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
            if (user is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML42] Mail ya da şifreniz yanlış.");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return await ResponseWrapper<TokenResponse>
                    .FailAsync("[ML44] Hesap beklemeye alındı, lütfen daha sonra tekrar deneyiniz.");
            }

            if (!user.IsActive)
            {
                return await ResponseWrapper<TokenResponse>
                    .FailAsync("[ML104] Hesap aktif değil, lütfen iletişime geçiniz.");
            }

            if (!user.EmailConfirmed)
            {
                return await ResponseWrapper<TokenResponse>
                    .FailAsync("[ML105] Email adresinizi onaylayınız.");
            }

            if (!await _userManager.CheckPasswordAsync(user, tokenRequest.Password))
            {
                await _userManager.AccessFailedAsync(user);
                return await ResponseWrapper<TokenResponse>
                    .FailAsync("[ML46] Mail ya da şifreniz yanlış.");
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var jwtToken = await GenerateJWTAsync(user);
            var response = new TokenResponse
            (
                id: user.Id,
                email: user.Email,
                firstName: user.FirstName,
                lastName: user.LastName,
                userName: user.UserName,
                phone: user.PhoneNumber,
                roles: userRoles.ToArray(),
                exp: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes).ToString(),
                Token: jwtToken,
                RefreshToken: user.RefreshToken,
                RefreshTokenExpiryTime: user.RefreshTokenExpiryDate
            );

            // Başarılı response data varsa, tek mesaj ile döndürülüyor.
            return await ResponseWrapper<TokenResponse>.SuccessAsync(response, "[ML65] Token başarıyla oluşturuldu.");
        }

        public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML47] Bilinmeyen token.");
            }

            var principal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var userEmail = principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML48] Hesap bulunamadı.");
            }

            if (user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryDate <= DateTime.UtcNow)
            {
                return await ResponseWrapper<TokenResponse>.FailAsync("[ML49] Bilinmeyen token.");
            }

            var newJwtToken = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var response = new TokenResponse
            (
                id: user.Id,
                email: user.Email,
                firstName: user.FirstName,
                lastName: user.LastName,
                userName: user.UserName,
                phone: user.PhoneNumber,
                roles: userRoles.ToArray(),
                exp: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes).ToString(),
                Token: newJwtToken,
                RefreshToken: user.RefreshToken,
                RefreshTokenExpiryTime: user.RefreshTokenExpiryDate
            );

            return await ResponseWrapper<TokenResponse>.SuccessAsync(response, "[ML65] Token başarıyla yenilendi.");
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateJWTAsync(ApplicationUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secretBytes = Encoding.UTF8.GetBytes(_appConfiguration.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
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
                var rolePermissions = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(rolePermissions);
            }

            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("email", user.Email),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("userName", user.UserName),
                new Claim("phone", user.PhoneNumber ?? string.Empty)
            };

            return claims
                .Union(userClaims)
                .Union(roleClaims)
                .Union(permissionClaims);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Süre kontrolünü elle yapıyoruz.
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, System.StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("[ML50] Bilinmeyen token.");
            }

            return principal;
        }
    }
}
