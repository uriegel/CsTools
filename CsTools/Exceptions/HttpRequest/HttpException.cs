namespace CsTools.HttpRequest;

public class HttpException(string message, Exception? innerException = null)
    : Exception(message, innerException) { }