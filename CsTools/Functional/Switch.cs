namespace CsTools.Functional;

public static class ChooseExtensions
{
    public static TResult? Choose<TResult, T>(this T t, params SwitchType<T, TResult>[] switches)
        where T : notnull
        where TResult : class
        => switches
            .FirstOrDefault(s => s.Predicate(t))
            ?.Selector(t);
    public record SwitchType<T, TResult>(Predicate<T> Predicate, Func<T, TResult> Selector)
        where T : notnull
        where TResult : notnull
    {
        public static SwitchType<T, TResult> Switch(Predicate<T> predicate, Func<T, TResult> selector)
            => new(predicate, selector);
        public static SwitchType<T, TResult> Default(Func<T, TResult> selector)
            => new(_ => true, selector);
    };
}

