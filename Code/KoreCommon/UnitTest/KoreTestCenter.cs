using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestCenter
{
    public static KoreTestLog RunCoreTests()
    {
        KoreTestLog testLog = new KoreTestLog();

        try
        {
            KoreTestMath.RunTests(testLog);
            KoreTestXYZVector.RunTests(testLog);
            KoreTestLine.RunTests(testLog);
            KoreTestTriangle.RunTests(testLog);

            KoreTestPosition.RunTests(testLog);
            KoreTestPositionLLA.RunTests(testLog);
            KoreTestRoute.RunTests(testLog);
            //KoreTestPlotter.RunTests(testLog);
            KoreTestList1D.RunTests(testLog);
            KoreTestList2D.RunTests(testLog);
            KoreTestMesh.RunTests(testLog);

            KoreTestColor.RunTests(testLog);

            KoreTestDatabase.RunTests(testLog);

            KoreTestStringDictionary.RunTests(testLog);
        }
        catch (Exception)
        {
            testLog.AddResult("Test Centre Run", false, "Exception");
        }

        return testLog;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreTestCenter.RunAdHocTests()
    public static KoreTestLog RunAdHocTests()
    {
        KoreTestLog testLog = new KoreTestLog();

        try
        {
            KoreTestXYZVector.TestArbitraryPerpendicular(testLog);
        }
        catch (Exception)
        {
            testLog.AddResult("Test Centre Run", false, "Exception");
        }

        return testLog;
    }
}

