using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using MiniValidation;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Register;

public class RegisterRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("register");
        path.MapGet("/", GetRegister);
        path.MapPost("/", SubmitRegisterForm);
    }
    
    
    private static IResult GetRegister(HttpContext context)
    {
        var user = context.GetUser();
        if (user != null)
            return Results.Redirect("/");
        
        var bodyFragment = new RegisterComponent().GetFragment(new RegisterComponent.Input());
        return RenderHelper.RenderMainLayout(context, bodyFragment, "RegisterComponent - Conduit");
    }
    
    private record RegisterFormInput(string Email, string Username, string Password);

    private static async Task<IResult> SubmitRegisterForm(RegisterFormInput input,
        IConduitApiClient client, HttpContext context)
    {
        MiniValidator.TryValidate(input, out var validationErrors); //todo: fix this later

        try
        {
            var user = await client.RegisterUserAsync(new NewUser(input.Username, input.Email, input.Password));
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
            return new RegisterFormComponent().GetResult(new RegisterFormComponent.Input(apiException.ErrorList));
        }
    }
}