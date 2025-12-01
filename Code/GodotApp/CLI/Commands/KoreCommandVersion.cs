using System.Collections.Generic;

// KoreCommandVersion

using KoreCommon;
using KoreSim;
using KoreGIS;

public class KoreCommandVersion : KoreCommand
{
    public KoreCommandVersion()
    {
        Signature.Add("version");
    }

    public override string Execute(List<string> parameters)
    {
        string Text = $"{KoreAppConst.Version}";
        Text += $"\n{KoreGodotCommonConst.Version}";
        Text += $"\n{KoreSimConst.Version}";
        Text += $"\n{KoreGISConst.Version}";
        Text += $"\n{KoreCommonConst.Version}";

        return Text;
    }

}
