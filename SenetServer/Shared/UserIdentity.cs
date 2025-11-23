using Microsoft.AspNetCore.Http;
using System;

namespace SenetServer.Shared
{
    public static class UserIdentity
    {
        public const string CookieName = "SenetUserId";

        public static string GetOrCreateUserId(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            if (httpContext.Request.Cookies.TryGetValue(CookieName, out var existing) && Guid.TryParse(existing, out _))
            {
                return existing;
            }

            var newId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            };
            httpContext.Response.Cookies.Append(CookieName, newId, cookieOptions);
            return newId;
        }
    }
}