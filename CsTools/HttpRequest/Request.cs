using System.Net;
using System.Net.Sockets;
using CsTools.Extensions;
using CsTools.Async;
using CsTools.Functional;

using static CsTools.Core;

namespace CsTools.HttpRequest;

public static class Request
{
    public static AsyncResult<HttpResponseMessage, RequestError> Run(Settings settings)
        => ResultAsync(settings, false)
            .ToAsyncResult();

    public static AsyncResult<HttpResponseMessage, RequestError> Run(Settings settings, bool onlyHeaders)
        => ResultAsync(settings, onlyHeaders)
            .ToAsyncResult();

    async static Task<Result<HttpResponseMessage, RequestError>> ResultAsync(Settings settings, bool onlyHeaders)
    {
        try 
        {
            var msg = await RawRunAsync(settings, onlyHeaders);
            return Ok<HttpResponseMessage, RequestError>(msg);
        }
        catch (HttpException he) when (he.InnerException is System.Net.Http.HttpRequestException hre 
                && hre.HttpRequestError == HttpRequestError.ConnectionError)
        {
            return Error<HttpResponseMessage, RequestError>(RequestError.Custom(CustomRequestError.ConnectionError, hre.Message));
        }
        catch (HttpException he) when 
            (he.InnerException is System.Net.Http.HttpRequestException hre 
                && hre.HttpRequestError == HttpRequestError.NameResolutionError)
        {
            return Error<HttpResponseMessage, RequestError>(RequestError.Custom(CustomRequestError.NameResolutionError, hre.Message));
        }
        catch (HttpException he) when (he.InnerException is HttpRequestException hre) 
        {
            return Error<HttpResponseMessage, RequestError>(RequestError.Custom(hre.Code, hre.Message));
        }
        catch (Exception e)
        {
            return Error<HttpResponseMessage, RequestError>(RequestError.Custom(CustomRequestError.Unknown, e.Message));
        }
    }



    public static Task<HttpResponseMessage> RunAsync(Settings settings)
        => RawRunAsync(settings, false)
            .MapRequestException();

    public static Task<HttpResponseMessage> RunAsync(Settings settings, bool onlyHeaders)
        => RawRunAsync(settings, onlyHeaders)
            .MapRequestException();

    public static Task<string> GetStringAsync(Settings settings)
        =>(from n in RawRunAsync(settings, false)
           from m in n.Content.ReadAsStringAsync()
           select m)
            .MapRequestException();

    public static Func<Task<HttpResponseMessage>> RunAsyncApply(Settings settings)
        => () => RunAsync(settings);

    public static Func<Task<string>> GetStringAsyncApply(Settings settings)
        => () => GetStringAsync(settings);

    /// <summary>
    /// Gets the response stream as LengthStream with content length
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static Stream GetResponseStream(this HttpResponseMessage msg)
        => msg
            .Content
            .ReadAsStream()
            .WithLength(msg.Content.Headers.ContentLength ?? 0);

    public static string? GetHeaderValue(this HttpResponseMessage msg, string name)
        => msg.Headers.TryGetValues(name, out var res)
        ? res.First()
        : msg.Content.Headers.TryGetValues(name, out var contentRes)
            ? contentRes.First()
            : null;

    public static long? GetHeaderLongValue(this HttpResponseMessage msg, string name)
        => msg.GetHeaderValue(name)?.ParseLong();

    static HttpRequestMessage CreateRequest(Settings settings)
        => new(settings.Method, (settings.BaseUrl ?? "") + settings.Url)
        {
            Version = new(settings.Version.Major, settings.Version.Minor)
        };

    static void AddHeaders(this HttpRequestMessage msg, Settings settings)
    {
        void AddHeader(Header header)
        {
            if (!msg.Headers.TryAddWithoutValidation(header.Key, header.Value) && msg.Content != null)
                msg.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        settings.Headers?.ForEach(n => AddHeader(n));
    }

    static async Task<HttpResponseMessage> RawRunAsync(Settings settings, bool onlyHeaders)
    {
        var request = CreateRequest(settings);
        if (settings.AddContent != null)
            request.Content = settings.AddContent();
        request.AddHeaders(settings);
        var response = settings.Timeout.HasValue
            ? await Client.Get().SendAsync(request, onlyHeaders ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, new CancellationTokenSource(settings.Timeout.Value).Token)
            : await Client.Get().SendAsync(request, onlyHeaders ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead);
        return 
            response.StatusCode == HttpStatusCode.OK 
            || response.StatusCode == HttpStatusCode.NoContent
            || response.StatusCode == HttpStatusCode.NotModified
        ? response
        : throw new HttpRequestException(response.StatusCode, response.ReasonPhrase ?? $"{response.StatusCode}", response);
    }

    static Task<T> MapRequestException<T>(this Task<T> t)
            where T : notnull
        => t.MapException(ex => ex switch
            {
                InvalidOperationException ioe => new RequestInvalidOperationException(ioe),
                TaskCanceledException => new TimeoutException(),
                HttpRequestException hre when hre.InnerException is SocketException se && se.SocketErrorCode == SocketError.HostNotFound 
                    => new HostNotFoundException(hre.Message),
                HttpRequestException hre when hre.InnerException is SocketException se
                    => new RequestSocketException(se),
                Exception e => new HttpException(e.Message, e)
            });
}