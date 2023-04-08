namespace CsTools.HttpRequest;

public class RequestInvalidOperationException : HttpException
{
    public RequestInvalidOperationException(InvalidOperationException ioe) : base(ioe.Message, ioe) {} 
}