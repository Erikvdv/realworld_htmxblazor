using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.HttpResults;

namespace RealworldBlazorHtmx.App.Components.Shared;

public static class RazorResult
{
    public static RazorComponentResult<T> GetComponent<T, TU>(BaseComponentProps? props)
        where T : BaseComponent<TU> where TU : BaseComponentProps
    {
        var parameters = new Dictionary<string, object>();
        if (props is not null)
            parameters.Add(nameof(BaseComponent<TU>.Props), props);
        return new RazorComponentResult<T>(parameters!);
    }
    
    public static RenderFragment GetComponentFragment<T>(BaseComponentProps? props) where T : IComponent
    {
        return builder =>
        {
            builder.OpenComponent<T>(0);
            builder.AddAttribute(1, nameof(BaseComponent<BaseComponentProps>.Props), props);
            builder.CloseComponent();
        };
    }
}