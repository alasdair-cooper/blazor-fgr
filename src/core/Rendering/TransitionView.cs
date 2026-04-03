using BlazorFgr.Core.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorFgr.Core.Rendering;

public class TransitionView : IComponent, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private RenderHandle _renderHandle;
    private bool _hasRendered;
    private IJSObjectReference? _libModuleRef;

    [Parameter]
    [EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    public TransitionView(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (_hasRendered)
        {
            await InvokeViewTransition(
                async () =>
                {
                    await _renderHandle.Dispatcher.InvokeAsync(() => _renderHandle.Render(ChildContent));
                    await Task.Yield();
                });
        }
        else
        {
            _renderHandle.Render(ChildContent);
        }

        _hasRendered = true;

        return;

        async Task InvokeViewTransition(Func<ValueTask> callback)
        {
            _libModuleRef ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorFgr.Core/lib.js");

            using var callbackRef = DotNetObjectReference.Create(new AnonymousCallback(callback));

            await _libModuleRef.InvokeVoidAsync("viewTransition", callbackRef);
        }
    }


    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_libModuleRef is not null)
        {
            try
            {
                await _libModuleRef.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // ignore
            }
        }
    }
}