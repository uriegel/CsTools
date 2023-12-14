namespace LinqTools.Functional;

public static class Conditional
{
    public static R If<R, T>(this T t, bool condition, Func<T, R> trueFunc, Func<T, R> falseFunc)
        => condition
            ? trueFunc(t)
            : falseFunc(t);

    public static T If<T>(this T t, bool condition, Func<T, T> func)
        => t.If(condition, func, t => t);
}