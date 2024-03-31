using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RealworldBlazorHtmx.App.Components;

public abstract record BaseComponentProps;

public abstract class BaseComponent<T> : ComponentBase where T : BaseComponentProps
{
    [Parameter] public T Props { get; set; } = default!;

    public RazorComponentResult GetRazorComponentResult(T props)
    {
        var parameters = new Dictionary<string, object> {{nameof(BaseComponent<BaseComponentProps>.Props), props}};
        return new RazorComponentResult(GetType(), parameters!);
    }

    public RenderFragment GetRenderFragment(T props)
    {
        var component = new RenderFragment(builder =>
        {
            builder.OpenComponent(0, GetType());
            builder.AddAttribute(1, nameof(BaseComponent<BaseComponentProps>.Props), props);
            builder.CloseComponent();
        });
        return component;
    }
}