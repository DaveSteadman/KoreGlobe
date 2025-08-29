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
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove normals that don't have supporting vertex IDs
    /// </summary>
    public static void RemoveBrokenNormals(KoreMeshData mesh)
    {
        var invalidNormalIds = mesh.Normals.Keys.Where(id => !mesh.Vertices.ContainsKey(id)).ToList();
        foreach (int normalId in invalidNormalIds)
        {
            mesh.Normals.Remove(normalId);
        }
    }

    /// <summary>
    /// Create missing normals for vertices
    /// </summary>
    public static void CreateMissingNormals(KoreMeshData mesh, KoreXYZVector? defaultNormal = null)
    {
        KoreXYZVector normal = defaultNormal ?? new KoreXYZVector(0, 1, 0); // Default to up vector

        foreach (int vertexId in mesh.Vertices.Keys)
        {
            if (!mesh.Normals.ContainsKey(vertexId))
            {
                mesh.Normals[vertexId] = normal;
            }
        }
    }

    /// <summary>
    /// Calculate normal for a triangle
    /// </summary>
    public static KoreXYZVector NormalForTriangle(KoreMeshData mesh, int triangleId)
    {
        if (!mesh.Triangles.ContainsKey(triangleId))
            return new KoreXYZVector(0, 1, 0);

        var triangle = mesh.Triangles[triangleId];

        if (!mesh.Vertices.ContainsKey(triangle.A) ||
            !mesh.Vertices.ContainsKey(triangle.B) ||
            !mesh.Vertices.ContainsKey(triangle.C))
            return new KoreXYZVector(0, 1, 0);

        var vertexA = mesh.Vertices[triangle.A];
        var vertexB = mesh.Vertices[triangle.B];
        var vertexC = mesh.Vertices[triangle.C];

        // Calculate cross product for normal
        var edge1 = vertexB - vertexA;
        var edge2 = vertexC - vertexA;

        // Manual cross product calculation: edge1 × edge2
        var crossX = edge1.Y * edge2.Z - edge1.Z * edge2.Y;
        var crossY = edge1.Z * edge2.X - edge1.X * edge2.Z;
        var crossZ = edge1.X * edge2.Y - edge1.Y * edge2.X;

        var normal = new KoreXYZVector(crossX, crossY, crossZ);

        // Normalize the normal vector
        var length = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
        if (length > 0.0001) // Avoid division by zero
        {
            normal = new KoreXYZVector(normal.X / length, normal.Y / length, normal.Z / length);
        }
        else
        {
            normal = new KoreXYZVector(0, 1, 0); // Default up vector
        }

        return normal;
    }

    /// <summary>
    /// Set normal from first triangle that uses this vertex
    /// </summary>
    public static void SetNormalFromFirstTriangle(KoreMeshData mesh, int vertexId)
    {
        foreach (var triangleKvp in mesh.Triangles)
        {
            var triangle = triangleKvp.Value;
            if (triangle.A == vertexId || triangle.B == vertexId || triangle.C == vertexId)
            {
                var normal = NormalForTriangle(mesh, triangleKvp.Key);
                mesh.Normals[vertexId] = normal;
                return;
            }
        }

        // If no triangle found, set default normal
        mesh.Normals[vertexId] = new KoreXYZVector(0, 1, 0);
    }

    /// <summary>
    /// Set normals from triangles for all vertices
    /// </summary>
    public static void SetNormalsFromTriangles(KoreMeshData mesh)
    {
        // Clear existing normals
        mesh.Normals.Clear();

        // Calculate normals for each vertex based on triangles
        foreach (int vertexId in mesh.Vertices.Keys)
        {
            SetNormalFromFirstTriangle(mesh, vertexId);
        }
    }

    /// <summary>
    /// Calculate normals for each triangle and assign to vertices
    /// Usage: KoreMeshDataEditOps.CalcNormalsForAllTriangles(mesh);
    /// </summary>
    public static void CalcNormalsForAllTriangles(KoreMeshData mesh)
    {
        foreach (var kvp in mesh.Triangles)
        {
            CalcNormalsForTriangle(mesh, kvp.Key);
        }
    }

    /// <summary>
    /// Calculate normal for a specific triangle and assign to its vertices
    /// </summary>
    public static KoreXYZVector CalcNormalsForTriangle(KoreMeshData mesh, int triangleId)
    {
        if (!mesh.Triangles.ContainsKey(triangleId))
            return KoreXYZVector.Zero;

        // Get the vertices
        KoreMeshTriangle triangle = mesh.Triangles[triangleId];
        KoreXYZVector a = mesh.Vertices[triangle.A];
        KoreXYZVector b = mesh.Vertices[triangle.B];
        KoreXYZVector c = mesh.Vertices[triangle.C];

        // Calculate the face normal using cross product
        KoreXYZVector ab = b - a;  // Vector from A to B
        KoreXYZVector ac = c - a;  // Vector from A to C
        KoreXYZVector faceNormal = KoreXYZVector.CrossProduct(ab, ac).Normalize();

        // Normalize the face normal (no inversion needed with CW triangles)
        faceNormal = faceNormal.Normalize();

        // Set the normals
        mesh.Normals[triangle.A] = faceNormal;
        mesh.Normals[triangle.B] = faceNormal;
        mesh.Normals[triangle.C] = faceNormal;

        return faceNormal;
    }

}
