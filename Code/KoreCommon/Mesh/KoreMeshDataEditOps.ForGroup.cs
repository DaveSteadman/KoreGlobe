using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable

namespace KoreCommon;


// KoreMeshDataEditOps: A static class to hold functions to edit a mesh

public static partial class KoreMeshDataEditOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Vertex
    // --------------------------------------------------------------------------------------------

    public static Dictionary<int, KoreXYZVector> VerticesForGroup(KoreMeshData meshData, string groupName)
    {
        var vertexDict = new Dictionary<int, KoreXYZVector>();

        if (!meshData.HasNamedGroup(groupName))
            return vertexDict;

        KoreMeshTriangleGroup? group = meshData.NamedGroup(groupName);
        if (group == null)
            return vertexDict;

        KoreMeshTriangleGroup groupValue = group.Value;
        foreach (int triangleId in groupValue.TriangleIds)
        {
            var triangle = meshData.Triangles[triangleId];
            int[] vertexIds = { triangle.A, triangle.B, triangle.C };

            foreach (var vId in vertexIds)
            {
                if (meshData.Vertices.ContainsKey(vId) && !vertexDict.ContainsKey(vId))
                {
                    vertexDict[vId] = meshData.Vertices[vId];
                }
            }
        }

        return vertexDict;
    }

    // Helper method to get all vertex IDs for a group (reduces code duplication)
    private static HashSet<int> GetVertexIdsForGroup(KoreMeshData meshData,string groupName)
    {
        HashSet<int> relevantVertexIds = new HashSet<int>();

        if (!meshData.NamedTriangleGroups.ContainsKey(groupName))
            return relevantVertexIds;

        KoreMeshTriangleGroup? sourceGroup = meshData.NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            if (meshData.Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = meshData.Triangles[triangleId];
                relevantVertexIds.Add(triangle.A);
                relevantVertexIds.Add(triangle.B);
                relevantVertexIds.Add(triangle.C);
            }
        }

        return relevantVertexIds;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normal
    // --------------------------------------------------------------------------------------------

    // Get normals for all vertices in a specific group
    public static List<KoreXYZVector?> NormalsForGroup(KoreMeshData meshData, string groupName)
    {
        List<KoreXYZVector?> groupNormals = new List<KoreXYZVector?>();

        if (!meshData.NamedTriangleGroups.ContainsKey(groupName))
            return groupNormals;

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(meshData, groupName);

        foreach (int currVertexId in relevantVertexIds)
        {
            KoreXYZVector? normal = meshData.Normals.ContainsKey(currVertexId) ? meshData.Normals[currVertexId] : null;
            groupNormals.Add(normal);
        }

        return groupNormals;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UV
    // --------------------------------------------------------------------------------------------

    // Get UVs for all vertices in a specific group
    public static List<KoreXYVector?> UVsForGroup(KoreMeshData meshData, string groupName)
    {
        List<KoreXYVector?> groupUVs = new List<KoreXYVector?>();

        if (!meshData.NamedTriangleGroups.ContainsKey(groupName))
            return groupUVs;

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(meshData, groupName);

        foreach (int currVertexId in relevantVertexIds)
        {
            KoreXYVector? uv = meshData.UVs.ContainsKey(currVertexId) ? meshData.UVs[currVertexId] : null;
            groupUVs.Add(uv);
        }

        return groupUVs;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Vertex Color
    // --------------------------------------------------------------------------------------------

    // Get vertex colors for all vertices in a specific group
    public static List<KoreColorRGB?> VertexColorsForGroup(KoreMeshData meshData, string groupName)
    {
        List<KoreColorRGB?> groupColors = new List<KoreColorRGB?>();

        // If no group found, return empty list
        if (!meshData.NamedTriangleGroups.ContainsKey(groupName))
            return groupColors;

        // Get the relevant vertex IDs for the group
        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(meshData, groupName);

        foreach (int currVertexId in relevantVertexIds)
        {
            KoreColorRGB? color = meshData.VertexColors.ContainsKey(currVertexId) ? meshData.VertexColors[currVertexId] : null;
            groupColors.Add(color);
        }

        return groupColors;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle
    // --------------------------------------------------------------------------------------------

    // Get triangles for a specific group (with vertex IDs preserved)
    public static List<KoreMeshTriangle> TrianglesForGroup(KoreMeshData meshData, string groupName)
    {
        List<KoreMeshTriangle> groupTriangles = new List<KoreMeshTriangle>();

        // if no group found, return empty list
        if (!meshData.NamedTriangleGroups.ContainsKey(groupName))
            return groupTriangles;

        // Get the group and copy out the triangle list
        KoreMeshTriangleGroup? sourceGroup = meshData.NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            if (meshData.Triangles.ContainsKey(triangleId))
            {
                groupTriangles.Add(meshData.Triangles[triangleId]);
            }
        }

        return groupTriangles;
    }



}
