using System.Net.Http.Json;
using CsTools.Functional;

using static CsTools.HttpRequest.Core;
using static CsTools.Core;
using System.Net;

namespace CsTools.HttpRequest;

public class JsonRequest(string baseUrl)
{
    public AsyncResult<TR, RequestError> Post<T, TR>(RequestType<T> request)
        where TR : notnull
        => PostAsync<T, TR>(request)
            .ToAsyncResult();

    async Task<Result<TR, RequestError>> PostAsync<T, TR>(RequestType<T> request)
        where TR: notnull
    {
        try 
        {
            using var msg = await Request.RunAsync(DefaultSettings with
            {
                Method = HttpMethod.Post,
                BaseUrl = baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/",
                Url = request.Method,
                AddContent = () => JsonContent.Create(request.Payload)
            }, true);
            return await msg.Content.ReadFromJsonAsync<Result<TR, RequestError>>();
        }
        catch (HttpException he) when (he.InnerException is System.Net.Http.HttpRequestException hre 
                && hre.HttpRequestError == HttpRequestError.ConnectionError)
        {
            return Error<TR, RequestError>(new(1001, hre.Message));
        }
        catch (HttpException he) when 
            (he.InnerException is System.Net.Http.HttpRequestException hre 
                && hre.HttpRequestError == HttpRequestError.NameResolutionError)
        {
            return Error<TR, RequestError>(new(1002, hre.Message));
        }
        catch (HttpException he) when (he.InnerException is HttpRequestException hre) 
        {
            return Error<TR, RequestError>(new((int)hre.Code + 1000, hre.Message));
        }
        catch (Exception e)
        {
            return Error<TR, RequestError>(new(1000, e.Message));
        }
    }
}

public record RequestType<T>(string Method, T Payload);

public enum CustomRequestError 
{
    Unknown = 1000,
    ConnectionError,
    NameResolutionError,
    ResponseEnded,
}

public record RequestError(
    int Status,
    string StatusText
) {
    public static RequestError Custom(CustomRequestError error, string message)
        => new((int)error, message);
    public static RequestError Custom(HttpStatusCode httpStatus, string message)
        => new((int)CustomRequestError.Unknown + (int)httpStatus, message);
}