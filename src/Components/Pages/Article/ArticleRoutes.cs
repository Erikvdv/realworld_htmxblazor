using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Article;

public class ArticleRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("article");
        path.MapGet("/{slug}", GetArticle);
    }

    private static async Task<RazorComponentResult> GetArticle(HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        User? user = null;

        if (isAuthenticated) user = AuthenticationHelper.GetUser(context);

        var article = await client.GetArticleAsync(slug, default);

        var bodyFragment =
            new Article().GetRenderFragment(new Article.Model(isAuthenticated, article,
                article.Author.Username == user?.Username));

        context.Response.Headers.CacheControl = $"max-age={TimeSpan.FromSeconds(60).TotalSeconds}";
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }
}