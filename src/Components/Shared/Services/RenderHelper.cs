using Htmx;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RealworldBlazorHtmx.App.Components.Shared.Services;

public static class RenderHelper
{
    public static RazorComponentResult RenderMainLayout(HttpContext context,
        RenderFragment bodyFragment,
        string pageTitle)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        var layoutFragment = new MainLayout()
            .GetRenderFragment(new MainLayout.Model(bodyFragment, isAuthenticated, context.Request.Path));

        var isPartial = context.Request.IsHtmx();
        var props = new App.Model(layoutFragment, pageTitle, isPartial);

        return new App().GetRazorComponentResult(props);
    }
}