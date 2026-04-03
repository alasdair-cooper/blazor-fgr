using BlazorFgr.Core.Rendering;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Primitives.Resource;

public class Resource<TSource, TResult> : ISource, IDependent, IGettable<ResourceValue<TResult>>, IWaitable where TSource : IEquatable<TSource>
{
    private readonly HashSet<ISource> _sources = [];
    private readonly HashSet<IDependent> _subscribers = [];

    private ResourceValue<TResult> _value = new ResourceValue<TResult>.Loading();
    private Utilities.Option<TSource> _currentSource = new Utilities.Option<TSource>.None();
    private readonly Func<TSource> _sourceFunc;
    private readonly Func<TSource, ValueTask<TResult>> _loadFunc;

    public Resource(Func<TSource> sourceFunc, Func<TSource, ValueTask<TResult>> loadFunc)
    {
        _sourceFunc = sourceFunc;
        _loadFunc = loadFunc;

        Recompute();
    }

    public ResourceValue<TResult> Value
    {
        get
        {
            FgrContext.Register(this);

            return _value;
        }
    }

    private void Recompute()
    {
        foreach (var source in _sources.ToList())
        {
            source.Unsubscribe(this);
        }

        _sources.Clear();

        using (FgrContext.CreateScope(this))
        {
            var val = _sourceFunc();

            if (_currentSource is Utilities.Option<TSource>.Some(var current) && current.Equals(val)) return;

            _currentSource = val;

            Load(val);
        }
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
        foreach (var sub in _subscribers.ToList())
        {
            sub.Invalidate();
        }
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

        foreach (var subscriber in _subscribers.ToList())
        {
            subscriber.Invalidate();
        }
    }

    public static implicit operator RenderFragment(Resource<TSource, TResult> resource) => FgrView<ResourceValue<TResult>>.FromGettable(resource);

    public bool IsReady() => _value is ResourceValue<TResult>.Loaded;
}