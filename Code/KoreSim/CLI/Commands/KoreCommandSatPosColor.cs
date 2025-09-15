using System.Collections.Generic;
using System.Text;

// KoreCommandElePrep

using KoreCommon;

namespace KoreSim;

// CLI Usage: set loadtile c:/Util/_temp/sat_image_10_10_20_20.png 10 10 20 20
// CLI Usage: sat poscolor 15 15

#nullable enable

public class KoreCommandSatPosColor : KoreCommand
{
    public KoreCommandSatPosColor()
    {
        Signature.Add("sat");
        Signature.Add("poscolor");
    }

    public override string HelpString => $"{SignatureString} <lat degs> <lon degs>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        if (parameters.Count < 2)
        {
            return "KoreCommandEleLoadArc.Execute -> insufficient parameters";
        }

        double inLatDegs = double.Parse(parameters[0]);
        double inLonDegs = double.Parse(parameters[1]);

        KoreLLPoint checkPos = new KoreLLPoint() { LatDegs = inLatDegs, LonDegs = inLonDegs };

        // Get the color for the specified position
        KoreColorRGB color = KoreSimFactory.Instance.ImageManager.ColorForPoint(checkPos);

        sb.AppendLine($"Satellite Position Color:");
        sb.AppendLine($"- Check Position: {checkPos}");
        sb.AppendLine($"- Color: {KoreColorOps.ColorName(color)}");

        return sb.ToString();
    }
}