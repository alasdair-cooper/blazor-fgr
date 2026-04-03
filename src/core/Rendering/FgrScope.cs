using BlazorFgr.Core.Primitives;
using BlazorFgr.Core.Utilities;
using BlazorFgr.Core.Utilities.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorFgr.Core.Rendering;

public abstract class FgrScope : IComponent, IDependent
{
    private readonly HashSet<ISource> _sources = [];
    private readonly RenderFragment _renderFragment;
    private RenderHandle _renderHandle;
    private bool _hasRenderedAtLeastOnce;
    private int _renderCount;

    protected FgrScope() =>
        _renderFragment =
            builder =>
            {
                _renderCount++;

                builder.OpenComponent<FgrDiagnosticsAnchor>(0);
                builder.AddAttribute(1, nameof(FgrDiagnosticsAnchor.Diagnostics), new FgrDiagnostics(_renderCount));
                builder.CloseComponent();
                
                foreach (var source in _sources)
                {
                    source.Unsubscribe(this);
                }

                _sources.Clear();

                using (FgrContext.CreateScope(this))
                {
                    BuildRenderTree(builder);
                }
            };

    public void Register(ISource source)
    {
        if (_sources.Add(source))
        {
            source.Subscribe(this);
        }
    }

    public void Invalidate() => Render();

    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (_hasRenderedAtLeastOnce)
        {
            return Task.CompletedTask;
        }
        
        _hasRenderedAtLeastOnce = true;
        
        Render();

        return Task.CompletedTask;
    }

    private void Render() => _renderHandle.Render(_renderFragment);
    
    protected virtual void BuildRenderTree(RenderTreeBuilder _) { }
}