namespace CsTools.HttpRequest;

public class HttpException : Exception
{
    public HttpException(string message, Exception? innerException = null) : base(message, innerException)
    { }
}