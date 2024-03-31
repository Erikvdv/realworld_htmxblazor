using System.ComponentModel.DataAnnotations;

namespace RealworldBlazorHtmx.App.Components.Pages.Login;

public record LoginFormInput(string Email, string Password) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Email)) yield return new ValidationResult("can't be blank.", new[] {nameof(Email)});

        if (string.IsNullOrEmpty(Password))
            yield return new ValidationResult("can't be blank.", new[] {nameof(Password)});
    }
}