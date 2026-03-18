using Dunet;

namespace BlazorFgr.Core.Primitives.Resource;

[Union]
public partial record ResourceValue<T>
{
    public partial record Valid(T Value);

    public partial record Loading;
    
    public partial record Errored(Exception Error);
}