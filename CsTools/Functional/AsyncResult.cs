using System.Reflection.PortableExecutable;
using CsTools.Async;
using CsTools.Extensions;

namespace CsTools.Functional;

public readonly struct AsyncResult<T, TE> 
    where T : notnull
    where TE : notnull
{
    public async Task<Result<T, TE>> ToResult()
        => await resultTask;

    internal AsyncResult(Result<T, TE> value)
        => resultTask = value.ToAsync();

    internal AsyncResult(Task<Result<T, TE>> value)
        => resultTask = value;

    readonly internal Task<Result<T, TE>> resultTask;
}

public static class AsyncResultExtensions
{
    public static AsyncResult<T, TE> ToAsyncResult<T, TE>(this Result<T, TE> result)
        where T : notnull
        where TE : notnull
        => new(result);

    public static AsyncResult<T, TE> ToAsyncResult<T, TE>(this Task<Result<T, TE>> result)
        where T : notnull
        where TE : notnull
        => new(result);

    public static AsyncResult<TResult, TE> Select<TSource, TE, TResult>(this AsyncResult<TSource, TE> source, Func<TSource, TResult> func)
        where TSource : notnull
        where TE : notnull
        where TResult : notnull
    => new(from n in source.resultTask
           select n.Select(func));

    public static AsyncResult<T, TER> SelectError<T, TE, TER>(this AsyncResult<T, TE> source, Func<TE, TER> errorSelector)
        where T : notnull
        where TE : notnull
        where TER : notnull
    => new(from n in source.resultTask
           select n.SelectError(errorSelector));

    public static AsyncResult<TResult, TE> SelectAwait<TSource, TE, TResult>(this AsyncResult<TSource, TE> source, Func<TSource, Task<TResult>> selector)
        where TSource : notnull
        where TE : notnull
        where TResult : notnull
    => new((from n in source.resultTask
            select n.InternalSelectAwait(selector)).Unwrap());

    public static AsyncResult<TR, TE> Bind<T, TR, TE>(this AsyncResult<T, TE> source, Func<T, Result<TR, TE>> selector)
        where T : notnull
        where TR : notnull
        where TE : notnull
    {
        async Task<Result<TR, TE>> Bind()
        {
            var r = await source.resultTask;
            return Result<T, TE>.InternalBind(r, ok => selector(ok));
        }
        return new(Bind());
    }

    public static AsyncResult<TR, TE> BindAwait<T, TR, TE>(this AsyncResult<T, TE> source, Func<T, AsyncResult<TR, TE>> selector)
        where T : notnull
        where TR : notnull
        where TE : notnull
    {
        async Task<Result<TR, TE>> Bind()
        {
            var r = await source.resultTask;
            return await Result<T, TE>.InternalBindAwait(r, ok => selector(ok).ToResult());
        }
        return new(Bind());
    }

    public static AsyncResult<T, TE> BindError<T, TE>(this AsyncResult<T, TE> source, Func<TE, Result<T, TE>> selector)
        where T : notnull
        where TE : notnull

    {
        async Task<Result<T, TE>> Bind()
        {
            var r = await source.resultTask;
            return Result<T, TE>.InternalBindError(r, e => selector(e));
        }
        return new(Bind());
    }

    public static AsyncResult<T, TE> BindErrorAwait<T, TE>(this AsyncResult<T, TE> source, Func<TE, AsyncResult<T, TE>> selector)
        where T : notnull
        where TE : notnull

    {
        async Task<Result<T, TE>> Bind()
        {
            var r = await source.resultTask;
            return await Result<T, TE>.InternalBindErrorAwait(r, e => selector(e).ToResult());
        }
        return new(Bind());
    }
}


