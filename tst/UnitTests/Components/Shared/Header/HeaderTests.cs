using AngleSharp;
using FluentAssertions;
using RealworldBlazorHtmx.App.Components.Shared.Header;
using RealworldBlazorHtmx.App.ServiceClient;

namespace UnitTests.Components.Shared.Header;

public class HeaderTests
{
    private readonly BlazorRenderer _renderer = new();

    [Fact]
    public async Task RendersCorrectlyWhenNotAuthenticated()
    {
        var model = new AppHeaderComponent.Input(false, "/", null);
        var html = await _renderer.RenderComponent<AppHeaderComponent>(model);
        var document = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(html));
        var listItems = document.QuerySelectorAll("li");

        listItems[0].TextContent.Should().Be("HomeComponent");
        listItems[1].TextContent.Should().Be("Sign in");
        listItems[2].TextContent.Should().Be("Sign up");
    }

    [Fact]
    public async Task RendersCorrectlyWhenAuthenticated()
    {
        var user = new User
        {
            Username = "Erik van de Ven",
            Image = "/evdv.png",
            Bio = "",
            Email = "",
            Token = ""
        };
        var model = new AppHeaderComponent.Input(true, "/", user);

        var html = await _renderer.RenderComponent<AppHeaderComponent>(model);
        var document = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(html));

        var listItems = document.QuerySelectorAll("li");

        listItems[0].TextContent.Should().Be("HomeComponent");
        listItems[1].TextContent.Should().Be("New Article");
        listItems[2].TextContent.Should().Be("SettingsComponent");
        listItems[3].TextContent.Trim().Should().Be(user.Username);
    }
}