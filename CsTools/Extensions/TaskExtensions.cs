namespace CsTools.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// Important: the task must be created, so you have to use an "async Task" function, not a "Task function" 
    /// which returns "this Task&lt;T&gt; t" as the input parameter this extension method uses. Otherwise exceptions thrown on the calling thread (before the first await) are not catched!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="onException"></param>
    /// <returns></returns>
    public static async Task<T> Catch<T>(this Task<T> t, Func<Exception, Task<T>> onException)
    {
        try
        {
            return await t;
        }
        catch (Exception e)
        {
            return await onException(e);
        }
    }

    /// <summary>
    /// Important: the task must be created, so you have to use an "async Task" function, not a "Task function" 
    /// which returns "this Task t" as the input parameter this extension method uses. Otherwise exceptions thrown on the calling thread (before the first await) are not catched!
    /// </summary>
    /// <param name="t"></param>
    /// <param name="onException"></param>
    /// <returns></returns>
    public static async Task Catch(this Task t, Func<Exception, Task> onException)
    {
        try
        {
            await t;
        }
        catch (Exception e)
        {
            await onException(e);
        }
    }

    /// <summary>
    /// Important: the task must be created, so you have to use an "async Task" function, not a "Task function" 
    /// which returns "this Task&lt;T&gt; t" as the input parameter this extension method uses. Otherwise exceptions thrown on the calling thread (before the first await) are not catched!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="onException"></param>
    /// <returns></returns>
    public static async Task<T> Catch<T>(this Task<T> t, Func<Exception, T> onException)
    {
        try
        {
            return await t;
        }
        catch (Exception e)
        {
            return onException(e);
        }
    }

    /// <summary>
    /// Important: the task must be created, so you have to use an "async Task" function, not a "Task function" 
    /// which returns "this Task t" as the input parameter this extension method uses. Otherwise exceptions thrown on the calling thread (before the first await) are not catched!
    /// </summary>
    /// <param name="t"></param>
    /// <param name="onException"></param>
    /// <returns></returns>
    public static async Task Catch(this Task t, Action<Exception> onException)
    {
        try
        {
            await t;
        }
        catch (Exception e)
        {
            onException(e);
        }
    }

    /// <summary>
    /// Important: the task must be created, so you have to use an "async Task" function, not a "Task function" 
    /// which returns "this Task t" as the input parameter this extension method uses. Otherwise exceptions thrown on the calling thread (before the first await) are not catched!
    /// </summary>
    /// <param name="t"></param>
    /// <param name="mapException"></param>
    /// <returns></returns>
    public static async Task MapException(this Task t, Func<Exception, Exception> mapException)
    {
        try
        {
            await t;
        }
        catch (Exception e)
        {
            throw mapException(e);
        }
    }

    /// <summary>
    /// Important: the task must be created, so you have to use an "async Task" function, not a "Task function" 
    /// which returns "this Task&lt;T&gt; t" as the input parameter this extension method uses. Otherwise exceptions thrown on the calling thread (before the first await) are not catched!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="mapException"></param>
    /// <returns></returns>
    public static async Task<T> MapException<T>(this Task<T> t, Func<Exception, Exception> mapException)
    {
        try
        {
            return await t;
        }
        catch (Exception e)
        {
            throw mapException(e);
        }
    }
}
