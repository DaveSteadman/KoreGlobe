using System.Collections.Generic;

// KoreCommandEntityDelete

using KoreCommon;

namespace KoreSim;


public class KoreCommandEntityPosition : KoreCommand
{
    public KoreCommandEntityPosition()
    {
        Signature.Add("entity");
        Signature.Add("pos");
    }

    public override string HelpString => $"{SignatureString} <Entity_name> <LatDegs> <LonDegs> <AltMslM>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 4)
        {
            return "KoreCommandEntityPosition.Execute -> insufficient parameters";
        }

        string entityName = parameters[0];
        double latDegs = double.Parse(parameters[1]);
        double lonDegs = double.Parse(parameters[2]);
        double altMslM = double.Parse(parameters[3]);

        string retString = "";

        if (KoreEventDriver.DoesEntityExist(entityName))
        {
            KoreLLAPoint newLLA = new KoreLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, AltMslM = altMslM };

            KoreEventDriver.SetEntityStartLLA(entityName, newLLA);
            retString = $"Entity {entityName} Updated: Position: {newLLA}.";
        }
        else
        {
            retString = $"Entity {entityName} not found.";
        }

        KoreCentralLog.AddEntry($"KoreCommandEntityPosition.Execute -> {retString}");
        return retString;
    }
}
