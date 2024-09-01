using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RealworldBlazorHtmx.App.Components;


public abstract class BaseComponent<T> : ComponentBase where T : notnull
{
    [Parameter] public T Data { get; set; } = default!;

    public RazorComponentResult GetResult(T props)
    {
        var parameters = new Dictionary<string, object> {{nameof(Data), props}};
        return new RazorComponentResult(GetType(), parameters!);
    }

    public RenderFragment GetFragment(T props)
    {
        var component = new RenderFragment(builder =>
        {
            builder.OpenComponent(0, GetType());
            builder.AddAttribute(1, nameof(Data), props);
            builder.CloseComponent();
        });
        return component;
    }
}