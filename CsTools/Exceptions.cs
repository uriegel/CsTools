public partial class Core
{
    /// <summary>
    /// Functional way of handling exceptions. A function 'func' is being called. If an exception occurs, the function 'onException' 
    /// is being called, so that you can create a result of return type T
    /// </summary>
    /// <typeparam name="T">Return type of this function</typeparam>
    /// <param name="func">Function to be called</param>
    /// <param name="onException">Is called, when 'func' throws an exception</param>
    /// <returns>A result of type 'T'</returns>(
    public static T Try<T>(Func<T> func, Func<Exception, T> onException)
    {
        try 
        {
            return func();
        }
        catch (Exception e)
        {
            return onException(e);
        }
    }

    /// <summary>
    /// Functional way of handling exceptions. A function 'func' is being called. If an exception occurs, the function 'onException' 
    /// is being called, so that you can create a result of return type T
    /// </summary>
    /// <typeparam name="T">Return type of this function</typeparam>
    /// <param name="func">Function to be called</param>
    /// <param name="onException">Is called, when 'func' throws an exception</param>
    /// <returns></returns>
    public static async Task<T> Try<T>(Func<Task<T>> func, Func<Exception, T> onException)
    {
        try 
        {
            return await func();
        }
        catch (Exception e)
        {
            return onException(e);
        }
    }

    /// <summary>
    /// Functional way of handling exceptions. A function 'func' is being called. If an exception occurs, the function 'onException' 
    /// is being called, so that you can create a result of return type T
    /// </summary>
    /// <typeparam name="T">Return type of this function</typeparam>
    /// <param name="func">Function to be called</param>
    /// <param name="onException">Is called, when 'func' throws an exception</param>
    /// <returns></returns>
    public static async Task<T> Try<T>(Func<Task<T>> func, Func<Exception, Task<T>> onException)
    {
        try 
        {
            return await func();
        }
        catch (Exception e)
        {
            return await onException(e);
        }
    }
}