using System.Net.Http.Json;
using CsTools.Functional;

using static CsTools.HttpRequest.Core;
using static CsTools.Core;

namespace CsTools.HttpRequest;

public class JsonRequest(string baseUrl)
{
    public AsyncResult<TR, RequestError> Post<T, TR>(RequestType<T> request)
        where TR : notnull
        => PostAsync<T, TR>(request)
            .ToAsyncResult();

    public AsyncResult<TR, RequestError> Post<T, TR>(RequestType<T> request, Settings settings)
        where TR : notnull
        => PostAsync<T, TR>(request, settings)
            .ToAsyncResult();

    async Task<Result<TR, RequestError>> PostAsync<T, TR>(RequestType<T> request, Settings? settings = null)
        where TR: notnull
    {
        try 
        {
            using var msg = await Request.RunAsync((settings ?? DefaultSettings) with
            {
                Method = HttpMethod.Post,
                BaseUrl = baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/",
                Url = request.Method,
                AddContent = settings?.ContentLengthInJsonPost.HasValue == true
                    ? () => new CustomJsonContent(typeof(T), request.Payload!, settings?.JsonSerializerOptions)
                    : () => JsonContent.Create(request.Payload)
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

public record RequestError(
    int Status,
    string StatusText
);