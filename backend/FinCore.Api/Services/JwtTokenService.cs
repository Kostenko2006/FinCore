using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinCore.Api.Contracts.Auth;
using FinCore.Api.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace FinCore.Api.Services;

public class JwtTokenService(IConfiguration configuration)
{
    public AuthResponse CreateToken(User user)
    {
        var jwt = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresMinutes"] ?? "120"));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            new UserResponse(user.Id, user.Email, user.FullName, user.Role, user.CreatedAt));
    }
}
