using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

/// <summary>
/// Static validity and cleanup operations for KoreMeshData
/// Contains methods for mesh validation, cleanup, and population of missing data
/// </summary>
public static partial class KoreMeshDataEditOps
{


    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove triangles that don't have supporting vertex IDs
    /// </summary>
    public static void RemoveBrokenTriangles(KoreMeshData mesh)
    {
        var invalidTriangleIds = mesh.Triangles.Where(kvp =>
            !mesh.Vertices.ContainsKey(kvp.Value.A) ||
            !mesh.Vertices.ContainsKey(kvp.Value.B) ||
            !mesh.Vertices.ContainsKey(kvp.Value.C))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (int triangleId in invalidTriangleIds)
        {
            mesh.Triangles.Remove(triangleId);
        }
    }

    /// <summary>
    /// Remove duplicate triangles
    /// </summary>
    public static void RemoveDuplicateTriangles(KoreMeshData mesh)
    {
        var trianglesToRemove = new List<int>();
        var trianglesArray = mesh.Triangles.ToArray();

        for (int i = 0; i < trianglesArray.Length; i++)
        {
            for (int j = i + 1; j < trianglesArray.Length; j++)
            {
                var tri1 = trianglesArray[i].Value;
                var tri2 = trianglesArray[j].Value;

                // Check if triangles have the same vertices (any permutation)
                var vertices1 = new HashSet<int> { tri1.A, tri1.B, tri1.C };
                var vertices2 = new HashSet<int> { tri2.A, tri2.B, tri2.C };

                if (vertices1.SetEquals(vertices2))
                {
                    trianglesToRemove.Add(trianglesArray[j].Key);
                }
            }
        }

        foreach (int triangleId in trianglesToRemove)
        {
            mesh.Triangles.Remove(triangleId);
        }
    }
    
    
    // --------------------------------------------------------------------------------------------
    // MARK: Winding
    // --------------------------------------------------------------------------------------------

    public static void FlipTriangleWinding(KoreMeshData mesh, int triId)
    {
        if (!mesh.Triangles.ContainsKey(triId))
            return;

        KoreMeshTriangle triangle = mesh.Triangles[triId];
       
        // Swap the vertices to flip the winding
        int temp = triangle.B;
        triangle.B = triangle.C;
        triangle.C = temp;

        // Update the triangle in the dictionary
        mesh.Triangles[triId] = triangle;
    }

    public static void FlipAllTriangleWindings(KoreMeshData mesh)
    {
        // Loop through all triangles and flip their winding
        foreach (var kvp in mesh.Triangles.ToList())
        {
            int triangleId = kvp.Key;
            FlipTriangleWinding(mesh, triangleId);
        }
    }
    
}
