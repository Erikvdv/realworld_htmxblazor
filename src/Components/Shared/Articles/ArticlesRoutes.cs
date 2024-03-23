using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Shared.Articles;



public class ArticlesRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("articles");
        path.MapGet("", GetArticles);
        path.MapGet("/list", GetArticleList);
    }
    
    private static RazorComponentResult GetArticles(HttpContext context, 
        IConduitApiClient client, [AsParameters] ArticlesFilter filter)
    {
        context.Response.Htmx(h =>
        {
            h.WithTrigger("articlelist:pagechanged", new {tag = filter.Tag}, HtmxTriggerTiming.AfterSwap);
        });
        return new Articles().GetRazorComponentResult(new Articles.Model(false, filter));
    }
    
    private static async Task<RazorComponentResult> GetArticleList(HttpContext context, 
        IConduitApiClient client, [AsParameters] ArticlesFilter filter)
    {
        var articles = await client.GetArticleListAsync(new ArticlesQuery(filter.Tag, null, null, 10, (filter.Page - 1) * 10), null);

        var query = $"?page={filter.Page}";
        if (filter.Tag is not null)
        {
            query += $"&tag={filter.Tag}";
        }
        context.Response.Htmx(h =>
        {
            h.ReplaceUrl(query);
        });
   
        return new ArticleList().GetRazorComponentResult(new ArticleList.Model(articles, filter));
    }
    

}