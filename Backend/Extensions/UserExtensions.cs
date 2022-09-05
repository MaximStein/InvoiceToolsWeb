using Backend.Entities;
using MongoDB.Bson;
using System.Security.Claims;

namespace Backend.Extensions
{
    public static class UserExtensions
    {
        public static string? GetId(this ClaimsPrincipal principal)
        {
            var id = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (id == null)
                return null;

            return id.Value;
        }

        public static string? GetUserId(this IHttpContextAccessor principal)
        {
            return principal.HttpContext?.User.GetId();
        }

        public static bool IsUserAuthenticated(this IHttpContextAccessor principal)
        {
            return principal.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }
    }
}
