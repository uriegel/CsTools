namespace CsTools.Functional;

public static class NullableExtensions
{
    public static TR Match<T, TR>(this T? t, Func<T, TR> someFunc, Func<TR> noneFunc)
        => t != null
            ? someFunc(t)
            : noneFunc();

    public static T GetOrDefault<T>(this T? t, T defaultValue)
        where T : class
        => t ?? defaultValue;

    public static T GetOrDefault<T>(this T? t, T defaultValue)
        where T : struct
        => t.HasValue
            ? t.Value
            : defaultValue;
}
