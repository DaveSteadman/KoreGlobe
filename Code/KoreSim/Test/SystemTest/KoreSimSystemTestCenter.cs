
// A static class to manage system tests for the KoreSim namespace.

using KoreCommon;
using KoreCommon.UnitTest;

namespace KoreSim.SystemTest;


public static class KoreSimSystemTestCenter
{

    // Usage:
    // KoreTestLog testLog = new KoreTestLog();
    // KoreSimSystemTestCenter.RunAllTests(testLog);

    public static void RunAllTests(KoreTestLog testLog)
    {
        // Initialize the test log
        //KoreTestLog.Initialize(testLog);

        // Run individual test methods
        TestKoreElevationManager(testLog);
        TestKoreElevationTileIO(testLog);
        // Add more test methods as needed
    }

    // --------------------------------------------------------------------------------------------

    private static void TestKoreElevationManager(KoreTestLog testLog)
    {
        // Implement unit tests for KoreElevationManager
        testLog.AddComment("TestKoreElevationManager");

        // Create an elevation manager instance and setup some boilerplate elevation data
        KoreElevationManager elevationManager = new KoreElevationManager();

        // Setup test data
        KoreLLBox testBox = new KoreLLBox()
        {
            MinLatDegs = 50.0,
            MaxLatDegs = 51.0,
            MinLonDegs = -1.0,
            MaxLonDegs =  0.0
        };
        float testElevation = 12f;
        KoreNumeric2DArray<float> testarray = new KoreNumeric2DArray<float>(20, 20);
        testarray.SetAll(testElevation);

        // create the patch
        KoreElevationPatch testPatch = new KoreElevationPatch();
        testPatch.SetLLBox(testBox);
        testPatch.SetElevationArray(testarray);

        // Add the patch to the system
        elevationManager.AddPatch(testPatch);

        // Test getting the elevation from a position within the patch
        float elevation = elevationManager.GetPatchElevationAtPos(testBox.CenterPoint);
        testLog.AddResult($"Elevation at center point {testBox.CenterPoint}: {elevation}", KoreValueUtils.EqualsWithinTolerance(elevation, testElevation));

        // Example test logic
        // Assert conditions, log results, etc.

        testLog.AddComment("TestKoreElevationManager");
    }

    // --------------------------------------------------------------------------------------------

    public static void TestKoreElevationTileIO(KoreTestLog testLog)
    {
        // Implement unit tests for KoreElevationTileIO
        testLog.AddComment("TestKoreElevationTileIO");

        // Example test logic
        // Assert conditions, log results, etc.

        testLog.AddComment("TestKoreElevationTileIO");
    }
    // Add methods and properties for managing unit tests here
}