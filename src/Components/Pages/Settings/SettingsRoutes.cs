using Carter;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Settings;

public class SettingsRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("settings");
        path.MapGet("/", GetSettings);
    }

    private static Task<IResult> GetSettings(HttpContext context, IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated) return Task.FromResult(Results.Redirect("/signin"));

        var user = AuthenticationHelper.GetUser(context);
        var bodyFragment = new Settings().GetRenderFragment(new Settings.Model(user));
        return Task.FromResult<IResult>(RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user));
    }
}