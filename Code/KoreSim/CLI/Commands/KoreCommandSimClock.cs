using System.Collections.Generic;

// KoreCommandSimClock
using KoreCommon;

namespace KoreSim;

public class KoreCommandSimClock : KoreCommand
{
    public KoreCommandSimClock()
    {
        Signature.Add("sim");
        Signature.Add("clock");
    }

    public override string Execute(List<string> parameters)
    {
        return $"SimClock: {KoreEventDriver.SimSeconds()}Secs";
    }
}
