using System.Collections.Immutable;

namespace CsTools.Extensions;

public static class ImmutableDictionaryExtensions
{
    public static ImmutableDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this ImmutableDictionary<TKey, TValue> dictionary, TKey key, Func<TValue?, TValue> updateAction) 
        where TValue : class
        where TKey: notnull
        => dictionary?.ContainsKey(key) == true
            ? dictionary.SetItem(key, updateAction(dictionary[key]))
            : (dictionary ?? ImmutableDictionary<TKey, TValue>.Empty).SetItem(key, updateAction(null));

    public static ImmutableDictionary<TKey, TValue> SetItem<TKey, TValue>(this ImmutableDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> setAction)
        where TValue : class
        where TKey : notnull
        => dictionary?.ContainsKey(key) == true
            ? dictionary.SetItem(key, setAction(dictionary[key]))
            : dictionary ?? ImmutableDictionary<TKey, TValue>.Empty;
}
