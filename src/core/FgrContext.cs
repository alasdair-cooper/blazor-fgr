using BlazorFgr.Core.Primitives;
using BlazorFgr.Core.Utilities;

namespace BlazorFgr.Core;

internal static class FgrContext
{
    private static readonly AsyncLocal<IDependent?> Current = new();
    
    public static IDisposable CreateScope(IDependent dependent)
    {
        var prev = Current.Value;
        
        Current.Value = dependent;
        
        return new AnonymousDisposable(() => Current.Value = prev);
    }
    
    public static void Register(ISource source) => Current.Value?.Register(source);
}