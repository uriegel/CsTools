using System.Text.Json.Serialization;
using CsTools.Functional;

using static CsTools.Core;

namespace CsTools.Functional
{
    /// <summary>
    /// Functional Type (Discriminated Union) containing either a T Ok type or a TE Error type
    /// </summary>
    /// <typeparam name="T">Typeof OK</typeparam>
    /// <typeparam name="TE">Typeof Error</typeparam>
    public readonly struct Result<T, TE>
        where T : notnull
        where TE : notnull
    {
        internal Result(T value)
        {
            IsError = false;
            Ok = value;
            Error = default;
        }

        internal Result(TE value)
        {
            IsError = true;
            Ok = default;
            Error = value;
        }

        public static implicit operator Result<T, TE>(T value)
            => Ok<T, TE>(value);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsError { get; init; }

        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="successFunc"></param>
        /// <param name="failFunc"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<T, TResult> successFunc, Func<TE, TResult> failFunc)
            => IsError == false
            ? successFunc(Ok!)
            : failFunc(Error!);

        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="successFunc"></param>
        /// <param name="failFunc"></param>
        /// <returns></returns>
        public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> successFunc, Func<TE, TResult> failFunc)
            => IsError == false
            ? await successFunc(Ok!)
            : failFunc(Error!);

        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="successFunc"></param>
        /// <param name="failFunc"></param>
        /// <returns></returns>
        public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> successFunc, Func<TE, Task<TResult>> failFunc)
            => IsError == false
            ? await successFunc(Ok!)
            : await failFunc(Error!);

        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <param name="successAction"></param>
        /// <param name="failAction"></param>
        public void Match(Action<T> successAction, Action<TE> failAction)
        {
            if (IsError == false)
                successAction(Ok!);
            else
                failAction(Error!);
        }

        internal static async Task<TResult> MatchInternalAsync<TResult>(Task<Result<T, TE>> result,
            Func<T, TResult> successFunc, Func<TE, TResult> failFunc)
        {
            var awaitedsResult = await result;
            if (awaitedsResult.IsError == false)
                return successFunc(awaitedsResult.Ok!);
            else
                return failFunc(awaitedsResult.Error!);
        }

        internal static async Task<TResult> MatchInternalAsync<TResult>(Task<Result<T, TE>> result,
            Func<T, Task<TResult>> successFunc, Func<TE, TResult> failFunc)
        {
            var awaitedsResult = await result;
            if (awaitedsResult.IsError == false)
                return await successFunc(awaitedsResult.Ok!);
            else
                return failFunc(awaitedsResult.Error!);
        }

        internal static async Task<TResult> MatchInternalAsync<TResult>(Task<Result<T, TE>> result,
            Func<T, Task<TResult>> successFunc, Func<TE, Task<TResult>> failFunc)
        {
            var awaitedsResult = await result;
            if (awaitedsResult.IsError == false)
                return await successFunc(awaitedsResult.Ok!);
            else
                return await failFunc(awaitedsResult.Error!);
        }

        internal static Result<TR, TE> InternalBind<TR>(Result<T, TE> source, Func<T, Result<TR, TE>> selector)
            where TR: notnull
            => source.IsError == false
                ? selector(source.Ok!)
                : Error<TR, TE>(source.Error!);

        internal static Result<T, TE> InternalBindError(Result<T, TE> source, Func<TE, Result<T, TE>> selector)
            => source.IsError == false
                ? Ok<T, TE>(source.Ok!)
                : selector(source.Error!);

        internal static async Task<Result<TR, TE>> InternalBindAwait<TR>(Result<T, TE> source, Func<T, Task<Result<TR, TE>>> selector)
            where TR: notnull
            => source.IsError == false
                ? await selector(source.Ok!)
                : Error<TR, TE>(source.Error!);

        internal static async Task<Result<T, TE>> InternalBindErrorAwait(Result<T, TE> source, Func<TE, Task<Result<T, TE>>> selector)
            => source.IsError == false
                ? Ok<T, TE>(source.Ok!)
                : await selector(source.Error!);

        internal static async Task<Result<TResult, TE>> InternalSelectAwait<TResult>(Result<T, TE> source, Func<T, Task<TResult>> selector)
            where TResult : notnull
            => source.IsError == false
                ? Ok<TResult, TE>(await selector(source.Ok!))
                : Error<TResult, TE>(source.Error!);

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        T? Ok { get; init; }
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        TE? Error { get; init; }
    }

    public static class ResultExtensions
    {
        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="successFunc"></param>
        /// <param name="failFunc"></param>
        /// <returns></returns>
        public static Task<TResult> MatchAsync<TResult, T, TE>(this Task<Result<T, TE>> result,
            Func<T, TResult> successFunc, Func<TE, TResult> failFunc)
            where T : notnull
            where TE : notnull
            => Result<T, TE>.MatchInternalAsync(result, successFunc, failFunc);

        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="successFunc"></param>
        /// <param name="failFunc"></param>
        /// <returns></returns>
        public static Task<TResult> MatchAsync<TResult, T, TE>(this Task<Result<T, TE>> result,
            Func<T, Task<TResult>> successFunc, Func<TE, TResult> failFunc)
            where T : notnull
            where TE : notnull
            => Result<T, TE>.MatchInternalAsync(result, successFunc, failFunc);

        /// <summary>
        /// Pattern matching for Result 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="successFunc"></param>
        /// <param name="failFunc"></param>
        /// <returns></returns>
        public static Task<TResult> MatchAsync<TResult, T, TE>(this Task<Result<T, TE>> result,
            Func<T, Task<TResult>> successFunc, Func<TE, Task<TResult>> failFunc)
            where T : notnull
            where TE : notnull
            => Result<T, TE>.MatchInternalAsync(result, successFunc, failFunc);

        /// <summary>
        /// Get the Ok value or throws the Error exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static T GetOrThrow<T, TE>(this Result<T, TE> result)
            where T : notnull
            where TE : Exception
            => result.Match(ok => ok, error => throw error);

        /// <summary>
        /// Transforms the OK value (if present) to another type in a LINQ query 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Task<T> GetOrThrowAsync<T, TE>(this Task<Result<T, TE>> result)
            where T : notnull
            where TE : Exception
            => result.MatchAsync(ok => ok, error => throw error);

        /// <summary>
        /// Transforms the OK value (if present) to another type in a LINQ query 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="result"></param>
        /// <param name="selector">Function (selector) transforming T to R</param>
        /// <returns></returns>
        public static Result<R, TE> Select<T, TE, R>(this Result<T, TE> result, Func<T, R> selector)
            where T : notnull
            where TE : notnull
            where R : notnull
            => result.Match(
                t => new Result<R, TE>(selector(t)),
                e => new Result<R, TE>(e)
            );

        /// <summary>
        /// Transforms the OK value (if present) to another type in a LINQ query 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="result"></param>
        /// <param name="selector">Function (selector) transforming T to R</param>
        /// <returns></returns>
        public static async Task<Result<R, TE>> Select<T, TE, R>(this Result<T, TE> result, Func<T, Task<R>> selector)
            where T : notnull
            where TE : notnull
            where R : notnull
            => await result.MatchAsync(
                async t => new Result<R, TE>(await selector(t)),
                e => new Result<R, TE>(e)
            );

        /// <summary>
        /// Transforms the OK value (if present) to another type in a LINQ query 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="result"></param>
        /// <param name="selector">Function (selector) transforming T to R</param>
        /// <returns></returns>
        public static async Task<Result<R, TE>> Select<T, TE, R>(this Task<Result<T, TE>> result, Func<T, R> selector)
        where T : notnull
        where TE : notnull
        where R : notnull
        => (await result).Match(
            t => new Result<R, TE>(selector(t)),
            e => new Result<R, TE>(e)
        );

        /// <summary>
        /// Transforms the OK value (if present) to another type in a LINQ query 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="result"></param>
        /// <param name="selector">Function (selector) transforming T to R</param>
        /// <returns></returns>
        public static async Task<Result<R, TE>> Select<T, TE, R>(this Task<Result<T, TE>> result, Func<T, Task<R>> selector)
            where T : notnull
            where TE : notnull
            where R : notnull
            => await (await result).MatchAsync(
                async t => new Result<R, TE>(await selector(t)),
                e => new Result<R, TE>(e)
            );

        //public static Result<T, TE> Where<T, TE>(this Result<T, TE> result, Func<T, bool> predicate)
        //    where T : notnull
        //    where TE : notnull
        //    => result.Match(
        //        t => predicate(t) ? result : Error<T, TE>.New(new Exception()),
        //        e => Error<T, TE>.New(e)
        //    );

        //public static async Task<Result<T>> Where<T>(this Task<Result<T>> result, Func<T, bool> predicate)
        //    where T : notnull
        //    => await (await result).MatchAsync(
        //        async t => predicate(t) ? await result : Error<T>.New(new Exception()),
        //        e => Error<T>.New(e)
        //    );

        // public static Result<Unit, TE> ForEach<T, TE>(this Result<T, TE> result, Action<T> action)
        //     where T : notnull
        //     where TE : notnull
        //     => result.Map(action.ToUnit());


        /// <summary>
        /// Transforms the OK value (if present) to Result containing another type.
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TE">Exception type</typeparam>
        /// <typeparam name="TResult">Target Type</typeparam>
        /// <param name="result"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Result<TResult, TE> SelectMany<T, TE, TResult>(this Result<T, TE> result,
                Func<T, Result<TResult, TE>> selector)
            where T : notnull
            where TE : notnull
            where TResult : notnull
            => result.Match(
                t => selector(t),
                e => new Result<TResult, TE>(e)
            );

        /// <summary>
        /// Transforms the OK value (if present) to Result containing another type. For use in a LINQ query 
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TE">Exception type</typeparam>
        /// <typeparam name="R">Intermediate type</typeparam>
        /// <typeparam name="TResult">Target Type</typeparam>
        /// <param name="result"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Result<TResult, TE> SelectMany<T, TE, R, TResult>(this Result<T, TE> result,
                Func<T, Result<R, TE>> selector, Func<T, R, TResult> resultSelector)
            where T : notnull
            where TE : notnull
            where R : notnull
            where TResult : notnull
            => result.Match(
                t => selector(t).Match(
                    r => new Result<TResult, TE>(resultSelector(t, r)),
                    e => new Result<TResult, TE>(e)
                ),
                e => new Result<TResult, TE>(e)
            );

        /// <summary>
        /// Transforms the OK value (if present) to Result containing another type. For use in a LINQ query 
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TE">Exception type</typeparam>
        /// <typeparam name="R">Intermediate type</typeparam>
        /// <typeparam name="TResult">Target Type</typeparam>
        /// <param name="result"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static async Task<Result<TResult, TE>> SelectMany<T, TE, R, TResult>(this Task<Result<T, TE>> result,
                Func<T, Task<Result<R, TE>>> selector, Func<T, R, Task<TResult>> resultSelector)
            where T : notnull
            where TE : notnull
            where R : notnull
            where TResult : notnull
            => await (await result).MatchAsync(
                async t => await (await selector(t)).MatchAsync(
                    async r => new Result<TResult, TE>(await resultSelector(t, r)),
                    e => new Result<TResult, TE>(e)
                ),
                e => new Result<TResult, TE>(e)
            );

        /// <summary>
        /// Transforms the OK value (if present) to Result containing another type. For use in a LINQ query 
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TE">Exception type</typeparam>
        /// <typeparam name="R">Intermediate type</typeparam>
        /// <typeparam name="TResult">Target Type</typeparam>
        /// <param name="result"></param>
        /// <param name="selector"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static async Task<Result<TResult, TE>> SelectMany<T, TE, R, TResult>(this Task<Result<T, TE>> result,
                Func<T, Task<Result<R, TE>>> selector, Func<T, R, TResult> resultSelector)
            where T : notnull
            where TE : notnull
            where R : notnull
            where TResult : notnull
            => await (await result).MatchAsync(
                async t => (await selector(t)).Match(
                    r => new Result<TResult, TE>(resultSelector(t, r)),
                    e => new Result<TResult, TE>(e)
                ),
                e => new Result<TResult, TE>(e)
            );

        /// <summary>
        /// Transforms the Error value (if present) to Result.
        /// </summary>
        /// <typeparam name="T">Source type</typeparam>
        /// <typeparam name="TE">Error type</typeparam>
        /// <param name="result"></param>
        /// <param name="errorSelector"></param>
        /// <returns></returns>
        public static Result<T, TE> BindError<T, TE>(this Result<T, TE> result,
                Func<TE, Result<T, TE>> errorSelector)
            where T : notnull
            where TE : notnull
            => result.Match(
                t => t,
                e => errorSelector(e)
            );

        /// <summary>
        /// Transforms the Error type to another rype
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TER"></typeparam>
        /// <param name="result"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Result<T, TER> SelectError<T, TE, TER>(this Result<T, TE> result, Func<TE, TER> selector)
            where T : notnull
            where TE : notnull
            where TER : notnull
            => result.Match(
                t => t,
                e => new Result<T, TER>(selector(e))
            );

        /// <summary>
        /// Transforms the SelectError type to another rype
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <typeparam name="TER"></typeparam>
        /// <param name="result"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Task<Result<T, TER>> SelectError<T, TE, TER>(this Task<Result<T, TE>> result, Func<TE, TER> selector)
            where T : notnull
            where TE : notnull
            where TER : notnull
            => result.MatchAsync(
                t => t,
                e => new Result<T, TER>(selector(e))
            );

        /// <summary>
        /// Gets the Ok value, or get a default value instead the Error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetOrDefault<T, TE>(this Result<T, TE> result, T defaultValue)
            where T : notnull
            where TE : notnull
            => result.Match(val => val, e => defaultValue);

        /// <summary>
        /// Gets the Ok value, or get a default value instead the Error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="result"></param>
        /// <param name="getErrorValue">Function retrieving result from error value</param>
        /// <returns></returns>
        public static T Get<T, TE>(this Result<T, TE> result, Func<TE, T> getErrorValue)
            where T : notnull
            where TE : notnull
            => result.Match(val => val, getErrorValue);

        internal static Task<Result<TResult, TE>> InternalSelectAwait<TResult, T, TE>(this Result<T, TE> source, Func<T, Task<TResult>> selector)
            where T : notnull
            where TE : notnull
            where TResult : notnull
            => Result<T, TE>.InternalSelectAwait(source, selector);
    }
}

namespace CsTools
{
    public static partial class Core
    {
        /// <summary>
        /// Creating a Result with an OK value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result<T, TE> Ok<T, TE>(T value)
            where T : notnull
            where TE : notnull
            => new(value);

        /// <summary>
        /// Creating a Result with an Error value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Result<T, TE> Error<T, TE>(TE e)
            where T : notnull
            where TE : notnull
            => new(e);

        /// <summary>
        /// Runs code and returns a Result containing the result, or if it throws an exception the exception as Error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="func"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public static Result<T, TE> Try<T, TE>(Func<T> func, Func<Exception, TE> onException)
            where T : notnull
            where TE : notnull
        {
            try
            {
                return Ok<T, TE>(func());
            }
            catch (Exception ex)
            {
                return Error<T, TE>(onException(ex));
            }
        }

        /// <summary>
        /// Runs code and returns 'Nothing', or if it throws an exception the exception as Error
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <param name="action"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public static Result<Nothing, TE> Try<TE>(Action action, Func<Exception, TE> onException)
            where TE : notnull
        {
            try
            {
                action();
                return 0.ToNothing();
            }
            catch (Exception ex)
            {
                return Error<Nothing, TE>(onException(ex));
            }
        }

        /// <summary>
        /// Runs code and returns a Result containing the result, or if it throws an exception the exception as Error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="func"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public static async Task<Result<T, TE>> TryAsync<T, TE>(Func<Task<T>> func, Func<Exception, TE> onException)
            where T : notnull
            where TE : notnull
        {
            try
            {
                return Ok<T, TE>(await func());
            }
            catch (Exception ex)
            {
                return Error<T, TE>(onException(ex));
            }
        }

        /// <summary>
        /// Runs async code and returns 'Nothing' containing the result, or if it throws an exception the exception as Error
        /// </summary>
        /// <typeparam name="TE"></typeparam>
        /// <param name="func"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public static async Task<Result<Nothing, TE>> TryAsync<TE>(Func<Task> func, Func<Exception, TE> onException)
            where TE : notnull
        {
            try
            {
                await func();
                return 0.ToNothing();
            }
            catch (Exception ex)
            {
                return Error<Nothing, TE>(onException(ex));
            }
        }
    }
}