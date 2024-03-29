using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared.Articles;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;
using ArticleList = RealworldBlazorHtmx.App.Components.Shared.Articles.ArticleList;

namespace RealworldBlazorHtmx.App.Components.Pages.Home;

public class HomeRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetHome);
        app.MapGet("/home/articleslist", GetHomeArticlesList);
        app.MapGet("/home/tags", GetTags);
    }

    private static RazorComponentResult GetHome(HttpContext context, [AsParameters] ArticlesFilter filter)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        User? user = null;

        if (isAuthenticated)
        {
            user = AuthenticationHelper.GetUser(context);
            if (filter.MyFeed is null && filter.Tag is null) filter = filter with {MyFeed = true};
        }

        var bodyFragment = new Home().GetRenderFragment(new Home.Model(isAuthenticated, filter));


        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }

    private static async Task<RazorComponentResult> GetHomeArticlesList(HttpContext context,
        IConduitApiClient client, [AsParameters] ArticlesFilter filter)
    {
        var articles = await client.GetArticleListAsync(new ArticlesQuery(null, null, null), null);
        context.Response.Headers.CacheControl = $"max-age={TimeSpan.FromSeconds(60).TotalSeconds}";
        return new ArticleList().GetRazorComponentResult(new ArticleList.Model(articles, filter));
    }

    private static async Task<RazorComponentResult> GetTags(HttpContext context, IConduitApiClient client)
    {
        var tags = await client.GetTagListAsync();
        context.Response.Headers.CacheControl = $"max-age={TimeSpan.FromSeconds(60).TotalSeconds}";
        return new Tags().GetRazorComponentResult(new Tags.Model(tags.ToList()));
    }
}