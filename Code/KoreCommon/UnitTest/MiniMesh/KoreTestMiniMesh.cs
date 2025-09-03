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
        TestSphere(testLog);
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

    private static void TestSphere(KoreTestLog testLog)
    {
        testLog.AddComment("=== Testing Sphere Primitives ===");

        // Create both sphere types for comparison
        KoreXYZVector center = new KoreXYZVector(0, 0, 0);
        double radius = 1.0;
        int latSegments = 8; // Low res for testing
        var material = KoreMiniMeshMaterialPalette.Find("BlueShiny");
        var lineColor = new KoreColorRGB(255, 255, 0); // Yellow wireframe

        // Test BasicSphere
        testLog.AddComment("--- Basic Sphere ---");
        KoreMiniMesh basicSphere = KoreMiniMeshPrimitives.BasicSphere(center, radius, latSegments, material, lineColor);
        
        testLog.AddComment($"Basic Sphere Vertices: {basicSphere.Vertices.Count}");
        testLog.AddComment($"Basic Sphere Groups: {basicSphere.Groups.Count}");
        testLog.AddComment($"Basic Sphere Materials: {basicSphere.Materials.Count}");
        testLog.AddComment($"Basic Sphere Lines: {basicSphere.Lines.Count}");

        // Test OptimizedSphere
        testLog.AddComment("--- Optimized Sphere ---");
        KoreMiniMesh optimizedSphere = KoreMiniMeshPrimitives.OptimizedSphere(center, radius, latSegments, material, lineColor);
        
        testLog.AddComment($"Optimized Sphere Vertices: {optimizedSphere.Vertices.Count}");
        testLog.AddComment($"Optimized Sphere Groups: {optimizedSphere.Groups.Count}");
        testLog.AddComment($"Optimized Sphere Materials: {optimizedSphere.Materials.Count}");
        testLog.AddComment($"Optimized Sphere Lines: {optimizedSphere.Lines.Count}");

        // Calculate efficiency improvement
        float vertexReduction = (float)(basicSphere.Vertices.Count - optimizedSphere.Vertices.Count) / basicSphere.Vertices.Count * 100f;
        testLog.AddComment($"Vertex count reduction: {vertexReduction:F1}%");

        // Test JSON serialization for both
        string basicJson = KoreMiniMeshIO.ToJson(basicSphere);
        string optimizedJson = KoreMiniMeshIO.ToJson(optimizedSphere);
        
        var loadedBasic = KoreMiniMeshIO.FromJson(basicJson);
        var loadedOptimized = KoreMiniMeshIO.FromJson(optimizedJson);
        
        testLog.AddResult("Basic Sphere JSON roundtrip vertices", loadedBasic.Vertices.Count == basicSphere.Vertices.Count);
        testLog.AddResult("Optimized Sphere JSON roundtrip vertices", loadedOptimized.Vertices.Count == optimizedSphere.Vertices.Count);

        // Save both to OBJ/MTL for visual comparison
        var (basicObjContent, basicMtlContent) = KoreMiniMeshIO.ToObjMtl(basicSphere, "BasicSphere", "BasicSphereMaterials");
        File.WriteAllText("UnitTestArtefacts/BasicSphere.obj", basicObjContent);
        File.WriteAllText("UnitTestArtefacts/BasicSphereMaterials.mtl", basicMtlContent);

        var (optimizedObjContent, optimizedMtlContent) = KoreMiniMeshIO.ToObjMtl(optimizedSphere, "OptimizedSphere", "OptimizedSphereMaterials");
        File.WriteAllText("UnitTestArtefacts/OptimizedSphere.obj", optimizedObjContent);
        File.WriteAllText("UnitTestArtefacts/OptimizedSphereMaterials.mtl", optimizedMtlContent);
        
        testLog.AddComment("Sphere comparison completed - check UnitTestArtefacts/BasicSphere.obj vs OptimizedSphere.obj");

    }

}
