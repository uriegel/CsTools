namespace CsTools.Functional;

public static class Incrementor
{
    public static Func<int> UseInt()
        => State.Use<int>(i => Interlocked.Increment(ref i));

    public static Func<long> UseLong()
        => State.Use<long>(i => Interlocked.Increment(ref i));
}
