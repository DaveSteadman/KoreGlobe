using System.Collections.Generic;

// KoreCommandEntityDelete
using KoreCommon;

namespace KoreSim;

public class KoreCommandEntityCourse : KoreCommand
{
    public KoreCommandEntityCourse()
    {
        Signature.Add("entity");
        Signature.Add("course");
    }

    public override string HelpString => $"{SignatureString} <Entity_name> <HeadingDegs> <SpeedKph>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "KoreCommandEntityCourse.Execute -> insufficient parameters";
        }

        string entityName    = parameters[0];
        double headingDegs = double.Parse(parameters[1]);
        double speedKph    = double.Parse(parameters[2]);

        string retString = "";

        if (KoreEventDriver.DoesEntityExist(entityName))
        {
            KoreCourse newCourse = new KoreCourse() { HeadingDegs = headingDegs, SpeedKph = speedKph };

            KoreEventDriver.SetEntityCourse(entityName, newCourse);
            retString = $"Entity {entityName} Updated: Course: {newCourse}.";
        }
        else
        {
            retString = $"Entity {entityName} not found.";
        }

        KoreCentralLog.AddEntry($"KoreCommandEntityCourse.Execute -> {retString}");
        return retString;
    }
}
