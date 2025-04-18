using CsTools.Functional;
using CsTools.HttpRequest;

using static CsTools.Core;

namespace CsTools.Extensions;

public static class HttpResponseMessageExtensions
{
    public static AsyncResult<HttpResponseMessage, RequestError> CopyToStream(this HttpResponseMessage msg, Stream target, CancellationToken? cancellationToken = null)
    {
        return CopyToStreamAsync(msg, target, cancellationToken).ToAsyncResult();

        static async Task<Result<HttpResponseMessage, RequestError>> CopyToStreamAsync(HttpResponseMessage msg, Stream target, CancellationToken? cancellationToken)
        {
            try 
            {
                await msg
                    .Content
                    .ReadAsStream()
                    .CopyToAsync(target, cancellationToken ?? CancellationToken.None);
                return Ok<HttpResponseMessage, RequestError>(msg);
            }
            catch (HttpIOException hie) when (hie.HttpRequestError == HttpRequestError.ResponseEnded)
            {
                return Error<HttpResponseMessage, RequestError>(RequestError.Custom(CustomRequestError.ResponseEnded, hie.Message));
            }
            catch (TaskCanceledException te)
            {
                return Error<HttpResponseMessage, RequestError>(RequestError.Custom(CustomRequestError.TaskCanceled, te.Message));
            }
            catch (Exception e)
            {
                return Error<HttpResponseMessage, RequestError>(RequestError.Custom(CustomRequestError.Unknown, e.Message));
            }
        }
    }

    public static AsyncResult<string, RequestError> ReadAsStringAwait(this HttpResponseMessage msg)
    {
        return ReadAsStringAwait().ToAsyncResult();
        
        async Task<Result<string, RequestError>> ReadAsStringAwait()
        {
            try 
            {
                var text = await msg
                                    .Content
                                    .ReadAsStringAsync();
                return Ok<string, RequestError>(text);
            }
            catch (HttpIOException hie) when (hie.HttpRequestError == HttpRequestError.ResponseEnded)
            {
                return Error<string, RequestError>(RequestError.Custom(CustomRequestError.ResponseEnded, hie.Message));
            }
            catch (Exception e)
            {
                return Error<string, RequestError>(RequestError.Custom(CustomRequestError.Unknown, e.Message));
            }
        }
    }
    
}
