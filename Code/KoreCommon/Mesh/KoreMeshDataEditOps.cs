using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;


// KoreMeshDataEditOps: A static class to hold functions to edit a mesh

public static partial class KoreMeshDataEditOps
{

    // --------------------------------------------------------------------------------------------
    // MARK: VERTICES
    // --------------------------------------------------------------------------------------------

    public static void OffsetVertex(KoreMeshData mesh, int vertexId, KoreXYZVector offset)
    {
        // We want to throw here, because we have a unique ID concept and random new additions break this
        if (!mesh.Vertices.ContainsKey(vertexId))
            throw new ArgumentOutOfRangeException(nameof(vertexId), "Vertex ID is not found.");

        // Offset the vertex by the given offset vector
        mesh.Vertices[vertexId] = mesh.Vertices[vertexId] + offset;
    }

    public static void OffsetAllVertices(KoreMeshData mesh, KoreXYZVector offset)
    {
        foreach (var vertexId in mesh.Vertices.Keys)
        {
            OffsetVertex(mesh, vertexId, offset);
        }
    }

    /// <summary>
    /// Creates a duplicate vertex with all associated data (normal, UV, color)
    /// </summary>
    private static int DuplicateVertex(KoreMeshData mesh, int originalVertexId)
    {
        if (!mesh.Vertices.ContainsKey(originalVertexId))
            return originalVertexId;

        // Get original vertex data
        KoreXYZVector vertex = mesh.Vertices[originalVertexId];

        KoreXYZVector? normal = mesh.Normals.ContainsKey(originalVertexId) ? mesh.Normals[originalVertexId] : null;
        KoreXYVector? uv = mesh.UVs.ContainsKey(originalVertexId) ? mesh.UVs[originalVertexId] : null;
        KoreColorRGB? color = mesh.VertexColors.ContainsKey(originalVertexId) ? mesh.VertexColors[originalVertexId] : null;

        // Create new vertex with all associated data
        return mesh.AddVertex(vertex, normal, color, uv);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TRIANGLES
    // --------------------------------------------------------------------------------------------

    // Swap the BC points in an ABC triangle to face the other direction
    public static void ReorientFace(KoreMeshData mesh, int triId)
    {
        if (mesh.Triangles.ContainsKey(triId))
        {
            KoreMeshTriangle triangle = mesh.Triangles[triId];
            // Swap the vertices to reorient the triangle
            int temp = triangle.B;
            triangle.B = triangle.C;
            triangle.C = temp;
            mesh.Triangles[triId] = triangle;
        }
    }

    // Usage: KoreMeshDataEditOps.ReorientAllFaces(mesh);
    public static void ReorientAllFaces(KoreMeshData mesh)
    {
        foreach (var kvp in mesh.Triangles)
        {
            ReorientFace(mesh, kvp.Key);
        }
    }

    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Isolates a triangle by creating duplicate vertices if they are shared with other triangles.
    /// This ensures the triangle has its own unique vertices that can be modified independently.
    /// </summary>
    public static void IsolateTriangle(KoreMeshData mesh, int triId)
    {
        if (!mesh.Triangles.ContainsKey(triId))
            return;

        KoreMeshTriangle triangle = mesh.Triangles[triId];

        // Find which vertices are shared with other triangles
        HashSet<int> sharedVertices = FindSharedVertices(mesh, triId);

        // Create new vertices for any shared ones
        int newA = sharedVertices.Contains(triangle.A) ? DuplicateVertex(mesh, triangle.A) : triangle.A;
        int newB = sharedVertices.Contains(triangle.B) ? DuplicateVertex(mesh, triangle.B) : triangle.B;
        int newC = sharedVertices.Contains(triangle.C) ? DuplicateVertex(mesh, triangle.C) : triangle.C;

        // Update the triangle with new vertex IDs
        mesh.Triangles[triId] = new KoreMeshTriangle(newA, newB, newC);
    }

    /// <summary>
    /// Finds vertices of a triangle that are shared with other triangles
    /// </summary>
    private static HashSet<int> FindSharedVertices(KoreMeshData mesh, int targetTriangleId)
    {
        if (!mesh.Triangles.ContainsKey(targetTriangleId))
            return new HashSet<int>();

        KoreMeshTriangle targetTriangle = mesh.Triangles[targetTriangleId];
        HashSet<int> targetVertices = new HashSet<int> { targetTriangle.A, targetTriangle.B, targetTriangle.C };
        HashSet<int> sharedVertices = new HashSet<int>();

        // Check all other triangles for shared vertices
        foreach (var kvp in mesh.Triangles)
        {
            if (kvp.Key == targetTriangleId)
                continue;

            KoreMeshTriangle otherTriangle = kvp.Value;

            if (targetVertices.Contains(otherTriangle.A)) sharedVertices.Add(otherTriangle.A);
            if (targetVertices.Contains(otherTriangle.B)) sharedVertices.Add(otherTriangle.B);
            if (targetVertices.Contains(otherTriangle.C)) sharedVertices.Add(otherTriangle.C);
        }

        return sharedVertices;
    }

    // Usage: KoreMeshDataEditOps.IsolateAllTriangles(mesh);
    public static void IsolateAllTriangles(KoreMeshData mesh)
    {
        foreach (var kvp in mesh.Triangles)
        {
            IsolateTriangle(mesh, kvp.Key);
        }
    }

}


