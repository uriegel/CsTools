namespace CsTools.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        => dictionary.TryGetValue(key, out var result) ? result : defaultValue;

    // TODO test with string
    // TODO test with int
    // TODO this string? check ?!!!
    public static V? GetValue<K, V>(this IDictionary<K, V> dictionary, K key)
        where V : notnull
    => dictionary.TryGetValue(key, out var value)
        ? value != null
            ? value
            : default
        : default;
}
