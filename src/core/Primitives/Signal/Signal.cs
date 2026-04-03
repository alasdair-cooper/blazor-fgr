using BlazorFgr.Core.Rendering;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Primitives.Signal;

public class Signal<T>(T value) : ISource, IGettable<T> where T : IEquatable<T>
{
    private T _value = value;
    private readonly HashSet<IDependent> _subscribers = [];

    public T Value
    {
        get
        {
            FgrContext.Register(this);
        
            return _value;
        }
        set
        {
            if (value.Equals(_value))
            {
                return;
            }

            _value = value;

            foreach (var subscriber in _subscribers.ToList())
            {
                subscriber.Invalidate();
            }
        }
    }

    public void Update(Func<T, T> updater) => Value = updater(Value);
    
    public void Subscribe(IDependent dependent) => _subscribers.Add(dependent);

    public void Unsubscribe(IDependent dependent) => _subscribers.Remove(dependent);
    
    public static implicit operator RenderFragment(Signal<T> signal) => FgrView<T>.FromGettable(signal);
}
