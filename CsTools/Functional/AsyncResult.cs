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


    //     // public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> successFunc, Func<TResult> failFunc)
    //     //     => IsSome
    //     //         ? await successFunc(Value!)
    //     //         : failFunc();

    //     // public async Task<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> successFunc, Func<Task<TResult>> failFunc)
    //     //     => IsSome
    //     //         ? await successFunc(Value!)
    //     //         : await failFunc();

    //     public Option<T> WhenSome(Action<T> sideEffect)
    //     {
    //         if (IsSome)
    //             sideEffect(Value!);
    //         return this;
    //     }

    //     public Option<T> WhenNone(Action sideEffect)
    //     {
    //         if (!IsSome)
    //             sideEffect();
    //         return this;
    //     }

    //     // public Option<B> As<B>() where B : class
    //     //     => Match(o => (o as B).FromNullable(), () => None);

    //     // internal static async Task<TResult> MatchAsync<TResult>(Task<Option<T>> option, Func<T, TResult> successFunc, Func<TResult> failFunc)
    //     // {
    //     //     var awaitedOption = await option;
    //     //     return awaitedOption.IsSome
    //     //         ? successFunc(awaitedOption.Value!)
    //     //         : failFunc();
    //     // }

    //     // internal static async Task<TResult> MatchAsync<TResult>(Task<Option<T>> option, Func<T, Task<TResult>> successFunc, Func<Task<TResult>> failFunc)
    //     // {
    //     //     var awaitedOption = await option;
    //     //     return awaitedOption.IsSome
    //     //         ? await successFunc(awaitedOption.Value!)
    //     //         : await failFunc();
    //     // }

    //     // internal static async Task<Option<T>> StaticSideEffectAsync(Task<Option<T>> option, Action<T> sideEffect)
    //     // {
    //     //     var awaitedOption = await option;
    //     //     if (awaitedOption.IsSome)
    //     //         sideEffect(awaitedOption.Value!);
    //     //     return awaitedOption;
    //     // }

    //     // internal static async Task<Option<T>> StaticSideEffectAsync(Task<Option<T>> option, Func<T, Task> sideEffect)
    //     // {
    //     //     var awaitedOption = await option;
    //     //     if (awaitedOption.IsSome)
    //     //         await sideEffect(awaitedOption.Value!);
    //     //     return awaitedOption;
    //     // }

    //     // internal static async Task<Option<T>> StaticSideEffectOnNoneAsync(Task<Option<T>> option, Action sideEffect)
    //     // {
    //     //     var awaitedOption = await option;
    //     //     if (!awaitedOption.IsSome)
    //     //         sideEffect();
    //     //     return awaitedOption;
    //     // }

    //     T? Value { get; }
}

public static class AsyncResultExtensions
{
    public static AsyncResult<T, TE> ToAsyncResult<T, TE>(this Result<T, TE> result)
        where T : notnull
        where TE : notnull
        => new(result);

    public static AsyncResult<TResult, TE> Select<TSource, TE, TResult>(this AsyncResult<TSource, TE> source, Func<TSource, TResult> func)
        where TSource : notnull
        where TE : notnull
        where TResult : notnull
    => new(from n in source.resultTask
           select n.Select(func));

    public static AsyncResult<TResult, TE> SelectAwait<TSource, TE, TResult>(this AsyncResult<TSource, TE> source, Func<TSource, Task<TResult>> selector)
        where TSource : notnull
        where TE : notnull
        where TResult : notnull
    => new((from n in source.resultTask
            select n.InternalSelectAwait(selector)).Unwrap());

    public static AsyncResult<T, TE> BindExceptionAwait<T, TE>(this AsyncResult<T, TE> source, Func<TE, AsyncResult<T, TE>> selector)
        where T : notnull
        where TE : notnull

    {
        async Task<Result<T, TE>> Bind()
        {
            var r = await source.resultTask;
            return await Result<T, TE>.InternalBindExceptionAwait(r, e => selector(e).ToResult());
        }
        return new(Bind());
    }
}

//     // public static Option<R> Map<T, R>(this Option<T> opt, Func<T, R> func)
//     //     where R : notnull
//     //     where T : notnull
//     //     => opt.Match(
//     //         some => Some(func(some)),
//     //         ()   => None
//     //     );

//     // public static Task<TResult> MatchAsync<T, TResult>(this Task<Option<T>> option, Func<T, Task<TResult>> successFunc, Func<Task<TResult>> failFunc)
//     //     where T : notnull
//     //     where TResult : notnull
//     //     => Option<T>.MatchAsync(option, successFunc, failFunc);

//     // public static async Task<Option<R>> MapAsync<T, R>(this Task<Option<T>> option, Func<T, R> func)
//     //     where R : notnull
//     //     where T : notnull
//     //     => (await option).Match(
//     //         some => Some(func(some)),
//     //         () => None
//     //     );

//     // public static async Task<Option<R>> MapAsync<T, R>(this Task<Option<T>> option, Func<T, Task<R>> func)
//     //     where R : notnull
//     //     where T : notnull
//     //     => await (await option).MatchAsync(
//     //         async some => Some(await func(some)),
//     //         () => None
//     //     );

//     // public static Option<R> Bind<T, R>(this Option<T> opt, Func<T, Option<R>> func)
//     //     where R : notnull
//     //     where T : notnull
//     //     => opt.Match(
//     //         t => func(t).Match(
//     //             Some,
//     //             ()   => None
//     //         ),
//     //         ()       => None
//     //     );

//     // public static async Task<Option<R>> BindAsync<T, R>(this Task<Option<T>> option, Func<T, Task<Option<R>>> func)
//     //     where R : notnull
//     //     where T : notnull
//     //     => await (await option).MatchAsync(
//     //         async some => await (await func(some)).MatchAsync(
//     //             some  => Task.FromResult(Some(some)),
//     //             ()    => None
//     //         ),
//     //         () => None
//     //     );

//     public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
//         where T : notnull
//         => option.Match(
//             t => predicate(t) ? option : None,
//             () => None
//         );

//     // public static async Task<Option<T>> Where<T>(this Task<Option<T>> option, Func<T, bool> predicate)
//     //     where T : notnull
//     //     => await (await option).MatchAsync(
//     //         async t => predicate(t) ? await option : None,
//     //         () => None
//     //     );

//     // public static async Task<Option<T>> Where<T>(this Task<Option<T>> option, Func<T, Task<bool>> predicate)
//     //     where T : notnull
//     //     => await (await option).MatchAsync(
//     //         async t => await predicate(t) ? await option : None,
//     //         () => None
//     //     );

//     // public static Option<Unit> ForEach<T>(this Option<T> opt, Action<T> action)
//     //     where T : notnull
//     //     => opt.Map(action.ToUnit());

//     // public static Option<Unit> ForEach<T>(this Option<T> opt, Func<T, Unit> action)
//     //     where T : notnull
//     //     => opt.Map(action);

//     public static Option<RR> SelectMany<T, R, RR>(this Option<T> opt, Func<T, Option<R>> bind, Func<T, R, RR> project)
//         where RR : notnull
//         where R : notnull
//         where T : notnull
//         => opt.Match(
//             t => bind(t).Match(
//                 r => Some(project(t, r)),
//                 () => None
//             ),
//             () => None
//         );

//     // public static async Task<Option<RR>> SelectMany<T, R, RR>(this Task<Option<T>> option,
//     //         Func<T, Task<Option<R>>> bind, Func<T, R, Task<RR>> project)
//     //     where RR : notnull
//     //     where R : notnull
//     //     where T : notnull
//     //     => await (await option).MatchAsync(
//     //         async t => await (await bind(t)).MatchAsync(
//     //             async r => Some(await project(t, r)),
//     //             () => None
//     //         ),
//     //         () => None
//     //     );

//     // public static async Task<Option<RR>> SelectMany<T, R, RR>(this Task<Option<T>> result,
//     //         Func<T, Task<Option<R>>> bind, Func<T, R, RR> project)
//     //     where RR : notnull
//     //     where R : notnull
//     //     where T : notnull
//     //     => await (await result).MatchAsync(
//     //         async t => (await bind(t)).Match(
//     //             r  => Some(project(t, r)),
//     //             () => None
//     //         ),
//     //         () => None
//     //     );
//     public static Option<R> Select<T, R>(this Option<T> opt, Func<T, R> func) 
//         where T : notnull
//         where R : notnull 
//         => Map(opt, func);

//     // public static Task<Option<R>> Select<T, R>(this Task<Option<T>> option, Func<T, Task<R>> func)
//     //     where T : notnull
//     //     where R : notnull 
//     //     => MapAsync(option, func);

//     // public static Task<Option<R>> Select<T, R>(this Task<Option<T>> option, Func<T, R> func)
//     //     where T : notnull
//     //     where R : notnull
//     //     => MapAsync(option, t => Task.FromResult(func(t)));

//     // public static Task<Option<R>> SelectMany<T, R>(this Task<Option<T>> result, Func<T, int, Task<Option<R>>> func)
//     //     where T : notnull
//     //     where R : notnull
//     //     => BindAsync(result, async y => await func(y, 0));

//     //public static Option<R> SelectMany<T, R>(this Option<T> opt, Func<T, Option<R>> func) => Bind(opt, func);
// }

// public struct NoneType { }

// public static class NoneTypeExtensions
// {
//     public static NoneType SideEffect(this NoneType none, Action sideEffect)
//     {
//         sideEffect();
//         return none;
//     }

// public static partial class Core
// {
//     public static Option<T> Some<T>(T value)
//         where T : notnull
//         => new(value);
//     public static NoneType None => default;

//     public static Option<(U, V)> OptionFromTwoOptions<U, V>(Option<U> option1, Option<V> option2)
//         where U : notnull
//         where V : notnull
//         => (option1, option2) switch
//         {
//             (Option<U> { IsSome: true }, Option<V> { IsSome: true }) => Some((option1.ThrowOnNone(), option2.ThrowOnNone())),
//             _ => None
//         };

//     public static Option<(U, V)> OptionFromTwoOptions<U, V>(Func<Option<U>> getOption1, Func<Option<V>> getOption2)
//         where U : notnull
//         where V : notnull
//         => OptionFromTwoOptions(getOption1(), getOption2());

