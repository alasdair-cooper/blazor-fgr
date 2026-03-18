using Dunet;

namespace BlazorFgr.Core.Primitives.Memo;

[Union]
public partial record MemoValue<T>
{
    public partial record Valid(T Value);

    public partial record Dirty;
}