namespace CsTools.HttpRequest;

public class RequestInvalidOperationException(InvalidOperationException ioe) 
    : HttpException(ioe.Message, ioe) { }