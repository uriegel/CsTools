namespace CsTools.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        => dictionary.TryGetValue(key, out var result) ? result : defaultValue;

    public static V? GetValue<K, V>(this IDictionary<K, V> dictionary, K key)
        where V : class 
    => dictionary.TryGetValue(key, out var value)
        ? value != null
            ? value
            : default
        : default;

    public static V? TryGetValue<K, V>(this IDictionary<K, V> dictionary, K key)
        where V : struct 
    => dictionary.TryGetValue(key, out var value)
        ? value
        : null;
}
    