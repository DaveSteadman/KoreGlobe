using System.Collections.Generic;

// KoreCommandSimReset
using KoreCommon;

namespace KoreSim;

public class KoreCommandSimReset : KoreCommand
{
    public KoreCommandSimReset()
    {
        Signature.Add("sim");
        Signature.Add("reset");
    }

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandSimReset.Execute");

        KoreSimFactory.Instance.EntityManager.Reset(); // KoreEventDriver this
        KoreEventDriver.SimClockReset();

        return "EntityManager Reset (Entity positions to start)";
    }
}
