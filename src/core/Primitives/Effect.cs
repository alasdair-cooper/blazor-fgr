namespace BlazorFgr.Core.Primitives;

public class Effect(Func<ValueTask> action, Dispatcher dispatcher)
{
    private static readonly Dispatcher DefaultDispatcher = x => Task.Run(x);

    public static Effect Create(Action action) =>
        new(
            () =>
            {
                action();
                return ValueTask.CompletedTask;
            },
            DefaultDispatcher);

    public static Effect Create(Func<Task> action) => new(async () => { await action(); }, DefaultDispatcher);

    public void Schedule() => dispatcher(async () =>
    {
        AppContext.CurrentEffect.Value = this;
        await action();
        AppContext.CurrentEffect.Value = null;
    });
}