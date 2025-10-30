// <fileheader>

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace KoreCommon;

class KoreUdpSender : KoreCommonConnection
{
    public string ipAddrStr;
    public int port;
    private IPEndPoint? remoteHost;

    private UdpClient? UdpClient;

    // ------------------------------------------------------------------------------------------------------------

    public KoreUdpSender()
    {
        // Initialize send queue and threads
        ipAddrStr = string.Empty;
        port = 0;
        remoteHost = null;
        UdpClient = null;
    }

    public void setConnectionDetails(string inIpAddrStr, int inPort)
    {
        ipAddrStr = inIpAddrStr;
        port = inPort;
        remoteHost = new IPEndPoint(IPAddress.Parse(ipAddrStr), port);
    }

    // ========================================================================================
    // override methods
    // ========================================================================================

    public override string type()
    {
        return "UdpSender";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override string connectionDetailsString()
    {
        return "type:UdpSender // name:" + Name + " // " + ipAddrStr + ":" + port;
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void startConnection()
    {
        // create Udp Port
        UdpClient = new UdpClient();

        // Not a blocking call.
        if (remoteHost != null)
            UdpClient?.Connect(remoteHost);
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void stopConnection()
    {
        // Stop threads and close Udp client
        UdpClient?.Close();
    }

    // ========================================================================================

    public override void sendMessage(string msgData)
    {
        // Start the async task, but don't wait for it to complete.
        // Be aware that this means exceptions from the task will be ignored.
        var _ = sendMessageAsync(msgData);
    }

    private async Task sendMessageAsync(string msgData)
    {
        try
        {
            if (UdpClient != null)
                await UdpClient.SendAsync(Encoding.ASCII.GetBytes(msgData), msgData.Length);
        }
        catch
        {
            // Handle sending error
        }
    }
    // ------------------------------------------------------------------------------------------------------------
}



