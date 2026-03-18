namespace BlazorFgr.Core.Primitives;

public interface ISource
{
    public void Subscribe(IDependent dependent);
    
    public void Unsubscribe(IDependent dependent);
}

public interface IGettable<out T> 
{
    public T Get();
}