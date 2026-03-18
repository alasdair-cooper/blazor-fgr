namespace BlazorFgr.Core.Primitives;

public interface IDependent
{
    public void Register(ISource source);
    
    public void Invalidate();
}