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
            catch (Exception e)
            {
                // TODO which errors can occur?
                return Error<HttpResponseMessage, RequestError>(new(1001, "Could not copy"));
            }
        }
    }
}
