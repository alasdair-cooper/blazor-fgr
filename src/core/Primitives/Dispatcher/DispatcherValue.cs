using Dunet;

namespace BlazorFgr.Core.Primitives.Dispatcher;

[Union]
public partial record DispatcherValue
{
    public partial record Running;

    public partial record Sleeping;

    public partial record Errored(Exception Error);
}