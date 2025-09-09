// <fileheader>

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public static partial class KoreColorMeshOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Vertices
    // --------------------------------------------------------------------------------------------

    // Usage: KoreColorRGB color = KoreColorMeshOps.FirstColorForVertex(mesh, vertexId);

    public static KoreColorRGB FirstColorForVertex(KoreColorMesh mesh, int vertexId)
    {
        foreach (var tri in mesh.Triangles.Values)
        {
            if (tri.A == vertexId || tri.B == vertexId || tri.C == vertexId)
                return tri.Color;
        }

        return KoreColorRGB.White; // Default color if no triangles found
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    // Usage: KoreXYZVector triNorm = KoreColorMeshOps.CalculateFaceNormal(mesh, tri);

    public static KoreXYZVector CalculateFaceNormal(KoreColorMesh mesh, KoreColorMeshTri tri)
    {
        // Get the vertex positions
        var vA = mesh.GetVertex(tri.A);
        var vB = mesh.GetVertex(tri.B);
        var vC = mesh.GetVertex(tri.C);

        // Calc the edges
        var abEdge = vA.XYZTo(vB);
        var acEdge = vA.XYZTo(vC);

        // Cross product and magnitude
        KoreXYZVector normal = KoreXYZVectorOps.CrossProduct(acEdge, abEdge);
        double length = normal.Magnitude;

        // Normalize the normal vector
        if (length > 0)
            return normal.Normalize();

        return KoreXYZVector.Zero;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    // Define four points of a face, in CW order, to be stored as two new triangles.
    // return a list of the new triangle IDs

    // A -- B
    // |    |
    // D -- C

    public static List<int> AddFace(KoreColorMesh mesh, int a, int b, int c, int d, KoreColorRGB color)
    {
        var triangleIds = new List<int>();

        // Split the quad into two triangles using a fan from vertex a
        // Triangle 1: a -> b -> c
        triangleIds.Add(mesh.AddTriangle(new KoreColorMeshTri(a, b, c, color)));

        // Triangle 2: a -> c -> d
        KoreColorRGB col2 = KoreColorOps.Lerp(color, KoreColorRGB.Black, 0.15f); // Slightly different color for second triangle
        triangleIds.Add(mesh.AddTriangle(new KoreColorMeshTri(a, c, d, col2)));

        return triangleIds;
    }


}
