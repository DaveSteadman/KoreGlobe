using System.Collections.Generic;

// KoreCommandSimPause
using KoreCommon;

namespace KoreSim;

public class KoreCommandSimPause : KoreCommand
{
    public KoreCommandSimPause()
    {
        Signature.Add("sim");
        Signature.Add("pause");
    }

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandSimPause.Execute");

        KoreSimFactory.Instance.ModelRun.Pause();

        return "Simulation paused";
    }
}