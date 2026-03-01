using BlazorFgr.Core.Rendering;
using Microsoft.AspNetCore.Components;

namespace BlazorFgr.Core.Primitives;

public class Signal<T>
{
    private T _value;
    private readonly HashSet<Effect> _subs = [];

    public Signal(T value) => _value = value;

    public T Get()
    {
        var effect = AppContext.CurrentEffect.Value;
        if (effect is not null) _subs.Add(effect);

        return _value;
    }

    public void Set(T value)
    {
        _value = value;

        foreach (var sub in _subs) sub.Schedule();
    }
    
    public void Update(Func<T, T> updater) => Set(updater(Get()));

    public static implicit operator RenderFragment(Signal<T> signal) => SignalView<T>.FromSignal(signal);
}

public static class Signal
{
    public static Signal<T> Create<T>(T value) => new(value);

    public static Signal<T> Create<T>(Func<T> compute, T initialValue = default!) => new ComputedSignal<T>(() => ValueTask.FromResult(compute()), initialValue);

    public static Signal<T> Create<T>(Func<ValueTask<T>> compute, T initialValue = default!) => new ComputedSignal<T>(compute, initialValue);
}