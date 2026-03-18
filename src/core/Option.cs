using Dunet;

namespace BlazorFgr.Core;

[Union]
internal partial record Option<T>
{
    public partial record Some(T Value);

    public partial record None;
}