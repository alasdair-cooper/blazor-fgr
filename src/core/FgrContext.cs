using BlazorFgr.Core.Primitives;

namespace BlazorFgr.Core;

internal static class FgrContext
{
    public static readonly AsyncLocal<IDependent?> Current = new();
}