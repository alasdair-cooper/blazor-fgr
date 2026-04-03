namespace BlazorFgr.Core.Primitives;

public interface IGettable<out T> 
{
    public T Value { get; }
}