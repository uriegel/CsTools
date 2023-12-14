using CsTools.Functional;

using static CsTools.Core;

namespace CsTools.Async
{
    /// <summary>
    /// Extension Functions for async functions to make async function a Monad/Functor
    /// </summary>
    public static class AsyncTask
    {
        /// <summary>
        /// To use an awaitable function with LINQ Query Syntax (from n in (awaitable))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<TResult> Select<T, TResult>(this Task<T> task, Func<T, TResult> func)
            => func(await task);

        /// <summary>
        /// To use an awaitable function with LINQ Query Syntax (from n in (awaitable)  from n in AwaitableFunc(m)).
        /// The result is flattened
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="bind"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public static async Task<TResult> SelectMany<T, R, TResult>
                (this Task<T> task, Func<T, Task<R>> bind, Func<T, R, TResult> project)
        {
            var t = await task;
            return project(t, await bind(t));
        }

        public static async Task<TResult> SelectMany<T, TResult>
                (this Task<T> task, Func<T, Task<TResult>> bind)
        {
            var t = await task;
            return await bind(t);
        }

        public static async Task<Nothing> ToNothing(this Task t)
        {
            await t;
            return nothing;        
        }
    }
}

namespace CsTools
{
    public static partial class Core
    {
        /// <summary>
        /// If you need to an awaitale value, and you have only a normal value, you can create an 
        /// Async monad 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Task<T> Async<T>(T t) => Task.FromResult(t);
    }
}

