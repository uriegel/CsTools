using System.Net.Sockets;

namespace CsTools.Extensions;

/// <summary>
/// Erweiterungsklasse für <see cref="System.Net.Sockets.Socket"/>
/// </summary>
public static class SocketExtensions
{
    /// <summary>
    /// Einstellen des DualModes, also der gleichzeitigen Unterstützung für IPv6 und IPv4.
    /// <list type="bullet">
    /// <listheader>
    /// <description>Einstellung für TcpServer:</description>
    /// </listheader>
    /// <item>
    /// <description>tcpServer.Server.SetDualMode()</description>
    /// </item>
    /// </list>
    /// <list type="bullet">
    /// <listheader>
    /// <description>Einstellung für TcpClient:</description>
    /// </listheader>
    /// <item>
    /// <description>tcpClient.Client.SetDualMode()</description>
    /// </item>
    /// </list>
    /// <remarks>
    /// Bei TcpServer "IPAddress.IPv6Any" angeben, bei TcpClient "AddressFamily.InterNetworkV6".
    /// </remarks>
    /// </summary>
    /// <param name="socket">Die Socket, in der der DulaMode aktiviert werden soll, bitte als Erweiterungsmethode aufrufen (TcpServer.Server.SetDualMode())!</param>
    public static void SetDualMode(this Socket socket) => socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)27, 0);
}
