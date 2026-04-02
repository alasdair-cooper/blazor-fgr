using BlazorFgr.Core.Rendering;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Primitives.Dispatcher;

public class Dispatcher<T>(Func<T, ValueTask> func) : ISource, IGettable<DispatcherValue>
{
    private DispatcherValue _value = new DispatcherValue.Sleeping();
    private readonly HashSet<IDependent> _subscribers = [];

    public async void Dispatch(T source)
    {
        try
        {
            _value = new DispatcherValue.Running();
            Notify();
            
            await func(source).ConfigureAwait(false);
            
            _value = new DispatcherValue.Sleeping();
            Notify();
        }
        catch (Exception ex)
        {
            _value = new DispatcherValue.Errored(ex);
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

    public DispatcherValue Get()
    {
        FgrContext.Register(this);
        
        return _value;
    }

    public void Subscribe(IDependent dependent) => _subscribers.Add(dependent);

    public void Unsubscribe(IDependent dependent) => _subscribers.Remove(dependent);
    
    public static implicit operator RenderFragment(Dispatcher<T> dispatcher) => FgrView<DispatcherValue>.FromGettable(dispatcher);
}