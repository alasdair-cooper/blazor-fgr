using BlazorFgr.Core.Rendering;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Primitives.Resource;

public class Resource<TSource, TResult> : ISource, IDependent, IGettable<ResourceValue<TResult>> where TSource : IEquatable<TSource>
{
    private readonly HashSet<ISource> _sources = [];
    private readonly HashSet<IDependent> _subscribers = [];

    private ResourceValue<TResult> _value = new ResourceValue<TResult>.Loading();
    private Option<TSource> _currentSource = new Option<TSource>.None();
    private readonly Func<TSource> _sourceFunc;
    private readonly Func<TSource, ValueTask<TResult>> _loadFunc;

    public Resource(Func<TSource> sourceFunc, Func<TSource, ValueTask<TResult>> loadFunc)
    {
        _sourceFunc = sourceFunc;
        _loadFunc = loadFunc;

        Recompute();
    }

    public ResourceValue<TResult> Get()
    {
        FgrContext.Current.Value?.Register(this);

        return _value;
    }
    
    private void Recompute()
    {
        foreach (var source in _sources.ToList())
        {
            source.Unsubscribe(this);
        }
        
        _sources.Clear();
        
        var prev = FgrContext.Current.Value;
        FgrContext.Current.Value = this;

        var val = _sourceFunc();

        FgrContext.Current.Value = prev;

        if (_currentSource is Option<TSource>.Some(var current) && current.Equals(val)) return;
        
        _currentSource = val;
        
        Load(val);
    }

    private async void Load(TSource source)
    {
        try
        {
            _value = new ResourceValue<TResult>.Loading();
            Notify();
            _value = await _loadFunc(source).ConfigureAwait(false);
            Notify();
        }
        catch (Exception ex)
        {
            _value = new ResourceValue<TResult>.Errored(ex);
            Notify();
        }
    }

    private void Notify()
    {
        foreach (var sub in _subscribers.ToList()) sub.Invalidate();
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
        Recompute();

        foreach (var subscriber in _subscribers.ToList()) subscriber.Invalidate();
    }
    
    public static implicit operator RenderFragment(Resource<TSource, TResult> resource) => FgrView<ResourceValue<TResult>>.FromGettable(resource);
}