// <fileheader>

using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KoreCommon;

public class KoreCommandPause : KoreCommand
{
    public KoreCommandPause()
    {
        Signature.Add("pause");
    }

    public override string HelpString => $"{SignatureString} <duration seconds>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new ();

        if (parameters.Count != 1)
        {
            return "KoreCommandPause: Wrong number of parameters. Expected 1. Got " + parameters.Count;
        }

        if (!float.TryParse(parameters[0], out float duration))
        {
            return "KoreCommandPause: Invalid duration. Please provide a valid number.";
        }

        // Pause the console for the specified duration
        Thread.Sleep((int)(duration * 1000));
        sb.AppendLine($"KoreCommandPause.Execute -> Paused for {duration} seconds");

        // -------------------------------------------------

        return sb.ToString();
    }

}


