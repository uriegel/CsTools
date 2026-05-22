using CsTools.Functional;

namespace CsTools;

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

    public static async Task<T> RepeatOnError<T>(Func<Task<T>> func, RepeatOnErrorOptions? options = null)
    {
        var i = 0;
        while (true)
        {
            try
            {
                return await func();
            }
            catch 
            {
                if (options?.RepeatCount == i)
                    throw;
                if (options?.WaitTime.HasValue == true)
                    await Task.Delay(options.WaitTime.Value);
            }
            i++;
        }
    }

    public record RepeatOnErrorOptions(int RepeatCount = 1, TimeSpan? WaitTime = null);
}
