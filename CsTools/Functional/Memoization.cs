using System.Collections.Immutable;
using LinqTools;

namespace CsTools.Functional;

/// <summary>
/// Functions for caching function return values. Caching can be reset with the help of the resetter
/// </summary>
public static class Memoization
{
	public static Func<T> Memoize<T>(Func<T> functionToMemoize, Resetter? resetter = null) 
		where T : notnull
	{
		var refCell = new RefCell<(T value, bool valid)>();
		resetter?.SetResetAction(() => refCell.Value.valid = false);
		var locker = new object();
		return () =>
		{
            if (refCell.Value.valid)
                return refCell.Value.value;
			else
			{
				lock (locker)
				{
                    if (refCell.Value.valid)
                        return refCell.Value.value;
                    else
                    {
						refCell.Value.value = functionToMemoize();
                        refCell.Value.valid = true;
                        return refCell.Value.value;
					}
				}
			}
		};
	}

    public static Func<string, TResult?> Memoize<TResult>(Func<string, TResult> functionToMemoize,
		bool caseInsensitive, Resetter? resetter = null, Func<string[]>? getIDs = null)
        where TResult : class
    => Memoize(caseInsensitive
            ? ImmutableDictionary<string, RefCell<(TResult value, bool valid)>>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase)
            : ImmutableDictionary<string, RefCell<(TResult value, bool valid)>>.Empty,
        (a, b) => string.Compare(a, b, caseInsensitive) == 0,
        (k, ov) => functionToMemoize(k), resetter, getIDs);

    public static Func<string, TResult?> Memoize<TResult>(Func<string, TResult?, TResult> functionToMemoize,
		bool caseInsensitive, Resetter? resetter = null, Func<string[]>? getIDs = null)
        where TResult : class
    => Memoize(caseInsensitive
            ? ImmutableDictionary<string, RefCell<(TResult value, bool valid)>>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase)
            : ImmutableDictionary<string, RefCell<(TResult value, bool valid)>>.Empty,
        (a, b) => string.Compare(a, b, caseInsensitive) == 0,
        functionToMemoize, resetter, getIDs);

    public static Func<Task<T>> MemoizeAsync<T>(Func<Task<T>> functionToMemoize, Resetter? resetter = null) 
		where T : notnull
        => MemoizeAsync((T? oldValue) => functionToMemoize(), resetter)!;

	public static Func<Task<T>> MemoizeAsync<T>(Func<T?, Task<T>> functionToMemoize, Resetter? resetter = null) 
		where T : notnull
    {
        var refCell = new RefCell<(T? value, bool valid)>();
        resetter?.SetResetAction(() => refCell.Value.valid = false);
        var locker = new SemaphoreSlim(1, 1);
        return async () =>
        {
            if (refCell.Value.valid)
                return refCell.Value.value!;
            else
            {
                await locker.WaitAsync(); // Nachfolgende Threads warten hier auf das Release
                try
                {
                    if (refCell.Value.valid)
                        return refCell.Value.value!;
                    else
                    {
                        refCell.Value.value = await functionToMemoize(refCell.Value.value);
                        refCell.Value.valid = true;
                        return refCell.Value.value!;
                    }
                }
                finally
                {
                    locker.Release(); // Hier (am Besten in einem finally) releasen, damit der nächste Wartende aus dem obigen await raus kommt
                }
            }
        };
    }

	public static Func<TKey, Task<TResult?>> MemoizeAsync<TKey, TResult>(Func<TKey, TResult?, Task<TResult>> functionToMemoize, 
		Resetter? resetter = null, Func<Task<TKey[]>>? getIDsAsync = null) 
			where TKey : notnull
			where TResult : class
		=> MemoizeAsync(ImmutableDictionary<TKey, RefCell<(TResult value, bool valid)>>.Empty, 
			(a, b) => a.Equals(b), functionToMemoize, resetter, getIDsAsync);

	public static Func<string, Task<TResult?>> MemoizeAsync<TResult>(Func<string, TResult?, Task<TResult>> functionToMemoize,
		bool caseInsensitive, Resetter? resetter = null, Func<Task<string[]>>? getIDsAsync = null)
			where TResult : class
		=> MemoizeAsync(caseInsensitive
            ? ImmutableDictionary<string, RefCell<(TResult value, bool valid)>>.Empty.WithComparers(StringComparer.InvariantCultureIgnoreCase)
            : ImmutableDictionary<string, RefCell<(TResult value, bool valid)>>.Empty, 
			(a, b) => string.Compare(a, b, caseInsensitive) == 0,
			functionToMemoize, resetter, getIDsAsync);

    static Func<TKey, Task<TResult?>> MemoizeAsync<TKey, TResult>(ImmutableDictionary<TKey, RefCell<(TResult value, bool valid)>> cache, 
		Func<TKey, TKey, bool> comparer,
		Func<TKey, TResult?, Task<TResult>> functionToMemoize, Resetter? resetter, Func<Task<TKey[]>>? getIDsAsync) 
			where TKey : notnull
			where TResult : class
	{
		var locker = new SemaphoreSlim(1, 1);
        var cacheDictionary = cache;
		resetter?.SetResetAction(() => cacheDictionary.Values.ForEach(v => v.Value.valid = false));

		async Task<TResult?> GetValueAsync(TKey id)
        {
			if (cacheDictionary.TryGetValue(id, out var result) && result.Value.valid)
				return result.Value.value;
            else
            {
				await locker.WaitAsync(); // Nachfolgende Threads warten hier auf das Release
				try
				{
					if (cacheDictionary.TryGetValue(id, out result) && result.Value.valid)
						return result.Value.value;

					var ids = getIDsAsync != null ? await getIDsAsync() : null;

					// TODO: nur einmal pro Reset-Aktion
					if (ids != null)
                    {
						foreach (var i in ids)
                        {
							if (cacheDictionary.ContainsKey(i))
								cacheDictionary = cacheDictionary.Remove(i);
						}
                    }

					var refcell = new RefCell<(TResult? value, bool)>((ids?.Any(n => comparer(n, id)) ?? true
						? await functionToMemoize(id, (cacheDictionary.TryGetValue(id, out result) ? result : null)?.Value.value)
						: null, true));

					cacheDictionary = cacheDictionary.SetItem(id, refcell!);
					return refcell.Value.value;
				}
				finally
				{
					locker.Release(); // Hier (am Besten in einem finally) releasen, damit der nächste Wartende aus dem obigen await raus kommt
				}
			}
		}
		return GetValueAsync;
	}

    static Func<TKey, TResult?> Memoize<TKey, TResult>(ImmutableDictionary<TKey, RefCell<(TResult value, bool valid)>> cache,
        Func<TKey, TKey, bool> comparer,
        Func<TKey, TResult?, TResult> functionToMemoize, Resetter? resetter, Func<TKey[]>? getIDs)
            where TKey : notnull
            where TResult : class
    {
        var locker = new object();
        var cacheDictionary = cache;
	    resetter?.SetResetAction(() => cacheDictionary.Values.ForEach(v => v.Value.valid = false));

        TResult? GetValue(TKey id)
        {
            if (cacheDictionary.TryGetValue(id, out var result) && result.Value.valid)
                return result.Value.value;
            else
            {
                lock (locker!)
                {
                    if (cacheDictionary.TryGetValue(id, out result) && result.Value.valid)
                        return result.Value.value;

                    var ids = getIDs != null ? getIDs() : null;

                    // TODO: nur einmal pro Reset-Aktion
                    if (ids != null)
                    {
                        foreach (var i in ids)
                        {
							if (cacheDictionary.ContainsKey(i))
								cacheDictionary = cacheDictionary.Remove(i);
                        }
                    }

                    var refcell = new RefCell<(TResult? value, bool)>((ids?.Any(n => comparer(n, id)) ?? true
                        ? functionToMemoize(id, (cacheDictionary.TryGetValue(id, out result) ? result : null)?.Value.value)
                        : null, true));

					cacheDictionary = cacheDictionary.SetItem(id, refcell!);
                    return refcell.Value.value;
                }
            }
        }
        return GetValue;
    }

    public class Resetter
	{
		public Resetter() => action = null;
		public Resetter(Action action) => this.action = action;
		public void Reset() => action.ForEach(n => n());
		public void SetResetAction(Action action) => this.action = action;
		Action? action;
	}
}
