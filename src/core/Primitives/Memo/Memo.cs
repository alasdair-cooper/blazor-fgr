namespace BlazorFgr.Core.Primitives.Memo;

public class Memo<T> : ISource, IDependent, IGettable<T>
{
    private MemoValue<T> _value = new MemoValue<T>.Dirty();

    private readonly HashSet<ISource> _sources = [];
    private readonly HashSet<IDependent> _subscribers = [];
    private readonly Func<T> _func;

    public Memo(Func<T> func)
    {
        _func = func;
        
        Recompute();
    }

    public T Get()
    {
        FgrContext.Current.Value?.Register(this);

        return _value switch
            {
                MemoValue<T>.Valid(var value) => value,
                _ => Recompute()
            };
    }

    private T Recompute()
    {
        foreach (var source in _sources.ToList())
        {
            source.Unsubscribe(this);
        }
        
        _sources.Clear();
        
        var prev = FgrContext.Current.Value;
        FgrContext.Current.Value = this;

        var val = _func();

        FgrContext.Current.Value = prev;

        _value = val;

        return val;
    }
    
    public void Subscribe(IDependent dependent) => _subscribers.Add(dependent);

    public void Unsubscribe(IDependent dependent) => _subscribers.Remove(dependent);

    public void Register(ISource source)
    {
        if (_sources.Add(source))
        {
            source.Subscribe(this);
        }
    }

    public void Invalidate()
    {
        if (_value is MemoValue<T>.Dirty) return;

        _value = new MemoValue<T>.Dirty();

        foreach (var subscriber in _subscribers.ToList()) subscriber.Invalidate();
    }
}