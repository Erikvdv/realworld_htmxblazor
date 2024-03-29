using Carter;
using Htmx;
using MiniValidation;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Login;

public class LoginRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("login");
        path.MapGet("/", GetLogin);
        path.MapPost("/", SubmitLogin);
    }

    private static IResult GetLogin(HttpContext context)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
            return Results.Redirect("/");

        var bodyFragment = new Login().GetRenderFragment(new Login.Model());
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Sign-in - Conduit");
    }

    private static async Task<IResult> SubmitLogin(LoginFormInput input, IConduitApiClient client,
        HttpContext context)
    {
        Dictionary<string, string[]> errors = new();
        MiniValidator.TryValidate(input, out var validationErrors);

        try
        {
            var user = await client.LoginAsync(new ServiceClient.Login
                {Email = input.Email, Password = input.Password});
            await AuthenticationHelper.LoginUser(context, user);
            context.Response.Htmx(h =>
            {
                h.Redirect("/");
                h.WithTrigger("UserLoggedIn");
            });
            return Results.Ok();
        }
        catch (ApiException apiException)
        {
            return new LoginForm().GetRazorComponentResult(new LoginForm.Model(apiException.ErrorList));
        }
    }
}