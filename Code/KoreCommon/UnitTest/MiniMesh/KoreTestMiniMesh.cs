using System;
using System.IO;
using KoreCommon;
using KoreCommon.SkiaSharp;

namespace KoreCommon.UnitTest;

public static partial class KoreTestMiniMesh
{
    public static void RunTests(KoreTestLog testLog)
    {
        testLog.AddComment("=== Testing KoreTestMiniMesh ===");

        TestSimpleJSON(testLog);
    }


    private static void TestSimpleJSON(KoreTestLog testLog)
    {
        // Create a cube with the primitives class
        KoreMiniMesh cubeMesh = KoreMiniMeshPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0), new KoreColorRGB(255, 255, 255));
        string json = KoreMiniMeshIO.ToJson(cubeMesh);
        testLog.AddComment($"Cube JSON: {json}");

        var loadedCube = KoreMiniMeshIO.FromJson(json);
        testLog.AddComment($"Loaded Cube: {loadedCube}");
    }

}
