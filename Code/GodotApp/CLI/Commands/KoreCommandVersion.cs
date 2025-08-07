using System.Collections.Generic;

// KoreCommandVersion

using KoreCommon;


public class KoreCommandVersion : KoreCommand
{
    public KoreCommandVersion()
    {
        Signature.Add("version");
    }

    public override string Execute(List<string> parameters)
    {
        return KoreGlobals.VersionString;
    }

}
