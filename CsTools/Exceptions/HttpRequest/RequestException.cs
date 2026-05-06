namespace CsTools.HttpRequest;

using System.Net;

public class RequestException(HttpStatusCode code, string text, HttpResponseMessage msg) 
    : HttpException(text)
{
    public HttpStatusCode Code { get; } = code;
    public HttpResponseMessage Msg { get; } = msg;
}