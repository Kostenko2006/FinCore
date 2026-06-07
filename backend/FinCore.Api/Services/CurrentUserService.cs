using System.Security.Claims;

namespace FinCore.Api.Services;

public static class CurrentUserService
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(value, out var id) ? id : throw new UnauthorizedAccessException("Invalid user token.");
    }
}
