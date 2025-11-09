using CareSync.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using CareSync.InfrastructureLayer.Common.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CareSync.InfrastructureLayer.Common.Models;

namespace CareSync.InfrastructureLayer.Services.Security;

public static class JwtTokenGenerator
{
    public static async Task<string> GenerateToken(T_Users user, UserManager<T_Users> userManager, RoleManager<T_Roles> roleManager)
    {
        JwtTokenSettings? settings = Configurations.GetConfigurationSectionByName<JwtTokenSettings>("JwtSettings");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings?.SecretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationTime = DateTime.UtcNow.AddMinutes(settings!.ExpiryMinutes);

        var userRoles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        foreach (var role in userRoles)
        {
            var identityRole = await roleManager.FindByNameAsync(role);
            if (identityRole != null)
            {
                var roleClaims = await roleManager.GetClaimsAsync(identityRole);
                claims.AddRange(roleClaims);
            }
        }

        var token = new JwtSecurityToken(
            issuer: settings?.Issuer,
            audience: settings?.Audience,
            claims: claims,
            expires: expirationTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public static string GenerateRefreshToken(string userId)
    {
        var settings = Configurations.GetConfigurationSectionByName<JwtTokenSettings>("JwtSettings");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(settings?.SecretKey!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("token_type", "refresh")
        }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public static ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        JwtTokenSettings? settings = Configurations.GetConfigurationSectionByName<JwtTokenSettings>("JwtSettings");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(settings?.SecretKey!);

        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true, // or false if you want to handle expiry manually
            ClockSkew = TimeSpan.Zero
        }, out _);

        return principal;
    }
}