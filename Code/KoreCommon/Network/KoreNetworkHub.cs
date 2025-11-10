// <fileheader>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KoreCommon;

#nullable enable

// Namespace encapulating the network comms classes and functionality.
// - The rest of the application should be able to deal solely with messages.

public class KoreNetworkHub
{
    // List of all the connections (from a CommonConnection parent class).
    public List<KoreCommonConnection> connections;

    // List of all incoming messages, containing the message data and a connection identifier.
    public BlockingCollection<KoreMessageText> IncomingQueue;

    private CancellationTokenSource _cts;
    //bool ClientConnectWatchdog;

    private int WatchDogActivityCount; // Simple counter to show watchdog is running.

    // ------------------------------------------------------------------------------------------------------------
    // MARK: Constructor / Destructor
    // ------------------------------------------------------------------------------------------------------------

    public KoreNetworkHub()
    {
        connections = new List<KoreCommonConnection>();
        IncomingQueue = new BlockingCollection<KoreMessageText>();

        //ClientConnectWatchdog = true;
        _cts = new CancellationTokenSource();
        WatchDogActivityCount = 0;
        Task.Run(async () => await ConnectionWatchdog(_cts.Token));
    }

    // Destructor stops each of the connections (with their threads and blocking calls) and clears down the connection collection.
    ~KoreNetworkHub()
    {
        // Stop all threads
        foreach (KoreCommonConnection connection in connections)
        {
            connection.StopConnection();
        }
        connections.Clear();

        _cts.Cancel();
        //ClientConnectWatchdog = false;
    }

    // ------------------------------------------------------------------------------------------------------------
    // MARK: Connections
    // ------------------------------------------------------------------------------------------------------------

    public void CreateConnection(string connName, KoreConnectionType connType, string ipAddrStr, int port)
    {
        // End any pre-existing connection with the name we would look to use.
        EndConnection(connName);

        switch (connType)
        {
            case KoreConnectionType.UdpSender:
                {
                    // Create connection
                    KoreUdpSender UdpSender = new KoreUdpSender()
                    {
                        Name = connName,
                        IncomingQueue = IncomingQueue,
                        IncomingMessageLog = new List<KoreMessageText>() // Assuming IncomingMessageLog is a List<CommsMessage>.
                    };

                    connections.Add(UdpSender);
                    UdpSender.SetConnectionDetails(ipAddrStr, port);
                    UdpSender.StartConnection();
                    break;
                }

            case KoreConnectionType.UdpReceiver:
                {
                    // Create connection
                    KoreUdpReceiver UdpReceiver = new KoreUdpReceiver()
                    {
                        Name = connName,
                        IncomingQueue = IncomingQueue,
                        IncomingMessageLog = new List<KoreMessageText>()
                    };

                    connections.Add(UdpReceiver);
                    UdpReceiver.SetConnectionDetails(port);
                    UdpReceiver.StartConnection();
                    break;
                }

            case KoreConnectionType.TcpClient:
                {
                    // Create connection
                    KoreTcpClientConnection clientConnection = new KoreTcpClientConnection()
                    {
                        Name = connName,
                        IncomingQueue = IncomingQueue,
                        IncomingMessageLog = new List<KoreMessageText>()
                    };

                    connections.Add(clientConnection);
                    clientConnection.SetConnectionDetails(ipAddrStr, port);
                    // clientConnection.startConnection(); Let the watchdog do it.
                    break;
                }

            case KoreConnectionType.TcpServer:
                {
                    // Create connection
                    KoreTcpServerConnection serverConnection = new KoreTcpServerConnection()
                    {
                        Name = connName,
                        IncomingQueue = IncomingQueue,
                        IncomingMessageLog = new List<KoreMessageText>()
                    };

                    serverConnection.commsHub = this;

                    connections.Add(serverConnection);
                    serverConnection.SetConnectionDetails(ipAddrStr, port);
                    serverConnection.StartConnection();
                    break;
                }

            default:
                KoreCentralLog.AddEntry($"Unsupported connection type: {connType}");
                break;
        }
    }

    // -------------------------------------------------------------------------------------------------------

    public void EndConnection(string connName)
    {
        // We may be removing a server connection, so first block deals with removing children on that connection:
        {
            List<KoreCommonConnection> toRemove = new List<KoreCommonConnection>();

            foreach (KoreCommonConnection conn in connections)
            {
                if (conn.Name == connName)
                {
                    toRemove.Add(conn);

                    // If this is a server connection, also remove its clients
                    if (conn is KoreTcpServerConnection)
                    {
                        foreach (var client in connections.OfType<KoreTcpServerClientConnection>())
                        {
                            if (client.ParentConnectionName == connName)
                            {
                                toRemove.Add(client);
                            }
                        }
                    }
                }
            }

            foreach (var conn in toRemove)
            {
                conn.StopConnection();
                connections.Remove(conn);
            }
        }

        // Remove the named connection
        connections.RemoveAll(conn =>
        {
            if (conn.Name == connName)
            {
                conn.StopConnection();
                return true;
            }
            return false;
        });
    }

    // -------------------------------------------------------------------------------------------------------

    public void EndAllConnections()
    {
        while (connections.Count > 0)
        {
            KoreCommonConnection currConn = connections.First();
            currConn.StopConnection();
            connections.Remove(currConn);
        }
    }

    // ------------------------------------------------------------------------------------------------------------
    // MARK: Send/output messages
    // ------------------------------------------------------------------------------------------------------------

    public void SendMessage(string connName, string msgData)
    {
        bool messageSent = false;
        foreach (KoreCommonConnection conn in connections)
        {
            if (conn.Name == connName)
            {
                conn.SendMessage(msgData);
                messageSent = true;
            }
        }

        if (!messageSent)
        {
            KoreCentralLog.AddEntry("SEND FAIL:" + connName + " // " + msgData);
        }
    }

    // ------------------------------------------------------------------------------------------------------------
    // MARK: Receive/Incoming messages
    // ------------------------------------------------------------------------------------------------------------

    public bool HasIncomingMessage()
    {
        return (IncomingQueue.Count > 0);
    }

    // -------------------------------------------------------------------------------------------------------

    public bool GetIncomingMessage(out KoreMessageText msg)
    {
        return IncomingQueue.TryTake(out msg, 100);
    }

    public KoreMessageText? GetIncomingMessage()
    {
        if (IncomingQueue.Count == 0)
        {
            return null;
        }
        return IncomingQueue.Take();
    }

    // ------------------------------------------------------------------------------------------------------------
    // MARK: Misc Functions
    // ------------------------------------------------------------------------------------------------------------

    public string LocalIPAddrStr()
    {
        string hostName = Dns.GetHostName();
        IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
        IPAddress[] addresses = hostEntry.AddressList;
        IPAddress[] ipv4Addresses = addresses.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

        foreach (IPAddress address in ipv4Addresses)
        {
            return address.ToString();
        }
        return "<noaddr>";
    }

    // -------------------------------------------------------------------------------------------------------

    public string Report()
    {
        string outTxt = "NetworkHub Report:\n";

        outTxt += $"- LocalIP {LocalIPAddrStr()}\n";
        outTxt += $"- WatchDogActivityCount {WatchDogActivityCount}\n";

        foreach (KoreCommonConnection conn in connections)
        {
            outTxt += "- " + conn.ConnectionDetailsString() + "\n";
        }

        return outTxt;
    }

    // -------------------------------------------------------------------------------------------------------

    public async Task ConnectionWatchdog(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            WatchDogActivityCount = (WatchDogActivityCount + 1) % 1000;

            foreach (KoreCommonConnection currConn in connections)
            {
                if (currConn is KoreTcpClientConnection)
                {
                    KoreTcpClientConnection? currConnTcp = currConn as KoreTcpClientConnection;

                    if (currConnTcp != null && !currConnTcp.connected)
                    {
                        KoreCentralLog.AddEntry("Watchdog starting: " + currConnTcp.ConnectionDetailsString());
                        currConnTcp.StartConnection();
                    }
                }
            }

            //Task.Delay(1000);
            await Task.Delay(5000, token);
        }
        KoreCentralLog.AddEntry($"Network Comms Hub: Watchdog Ended");
    }

    // -------------------------------------------------------------------------------------------------------

    // Debug function, to allow the application to inject incoming messages

    public void InjectIncomingMessage(string msgText)
    {
        KoreMessageText injectMsg = new KoreMessageText() { connectionName = "INJECTED", msgData = msgText };
        IncomingQueue.Add(injectMsg);

        KoreCentralLog.AddEntry($"Network Comms Hub: Injected message: {msgText}");
    }

}


