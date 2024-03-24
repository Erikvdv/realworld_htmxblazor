namespace RealworldBlazorHtmx.App.Components.Shared.Articles;

public record ArticlesFilter(string? Tag, string? Author, string? Favorited, int Page = 1, bool MyFeed = false)
{
    public string ToQueryString()
    {
        var parameters = new Dictionary<string, string?>
        {
            { "page", Page.ToString() },
            { "tag", Tag },
            { "author", Author},
            { "favorited", Favorited}
        };
        
        return "?" + string.Join("&", parameters.Where(p => !string.IsNullOrEmpty(p.Value)).Select(p => $"{p.Key}={p.Value}"));
    }
}