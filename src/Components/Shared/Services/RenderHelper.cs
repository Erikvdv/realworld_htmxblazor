using Htmx;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Shared.Services;

public static class RenderHelper
{
    public static RazorComponentResult RenderMainLayout(HttpContext context,
        RenderFragment bodyFragment,
        string pageTitle, User? user = null)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        var layoutFragment = new MainLayout()
            .GetFragment(new MainLayout.Input(bodyFragment, isAuthenticated, context.Request.Path, user));


        var props = new App.Input(layoutFragment, pageTitle);

        return new App().GetResult(props);
    }
}