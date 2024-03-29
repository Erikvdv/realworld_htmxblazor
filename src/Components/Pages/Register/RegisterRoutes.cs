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

    private static RazorComponentResult GetRegister(HttpContext context)
    {
        var bodyFragment = new Register().GetRenderFragment(new Register.Model());
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Register - Conduit");
    }

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
            return new RegisterForm().GetRazorComponentResult(new RegisterForm.Model(apiException.ErrorList));
        }
    }
}