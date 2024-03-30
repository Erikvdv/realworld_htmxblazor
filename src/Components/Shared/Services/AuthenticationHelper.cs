using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Shared.Services;

public static class AuthenticationHelper
{
    public static async Task LoginUser(HttpContext context, User user)
    {
        var claims = new List<Claim>
            {new("Username", user.Username), new("Token", user.Token), new("Image", user.Image)};

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
            {ExpiresUtc = DateTimeOffset.UtcNow.AddDays(60), IsPersistent = true};

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), authProperties);
    }

    public static User GetUser(HttpContext context)
    {
        var token = context.User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
        var image = context.User.Claims.FirstOrDefault(c => c.Type == "Image")?.Value;
        var username = context.User.Claims.FirstOrDefault(c => c.Type == "Username")?.Value;
        return new User
        {
            Token = token,
            Image = image,
            Username = username,
            Bio = "",
            Email = ""
        };
    }
}