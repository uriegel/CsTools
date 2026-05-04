using System.Collections;

namespace CsTools;

/// <summary>
/// Wraps an <see cref="IEnumerable{T}"/> and returns an enumerable which 
/// disposes all contained elements     
/// </summary>
/// <typeparam name="T">Arbitrary type which is disposable</typeparam>
public class DisposableEnumerable<T> : IEnumerable<T>, IDisposable
    where T : IDisposable
{
    public DisposableEnumerable(IEnumerable<T> items) => enumerable = items;

    readonly IEnumerable<T> enumerable;

    #region IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var item in enumerable)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                foreach (var item in enumerable)
                    item.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private bool disposedValue;

    #endregion

}