using Carter;
using Htmx;
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
        path.MapPost("/{slug}/favorite", FavoriteArticle);
        path.MapDelete("/{slug}/favorite", UnFavoriteArticle);
        path.MapGet("/{slug}/comments", GetComments);
        path.MapPost("/{slug}/comments", AddComment);
        path.MapDelete("/{slug}/comments/{commentId}", DeleteComment);
        path.MapPost("/followauthor/{authorUsername}", FollowAuthor);
        path.MapDelete("/followauthor/{authorUsername}", UnfollowAuthor);
        
    }

    private static async Task<RazorComponentResult> GetArticle(HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        User? user = null;

        if (isAuthenticated) user = AuthenticationHelper.GetUser(context);

        var article = await client.GetArticleAsync(slug, user?.Token);

        var bodyFragment = new Article().GetRenderFragment(new Article.Model(isAuthenticated, article, user));
        
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Home - Conduit", user);
    }
    
    private static async Task<IResult> FavoriteArticle(HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            context.Response.Htmx(h =>
            {
                h.Redirect("/login");
            });
            return Results.Unauthorized();
        }

        var user = AuthenticationHelper.GetUser(context);
        var article = await client.FavoriteArticleAsync(slug, user.Token);
        
        return new Article().GetRazorComponentResult(new Article.Model(isAuthenticated, article, user));
    }
    
    private static async Task<IResult> UnFavoriteArticle(HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated) return Results.Forbid();

        var user = AuthenticationHelper.GetUser(context);
        var article = await client.UnfavoriteArticleAsync(slug, user.Token);

        return new Article().GetRazorComponentResult(new Article.Model(isAuthenticated, article, user));
    }


    
    private static async Task<IResult> GetComments(HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated) return Results.Forbid();
        
        var user = AuthenticationHelper.GetUser(context);
        var comments = await client.GetArticleCommentsAsync(slug, user.Token);

        return new Comments().GetRazorComponentResult(new Comments.Model(slug,comments, user));
    }
    
    public record NewComment(string Comment);
    private static async Task<IResult> AddComment(NewComment newComment, HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated) return Results.Forbid();
        
        var user = AuthenticationHelper.GetUser(context);
        var comment = await client.AddCommentAsync(slug, newComment.Comment, user.Token);
        var comments = await client.GetArticleCommentsAsync(slug, user.Token);

        return new Comments().GetRazorComponentResult(new Comments.Model(slug,comments, user));
    }
    
    private static async Task<IResult> DeleteComment(int commentId, HttpContext context, string slug,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated) return Results.Forbid();
        
        var user = AuthenticationHelper.GetUser(context);
        await client.DeleteCommentAsync(slug, commentId, user.Token);
        var comments = await client.GetArticleCommentsAsync(slug, user.Token);

        return new Comments().GetRazorComponentResult(new Comments.Model(slug, comments, user));
    }
    
    private static async Task<IResult> FollowAuthor(HttpContext context, string authorUsername,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            context.Response.Htmx(h =>
            {
                h.Redirect("/login");
            });
            return Results.Unauthorized();
        }

        
        var user = AuthenticationHelper.GetUser(context);
        var profile = await client.FollowProfileAsync(authorUsername, user.Token);
        
        return new ArticleAuthor().GetRazorComponentResult(new ArticleAuthor.Model(profile));
    }
    
    private static async Task<IResult> UnfollowAuthor(HttpContext context, string authorUsername,
        IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            return Results.Unauthorized();
        }

        
        var user = AuthenticationHelper.GetUser(context);
        var profile = await client.UnFollowProfileAsync(authorUsername, user.Token);
        
        return new ArticleAuthor().GetRazorComponentResult(new ArticleAuthor.Model(profile));
    }
}