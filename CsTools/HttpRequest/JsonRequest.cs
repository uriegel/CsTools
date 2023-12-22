using System.Net.Http.Json;
using CsTools.Functional;
using static CsTools.HttpRequest.Core;

namespace CsTools.HttpRequest;

public class JsonRequest(string baseUrl)
{
    public AsyncResult<TR, RequestError> Post<T, TR>(RequestType<T> request)
        where TR : notnull
        // TODO test with AspNetExtensions tester (WriteLine start AspNetExtensions tester)
        // TODO client exceptions + 1000
        // TODO no connection 1001
        // TODO unknown host 1002
        // TODO json parse error (wrong target type) 1003
        // TODO exn 1010
        // TODO wrong method (not found) 
        
        => PostAsync<T, TR>(request)
            .ToAsyncResult();

    async Task<Result<TR, RequestError>> PostAsync<T, TR>(RequestType<T> request)
        where TR: notnull
    {
        using var msg = await Request.RunAsync(DefaultSettings with
        {
            Method = HttpMethod.Post,
            BaseUrl = baseUrl,
            Url = request.Method,
            AddContent = () => JsonContent.Create(request.Payload)
        }, true);
        return await msg.Content.ReadFromJsonAsync<Result<TR, RequestError>>();
    }
}

public record RequestType<T>(string Method, T Payload);

public record RequestError(
    int Status,
    string StatusText
);