using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestMesh
{
    // Usage: KoreTestMesh.RunTests(testLog);
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestBasicCubeJson(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreTestMesh RunTests // Exception: ", false, ex.Message);
            return;
        }
    }


    // Test to create a basic cube - convert it to JSON (text) and then back again, to ensure all the lists are correctly serialized and deserialized.
    public static void TestBasicCubeJson(KoreTestLog testLog)
    {
        // Test for basic cube mesh creation
        var cubeMesh = KoreMeshDataPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0));

        // Add some minort customizations to the cube mesh to see if they are serialized correctly
        cubeMesh.SetVertexColor(1, KoreColorPalette.Colors["Green"]);
        cubeMesh.SetLineColor(2, KoreColorPalette.Colors["Blue"], KoreColorPalette.Colors["Cyan"]);
        cubeMesh.SetTriangleColor(3, KoreColorPalette.Colors["Yellow"]);

        bool denseJSON = false;
        string cubeJSON = KoreMeshDataIO.ToJson(cubeMesh, denseJSON);
        //Console.WriteLine($"Cube Mesh JSON:{cubeJSON}");
        testLog.AddComment($"KoreMeshDataIO ToJson BasicCube: {cubeJSON}");

        var deserializedCubeMesh = KoreMeshDataIO.FromJson(cubeJSON);
        string reserialisedCubeJSON = KoreMeshDataIO.ToJson(deserializedCubeMesh, denseJSON);

        testLog.AddComment($"KoreMeshDataIO FromJson BasicCube - JSON format: {reserialisedCubeJSON}");


        // testLog.AddResult("KoreMeshDataIO FromJson BasicCube", deserializedCubeMesh != null);

        // if (deserializedCubeMesh == null)
        // {
        //     testLog.AddComment("Deserialized mesh is null, check the JSON format or the KoreMeshDataIO implementation.");
        //     return;
        // }

        // // Check if the deserialized mesh is valid - comparing some select values and values
        // testLog.AddResult("KoreMeshDataIO FromJson BasicCube Name", deserializedCubeMesh.Name                 == cubeMesh.Name);
        // testLog.AddResult("KoreMeshDataIO PointCount",              deserializedCubeMesh.Vertices.Count       == cubeMesh.Vertices.Count);
        // testLog.AddResult("KoreMeshDataIO LineCount",               deserializedCubeMesh.Lines.Count          == cubeMesh.Lines.Count);
        // testLog.AddResult("KoreMeshDataIO TriangleCount",           deserializedCubeMesh.Triangles.Count      == cubeMesh.Triangles.Count);
        // testLog.AddResult("KoreMeshDataIO NormalCount",             deserializedCubeMesh.Normals.Count        == cubeMesh.Normals.Count);
        // testLog.AddResult("KoreMeshDataIO UVCount",                 deserializedCubeMesh.UVs.Count            == cubeMesh.UVs.Count);
        // testLog.AddResult("KoreMeshDataIO VertexColorCount",        deserializedCubeMesh.VertexColors.Count   == cubeMesh.VertexColors.Count);
        // testLog.AddResult("KoreMeshDataIO LineColorCount",          deserializedCubeMesh.LineColors.Count     == cubeMesh.LineColors.Count);
        // testLog.AddResult("KoreMeshDataIO TriangleColorCount",      deserializedCubeMesh.TriangleColors.Count == cubeMesh.TriangleColors.Count);
    }

}