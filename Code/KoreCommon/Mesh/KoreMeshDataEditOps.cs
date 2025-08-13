using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;


// KoreMeshDataEditOps: A static class to hold functions to edit a mesh

public static class KoreMeshDataEditOps
{

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


}


