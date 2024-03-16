public static class NullableExtensions
{
    public static TR Match<T, TR>(this T? t, Func<T, TR> someFunc, Func<TR> noneFunc)
        => t != null
            ? someFunc(t)
            : noneFunc();

}