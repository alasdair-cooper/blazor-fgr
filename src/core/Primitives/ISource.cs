namespace BlazorFgr.Core.Primitives;

public interface ISource
{
    public void Subscribe(IDependent dependent);
    
    public void Unsubscribe(IDependent dependent);
}