using BlazorFgr.Core.Primitives;

namespace BlazorFgr.Core;

public static class AppContext
{
    public static readonly AsyncLocal<Effect?> CurrentEffect = new();
}