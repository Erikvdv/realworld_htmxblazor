using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace RealworldBlazorHtmx.App.ServiceClient;

public record ArticlesQuery(string? Tag, string? Author, string? Favorited, int Limit = 20, int Offset = 0);

public enum FeedType
{
    Global,
    Private
}

public class Login
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}

public class User
{
    public string Bio { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Email { get; set; }
    public int Id { get; set; }
    public string Image { get; set; }
    public string? Password { get; set; }
    public string Token { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Username { get; set; }
}

public record NewUser(string Username, string Email, string Password);

public class Profile
{
    public string Username { get; set; }
    public string Bio { get; set; }
    public string Image { get; set; }
    public bool Following { get; set; }
}

public class NewArticle
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Body { get; set; } = "";
    public HashSet<string> TagList { get; set; } = new();
}

public class Comment
{
    public int Id { get; set; }

    public string Body { get; set; }

    public Profile Author { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class ArticleListFilter : ICloneable
{
    public FeedType FeedType { get; set; }
    public string? Tag { get; set; }
    public string? Author { get; set; }
    public string? Favorited { get; set; }
    public int Limit { get; set; } = 10;
    public int Offset { get; set; }

    public object Clone()
    {
        var serialized = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<ArticleListFilter>(serialized);
    }
}

public class ArticleList
{
    public List<Article> Articles { get; set; }

    public int ArticlesCount { get; set; }
}

public class Article
{
    public string Slug { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Body { get; set; }

    public string[] TagList { get; set; }

    public bool Favorited { get; set; }
    public int FavoritesCount { get; set; }
    public Profile Author { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public class GetTagsResponse
{
    public string[]? Tags { get; set; }
}

public class LoginRequest
{
    public Login? User { get; set; }
}

public class LoginResponse
{
    public User? User { get; set; }
}

public class ArticleObject
{
    public Article? Article { get; set; }
}

public class NewArticleRequest
{
    public NewArticle? Article { get; set; }
}

public class CommentsResponse
{
    public List<Comment>? Comments { get; set; }
}

public class ProfileResponse
{
    public Profile? Profile { get; set; }
}

public class UserResponse
{
    public User? User { get; set; }
}

public class UserUpdateRequest
{
    public User? User { get; set; }
}

public record NewUserRequest(NewUser User);

public class ErrorResponse
{
    public Dictionary<string, string[]>? Errors { get; set; }
}