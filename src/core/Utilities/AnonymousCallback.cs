using Microsoft.JSInterop;

namespace BlazorFgr.Core.Utilities;

internal class AnonymousCallback(Func<ValueTask> callback)
{
    [JSInvokable("call")]
    public ValueTask Invoke() => callback();
}