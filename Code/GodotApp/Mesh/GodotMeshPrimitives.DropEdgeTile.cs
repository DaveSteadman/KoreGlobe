
using System.Collections.Generic;

using KoreCommon;

public static class GodotMeshPrimitivesDropEdgeTile
{
    public static KoreMeshData DropEdgeTile(
        KoreNumeric2DArray<float> eledata,
        KoreLLBox tileBox)
    {
        KoreMeshData mesh = new KoreMeshData();


        return mesh;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreMeshData ribbonMesh = GodotMeshPrimitivesDropEdgeTile.TestRibbon();
    public static KoreMeshData TestRibbon()
    {
        KoreMeshData mesh = new KoreMeshData();

        // Create two lists of points
        List<KoreXYZVector> leftPoints = new List<KoreXYZVector>();
        List<KoreXYZVector> rightPoints = new List<KoreXYZVector>();

        List<KoreXYVector> leftUVs = new List<KoreXYVector>();
        List<KoreXYVector> rightUVs = new List<KoreXYVector>();

        for (int i = 0; i < 10; i++)
        {
            leftPoints.Add(new KoreXYZVector(i, 0, 0));
            rightPoints.Add(new KoreXYZVector(i, 1, 0));
            leftUVs.Add(new KoreXYVector(i / 10f, 0));
            rightUVs.Add(new KoreXYVector(i / 10f, 1));
        }

        mesh = KoreMeshDataPrimitives.Ribbon(leftPoints, leftUVs, rightPoints, rightUVs);

        return mesh;

    }


}