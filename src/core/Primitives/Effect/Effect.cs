namespace BlazorFgr.Core.Primitives.Effect;

public class Effect(Action func) : IDependent
{
    private readonly HashSet<ISource> _sources = [];

    private void Run()
    {
        foreach (var source in _sources)
        {
            source.Unsubscribe(this);
        }

        _sources.Clear();

        using (FgrContext.CreateScope(this))
        {
            func();
        }
    }

    public void Register(ISource source)
    {
        if (_sources.Add(source))
        {
            source.Subscribe(this);
        }
    }

    public void Invalidate() => Run();
}