using System.ComponentModel.DataAnnotations;
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
    
    private record LoginFormInput(string Email, string Password) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Email)) 
                yield return new ValidationResult("can't be blank.", new[] {nameof(Email)});
            if (string.IsNullOrEmpty(Password))
                yield return new ValidationResult("can't be blank.", new[] {nameof(Password)});
        }
    }

    private static IResult GetLogin(HttpContext context)
    {
        var user = context.GetUser();

        if (user is not null)
            return Results.Redirect("/");

        var bodyFragment = new LoginComponent().GetFragment(new LoginComponent.Input());
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
            return new LoginFormComponent().GetResult(new LoginFormComponent.Input(apiException.ErrorList));
        }
    }
}