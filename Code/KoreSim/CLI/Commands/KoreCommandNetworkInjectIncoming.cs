using System.Collections.Generic;

// KoreCommandNetworkReport
using KoreCommon;

namespace KoreSim;

public class KoreCommandNetworkInjectIncoming : KoreCommand
{
    public KoreCommandNetworkInjectIncoming()
    {
        Signature.Add("network");
        Signature.Add("ii");
    }

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandNetworkInjectIncoming.Execute");

        // fail if there are no further parameters
        if (parameters.Count < 1)
        {
            return "Error: No parameters provided";
        }

        // concatenate the parameters into a single string with spaces
        string message = string.Join(" ", parameters);

        KoreEventDriver.NetworkInjectIncoming(message);

        // trim the message to 100 characters
        if (message.Length > 100)
        {
            message = message.Substring(0, 100);
            message += "...";
        }

        return $"Message injected: {message}";
    }
}