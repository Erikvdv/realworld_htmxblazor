using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealworldBlazorHtmx.App.Components;

namespace UnitTests;

internal class BlazorRenderer : IAsyncDisposable
{
    private readonly HtmlRenderer _htmlRenderer;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ServiceProvider _serviceProvider;

    public BlazorRenderer()
    {
        // Build all the dependencies for the HtmlRenderer
        var services = new ServiceCollection();
        services.AddLogging();
        _serviceProvider = services.BuildServiceProvider();
        _loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        _htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);
    }

    // Dispose the services and DI container we created
    public async ValueTask DisposeAsync()
    {
        await _htmlRenderer.DisposeAsync();
        _loggerFactory.Dispose();
        await _serviceProvider.DisposeAsync();
    }

    // The other public methods are identical
    public Task<string> RenderComponent<T>() where T : IComponent
    {
        return RenderComponent<T>(ParameterView.Empty);
    }

    public Task<string> RenderComponent<T>(Dictionary<string, object?> dictionary) where T : IComponent
    {
        return RenderComponent<T>(ParameterView.FromDictionary(dictionary));
    }

    public Task<string> RenderComponent<T>(BaseComponentProps model) where T : IComponent
    {
        var dictionary = new Dictionary<string, object>
        {
            {nameof(BaseComponent<BaseComponentProps>.Props), model}
        };

        return RenderComponent<T>(ParameterView.FromDictionary(dictionary));
    }

    private Task<string> RenderComponent<T>(ParameterView parameters) where T : IComponent
    {
        return _htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await _htmlRenderer.RenderComponentAsync<T>(parameters);
            return output.ToHtmlString();
        });
    }
}