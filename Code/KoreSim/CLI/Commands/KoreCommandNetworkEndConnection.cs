using System.Collections.Generic;
using System.Text;

// KoreCommandNetworkReport
using KoreCommon;

namespace KoreSim;

public class KoreCommandNetworkEndConnection : KoreCommand
{
    public KoreCommandNetworkEndConnection()
    {
        Signature.Add("network");
        Signature.Add("endconn");
    }

    public override string HelpString => $"{SignatureString} <connection_name>";

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandNetworkReport.Execute");

        if (parameters.Count != 1)
            return "KoreCommandEleSaveTile.Execute -> parameter count error";

        string connectionName = parameters[0];

        StringBuilder sb = new StringBuilder();

        sb.Append($"Ending connection: {connectionName}\n");

        KoreEventDriver.NetworkDisconnect(connectionName);

        sb.Append("Done.");

        return sb.ToString();
    }
}