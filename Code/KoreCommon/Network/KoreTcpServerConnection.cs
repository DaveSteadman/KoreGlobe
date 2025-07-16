using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#nullable enable

namespace KoreCommon;


class KoreTcpServerConnection : KoreCommonConnection
{
    public string ipAddrStr;
    public IPAddress? ipAddress;
    public int port;
    public bool running;
    public TcpListener? listener;

    public KoreNetworkHub? commsHub;

    // A flag to indicate whether the server is running.
    private Thread? serverThread;

    private string StatusString;
    // ------------------------------------------------------------------------------------------------------------

    public KoreTcpServerConnection()
    {
        ipAddrStr = "";
        ipAddress = null;
        port = 0;

        running = false;
        listener = null;

        StatusString = "No Status";

        commsHub = null;
        serverThread = null;
    }

    public void setConnectionDetails(string inIpAddrStr, int inPort)
    {
        ipAddrStr = inIpAddrStr;
        ipAddress = IPAddress.Parse(ipAddrStr);
        port = inPort;
    }

    // ========================================================================================
    // override methods
    // ========================================================================================

    public override string type()
    {
        return "TcpServer";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override string connectionDetailsString()
    {
        return $"type:TcpServer // name:{Name} // addr:{ipAddrStr}:{port} // Status:{StatusString}";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void startConnection()
    {
        // Process the client connection in a new thread.
        serverThread = new Thread(new ThreadStart(serverThreadFunc));
        serverThread?.Start();
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void stopConnection()
    {
        // Set the running flag to false.
        running = false;
        listener?.Stop();
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void sendMessage(string msgData)
    {
        // byte[] messageBuffer = Encoding.ASCII.GetBytes(msgData);
        // clientConfig.stream.Write(messageBuffer, 0, messageBuffer.Length);
    }

    // ========================================================================================

    private void serverThreadFunc()
    {
        KoreCentralLog.AddEntry($"Server Thread: Starting");

        // Create a new Tcp listener object.
        // Start listening for client connections.
        try
        {
            if (ipAddress == null) throw new Exception("ipAddress == null");

            listener = new TcpListener(ipAddress, port);
            listener.Start();

            // Set the running flag to true.
            running = true;

        }
        catch (SocketException ex)
        {
            StatusString = $"FAIL new TcpListener: {ex.Message}";
            running = false;
        }

        // Enter an infinite loop to process client connections.
        while (running)
        {
            TcpClient newClient;
            StatusString = "Awaiting Connection";

            try
            {
                // Accept a new client connection.
                // BLOCKING. Will throw on listener.Stop() call when stopping thread
                KoreCentralLog.AddEntry("Server Thread: Waiting for connections...");
                if (listener == null)
                    throw new InvalidOperationException("TcpListener is not initialized.");
                newClient = listener.AcceptTcpClient();
                StatusString = "AcceptTcpClient";
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.Interrupted)
                    KoreCentralLog.AddEntry("EXCEPTION: SocketError.Interrupted // TcpClientConnection.receiveThreadFunc");

                StatusString = "EXCEPTION: SocketError.Interrupted";

                // listener was closed or broken, exit loop
                running = false;
                continue; // to top of loop to exit
            }

            StatusString = "New connection";

            // Create the new connection object and add it to the collection.
            KoreCentralLog.AddEntry("KoreTcpServerConnection New connection...");
            KoreTcpServerClientConnection newClientConnection = new KoreTcpServerClientConnection()
            {
                Name = Name + "_client",
                IncomingQueue = IncomingQueue,
                IncomingMessageLog = new List<KoreMessageText>() // Fix: Change the type to List<KoreNetworking.CommsMessage>
            };
            newClientConnection.stream = newClient.GetStream();
            newClientConnection.client = newClient;
            newClientConnection.lastUpdateTime = DateTime.Now;
            newClientConnection.setupIncomingQueue(IncomingQueue);
            newClientConnection.ParentConnectionName = Name;

            if (commsHub != null)
                commsHub.connections.Add(newClientConnection);

            // Start the connection
            newClientConnection.startConnection();
        }

        // Stop the listener.
        listener?.Stop();
    }

} // class

