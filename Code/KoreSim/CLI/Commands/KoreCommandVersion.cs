using System.Collections.Generic;

// KoreCommandVersion

using KoreCommon;

namespace KoreSim;


public class KoreCommandVersion : KoreCommand
{
    public KoreCommandVersion()
    {
        Signature.Add("version");
    }

    public override string Execute(List<string> parameters)
    {
        // KoreCentralLog.AddEntry("KoreCommandVersion.Execute: " + KoreGlobals.VersionString);
        // return KoreGlobals.VersionString;

        return "Version";

    }

}
