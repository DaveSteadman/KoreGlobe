using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static partial class KoreTestPosition
{
    private static void TestKoreXYPoint(KoreTestLog testLog)
    {
        // Example: Test creation of KoreXYPoint points and basic operations
        var pointA = new KoreXYPoint(1, 2);
        var pointB = new KoreXYPoint(4, 5);

        testLog.AddResult("KoreXYPoint Creation", pointA.X == 1 && pointA.Y == 2);

        double calcDistance = Math.Sqrt((4 - 1) * (4 - 1) + (5 - 2) * (5 - 2));
        testLog.AddResult("KoreXYPoint Distance", KoreValueUtils.EqualsWithinTolerance(pointA.DistanceTo(pointB), calcDistance, 0.001));


        // Add more tests for KoreXYZPoint
    }

    private static void TestKoreXYLine(KoreTestLog testLog)
    {

    }



}
