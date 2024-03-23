using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Shared.Services;

public static class AuthenticationHelper
{
    public static async Task LoginUser(HttpContext context, User user)
    {
        var claims = new List<Claim> {new("Username", user.Username), new("Token", user.Token)};

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
            {ExpiresUtc = DateTimeOffset.UtcNow.AddDays(60), IsPersistent = true};

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);
    }
}