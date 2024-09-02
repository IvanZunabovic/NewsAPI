using Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services;

public class ClaimsService : IClaimsService
{
    public async Task CreateClaims(IHttpContextAccessor contextAccessor, string userFullName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userFullName)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // The time at which the authentication ticket expires. A 
            // value set here overrides the ExpireTimeSpan option of 
            // CookieAuthenticationOptions set with AddCookie.

            //IsPersistent = true,
            // Whether the authentication session is persisted across 
            // multiple requests. When used with cookies, controls
            // whether the cookie's lifetime is absolute (matching the
            // lifetime of the authentication ticket) or session-based.

            IssuedUtc = DateTimeOffset.UtcNow,
            // The time at which the authentication ticket was issued.
        };

        await contextAccessor.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    public async Task SignoutAsync(IHttpContextAccessor httpContextAccessor)
    {
        await httpContextAccessor.HttpContext.SignOutAsync();
    }
}
