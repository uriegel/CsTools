using LinqTools;

using static LinqTools.Core;

namespace CsTools.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        => dictionary.TryGetValue(key, out var result) ? result : defaultValue;

    public static Option<V> GetValue<K, V>(this IDictionary<K, V> dictionary, K key)
        where V : notnull
    => dictionary.TryGetValue(key, out var value)
        ? value != null
            ? Some(value)
            : None
        : None;
}
