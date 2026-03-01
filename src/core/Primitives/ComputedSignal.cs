namespace BlazorFgr.Core.Primitives;

public class ComputedSignal<T> : Signal<T>
{
    public ComputedSignal(Func<ValueTask<T>> compute, T initialValue) : base(initialValue)
    {
        var effect = Effect.Create(async () => Set(await compute()));
        effect.Schedule();
    }
}