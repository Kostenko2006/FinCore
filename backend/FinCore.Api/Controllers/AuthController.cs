using FinCore.Api.Contracts.Auth;
using FinCore.Api.Data;
using FinCore.Api.Domain.Entities;
using FinCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(FinCoreDbContext db, JwtTokenService jwtTokenService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email))
        {
            return Conflict(new { message = "User with this email already exists." });
        }

        var user = new User
        {
            Email = email,
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Me), jwtTokenService.CreateToken(user));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(jwtTokenService.CreateToken(user));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Me()
    {
        var user = await db.Users.FindAsync(User.GetUserId());
        return user is null ? Unauthorized() : Ok(user.ToResponse());
    }
}
