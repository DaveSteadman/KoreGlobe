
using System;

using KoreCommon;
namespace KoreSim;

#nullable enable

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class KoreEventDriver
{
    public static void StartNetworking()
    {

    }

    public static void StopNetworking()
    {

    }

    // Usage: KoreEventDriver.NetworkConnect("TcpClient", "TcpClient", "127.0.0.1", 12345);
    public static void NetworkConnect(string connName, string connType, string ipAddrStr, int port) => KoreSimFactory.Instance.NetworkHub.createConnection(connName, connType, ipAddrStr, port);

    public static void NetworkDisconnect(string connName) => KoreSimFactory.Instance.NetworkHub.endConnection(connName);

    public static string ReportLocalIP() => KoreSimFactory.Instance.NetworkHub.localIPAddrStr();

    // Usage: KoreEventDriver.NetworkReport
    public static string NetworkReport() => KoreSimFactory.Instance.NetworkHub.Report();

    public static void NetworkInjectIncoming(string message) => KoreSimFactory.Instance.NetworkHub.InjectIncomingMessage(message);

    public static bool HasIncomingMessage() => KoreSimFactory.Instance.NetworkHub.HasIncomingMessage();

    public static KoreMessageText? GetIncomingMessage() => KoreSimFactory.Instance.NetworkHub.GetIncomingMessage();

}