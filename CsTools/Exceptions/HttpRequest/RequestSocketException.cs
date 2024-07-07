using System.Net.Sockets;

namespace CsTools.HttpRequest;

public class RequestSocketException(SocketException se) 
    : HttpException(se.Message, se) { }