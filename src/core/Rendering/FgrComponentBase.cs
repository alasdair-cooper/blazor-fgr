using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Rendering;

public abstract class FgrComponentBase : FgrScope
{
    private bool _hasInitialized;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        
        if (_hasInitialized)
        {
            return;
        }

        await OnInitializedAsync();

        _hasInitialized = true;
    }

    protected virtual Task OnInitializedAsync() => Task.CompletedTask;
}