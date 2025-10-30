// <fileheader>

using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestTriangle
{
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestInternalAnglesSum(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreTestTriangle RunTests", false, ex.Message);
        }
    }

    private static void TestInternalAnglesSum(KoreTestLog testLog)
    {
        var tri = new KoreXYTriangle(new KoreXYVector(0, 0), new KoreXYVector(1, 0), new KoreXYVector(0, 1));

        double angAB = tri.InternalAngleABRads();
        double angBC = tri.InternalAngleBCRads();
        double angCA = tri.InternalAngleCARads();
        double sum = angAB + angBC + angCA;

        bool withinRange =KoreNumericRange<double>.ZeroToPiRadians.IsInRange(angAB) &&
                          KoreNumericRange<double>.ZeroToPiRadians.IsInRange(angBC) &&
                          KoreNumericRange<double>.ZeroToPiRadians.IsInRange(angCA);
        testLog.AddResult("KoreXYTriangle Angles < π", withinRange);
        testLog.AddResult("KoreXYTriangle Angles Sum", KoreValueUtils.EqualsWithinTolerance(sum, Math.PI));
    }
}
