// <fileheader>

using System;

namespace KoreCommon;

#nullable enable

// Enum representing the different types of network connections supported.
public enum KoreConnectionType
{
    UdpSender,
    UdpReceiver,
    TcpClient,
    TcpServer,
    TcpServerClient
}

// Extension methods for KoreConnectionType enum to handle string conversion.
public static class KoreConnectionTypeExtensions
{
    // Converts the enum value to its string representation.
    // Usage: var typeString = KoreConnectionType.UdpSender.ToStringValue();
    public static string ToStringValue(this KoreConnectionType type)
    {
        return type switch
        {
            KoreConnectionType.UdpSender => "UdpSender",
            KoreConnectionType.UdpReceiver => "UdpReceiver",
            KoreConnectionType.TcpClient => "TcpClient",
            KoreConnectionType.TcpServer => "TcpServer",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown connection type")
        };
    }

    // Parses a string into a KoreConnectionType enum value.
    // Usage: var success = KoreConnectionTypeExtensions.TryParse("UdpSender", out var type);
    public static bool TryParse(string value, out KoreConnectionType type)
    {
        return value switch
        {
            "UdpSender" => SetType(out type, KoreConnectionType.UdpSender),
            "UdpReceiver" => SetType(out type, KoreConnectionType.UdpReceiver),
            "TcpClient" => SetType(out type, KoreConnectionType.TcpClient),
            "TcpServer" => SetType(out type, KoreConnectionType.TcpServer),
            _ => SetType(out type, default)
        };
    }

    private static bool SetType(out KoreConnectionType type, KoreConnectionType value)
    {
        type = value;
        return type != default || value == default;
    }

    // Parses a string into a KoreConnectionType enum value, throwing an exception if invalid.
    public static KoreConnectionType Parse(string value)
    {
        if (TryParse(value, out var type))
        {
            return type;
        }
        throw new ArgumentException($"Invalid connection type: {value}", nameof(value));
    }
}
