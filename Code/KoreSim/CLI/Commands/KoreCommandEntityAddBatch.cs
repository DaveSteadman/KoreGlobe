using System.Collections.Generic;

// KoreCommandEntityAdd
using KoreCommon;

namespace KoreSim;

public class KoreCommandEntityAddBatch : KoreCommand
{
    public KoreCommandEntityAddBatch()
    {
        Signature.Add("entity");
        Signature.Add("addbatch");
    }

    public override string HelpString => $"{SignatureString} <number_of_entities> <min lat degs> <max lat degs> <min lon degs> <max lon degs> <min alt msl m> <max alt msl m> <min speed m/s> <max speed m/s>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 8)
        {
            return "KoreCommandEntityAdd.Execute -> insufficient parameters";
        }

        string entityCount = parameters[0];
        string entityMinLat = parameters[1];
        string entityMaxLat = parameters[2];
        string entityMinLon = parameters[3];
        string entityMaxLon = parameters[4];
        string entityMinAlt = parameters[5];
        string entityMaxAlt = parameters[6];
        string entityMinSpeed = parameters[7];
        string entityMaxSpeed = parameters[8];

        //string retString = "";

        // convert to types
        int numEntities = int.Parse(entityCount);
        double minLat = double.Parse(entityMinLat);
        double maxLat = double.Parse(entityMaxLat);
        double minLon = double.Parse(entityMinLon);
        double maxLon = double.Parse(entityMaxLon);
        double minAlt = double.Parse(entityMinAlt);
        double maxAlt = double.Parse(entityMaxAlt);
        double minSpeed = double.Parse(entityMinSpeed);
        double maxSpeed = double.Parse(entityMaxSpeed);

        // check the number of entities is valid
        if ((numEntities < 1) || (numEntities > 99)) { return "KoreCommandEntityAdd.Execute -> invalid number of entities"; }

        // check the LLA bounds are okay
        if (!KoreNumericRange<double>.Minus90To90Degrees.IsInRange(minLat)) { return "KoreCommandEntityAdd.Execute -> invalid min latitude"; }
        if (!KoreNumericRange<double>.Minus90To90Degrees.IsInRange(maxLat)) { return "KoreCommandEntityAdd.Execute -> invalid max latitude"; }
        if (!KoreNumericRange<double>.Minus180To180Degrees.IsInRange(minLon)) { return "KoreCommandEntityAdd.Execute -> invalid min longitude"; }
        if (!KoreNumericRange<double>.Minus180To180Degrees.IsInRange(maxLon)) { return "KoreCommandEntityAdd.Execute -> invalid max longitude"; }

        KoreNumericRange<double> TypicalAltMSLRange = new KoreNumericRange<double>(100, 50000, RangeBehavior.Limit); // 100 to 50000
        if (!TypicalAltMSLRange.IsInRange(minAlt)) { return "KoreCommandEntityAdd.Execute -> invalid min altitude"; }
        if (!TypicalAltMSLRange.IsInRange(maxAlt)) { return "KoreCommandEntityAdd.Execute -> invalid max altitude"; }

        KoreNumericRange<double> TypicalSpeedMPSRange = new KoreNumericRange<double>(10, 1000, RangeBehavior.Limit); // 10 to 1000
        if (!TypicalSpeedMPSRange.IsInRange(minSpeed)) { return "KoreCommandEntityAdd.Execute -> invalid min speed"; }
        if (!TypicalSpeedMPSRange.IsInRange(maxSpeed)) { return "KoreCommandEntityAdd.Execute -> invalid max speed"; }

        // Now loop through the number of entities to add, creating random LLA and speed values within the specified ranges
        for (int i = 0; i < numEntities; i++)
        {
            string entityName = $"Entity_{i}";

            // Create the random attributes
            double randomLat = KoreNumericUtils.RandomInRange<double>(minLat, maxLat);
            double randomLon = KoreNumericUtils.RandomInRange<double>(minLon, maxLon);
            double randomAlt = KoreNumericUtils.RandomInRange<double>(minAlt, maxAlt);
            double randomSpeed = KoreNumericUtils.RandomInRange<double>(minSpeed, maxSpeed);
            double randomHeadingDegs = KoreNumericUtils.RandomInRange<double>(0, 360);

            KoreLLAPoint entityLLA = new KoreLLAPoint() { LatDegs = randomLat, LonDegs = randomLon, AltMslM = randomAlt };
            KoreCourse entityCourse = new KoreCourse() { HeadingDegs = randomHeadingDegs, SpeedMps = randomSpeed };

            // Call the event driver to perform the add
            KoreEventDriver.AddEntity(entityName);
            KoreEventDriver.SetEntityPosition(entityName, entityLLA);
            KoreEventDriver.SetEntityCourse(entityName, entityCourse);
        }

        KoreCentralLog.AddEntry($"KoreCommandEntityAddBatch.Execute -> DONE");
        return "DONE";
    }
}
