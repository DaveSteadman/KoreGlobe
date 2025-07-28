using System.Collections.Generic;

// KoreCommandSimStop
using KoreCommon;

namespace KoreSim;

public class KoreCommandSimStop : KoreCommand
{
    public KoreCommandSimStop()
    {
        Signature.Add("sim");
        Signature.Add("stop");
    }

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandSimStop.Execute");

        KoreSimFactory.Instance.ModelRun.Stop();

        return "Simulation stopped";
    }
}
