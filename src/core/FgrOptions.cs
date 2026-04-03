using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core;

public class FgrOptions
{
    public bool AreDiagnosticsEnabled { get; set; }

    public RenderFragment DiagnosticsPopoverContent { get; set; } = FgrDefaults.RenderFragments.DiagnosticsPopover;
    
    public string? DiagnosticsAnchorAdditionalClass { get; set; } 

    public void SetDiagnosticsPopoverContent<TComponent>() where TComponent : IComponent =>
        DiagnosticsPopoverContent = builder =>
        {
            builder.OpenComponent<TComponent>(0);
            builder.CloseComponent();
        };
}