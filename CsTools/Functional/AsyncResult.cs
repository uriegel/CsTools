using CsTools.Async;
using CsTools.Extensions;
using CsTools.Functional;

namespace CsTools.Functional
{
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

        /// <summary>
        /// Performs a side effect on the OK value (if present)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="sideEffect">Function performing side effect on the OK value</param>
        /// <returns></returns>
        public static AsyncResult<T, TE> SideEffectWhenOk<T, TE>(this AsyncResult<T, TE> result, Action<T> sideEffect)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> SideEffect()
            {
                var r = await result.resultTask;
                return r.SideEffectWhenOk(sideEffect);
            }
            return new(SideEffect());
        }

        /// <summary>
        /// Performs a side effect on the OK value (if present)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="sideEffect">Function performing side effect on the OK value</param>
        /// <returns></returns>
        public static AsyncResult<T, TE> SideEffectWhenOkAwait<T, TE>(this AsyncResult<T, TE> result, Func<T, Task> sideEffect)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> SideEffect()
            {
                var r = await result.resultTask;
                return await r.SideEffectWhenOkAsync(sideEffect);
            }
            return new(SideEffect());
        }

        /// <summary>
        /// Performs a sideeffect on the Error value (if present)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="sideEffect">Function performing side effect on the Error value</param>
        /// <returns></returns>
        public static AsyncResult<T, TE> SideEffectWhenError<T, TE>(this AsyncResult<T, TE> result, Action<TE> sideEffect)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> SideEffect()
            {
                var r = await result.resultTask;
                return r.SideEffectWhenError(sideEffect);
            }
            return new(SideEffect());
        }

        /// <summary>
        /// Performs a sideeffect on the Error value (if present)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="sideEffect">Function performing side effect on the Error value</param>
        /// <returns></returns>
        public static AsyncResult<T, TE> SideEffectWhenErrorAwait<T, TE>(this AsyncResult<T, TE> result, Func<TE, Task> sideEffect)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> SideEffect()
            {
                var r = await result.resultTask;
                return await r.SideEffectWhenErrorAsync(sideEffect);
            }
            return new(SideEffect());
        }

        public static async Task<T> GetOrThrowAsync<T, TE>(this AsyncResult<T, TE> result)
            where T : notnull
            where TE : Exception
            => (await result
                .ToResult())
                .Match(ok => ok, error => throw error);
    }
}

namespace CsTools
{
    public static partial class Core
    {
        public static AsyncResult<T, TE> RepeatOnError<T, TE>(Func<AsyncResult<T, TE>> func, int repeatCount, TimeSpan waitTime)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> RepeatOnError()
            {
                var i = 0;
                while (true)
                {
                    var res = await func().ToResult();
                    if (!res.IsError)
                        return res;
                    if (repeatCount == i)
                        return res;
                    await Task.Delay(waitTime);
                    i++;
                }
            }
            return RepeatOnError().ToAsyncResult();
        }

        public static AsyncResult<T, TE> RetryOnError<T, TE>(Func<AsyncResult<T, TE>> func, Func<TE, bool> retry)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> RetryOnError()
            {
                var res = await func().ToResult();
                if (res.IsError && res.Error != null && retry(res.Error))
                    return await func().ToResult();
                else
                    return res;
            }
            return RetryOnError().ToAsyncResult();
        }

        public static AsyncResult<T, Exception> TryAwait<T>(Func<Task<T>> func)
            where T : notnull
        {
            async Task<Result<T, Exception>> GetResult()
            {
                try
                {
                    return Ok<T, Exception>(await func());
                }
                catch (Exception e)
                {
                    return Error<T, Exception>(e);
                }
            }

            return GetResult().ToAsyncResult();
        }

        public static AsyncResult<TR, Exception> TryAwait<TR, T>(Func<T, Task<TR>> func, T t)
            where TR : notnull
            => TryAwait(() => func(t), e => e);

        public static AsyncResult<TR, Exception> TryAwait<TR, T1, T2>(Func<T1, T2, Task<TR>> func, T1 t1, T2 t2)
            where TR : notnull
            => TryAwait(() => func(t1, t2), e => e);

        public static AsyncResult<TR, Exception> TryAwait<TR, T1, T2, T3>(Func<T1, T2, T3, Task<TR>> func, T1 t1, T2 t2, T3 t3)
            where TR : notnull
            => TryAwait(() => func(t1, t2, t3), e => e);

        public static AsyncResult<TR, Exception> TryAwait<TR, T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task<TR>> func, T1 t1, T2 t2, T3 t3, T4 t4)
            where TR : notnull
            => TryAwait(() => func(t1, t2, t3, t4), e => e);

        public static AsyncResult<T, TE> TryAwait<T, TE>(Func<Task<T>> func, Func<Exception, TE> onException)
            where T : notnull
            where TE : notnull
        {
            async Task<Result<T, TE>> GetResult()
            {
                try
                {
                    return Ok<T, TE>(await func());
                }
                catch (Exception e)
                {
                    return Error<T, TE>(onException(e));
                }
            }

            return GetResult().ToAsyncResult();
        }
    }
}