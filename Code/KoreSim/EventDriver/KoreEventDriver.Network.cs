
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
    public static void NetworkConnect(string connName, string connType, string ipAddrStr, int port)
    {
        var success = KoreConnectionTypeExtensions.TryParse(connType, out var type);
        if (success)
            KoreSimFactory.Instance.NetworkHub.CreateConnection(connName, type, ipAddrStr, port);
    }

    public static void NetworkDisconnect(string connName) => KoreSimFactory.Instance.NetworkHub.EndConnection(connName);

    public static string ReportLocalIP() => KoreSimFactory.Instance.NetworkHub.LocalIPAddrStr();

    // Usage: KoreEventDriver.NetworkReport
    public static string NetworkReport() => KoreSimFactory.Instance.NetworkHub.Report();

    public static void NetworkInjectIncoming(string message) => KoreSimFactory.Instance.NetworkHub.InjectIncomingMessage(message);

    public static bool HasIncomingMessage() => KoreSimFactory.Instance.NetworkHub.HasIncomingMessage();

    public static KoreMessageText? GetIncomingMessage() => KoreSimFactory.Instance.NetworkHub.GetIncomingMessage();

}