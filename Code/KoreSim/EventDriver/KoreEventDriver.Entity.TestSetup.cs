
using System;
using System.Collections.Generic;

#nullable enable

using KoreCommon;

namespace KoreSim;

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public  static partial class KoreEventDriver
{
    public static void SetupTestEntities()
    {
        KoreCentralLog.AddEntry("Creating Test Entity Entities");

        KoreLLAPoint loc1 = new KoreLLAPoint() { LatDegs = 52.1, LonDegs = -4.2, AltMslM = 5000 };
        KoreLLAPoint loc2 = new KoreLLAPoint() { LatDegs = 52.5, LonDegs = 0.3, AltMslM = 2000 };
        KoreLLAPoint loc3 = new KoreLLAPoint() { LatDegs = 52.9, LonDegs = 8.1, AltMslM = 1000 };
        KoreLLAPoint loc4 = new KoreLLAPoint() { LatDegs = 32.5, LonDegs = -117.2, AltMslM = 8000 };

        KoreCourse course1 = new KoreCourse() { HeadingDegs = 270, SpeedKph = 800.08 };
        KoreCourse course2 = new KoreCourse() { HeadingDegs = 180, SpeedKph = 800.08 };
        KoreCourse course3 = new KoreCourse() { HeadingDegs = 90, SpeedKph = 800.08 };
        KoreCourse course4 = new KoreCourse() { HeadingDegs = 30, SpeedKph = 200.00 };

        KoreCourseDelta courseDelta1 = new KoreCourseDelta() { HeadingChangeClockwiseDegsSec = 2, SpeedChangeMpMps = 0 };
        KoreCourseDelta courseDelta2 = new KoreCourseDelta() { HeadingChangeClockwiseDegsSec = -3, SpeedChangeMpMps = 0 };
        KoreCourseDelta courseDelta3 = new KoreCourseDelta() { HeadingChangeClockwiseDegsSec = 1.2, SpeedChangeMpMps = 0 };
        KoreCourseDelta courseDelta4 = new KoreCourseDelta() { HeadingChangeClockwiseDegsSec = 0.1, SpeedChangeMpMps = 0 };

        KoreAttitude att1 = new KoreAttitude() { PitchUpDegs = 0, RollClockwiseDegs = 20, YawClockwiseDegs = 0 };
        KoreAttitude att3 = new KoreAttitude() { PitchUpDegs = 0, RollClockwiseDegs = 12, YawClockwiseDegs = 0 };

        AddEntity("TEST-F16", "F16");
        SetEntityStart("TEST-F16", loc1, course1, att1);
        SetEntityCourseDelta("TEST-F16", courseDelta1);

        // AddEntity("TEST-F18", "F18");
        // SetEntityStartLLA("TEST-F18", loc2);
        // SetEntityCourse("TEST-F18", course2);
        // SetEntityCourseDelta("TEST-F18", courseDelta2);

        AddEntity("TEST-Torn", "Tornado");
        SetEntityStart("TEST-Torn", loc3, course3, att3);
        SetEntityCourseDelta("TEST-Torn", courseDelta3);

        // AddEntity("TEST-MQ9", "MQ9Reaper");
        // SetEntityStartLLA("TEST-MQ9", loc4);
        // SetEntityCourse("TEST-MQ9", course4);
        // SetEntityCourseDelta("TEST-MQ9", courseDelta4);

        // {
        //     KoreLLAPoint    loc         = new KoreLLAPoint()    { LatDegs = 52.8, LonDegs =    -4.28, AltMslM = 0 };
        //     KoreCourse      course      = new KoreCourse()      { HeadingDegs = 1850, SpeedKph = 10 };
        //     KoreCourseDelta courseDelta = new KoreCourseDelta() { HeadingChangeClockwiseDegsSec =  0, SpeedChangeMpMps = 0 };
        //     AddEntity("TEST-Ship1", "SupportShip");
        //     SetEntityStartLLA("TEST-Ship1", loc);
        //     SetEntityCourse("TEST-Ship1", course);
        //     SetEntityCourseDelta("TEST-Ship1", courseDelta);
        // }

        {
            KoreLLAPoint locIOW1 = new KoreLLAPoint() { LatDegs = 50.580463, LonDegs = -1.300776, AltMslM = 104.9 };
            KoreCourse courseIOW1 = new KoreCourse() { HeadingDegs = 90, SpeedKph = 0.0 };
            KoreAttitude attIOW1 = new KoreAttitude() { PitchUpDegs = 0, RollClockwiseDegs = 0, YawClockwiseDegs = 0 };

            AddEntity("TEST-SAM", "S400Radar");
            SetEntityStart("TEST-SAM", locIOW1, courseIOW1, attIOW1);
        }
    }

}


