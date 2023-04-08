using System.Net.Sockets;

namespace CsTools.HttpRequest;

public class RequestSocketException : HttpException
{
    public RequestSocketException(SocketException se) : base(se.Message, se) {} 
}