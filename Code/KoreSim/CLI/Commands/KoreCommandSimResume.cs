using System.Collections.Generic;

// KoreCommandSimStop

using KoreCommon;

namespace KoreSim;


public class KoreCommandSimResume : KoreCommand
{
    public KoreCommandSimResume()
    {
        Signature.Add("sim");
        Signature.Add("resume");
    }

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandSimResume.Execute");

        KoreSimFactory.Instance.ModelRun.Resume();

        return "Simulation resumed";
    }
}