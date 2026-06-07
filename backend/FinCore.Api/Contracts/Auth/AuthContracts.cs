using System.ComponentModel.DataAnnotations;
using FinCore.Api.Domain.Enums;

namespace FinCore.Api.Contracts.Auth;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, MaxLength(180)] string FullName);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record UserResponse(long Id, string Email, string FullName, UserRole Role, DateTime CreatedAt);

public record AuthResponse(string Token, DateTime ExpiresAt, UserResponse User);
