using Carter;
using Htmx;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RealworldBlazorHtmx.App.Components.Shared.Services;
using RealworldBlazorHtmx.App.ServiceClient;

namespace RealworldBlazorHtmx.App.Components.Pages.Editor;

public class EditorRoutes : CarterModule
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var path = app.MapGroup("editor");
        path.MapGet("/", GetEditor);
        path.MapGet("/{slug}", GetEditorForSlug);
        path.MapPost("/tags", AddTag);
        path.MapDelete("/tags/{tag}", DeleteTag);
        path.MapPost("/article", CreateArticle);
    }

    private static IResult GetEditor(HttpContext context)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
            return Results.Redirect("/");
        var user = AuthenticationHelper.GetUser(context);
        var errors = new Dictionary<string, string[]>();
        var bodyFragment = new Editor().GetRenderFragment(new Editor.Model(new UpdateArticle("", "", "", []),null,errors));
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Editor - Conduit", user);
    }
    
    private static async Task<IResult> GetEditorForSlug(HttpContext context, string slug, IConduitApiClient client)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
            return Results.Redirect("/");
        
        var user = AuthenticationHelper.GetUser(context);
        var article = await client.GetArticleAsync(slug, user.Token);
        var errors = new Dictionary<string, string[]>();
        var bodyFragment = new Editor().GetRenderFragment(new Editor.Model(new UpdateArticle(article.Title, article.Description, article.Body, article.TagList.ToList()),slug,errors));
        return RenderHelper.RenderMainLayout(context, bodyFragment, "Editor - Conduit", user);
    }


    
    private static Task<RazorComponentResult> AddTag(HttpContext context, AddTagRequest request,
        IConduitApiClient client)
    {
        var newTags = (request.Tags ?? [])
            .Concat(new[] { request.NewTag })
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        
        return Task.FromResult(new EditorTags().GetRazorComponentResult(new EditorTags.Model(newTags)));
    }
    
    
    private static Task<RazorComponentResult> DeleteTag(HttpContext context, [FromRoute] string tag, [FromBody] DeleteTagRequest request,
        IConduitApiClient client)
    {
        var newTags = (request.Tags ?? [])
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        newTags.Remove(tag);
        
        return Task.FromResult(new EditorTags().GetRazorComponentResult(new EditorTags.Model(newTags)));
    }

    
    private static async Task<IResult> CreateArticle(CreateArticleRequest request, IConduitApiClient client,
        HttpContext context)
    {
        Dictionary<string, string[]> errors = new();
        
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            context.Response.Htmx(h =>
            {
                h.Redirect("/login");
            });
            return Results.Unauthorized();
        }
    
        try
        {
            var user = AuthenticationHelper.GetUser(context);
            ServiceClient.Article article;
            if (string.IsNullOrEmpty(request.Slug))
            {
                var newArticle = new NewArticle
                {
                    Body = request.Body, 
                    Description = request.Description, 
                    Title = request.Title,
                    TagList = request.Tags?.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToHashSet() ?? []
                };
                 article = await client.CreateArticleAsync(newArticle, user.Token);
            }
            else
            {
                 article = await client.UpdateArticleAsync(
                    request.Slug,
                    new UpdateArticle(request.Title, request.Description, request.Body,
                        request.Tags?.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList() ?? []),
                    user.Token);
            }
            
            context.Response.Htmx(h =>
            {
                h.Redirect($"/article/{article.Slug}");
            });
            return Results.Ok();
        }
        catch (ApiException apiException)
        {
            var article = new UpdateArticle(request.Title, request.Description, request.Body, request.Tags?.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList() ?? []);
            return new Editor().GetRazorComponentResult(new Editor.Model(article, null, apiException.ErrorList));
        }
    }
}