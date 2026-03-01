namespace BlazorFgr.Core;

public delegate void Dispatcher(Func<ValueTask> action);