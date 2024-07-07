namespace CsTools.HttpRequest;

public class HostNotFoundException(string message) 
    : HttpException(message) { }
