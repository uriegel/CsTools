namespace CsTools.HttpRequest;

public class TimeoutException : HttpException
{
    public TimeoutException() : base("Timeout occurred") {}
}