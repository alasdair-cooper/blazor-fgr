using BlazorFgr.Core.Primitives;
using BlazorFgr.Core.Utilities.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorFgr.Core.Rendering;

public abstract class FgrScope : IComponent, IDependent
{
    private readonly HashSet<ISource> _sources = [];
    private RenderFragment? _renderFragment;
    private RenderHandle _renderHandle;
    private bool _hasRenderedAtLeastOnce;
    private int _renderCount;


    public void Register(ISource source)
    {
        if (_sources.Add(source))
        {
            source.Subscribe(this);
        }
    }

    public async void Invalidate()
    {
        try
        {
            await Render();
        }
        catch (Exception ex)
        {
            await _renderHandle.DispatchExceptionAsync(ex);
        }
    }

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public virtual async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (_hasRenderedAtLeastOnce)
        {
            return;
        }

        _hasRenderedAtLeastOnce = true;

        await Render();
    }

    private async Task Render()
    {
        _renderFragment ??= RenderFragment;
        await _renderHandle.Dispatcher.InvokeAsync(() => _renderHandle.Render(_renderFragment));
    }

    protected virtual RenderFragment RenderFragment => builder =>
    {
        _renderCount++;

        foreach (var source in _sources)
        {
            source.Unsubscribe(this);
        }

        _sources.Clear();

        using (FgrContext.CreateScope(this))
        {
            BuildRenderTree(builder);
        }

        if (_renderHandle.RendererInfo.IsInteractive)
        {
            builder.OpenComponent<FgrDiagnosticsAnchor>(int.MaxValue - 1);
            builder.AddAttribute(int.MaxValue, nameof(FgrDiagnosticsAnchor.Diagnostics), () => new FgrDiagnostics(_renderCount, _sources.Count));
            builder.CloseComponent();
        }
    }; 

    protected virtual void BuildRenderTree(RenderTreeBuilder _) { }
}