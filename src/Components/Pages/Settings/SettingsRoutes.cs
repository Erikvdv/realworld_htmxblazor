using Carter;
using Htmx;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Settings;

public class SettingsRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("settings");
        path.MapGet("/", GetSettings);
        path.MapPost("/logout", Logout);
    }

    private static Task<IResult> GetSettings(HttpContext context, IConduitApiClient client)
    {
        var user = context.GetUser();
        
        if (user is null) 
            return Task.FromResult(Results.Redirect("/signin"));
        
        var bodyFragment = new SettingsComponent().GetFragment(new SettingsComponent.Input(user));
        return Task.FromResult<IResult>(RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user));
    }
    
    private static async Task<IResult> Logout(HttpContext context)
    {
        await AuthenticationHelper.Logout(context);
        context.Response.Htmx(h =>
        {
            h.Redirect("/");
        });
        return Results.Ok();
    }
}