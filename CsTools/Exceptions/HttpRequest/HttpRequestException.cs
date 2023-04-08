namespace CsTools.HttpRequest;

using System.Net;

public class HttpRequestException : HttpException
{
    public HttpRequestException(HttpStatusCode code, string text, HttpResponseMessage msg)
        : base(text)
    {
        Code = code;
        Msg = msg;
    }

    public HttpStatusCode Code { get; }
    public HttpResponseMessage Msg { get; }
}