// <fileheader>

using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestXYZVector
{
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestConstruction(testLog);
            TestNormalization(testLog);
            TestMagnitude(testLog);
            TestAddition(testLog);
            TestSubtraction(testLog);
            TestScaling(testLog);
            TestDotProduct(testLog);
            TestCrossProduct(testLog);
            TestArbitraryPerpendicular(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreXYZVector Tests", false, ex.Message);
        }
    }

    private static void TestConstruction(KoreTestLog testLog)
    {
        var v = new KoreXYZVector(1, 2, 3);
        testLog.AddResult("KoreXYZVector Construction", v.X == 1 && v.Y == 2 && v.Z == 3);
    }

    private static void TestNormalization(KoreTestLog testLog)
    {
        var v = new KoreXYZVector(3, 0, 4);
        var norm = v.Normalize();
        double mag = Math.Sqrt(norm.X * norm.X + norm.Y * norm.Y + norm.Z * norm.Z);
        testLog.AddResult("KoreXYZVector Normalize", KoreValueUtils.EqualsWithinTolerance(mag, 1.0));
    }

    private static void TestMagnitude(KoreTestLog testLog)
    {
        // Magnitude using a quick 3, 4, 5 triangle example:
        var v = new KoreXYZVector(3, 4, 0);
        double mag = v.Magnitude;
        testLog.AddResult("KoreXYZVector Magnitude", KoreValueUtils.EqualsWithinTolerance(mag, 5.0));
    }

    private static void TestAddition(KoreTestLog testLog)
    {
        var v1 = new KoreXYZVector(1, 2, 3);
        var v2 = new KoreXYZVector(4, 5, 6);
        var sum = v1 + v2;
        testLog.AddResult("KoreXYZVector Addition", sum.X == 5 && sum.Y == 7 && sum.Z == 9);
    }

    private static void TestSubtraction(KoreTestLog testLog)
    {
        var v1 = new KoreXYZVector(4, 5, 6);
        var v2 = new KoreXYZVector(1, 2, 3);
        var diff = v1 - v2;
        testLog.AddResult("KoreXYZVector Subtraction", diff.X == 3 && diff.Y == 3 && diff.Z == 3);
    }

    private static void TestScaling(KoreTestLog testLog)
    {
        var v = new KoreXYZVector(1, -2, 3);
        var scaled = v * 2;
        testLog.AddResult("KoreXYZVector Scaling", scaled.X == 2 && scaled.Y == -4 && scaled.Z == 6);
    }

    private static void TestDotProduct(KoreTestLog testLog)
    {
        // Quick explanation of dot product and the answer here:
        // - The dot product of two vectors gives a scalar value that represents the cosine of
        //   the angle between them multiplied by their magnitudes.
        // - The dot product of two vectors (a, b, c) and (d, e, f) is calculated as:
        // -     a*d + b*e + c*f
        // - For vectors (1, 2, 3) and (4, -5, 6), the dot product is:
        // -     1*4 + 2*(-5) + 3*6 = 4 - 10 + 18 = 12
        // - So the expected result is 12.
        var v1 = new KoreXYZVector(1, 2, 3);
        var v2 = new KoreXYZVector(4, -5, 6);
        double dot = KoreXYZVector.DotProduct(v1, v2);
        double expectedDot = 1 * 4 + 2 * (-5) + 3 * 6; // This should equal 12
        testLog.AddResult("KoreXYZVector DotProduct", KoreValueUtils.EqualsWithinTolerance(dot, expectedDot));
    }

    private static void TestCrossProduct(KoreTestLog testLog)
    {
        // Quick explanation of cross product and the answer here:
        // - The cross product of two vectors results in a vector that is perpendicular to both.
        // - The formula for the cross product of vectors (a, b, c) and (d, e, f) is:
        // -     (bf - ce, cd - af, ae - bd)
        // - For vectors (1, 2, 3) and (4, 5, 6), the cross product is:
        // -     (2*6 - 3*5, 3*4 - 1*6, 1*5 - 2*4) = (-3, 6, -3)
        var v1 = new KoreXYZVector(1, 2, 3);
        var v2 = new KoreXYZVector(4, 5, 6);
        var cross = KoreXYZVector.CrossProduct(v1, v2);
        testLog.AddResult("KoreXYZVector CrossProduct", cross.X == -3 && cross.Y == 6 && cross.Z == -3);
    }

    public static void TestArbitraryPerpendicular(KoreTestLog testLog)
    {
        // Test for a non-zero vector
        var v = new KoreXYZVector(1, 2, 3);
        var perp = v.ArbitraryPerpendicular();
        testLog.AddComment($"Input: {v}, Perpendicular: {perp}");
        testLog.AddResult("KoreXYZVector ArbitraryPerpendicular", !perp.IsZero());

        // Test for a zero vector
        var zeroVector = new KoreXYZVector(0, 0, 0);
        var perpZero = zeroVector.ArbitraryPerpendicular();
        testLog.AddComment($"Zero Input: {zeroVector}, Perpendicular: {perpZero}");
        testLog.AddResult("KoreXYZVector ArbitraryPerpendicular Zero Vector", perpZero.X == 1 && perpZero.Y == 0 && perpZero.Z == 0);

        // Test for a fixed direction (X axis)
        var xAxis = new KoreXYZVector(1, 0, 0);
        var perpX = xAxis.ArbitraryPerpendicular();
        testLog.AddComment($"X Axis Input: {xAxis}, Perpendicular: {perpX}");
        testLog.AddResult("KoreXYZVector ArbitraryPerpendicular X Axis", !perpX.IsZero() && Math.Abs(KoreXYZVector.DotProduct(xAxis, perpX)) < 1e-10);

        // Test for a fixed direction (Y axis)
        var yAxis = new KoreXYZVector(0, 1, 0);
        var perpY = yAxis.ArbitraryPerpendicular();
        testLog.AddComment($"Y Axis Input: {yAxis}, Perpendicular: {perpY}");
        testLog.AddResult("KoreXYZVector ArbitraryPerpendicular Y Axis", !perpY.IsZero() && Math.Abs(KoreXYZVector.DotProduct(yAxis, perpY)) < 1e-10);

        // Test for a non-standard direction
        var custom = new KoreXYZVector(2, -5, 7);
        var perpCustom = custom.ArbitraryPerpendicular();
        testLog.AddComment($"Custom Input: {custom}, Perpendicular: {perpCustom}");
        testLog.AddResult("KoreXYZVector ArbitraryPerpendicular Custom", !perpCustom.IsZero() && Math.Abs(KoreXYZVector.DotProduct(custom, perpCustom)) < 1e-10);
    }
}


