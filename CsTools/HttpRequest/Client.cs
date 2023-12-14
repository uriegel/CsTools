using static CsTools.Functional.Memoization;

namespace CsTools.HttpRequest;

public static class Client
{
    public static void Init(int maxConnections, TimeSpan? timeout)
    {
        Client.maxConnections = maxConnections;
        Client.timeout = timeout;
    }

    public static Func<HttpClient> Get { get; } = Memoize(Init);

    static HttpClient Init()
        => new (new HttpClientHandler()
        {
            MaxConnectionsPerServer = maxConnections,
        })
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(100)
        };

    static int maxConnections = 8;
    static TimeSpan? timeout;
}

public static partial class Core
{
    public static Settings DefaultSettings { get; } = new(HttpMethod.Get, null, "", new(2, 0), null, null);
}