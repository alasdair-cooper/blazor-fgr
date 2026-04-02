namespace BlazorFgr.Core.Utilities;

internal class AnonymousDisposable(Action dispose) : IDisposable
{
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        dispose();
    }
}