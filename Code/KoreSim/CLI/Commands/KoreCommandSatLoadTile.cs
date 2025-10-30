using System.Collections.Generic;
using System.Text;

// KoreCommandElePrep

using KoreCommon;

namespace KoreSim;

// CLI Usage: ele prep <inEleFilename> <inTileCode> <inOutDir> <action>
// CLI Usage: ele prep c:/Util/KorebeLibrary_MapPrep/Europe/W005N50_UkCentral/Ele_BF_BF_50m.asc BF_BF C:/Util/_temp yes

#nullable enable

public class KoreCommandSatLoadTile : KoreCommand
{
    public KoreCommandSatLoadTile()
    {
        Signature.Add("sat");
        Signature.Add("loadtile");
    }

    public override string HelpString => $"{SignatureString} <Image Filename> <min lat degs> <min lon degs> <max lat degs> <max lon degs> ";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count < 5)
        {
            return "KoreCommandEleLoadArc.Execute -> insufficient parameters";
        }

        string inImageFilename = parameters[0];
        double inMinLatDegs  = double.Parse(parameters[1]);
        double inMinLonDegs  = double.Parse(parameters[2]);
        double inMaxLatDegs  = double.Parse(parameters[3]);
        double inMaxLonDegs  = double.Parse(parameters[4]);

        KoreLLBox llBox = new KoreLLBox() {
            MinLatDegs = inMinLatDegs,
            MinLonDegs = inMinLonDegs,
            MaxLatDegs = inMaxLatDegs,
            MaxLonDegs = inMaxLonDegs };

        sb.AppendLine($"Satellite Image Load:");
        sb.AppendLine($"- inImageFilename: {inImageFilename}");
        sb.AppendLine($"- LLBox: {llBox}");

        bool validOperation = true;

        // -------------------------------------------------

        // Convert and validate the inputs
        if (!System.IO.File.Exists(inImageFilename))
        {
            sb.AppendLine($"File not found: {inImageFilename}");
            validOperation = false;
        }

        // -------------------------------------------------

        // if (validOperation)
        // {
        //     sb.AppendLine($"Valid operation: Progressing...");

        //     (bool success, string message) = KoreSimFactory.Instance.ImageManager.LoadTile(llBox, inImageFilename);

        //     sb.AppendLine($"- LoadTile: {KoreStringOps.StringForBoolPF(success)} - {message}");

        //     //float testEleVal = newTile!.ElevationData[1,1];

        //     //sb.AppendLine($"ele[1,1] = {testEleVal}");
        // }

        // -------------------------------------------------

        sb.AppendLine($"Elevation System Report:");
        //sb.AppendLine(KoreSimFactory.Instance.EleSystem.Report());

        return sb.ToString();

    }
}