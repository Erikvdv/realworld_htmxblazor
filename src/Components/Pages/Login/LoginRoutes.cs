using Carter;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using RealworldBlazorHtmx.App.Components.Shared;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Login;

public class LoginRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("login");
        path.MapGet("/", GetLogin);
        path.MapPost("/", SubmitLogin)
            .DisableAntiforgery(); // todo: fix this later: https://andrewlock.net/exploring-the-dotnet-8-preview-form-binding-in-minimal-apis/
    }

    private static IResult GetLogin(HttpContext context)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (isAuthenticated)
            return Results.Redirect("/");
        
        var bodyFragment = RazorResult.GetComponentFragment<Login>(null);
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Sign-in - Conduit");
    }

    private static async Task<IResult> SubmitLogin([FromForm] LoginFormInput input, IConduitApiClient client,
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