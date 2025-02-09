using System.Net;
using System.Net.Sockets;
using CsTools.Extensions;

namespace CsTools;

public record DualModeTcpListener(TcpListener Listener, bool Ipv6)
{
    public static DualModeTcpListener Create(int port)
    {
        try
        {
            var listener = new TcpListener(IPAddress.IPv6Any, port);
            listener.Server.SetDualMode();
            return new(listener, true);
        }
        catch (SocketException se)
        {
            if (se.SocketErrorCode != SocketError.AddressFamilyNotSupported)
                throw;
            var listener = new TcpListener(IPAddress.Any, port);
            return new(listener, false);
        }
    }
}
