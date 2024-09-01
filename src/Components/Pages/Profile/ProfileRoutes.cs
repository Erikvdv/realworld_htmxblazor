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
        var user = context.GetUser();

        var profileName = filter.Author ?? filter.Favorited;

        var profile = await client.GetProfileAsync(profileName, user?.Token);
      
        var bodyFragment =
            new ProfileComponent().GetFragment(new ProfileComponent.Input(user is not null, profile, filter,
                profileName == user?.Username));

        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }
    
    
    private static async Task<IResult> FollowProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var user = context.GetUser();

        if (user is null)
        {
            context.Response.Htmx(h => h.Redirect("/login"));
            return Results.Unauthorized();
        }
        
        var profile = await client.FollowProfileAsync(profileName, user.Token);
        
        return new ProfileFollowingComponent().GetResult(new ProfileFollowingComponent.Input(profile));
    }
    
    private static async Task<IResult> UnfollowProfile(HttpContext context, string profileName,
        IConduitApiClient client)
    {
        var user = context.GetUser();

        if (user is null) 
            return Results.Unauthorized();
        
        var profile = await client.UnFollowProfileAsync(profileName, user.Token);
        
        return new ProfileFollowingComponent().GetResult(new ProfileFollowingComponent.Input(profile));
    }
}