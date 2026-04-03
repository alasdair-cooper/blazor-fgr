namespace BlazorFgr.Core.Primitives;

public interface IHost
{
    public void Register(IWaitable waitable);
}