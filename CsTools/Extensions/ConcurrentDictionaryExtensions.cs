using System.Collections.Concurrent;

namespace CsTools.Extensions;

public static class ConcurrentDictionaryExtensions
{
    public static TValue AddOrUpdateLocked<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, 
        TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        where TKey: notnull
    {
        lock (addOrUpdateLocker)
        {
            return dictionary.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }
    }

    static readonly object addOrUpdateLocker = new();
}