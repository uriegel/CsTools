namespace CsTools.HttpRequest;

public class HostNotFoundException : HttpException
{
    public HostNotFoundException(string message)
        : base(message)
    {}
}
