using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Shared.Articles;

public class ArticlesRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("articles");
        // path.MapGet("", GetArticles);
        path.MapGet("/list", GetArticleList);
    }

    private static async Task<RazorComponentResult> GetArticleList(HttpContext context,
        IConduitApiClient client, [AsParameters] ArticlesFilter filter)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        User? user = null;
        if (isAuthenticated) user = AuthenticationHelper.GetUser(context);

        ServiceClient.ArticleList? articles;
        if (filter.MyFeed == true && user is not null)
            articles = await client.GetArticleFeedAsync(
                new ArticlesQuery(filter.Tag, filter.Author, filter.Favorited, 10, (filter.Page - 1) * 10), user.Token);
        else
            articles = await client.GetArticleListAsync(
                new ArticlesQuery(filter.Tag, filter.Author, filter.Favorited, 10, (filter.Page - 1) * 10),
                user?.Token);

        var queryString = filter.ToQueryString();

        context.Response.Htmx(h => { h.ReplaceUrl(queryString); });

        context.Response.Headers.CacheControl = $"max-age={TimeSpan.FromSeconds(60).TotalSeconds}";
        
        return new ArticleList().GetRazorComponentResult(new ArticleList.Model(articles, filter));
    }
}