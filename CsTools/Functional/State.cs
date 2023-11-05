namespace CsTools.Functional;

public static class State
{
    public static Func<T> Use<T>(Func<T, T> change)
        where T : struct
    {
        T t = default;
        return () => t = change(t);
    }

    public static Func<T> Use<T>(Func<T, T> change, T seed)
        where T : class
    {
        T t = seed;
        return () => t = change(t);
    }
}
