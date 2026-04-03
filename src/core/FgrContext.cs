using BlazorFgr.Core.Primitives;
using BlazorFgr.Core.Utilities;

namespace BlazorFgr.Core;

internal static class FgrContext
{
    private static readonly AsyncLocal<IDependent?> Current = new();
    
    private static readonly AsyncLocal<IHost?> Host = new();
    
    public static IDisposable CreateScope(IDependent dependent)
    {
        var prev = Current.Value;
        
        Current.Value = dependent;
        
        return new AnonymousDisposable(() => Current.Value = prev);
    }

    public static IDisposable CreateHostScope(IHost host)
    {
        var prev = Host.Value;
        
        Host.Value = host;
        
        return new AnonymousDisposable(() => Host.Value = prev);
    }
    
    public static void Register(ISource source) => Current.Value?.Register(source);
    
    public static void Register<T>(T source) where T : ISource, IWaitable
    {
        Host.Value?.Register(source);
        Current.Value?.Register(source);
    }
}