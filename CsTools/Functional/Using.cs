namespace CsTools.Functional;

/// <summary>
/// Extension to avoid using blocks
/// </summary>
public static class Using
{
    public static void Use<TDisposable>(this TDisposable disposable, Action<TDisposable> actionWithDisposable) 
        where TDisposable : IDisposable
    {
        using var toDispose = disposable;
        actionWithDisposable(toDispose);
    }

    public static TResult Use<TDisposable, TResult>(this TDisposable disposable, Func<TDisposable, TResult> actionWithDisposable)
        where TDisposable : IDisposable
    {
        using var toDispose = disposable;
        return actionWithDisposable(disposable);
    }

    public static async Task UseAsync<TDisposable>(this TDisposable disposable, Func<TDisposable, Task> actionWithDisposable)
        where TDisposable : IDisposable
    {
        using var toDispose = disposable;
        await actionWithDisposable(disposable);
    }

    public static async Task<TResult> UseAsync<TDisposable, TResult>(this TDisposable disposable, Func<TDisposable, Task<TResult>> actionWithDisposable)
        where TDisposable : IDisposable
    {
        using var toDispose = disposable;
        return await actionWithDisposable(disposable);
    }

    public static void Finally(this Action action, Action finallyAction)
    {
        try
        {
            action();
        }
        finally
        {
            finallyAction();
        }
    }
}
