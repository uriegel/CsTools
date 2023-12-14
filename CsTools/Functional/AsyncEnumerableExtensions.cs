namespace CsTools.Functional;

public static class IAsyncEnumerableExtensions
{
    /// <summary>
    /// This extension method can be used to make an enumerable unique. The criterion is the result of a lambda expression 
    /// passed as a parameter
    /// </summary>
    /// <typeparam name="TSource">Type of enumerable</typeparam>
    /// <typeparam name="TKey">Typ of criterion</typeparam>
    /// <param name="source">Enumerable, which should be made unique, please call as extension method 'source.DistinctBy(...)'</param>
    /// <param name="keySelector">Function creating the criterion</param>
    /// <returns>Enumerable which is made distinct</returns>
    public async static IAsyncEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        await foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
                yield return element;
        }
    }

    public static async Task<T?> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> source, Func<T, Task<bool>> predicate)
    {
        await foreach (var element in source)
        {
            if (await predicate(element))
                return element;
        }
        return default;
    }

    /// <summary>
    /// Applies the sideEffect method to all elements of the source enumerable.
    /// Can be used for logging, among other things.        
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sideEffect"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<T> SideEffectForAll<T>(this IAsyncEnumerable<T> source, Action<T> sideEffect)
    {
        return source.Select(performSideEffect);

        T performSideEffect(T t)
        {
            sideEffect(t);
            return t;
        }
    }

    /// <summary>
    /// Applies the sideEffect method to all elements of the source enumerable.
    /// Can be used for logging, among other things.        
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sideEffect"></param>
    /// <returns></returns>
    public static IAsyncEnumerable<T> SideEffectForAll<T>(this IAsyncEnumerable<T> source, Func<T, Task> sideEffect)
    {
        return source.SelectAwait(performSideEffect);

        async ValueTask<T> performSideEffect(T t)
        {
            await sideEffect(t);
            return t;
        }
    }

    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this Task<T[]> asyncValues)
    {
        foreach (var val in await asyncValues)
            yield return val;
    }

    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this Task<IEnumerable<T>> asyncValues)
    {
        foreach (var val in await asyncValues)
            yield return val;
    }
}
