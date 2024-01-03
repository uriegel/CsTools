using System.Text.Json;
using System.Text.Json.Serialization;

namespace CsTools.HttpRequest;

public class SseClient : IDisposable
{
    public SseClient(string url, Action<string> onMsg) : this(url)
    {
        this.onMsg = onMsg;
        Loop();   
    }

    protected  SseClient(string url)
    {
        this.url = url;
        client.Timeout = TimeSpan.FromSeconds(5);
    }

    protected virtual void OnMsg(string msg)
        => onMsg?.Invoke(msg);

    protected async void Loop()
    {
        while (true)
        {
            try 
            {
                using var sr = new StreamReader(await client.GetStreamAsync(url));
                while (!sr.EndOfStream)
                {
                    var msg = await sr.ReadLineAsync();
                    if (msg?.Length > 5)
                        OnMsg(msg[5..]);
                }
            }
            catch 
            {
                await Task.Delay(client.Timeout);
            }
        }
    }

    Action<string>? onMsg;
    readonly HttpClient client = new();
    readonly string url;
 
    #region IDisposable

    public void Dispose()
    {
        // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Verwalteten Zustand (verwaltete Objekte) bereinigen
                client.Dispose();
                onMsg = null;
            }

            // Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            // Große Felder auf NULL setzen
            disposedValue = true;
        }
    }

    // // Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
    // ~SseClient()
    // {
    //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
    //     Dispose(disposing: false);
    // }

    protected bool disposedValue;

    #endregion
}

public class SseClient<T> : SseClient
{
    public SseClient(string url, Action<T> onMsg)
        : base(url)
    {
        onJsonMsg = onMsg;
        Loop();   
    }

    protected override void OnMsg(string msg)
    {
        var json = JsonSerializer.Deserialize<T>(msg, defaults);
        if (json != null)
            onJsonMsg?.Invoke(json);
    }

    static readonly JsonSerializerOptions defaults = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                // Verwalteten Zustand (verwaltete Objekte) bereinigen
                onJsonMsg = null;

            // Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
            // Große Felder auf NULL setzen
            base.Dispose(disposing);
        }
    }

    Action<T>? onJsonMsg;
}