using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestLine
{
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestCreateSimpleLine(testLog);
            TestOffsetLine(testLog);
            TestInterpolateAndExtendLine(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreTestLine RunTests", false, ex.Message);
            return;
        }
    }

    // Simple creation and attribute checks
    private static void TestCreateSimpleLine(KoreTestLog testLog)
    {
        var line = new KoreXYLine(new KoreXYPoint(0, 0), new KoreXYPoint(3, 4));

        // 3,4,5 triangle check
        testLog.AddResult("KoreXYLine Length", KoreValueUtils.EqualsWithinTolerance(line.Length, 5.0));

        // midpoint check
        var mid = line.MidPoint();
        testLog.AddResult("KoreXYLine MidPoint", KoreXYPoint.EqualsWithinTolerance(mid, new KoreXYPoint(1.5, 2.0)));
    }

    // Offset the line by a vector (2,3) and check the new points
    private static void TestOffsetLine(KoreTestLog testLog)
    {
        var line = new KoreXYLine(new KoreXYPoint(0, 0), new KoreXYPoint(1, 1));
        var offset = line.Offset(2, 3);
        bool p1 = KoreXYPoint.EqualsWithinTolerance(offset.P1, new KoreXYPoint(2, 3));
        bool p2 = KoreXYPoint.EqualsWithinTolerance(offset.P2, new KoreXYPoint(3, 4));
        testLog.AddResult("KoreXYLine Offset", p1 && p2);
    }

    private static void TestInterpolateAndExtendLine(KoreTestLog testLog)
    {
        var line = new KoreXYLine(new KoreXYPoint(0, 0), new KoreXYPoint(2, 0));
        var half = line.Fraction(0.5);
        testLog.AddResult("KoreXYLine Fraction 0.5", KoreXYPoint.EqualsWithinTolerance(half, new KoreXYPoint(1, 0)));

        // A positive extrapolation distance adds to the end of the line, so 2 + new 3 = 5
        KoreXYPoint extr = KoreXYLineOps.ExtrapolateDistance(line, 3);
        testLog.AddResult("KoreXYLine ExtrapolateDistance 3", KoreXYPoint.EqualsWithinTolerance(extr, new KoreXYPoint(5, 0)), extr.ToString());

        KoreXYPoint extr2 = KoreXYLineOps.ExtrapolateDistance(line, -2);
        testLog.AddResult("KoreXYLine ExtrapolateDistance -2", KoreXYPoint.EqualsWithinTolerance(extr2, new KoreXYPoint(-2, 0)), extr2.ToString());

        var extended = KoreXYLineOps.ExtendLine(line, 1, 1);
        bool p1 = KoreXYPoint.EqualsWithinTolerance(extended.P1, new KoreXYPoint(-1, 0));
        bool p2 = KoreXYPoint.EqualsWithinTolerance(extended.P2, new KoreXYPoint(3, 0));
        testLog.AddResult("KoreXYLineOps ExtendLine", p1 && p2 && KoreValueUtils.EqualsWithinTolerance(extended.Length, 4.0));
    }


}
