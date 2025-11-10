using System.Collections.Generic;
using System.Text;

// KoreCommandNetworkReport
using KoreCommon;

namespace KoreSim;

public class KoreCommandNetworkMakeConnection : KoreCommand
{
    public KoreCommandNetworkMakeConnection()
    {
        Signature.Add("network");
        Signature.Add("makeconn");
    }

    public override string HelpString => $"{SignatureString} <connection_name> <connection_type> [address] [port]";

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandNetworkReport.Execute");

        if (parameters.Count != 4)
            return "KoreCommandEleSaveTile.Execute -> parameter count error";

        string connectionName = parameters[0];
        string connectionType = parameters[1];
        string address = parameters[2];
        string port = parameters[3];

        StringBuilder sb = new StringBuilder();

        sb.Append($"Ending connection: {connectionName}\n");

        KoreEventDriver.NetworkDisconnect(connectionName);

        sb.Append("Done.");

        return sb.ToString();
    }
}