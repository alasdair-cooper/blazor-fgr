using BlazorFgr.Core.Utilities.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core;

public class FgrOptions
{
    public bool AreDiagnosticsEnabled { get; set; }

    public RenderFragment<FgrDiagnostics> DiagnosticsPopoverContent { get; set; } = FgrDiagnosticsPopover.DefaultDiagnosticsContent;
    
    public RenderFragment DiagnosticsPopoverShowButtonContent { get; set; } = FgrDiagnosticsPopoverShowButton.DefaultContent;
}