namespace BlazorFgr.Core.Primitives;

public interface IGettable<out T> 
{
    public T Get();
}