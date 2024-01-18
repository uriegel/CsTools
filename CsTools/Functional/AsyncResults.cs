namespace CsTools.Functional;

public class AsyncResults<T, TE>(IEnumerable<AsyncResult<T, TE>> asyncResults)
    where T: notnull
    where TE: notnull
{
    public async Task<IEnumerable<Result<T, TE>>> Collect()
        => await Task.WhenAll(results.Select(n => n.ToResult()).ToArray());
    
    readonly IEnumerable<AsyncResult<T, TE>> results = asyncResults;    
}

public static class AsyncResultsExtensions
{
    public static AsyncResults<T, TE> ToAsyncResults<T, TE>(this IEnumerable<AsyncResult<T, TE>> asyncResults)
        where T: notnull
        where TE: notnull
        => new(asyncResults);
}

