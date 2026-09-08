using System.Security.Claims;

namespace DigitalArs.Application.Security;

public static class CurrentUserHelper
{
    public static int GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("userId");

        if (userIdClaim is null)
        {
            throw new UnauthorizedAccessException(
                "El token no contiene un userId válido.");
        }

        if (!int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException(
                "El userId del token no es válido.");
        }

        return userId;
    }
}