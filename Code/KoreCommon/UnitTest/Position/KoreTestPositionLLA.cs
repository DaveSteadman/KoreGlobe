using System;
using System.Data.Entity.Hierarchy;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestPositionLLA
{
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestKoreLLAPointCreation(testLog);
            TestKoreLLAPointMovement(testLog);
            TestKoreLLAPoint_RangeBearing(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreTestPositionLLA RunTests", false, ex.Message);
        }
    }

    private static void TestKoreLLAPointCreation(KoreTestLog testLog)
    {
        var p = new KoreLLAPoint() { LatDegs = 10.0, LonDegs = 20.0, AltMslM = 1000.0 };

        bool okLat = KoreValueUtils.EqualsWithinTolerance(p.LatDegs, 10.0);
        bool okLon = KoreValueUtils.EqualsWithinTolerance(p.LonDegs, 20.0);
        bool okAlt = KoreValueUtils.EqualsWithinTolerance(p.AltMslM, 1000.0);
        testLog.AddResult("KoreLLAPoint Creation", okLat && okLon && okAlt);
    }

    private static void TestKoreLLAPointMovement(KoreTestLog testLog)
    {
        var start = new KoreLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 0.0 };
        var offset = new KoreAzElRange() { RangeM = 1000.0 };
        offset.AzDegs = 90.0;
        offset.ElDegs = 0.0;

        var dest = start.PlusPolarOffset(offset);

        double expectedLon = 1000.0 / KoreWorldConsts.EarthRadiusM * KoreConsts.RadsToDegsMultiplier;

        testLog.AddResult("KoreLLAPoint Movement Lat", KoreValueUtils.EqualsWithinTolerance(dest.LatDegs, 0.0, 0.0001));
        testLog.AddResult("KoreLLAPoint Movement Lon", KoreValueUtils.EqualsWithinTolerance(dest.LonDegs, expectedLon, 0.0001));
        testLog.AddResult("KoreLLAPoint Movement Alt", KoreValueUtils.EqualsWithinTolerance(dest.AltMslM, 0.0, 0.0001));

        var measured = start.StraightLinePolarOffsetTo(dest);
        testLog.AddResult("KoreLLAPoint PolarOffset Range", KoreValueUtils.EqualsWithinTolerance(measured.RangeM, 1000.0, 0.01));
        testLog.AddResult("KoreLLAPoint PolarOffset Az", KoreValueUtils.EqualsWithinTolerance(measured.AzDegs, 90.0, 0.001));

        var roundTrip = start.PlusPolarOffset(measured);
        bool sameLat = KoreValueUtils.EqualsWithinTolerance(roundTrip.LatDegs, dest.LatDegs, 0.01);
        bool sameLon = KoreValueUtils.EqualsWithinTolerance(roundTrip.LonDegs, dest.LonDegs, 0.01);
        bool sameAlt = KoreValueUtils.EqualsWithinTolerance(roundTrip.AltMslM, dest.AltMslM, 0.1);
        testLog.AddResult("KoreLLAPoint PolarOffset RoundTrip", sameLat && sameLon && sameAlt);
    }

    private static void TestKoreLLAPoint_RangeBearing(KoreTestLog testLog)
    {
        // Setup two points with non-trivial lat/lon/alt values
        var first  = new KoreLLAPoint() { LatDegs = 50.0, LonDegs = -1.0, AltMslM = 0.0 };
        var second = new KoreLLAPoint() { LatDegs = 50.1, LonDegs = -0.9, AltMslM = 0.0 };

        // Determine the delta between the two points
        KoreRangeBearing pointRangeBearing = first.RangeBearingTo(second);

        // Apply the range-bearing to the first point to get a new point, validating that the way we calculate it and the way we
        // apply it to a point are consistent.
        var moved = first.PlusRangeBearing(pointRangeBearing);

        bool sameLat = KoreValueUtils.EqualsWithinTolerance(moved.LatDegs, second.LatDegs, 0.0005);
        bool sameLon = KoreValueUtils.EqualsWithinTolerance(moved.LonDegs, second.LonDegs, 0.0005);
        bool sameAlt = KoreValueUtils.EqualsWithinTolerance(moved.AltMslM, second.AltMslM, 1.0);

        // create strings of all the comparisons, to expose some details of the checks.
        string latCompareStr = $"Lat: {first.LatDegs:F5} -> {second.LatDegs:F5}";
        string lonCompareStr = $"Lon: {first.LonDegs:F5} -> {second.LonDegs:F5}";
        string altCompareStr = $"Alt: {first.AltMslM:F1} -> {second.AltMslM:F1}";
        string rbStr = $"Range: {pointRangeBearing.RangeM:F1}m, Bearing: {pointRangeBearing.BearingDegs:F1}°";

        testLog.AddResult("KoreLLAPoint Delta Lat", sameLat, latCompareStr);
        testLog.AddResult("KoreLLAPoint Delta Lon", sameLon, lonCompareStr);
        testLog.AddResult("KoreLLAPoint Delta Alt", sameAlt, altCompareStr);
        testLog.AddComment($"KoreLLAPoint Delta RangeBearing: {rbStr}");
    }
}


