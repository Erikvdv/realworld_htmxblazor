using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Pages.Article;
using RealworldBlazorHtmx.App.Components.Shared.Articles;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Profile;

public class ProfileRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("profile");
        path.MapGet("/", GetProfile);
        path.MapPost("/{profileName}/follow", FollowProfile);
        path.MapDelete("/{profileName}/follow", UnfollowProfile);
    }

    private static async Task<RazorComponentResult> GetProfile(HttpContext context, [AsParameters] ArticlesFilter filter,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

        User? user = null;
        if (isAuthenticated) user = AuthenticationHelper.GetUser(context);

        var profileName = filter.Author ?? filter.Favorited;

        var profile = await client.GetProfileAsync(profileName, user?.Token);
      
        var bodyFragment =
            new Profile().GetRenderFragment(new Profile.Model(isAuthenticated, profile, filter,
                profileName == user?.Username));

        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }
    
    
    private static async Task<IResult> FollowProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            context.Response.Htmx(h =>
            {
                h.Redirect("/login");
            });
            return Results.Unauthorized();
        }

        
        var user = AuthenticationHelper.GetUser(context);
        var profile = await client.FollowProfileAsync(profileName, user.Token);
        
        return new ProfileFollowing().GetRazorComponentResult(new ProfileFollowing.Model(profile));
    }
    
    private static async Task<IResult> UnfollowProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            return Results.Unauthorized();
        }

        
        var user = AuthenticationHelper.GetUser(context);
        var profile = await client.UnFollowProfileAsync(profileName, user.Token);
        
        return new ProfileFollowing().GetRazorComponentResult(new ProfileFollowing.Model(profile));
    }
}