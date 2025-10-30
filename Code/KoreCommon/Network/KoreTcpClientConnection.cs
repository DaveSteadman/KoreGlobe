// <fileheader>

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

#nullable enable

namespace KoreCommon;


class KoreTcpClientConnection : KoreCommonConnection
{
    public string ipAddrStr;
    public IPAddress? ipAddress;
    public int port;

    public NetworkStream? stream;
    public TcpClient? client;

    private Thread? sendThread;
    private Thread? receiveThread;
    public bool connected;
    public DateTime lastUpdateTime;

    private int StartActivityCount;
    private int NumMsgsHandled;
    private string StatusString;

    public BlockingCollection<string> sendMsgQueue;

    // ------------------------------------------------------------------------------------------------------------

    public KoreTcpClientConnection()
    {
        ipAddrStr = "";
        ipAddress = null;
        port = 0;
        NumMsgsHandled = 0;

        stream = null;
        client = null;

        sendThread = null;
        receiveThread = null;
        connected = false;
        lastUpdateTime = DateTime.UtcNow;
        StartActivityCount = 0;

        sendMsgQueue = new BlockingCollection<string>();

        StatusString = "Startup";
    }

    // ------------------------------------------------------------------------------------------------------------

    public void setConnectionDetails(string inIpAddrStr, int inPort)
    {
        ipAddrStr = inIpAddrStr;
        port = inPort;
        ipAddress = IPAddress.Parse(ipAddrStr);
    }

    // ========================================================================================
    // override methods
    // ========================================================================================

    public override string type()
    {
        return "TcpClient";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override string connectionDetailsString()
    {
        string strConn = (connected) ? "Yes" : "No";
        return $"type:TcpClient // name:{Name} // addr:{ipAddrStr}:{port} // count:{NumMsgsHandled} // connected?:{strConn} // StartActivityCount:{StartActivityCount} // StatusString:{StatusString}";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void startConnection()
    {
        connected = false;
        StartActivityCount = (StartActivityCount + 1) % 1000;

        StatusString = "BeginConnect";

        client = new TcpClient();
        client.BeginConnect(ipAddress!, port, ConnectCallback, client);
    }

    public override void stopConnection()
    {
        // Stop threads and close UDP client
        connected = false;

        if (client != null)
            client.Close();

        if (sendThread != null)
            sendThread.Join();

        if (receiveThread != null)
            receiveThread.Join();
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void sendMessage(string msgData)
    {
        //byte[] messageBuffer = Encoding.ASCII.GetBytes(msgData);
        //stream.Write(messageBuffer, 0, messageBuffer.Length);

        sendMsgQueue.Add(msgData);
    }

    // ========================================================================================

    private void ConnectCallback(IAsyncResult result)
    {
        StatusString = "ConnectCallback";

        try
        {
            if (client != null)
            {
                // Complete the connection.
                client.EndConnect(result);

                if (client == null || !client.Connected)
                {
                    KoreCentralLog.AddEntry("Network: Connection failed: " + ipAddrStr + ":" + port);
                    connected = false;
                    StatusString = "Connection failed";
                    return;
                }

                stream = client.GetStream();
                connected = true;


                KoreCentralLog.AddEntry("Network: Connection success: " + ipAddrStr + ":" + port);
                StatusString = "Connected successfully";

                sendThread = new Thread(new ThreadStart(sendThreadFunc));
                receiveThread = new Thread(new ThreadStart(receiveThreadFunc));
                sendThread.Start();
                receiveThread.Start();
            }
            else
            {

                KoreCentralLog.AddEntry("Network: Server connection broken on creation: " + ipAddrStr + ":" + port);
                connected = false;
                StatusString = "Server connection broken on creation";
            }
        }
        catch (SocketException ex)
        {
            KoreCentralLog.AddEntry("Network: Socket Exception: " + ex.Message);
            connected = false;
            client?.Close();
            StatusString = "Socket Error: " + ex.Message;
        }
        catch (Exception ex)
        {
            KoreCentralLog.AddEntry("Network: General Exception: " + ex.Message);
            connected = false;
            client?.Close();
            StatusString = "Error: " + ex.Message;
        }
    }

    // ------------------------------------------------------------------------------------------------------------

    private void sendThreadFunc()
    {
        KoreCentralLog.AddEntry("Network: THREAD FUNC START: TcpClientConnection.sendThreadFunc() : " + Name);

        // Enter an infinite loop to process client connections.
        while (connected)
        {
            try
            {
                if (sendMsgQueue.TryTake(out string? nextMsg, 1000))
                {
                    if (nextMsg != null)
                    {
                        byte[] msgBuffer = Encoding.ASCII.GetBytes(nextMsg);
                        if (stream != null)
                            stream.Write(msgBuffer, 0, msgBuffer.Length);
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.Interrupted)
                    KoreCentralLog.AddEntry("EXCEPTION: SocketError.Interrupted // TcpClientConnection.sendThreadFunc");

                // listener was closed or broken, exit loop
                connected = false;
                client?.Close();
                continue; // to top of loop to exit
            }
        }
    }

    // ------------------------------------------------------------------------------------------------------------

    private void receiveThreadFunc()
    {
        KoreCentralLog.AddEntry("Network: THREAD FUNC START: TcpClientConnection.receiveThreadFunc() : " + Name);

        // Get the client and stream from the parameter.
        while (connected)
        {
            // Check if the client is still connected, of break out of the infinite loop, to the removal and exit.
            //if (!client.Connected)
            //{
            //    Console.WriteLine("Client disconnected");
            //    client.Close();
            //    break;
            //}
            try
            {
                //Console.WriteLine("BLOCKING ON READ: TcpClientConnection.receiveThreadFunc() : " + Name);
                KoreCentralLog.AddEntry("BLOCKING ON READ: TcpClientConnection.receiveThreadFunc() : " + Name);

                // BLOCKING: Read data from the client stream.
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                if (stream != null) bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    // listener was closed or broken, exit loop
                    connected = false;
                    continue; // to top of loop to exit
                }
                else
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // create a new message object to add to the incoming message queue.
                    QueueIncomingMessage(data);
                    if (NumMsgsHandled < 10000) NumMsgsHandled++;
                }
            }
            catch
            {
                //if (ex.SocketErrorCode == SocketError.Interrupted)
                //    Console.WriteLine("EXCEPTION: SocketError.Interrupted // TcpClientConnection.receiveThreadFunc");

                // listener was closed or broken, exit loop
                connected = false;
                client?.Close();
                continue; // to top of loop to exit
            }
        }
    }

    // ------------------------------------------------------------------------------------------------------------

}
