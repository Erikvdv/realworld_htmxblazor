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

    private record CreateArticleRequest(string Title, string Description, string Body, string[] Tags, string Slug);

    private record AddTagRequest(string NewTag, List<string>? Tags);

    private record DeleteTagRequest(List<string>? Tags);

    private static IResult GetEditor(HttpContext context)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
            return Results.Redirect("/");
        var user = AuthenticationHelper.GetUser(context);
        var errors = new Dictionary<string, string[]>();
        var bodyFragment = new EditorComponent().GetFragment(new EditorComponent.Input(new UpdateArticle("", "", "", []),null,errors));
        return RenderHelper.RenderMainLayout(context, bodyFragment, "EditorComponent - Conduit", user);
    }
    
    private static async Task<IResult> GetEditorForSlug(HttpContext context, string slug, IConduitApiClient client)
    {
        var user = context.GetUser();
        if (user == null)
            return Results.Redirect("/");
        
        var article = await client.GetArticleAsync(slug, user.Token);
        var errors = new Dictionary<string, string[]>();
        var bodyFragment = new EditorComponent().GetFragment(new EditorComponent.Input(new UpdateArticle(article.Title, article.Description, article.Body, article.TagList.ToList()),slug,errors));
        return RenderHelper.RenderMainLayout(context, bodyFragment, "EditorComponent - Conduit", user);
    }

    
    private static Task<RazorComponentResult> AddTag(HttpContext context, AddTagRequest request,
        IConduitApiClient client)
    {
        var newTags = (request.Tags ?? [])
            .Concat(new[] { request.NewTag })
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        
        return Task.FromResult(new EditorTagsComponent().GetResult(new EditorTagsComponent.Input(newTags)));
    }
    
    
    private static Task<RazorComponentResult> DeleteTag(HttpContext context, [FromRoute] string tag, [FromBody] DeleteTagRequest request,
        IConduitApiClient client)
    {
        var newTags = (request.Tags ?? [])
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToList();
        newTags.Remove(tag);
        
        return Task.FromResult(new EditorTagsComponent().GetResult(new EditorTagsComponent.Input(newTags)));
    }

    
    private static async Task<IResult> CreateArticle(CreateArticleRequest request, IConduitApiClient client,
        HttpContext context)
    {
        var user = context.GetUser();
        if (user == null)
        {
            context.Response.Htmx(h => h.Redirect("/login"));
            return Results.Unauthorized();
        }

        try
        {
            var article = string.IsNullOrEmpty(request.Slug)
                ? await client.CreateArticleAsync(
                    new NewArticle
                    {
                        Body = request.Body,
                        Description = request.Description,
                        Title = request.Title,
                        TagList = request.Tags?.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToHashSet() ?? []
                    }, user.Token
                )
                : await client.UpdateArticleAsync(
                    request.Slug, new UpdateArticle(
                        request.Title, request.Description, request.Body,
                        request.Tags?.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList() ?? new List<string>()
                    ), user.Token
                );

            context.Response.Htmx(h => h.Redirect($"/article/{article.Slug}"));
            return Results.Ok();
        }
        catch (ApiException apiException)
        {
            var article = new UpdateArticle(request.Title, request.Description, request.Body, request.Tags.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList());
            return new EditorComponent().GetResult(new EditorComponent.Input(article, null, apiException.ErrorList));
        }
    }
}