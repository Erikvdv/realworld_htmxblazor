namespace RealworldBlazorHtmx.App.Components.Pages.Editor;

public record CreateArticleRequest
{
    public string Title { get; init; }
    public string Description { get; init; }
    public string Body { get; init; }
    public string[] Tags { get; init; }
    public string Slug { get; init; }

    public CreateArticleRequest() : this("", "", "", new string[0], "")
    {
    }

    public CreateArticleRequest(string title, string description, string body, string[] tags, string slug)
    {
        Title = title;
        Description = description;
        Body = body;
        Tags = tags;
        Slug = slug;
    }
};
public record AddTagRequest(string NewTag, List<string>? Tags);
public record DeleteTagRequest(List<string>? Tags);