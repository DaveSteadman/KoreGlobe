using System.Collections.Generic;

// KoreCommandEntityCourseDelta
using KoreCommon;

namespace KoreSim;

public class KoreCommandEntityCourseDelta : KoreCommand
{
    public KoreCommandEntityCourseDelta()
    {
        Signature.Add("entity");
        Signature.Add("coursedelta");
    }

    public override string HelpString => $"{SignatureString} <Entity_name> <speedChangeMpMps> <headingChangeClockwiseDegsSec>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "KoreCommandEntityCourseDelta.Execute -> insufficient parameters";
        }

        string entityName    = parameters[0];
        double speedChangeMpMps              = double.Parse(parameters[1]);
        double headingChangeClockwiseDegsSec = double.Parse(parameters[2]);

        string retString = "";

        if (KoreEventDriver.DoesEntityExist(entityName))
        {
            KoreCourseDelta newCourseDelta = new KoreCourseDelta() {
                    SpeedChangeMpMps = speedChangeMpMps,
                    HeadingChangeClockwiseDegsSec = headingChangeClockwiseDegsSec };

            KoreEventDriver.SetEntityCourseDelta(entityName, newCourseDelta);
            retString = $"Entity {entityName} Updated: Course: {newCourseDelta}.";
        }
        else
        {
            retString = $"Entity {entityName} not found.";
        }

        KoreCentralLog.AddEntry($"KoreCommandEntityCourseDelta.Execute -> {retString}");
        return retString;
    }
}
