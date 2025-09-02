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
        KoreMiniMesh cubeMesh = KoreMiniMeshPrimitives.BasicCube(1.0f, KoreMiniMeshMaterialPalette.Find("MattOrange"), new KoreColorRGB(255, 255, 255));
        string json = KoreMiniMeshIO.ToJson(cubeMesh);
        testLog.AddComment($"Cube JSON: {json}");

        var loadedCube = KoreMiniMeshIO.FromJson(json);
        testLog.AddComment($"Loaded Cube: {loadedCube}");


        // Save to Obj/MTL
        var (objContent, mtlContent) = KoreMiniMeshIO.ToObjMtl(cubeMesh, "MyMesh", "MyMaterials");
        testLog.AddComment($"OBJ Content: {objContent}");
        testLog.AddComment($"MTL Content: {mtlContent}");

        // Save OBJMtl content to file
        File.WriteAllText("UnitTestArtefacts/MyMesh.obj", objContent);
        File.WriteAllText("UnitTestArtefacts/MyMaterials.mtl", mtlContent);

    }

}
