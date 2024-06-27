using System.Security.Claims;
using Rabbitlike.Utils.Claims;

namespace Rabbitlike.Utils.Extensions
{
    public static class ClaimExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal claims)
        {
            var _claim = claims?.FindFirst(UserClaimTypes.UserId.ToString());

            return _claim is not null ? Guid.Parse(_claim.Value) : Guid.Empty;
        }

        public static bool IsAdmin(this ClaimsPrincipal claims)
        {
            return claims.IsInRole("Admin");
        }
    }
}
