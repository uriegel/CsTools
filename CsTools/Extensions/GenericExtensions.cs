namespace CsTools.Extensions;
public static class GenericExtensions
{
    /// <summary>
    /// This SideEffect is useful if you want to apply some side effect to a value in a LINQ expression,
    /// such as logging the value. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T SideEffect<T>(this T t, Action<T> action)
    {
        action(t);
        return t;
    }

    /// <summary>
    /// This SideEffect is useful if you want to apply some side effect to a value in a LINQ expression,
    /// such as logging the value. The value is waitable and will be awaited to apply the side effect
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="sideEffect"></param>
    /// <returns></returns>
    public static async Task<T> SideEffectAsync<T>(this Task<T> value, Action<T> sideEffect)
    {
        var val = await value;
        sideEffect(val);
        return val;
    }

    /// <summary>
    /// This SideEffect is useful if you want to apply some awaitable side effect to a value in a LINQ expression,
    /// such as logging the value. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static async Task<T> SideEffect<T>(this T t, Func<T, Task> selector)
    {
        await selector(t);
        return t;
    }

    /// <summary>
    /// This SideEffect is useful if you want to apply some side effect to a value in a LINQ expression,
    /// such as logging the value. The sideEffect is only processed if the condition is true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="condition">Run the action if true</param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T SideEffectIf<T>(this T t, bool condition, Action<T> action)
    {
        if (condition)
            action(t);
        return t;
    }

    /// <summary>
    /// This SideEffect is useful if you want to apply some side effect to a value in a LINQ expression,
    /// such as logging the value. The trueEffect is processed if the condition is true, otherwise falseAction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="condition">Run the action if true</param>
    /// <param name="trueAction"></param>
    /// <param name="falseAction"></param>
    /// <returns></returns>
    public static T SideEffectChoose<T>(this T t, bool condition, Action<T> trueAction, Action<T> falseAction)
    {
        if (condition)
            trueAction(t);
        else
            falseAction(t);
        return t;
    }

    /// <summary>
    /// This SideEffect is useful if you want to apply some side effect to a value in a LINQ expression,
    /// such as logging the value. The sideEffect is only processed if the condition is true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="getCondition">Run the action if true</param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static T SideEffectIf<T>(this T t, Func<T, bool> getCondition, Action<T> action)
    {
        if (getCondition(t))
            action(t);
        return t;
    }

    /// <summary>
    /// This SideEffect is useful if you want to apply some side effect to a value in a LINQ expression,
    /// such as logging the value. The trueEffect is processed if the condition is true, otherwise falseAction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="getCocondition">Run the action if true</param>
    /// <param name="trueAction"></param>
    /// <param name="falseAction"></param>
    /// <returns></returns>
    public static T SideEffectChoose<T>(this T t, Func<T, bool> getCocondition, Action<T> trueAction, Action<T> falseAction)
    {
        if (getCocondition(t))
            trueAction(t);
        else
            falseAction(t);
        return t;
    }

    /// <summary>
    /// If you need to an awaitable value, and you have only a normal value, you can create an 
    /// Async monad with this extension function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Task<T> ToAsync<T>(this T t) => Task.FromResult(t);

    /// <summary>
    /// If you want to ignore some function result, you can apply this method. It returns void.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_"></param>
    public static void ToVoid<T>(this T _) { }

    /// <summary>
    /// Take the result from one operation and use it on another
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="t"></param>
    /// <param name="selector"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public static TResult With<T, R, TResult>(this T t, Func<T, R> selector, Func<R, TResult> resultSelector)
        => resultSelector(selector(t));

    /// <summary>
    /// Take a value of T and perform a function resulting TR
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <param name="t"></param>
    /// <param name="adapter"></param>
    /// <returns></returns>
    [Obsolete("please use Pipe")]
    public static TR Adapt<T, TR>(this T t, Func<T, TR> adapter)
        => adapter(t);

    /// <summary>
    /// Take a value of T and perform a function resulting TR
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <param name="t"></param>
    /// <param name="adapter"></param>
    /// <returns></returns>
    public static TR Pipe<T, TR>(this T t, Func<T, TR> adapter)
        => adapter(t);

    public static bool And<T>(this T t, params Func<T, bool>[] predicates)
        => predicates.Aggregate(true, (acc, predicate) => acc && predicate(t)); 

    public static bool Or<T>(this T t, params Func<T, bool>[] predicates)
        => predicates.Aggregate(true, (acc, predicate) => acc || predicate(t)); 
}
