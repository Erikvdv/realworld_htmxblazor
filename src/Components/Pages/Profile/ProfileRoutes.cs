using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared.Articles;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Profile;

public class ProfileRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("profile");
        path.MapGet("/{profileName}", GetProfile);
        path.MapGet("/{profileName}/favorites", GetProfileFavorited);
    }

    private static async Task<RazorComponentResult> GetProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

        User? user = null;
        if (isAuthenticated) user = AuthenticationHelper.GetUser(context);

        var profile = await client.GetProfileAsync(profileName, user?.Token);
        var filter = new ArticlesFilter(null, profileName, null, null);
        var bodyFragment =
            new Profile().GetRenderFragment(new Profile.Model(isAuthenticated, profile, filter,
                profileName == user?.Username));

        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }

    private static async Task<RazorComponentResult> GetProfileFavorited(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

        User? user = null;
        if (isAuthenticated) user = AuthenticationHelper.GetUser(context);
        var profile = await client.GetProfileAsync(profileName, null);
        var filter = new ArticlesFilter(null, null, profileName, null);
        var bodyFragment =
            new Profile().GetRenderFragment(new Profile.Model(isAuthenticated, profile, filter,
                profileName == user?.Username));

        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit");
    }
}