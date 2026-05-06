namespace CsTools.HttpRequest;

public class ConnectionRefusedException(string text) 
    : HttpException(text)
{
}