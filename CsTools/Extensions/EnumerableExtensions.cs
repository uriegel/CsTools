namespace CsTools.Extensions;

/// <summary>
/// Extension class for <see cref="System.Collections.Generic.IEnumerable{T}"/>
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// This can be used to make an enumerable unique. The criterion is the result of a lambda expression that is
    /// is passed as a parameter
    /// </summary>
    /// <typeparam name="TSource">Type of the enumerable</typeparam>
    /// <typeparam name="TKey">Criterion type</typeparam>
    /// <param name="source">Enumerable to be made unique, please call as extension method (source.DistinctBy)!</param>
    /// <param name="keySelector">Selector to create the criterion</param>
    /// <returns>The enumerable made unique</returns>
    internal static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
                yield return element;
        }
    }

    /// <summary>
    /// Functional ForEach, returning void
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sourceList"></param>
    /// <param name="action"></param>
    public static void ForEach<TSource>(this IEnumerable<TSource> sourceList, Action<TSource> action)
    {
        foreach (var source in sourceList)
            action(source);
    }

    /// <summary>
    /// Functional ForEach, returning Task till iteration finished
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="sourceList"></param>
    /// <param name="action"></param>
    public static async Task ForEachAsync<TSource>(this IEnumerable<TSource> sourceList, Func<TSource, Task> action)
    {
        foreach (var source in sourceList)
            await action(source);
    }

    /// <summary>
    /// Appends many Enumerables to one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <returns></returns> 
    public static IEnumerable<T> ConcatMany<T>(this IEnumerable<IEnumerable<T>> ts)
        => ts.Where(n => n != null).Aggregate(Enumerable.Empty<T>(), (p, c) => p.Concat(c)) ?? Enumerable.Empty<T>();

    /// <summary>
    /// Inserts the element "element" after each element from source.
    /// </summary>
    /// <typeparam name="T">Type of Elements</typeparam>
    /// <param name="source">Enumerable (Source)</param>
    /// <param name="element">element which is pushed between all elements of the source enumeration</param>
    /// <returns>The enumeration with every second element inserted</returns>
    /// <example>
    /// If the source { 1, 2, 3, 4 } is  interspersed with 0, you get { 1, 0, 2, 0, 3, 0 }
    /// </example>
    public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T element)
    {
        foreach (T value in source)
        {
            yield return value;
            yield return element;
        }
    }

    /// <summary>
    /// Transform a enumerable to an Enumerable with an array of fixed size of items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count">Size of Array</param>
    /// <returns></returns>
    public static IEnumerable<T[]> Windowed<T>(this IEnumerable<T> source, int count)
    {

        var enumerator = source.GetEnumerator();
        IEnumerable<T> getWindowed()
        {
            var index = 0;
            while (true)
            {
                if (index++ < count)
                {
                    if (!enumerator.MoveNext())
                        yield break;
                    var current = enumerator.Current;
                    yield return current;
                }
                else
                    yield break;
            }
        }

        while (true)
        {
            var res = getWindowed().ToArray();
            if (res.Length > 0)
                yield return res;
            else
                yield break;
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
    public static IEnumerable<T> SideEffectForAll<T>(this IEnumerable<T> source, Action<T> sideEffect)
    {
        return source.Select(performSideEffect);

        T performSideEffect(T element)
        {
            sideEffect(element);
            return element;
        }
    }

    /// <summary>
    /// A combination of select and where to simultaneously filter and convert using a selector that can return null. 
    /// Filtering and converting at the same time. The result does not contain null values.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <param name="selector">Filters by result == null and converts from type TSource to type TResult</param>
    /// <returns>Result-Enumerable</returns>
    public static IEnumerable<TResult> SelectFilterNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector)
        where TResult : class
        => (source
            .Select(selector)
            .Where(n => n != null) as IEnumerable<TResult>)!;

    /// <summary>
    /// Adds an item to an enumerable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static IEnumerable<T> Add<T>(this IEnumerable<T> enumerable, T t)
        => enumerable.Concat(new[] { t });
}

public static partial class Core
{
    /// <summary>
    /// Concatenates many Enumerables to one
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IEnumerable<T> ConcatEnumerables<T>(params IEnumerable<T>[] args)
    {
        var notNulls = args.Where(n => n != null);
        IEnumerable<T> concat(IEnumerable<T> e, IEnumerable<IEnumerable<T>> tail)
        {
            if (!tail.Any())
                return e ?? Enumerable.Empty<T>();
            else
                return e.Concat(concat(tail.FirstOrDefault()!, tail.Where((n, i) => i > 0)));
        }
        return concat(notNulls.FirstOrDefault()!, notNulls.Where((n, i) => i > 0));
    }
}

