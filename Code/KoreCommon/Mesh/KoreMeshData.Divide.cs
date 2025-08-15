using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

// KoreMeshData: A class to hold mesh data, including vertices, triangles, materials, and named groups
public partial class KoreMeshData
{
    // Usage: KoreMeshData.VerticesForGroup
    public List<KoreXYZVector> VerticesForGroup(string groupName)
    {
        List<KoreXYZVector> groupVertices = new List<KoreXYZVector>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupVertices;

        HashSet<int> relevantVertexIds = new HashSet<int>();

        // Loop through all of the triangles
        KoreMeshTriangleGroup? sourceGroup = NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            if (Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = Triangles[triangleId];
                relevantVertexIds.Add(triangle.A);
                relevantVertexIds.Add(triangle.B);
                relevantVertexIds.Add(triangle.C);
            }
        }

        // copy everything vertex related
        foreach (int currVertexId in relevantVertexIds)
        {
            if (Vertices.ContainsKey(currVertexId))
                groupVertices.Add(Vertices[currVertexId]);
        }

        return groupVertices;
    }

    // Get normals for all vertices in a specific group
    public List<KoreXYZVector?> NormalsForGroup(string groupName)
    {
        List<KoreXYZVector?> groupNormals = new List<KoreXYZVector?>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupNormals;

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(groupName);

        foreach (int currVertexId in relevantVertexIds)
        {
            KoreXYZVector? normal = Normals.ContainsKey(currVertexId) ? Normals[currVertexId] : null;
            groupNormals.Add(normal);
        }

        return groupNormals;
    }

    // Get UVs for all vertices in a specific group
    public List<KoreXYVector?> UVsForGroup(string groupName)
    {
        List<KoreXYVector?> groupUVs = new List<KoreXYVector?>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupUVs;

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(groupName);

        foreach (int currVertexId in relevantVertexIds)
        {
            KoreXYVector? uv = UVs.ContainsKey(currVertexId) ? UVs[currVertexId] : null;
            groupUVs.Add(uv);
        }

        return groupUVs;
    }

    // Get vertex colors for all vertices in a specific group
    public List<KoreColorRGB?> VertexColorsForGroup(string groupName)
    {
        List<KoreColorRGB?> groupColors = new List<KoreColorRGB?>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupColors;

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(groupName);

        foreach (int currVertexId in relevantVertexIds)
        {
            KoreColorRGB? color = VertexColors.ContainsKey(currVertexId) ? VertexColors[currVertexId] : null;
            groupColors.Add(color);
        }

        return groupColors;
    }

    // Get triangles for a specific group (with vertex IDs preserved)
    public List<KoreMeshTriangle> TrianglesForGroup(string groupName)
    {
        List<KoreMeshTriangle> groupTriangles = new List<KoreMeshTriangle>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupTriangles;

        KoreMeshTriangleGroup? sourceGroup = NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            if (Triangles.ContainsKey(triangleId))
            {
                groupTriangles.Add(Triangles[triangleId]);
            }
        }

        return groupTriangles;
    }

    // Helper method to get all vertex IDs for a group (reduces code duplication)
    private HashSet<int> GetVertexIdsForGroup(string groupName)
    {
        HashSet<int> relevantVertexIds = new HashSet<int>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return relevantVertexIds;

        KoreMeshTriangleGroup? sourceGroup = NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            if (Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = Triangles[triangleId];
                relevantVertexIds.Add(triangle.A);
                relevantVertexIds.Add(triangle.B);
                relevantVertexIds.Add(triangle.C);
            }
        }

        return relevantVertexIds;
    }

    // Comprehensive method to get all vertex data for a group in one call
    public (List<KoreXYZVector> vertices, List<KoreXYZVector?> normals, List<KoreXYVector?> uvs, List<KoreColorRGB?> colors, List<KoreMeshTriangle> triangles) GetGroupData(string groupName)
    {
        var vertices = new List<KoreXYZVector>();
        var normals = new List<KoreXYZVector?>();
        var uvs = new List<KoreXYVector?>();
        var colors = new List<KoreColorRGB?>();
        var triangles = new List<KoreMeshTriangle>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return (vertices, normals, uvs, colors, triangles);

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(groupName);

        // Get all vertex data in order
        foreach (int currVertexId in relevantVertexIds)
        {
            if (Vertices.ContainsKey(currVertexId))
            {
                vertices.Add(Vertices[currVertexId]);
                normals.Add(Normals.ContainsKey(currVertexId) ? Normals[currVertexId] : null);
                uvs.Add(UVs.ContainsKey(currVertexId) ? UVs[currVertexId] : null);
                colors.Add(VertexColors.ContainsKey(currVertexId) ? VertexColors[currVertexId] : null);
            }
        }

        // Get triangles
        triangles = TrianglesForGroup(groupName);

        return (vertices, normals, uvs, colors, triangles);
    }

    // Get group data with remapped vertex indices for rendering engines that expect 0-based contiguous indices
    public (List<KoreXYZVector> vertices, List<KoreXYZVector?> normals, List<KoreXYVector?> uvs, List<KoreColorRGB?> colors, List<(int A, int B, int C)> triangleIndices, Dictionary<int, int> vertexIdMapping) GetGroupDataRemapped(string groupName)
    {
        var vertices = new List<KoreXYZVector>();
        var normals = new List<KoreXYZVector?>();
        var uvs = new List<KoreXYVector?>();
        var colors = new List<KoreColorRGB?>();
        var triangleIndices = new List<(int A, int B, int C)>();
        var vertexIdMapping = new Dictionary<int, int>();

        if (!NamedTriangleGroups.ContainsKey(groupName))
            return (vertices, normals, uvs, colors, triangleIndices, vertexIdMapping);

        HashSet<int> relevantVertexIds = GetVertexIdsForGroup(groupName);

        // Create mapping from original vertex IDs to contiguous indices
        int remappedIndex = 0;
        foreach (int originalVertexId in relevantVertexIds.OrderBy(id => id))
        {
            if (Vertices.ContainsKey(originalVertexId))
            {
                vertexIdMapping[originalVertexId] = remappedIndex++;
                
                vertices.Add(Vertices[originalVertexId]);
                normals.Add(Normals.ContainsKey(originalVertexId) ? Normals[originalVertexId] : null);
                uvs.Add(UVs.ContainsKey(originalVertexId) ? UVs[originalVertexId] : null);
                colors.Add(VertexColors.ContainsKey(originalVertexId) ? VertexColors[originalVertexId] : null);
            }
        }

        // Get triangles with remapped indices
        KoreMeshTriangleGroup? sourceGroup = NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            if (Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = Triangles[triangleId];
                if (vertexIdMapping.ContainsKey(triangle.A) && 
                    vertexIdMapping.ContainsKey(triangle.B) && 
                    vertexIdMapping.ContainsKey(triangle.C))
                {
                    triangleIndices.Add((
                        vertexIdMapping[triangle.A],
                        vertexIdMapping[triangle.B], 
                        vertexIdMapping[triangle.C]
                    ));
                }
            }
        }

        return (vertices, normals, uvs, colors, triangleIndices, vertexIdMapping);
    }

    /// <summary>
    /// Create a new KoreMeshData containing only the geometry and material for a specific named group
    /// </summary>
    /// <param name="groupName">Name of the triangle group to extract</param>
    /// <returns>New mesh containing only the specified group's data</returns>
    public KoreMeshData CreateMeshForGroup(string groupName)
    {
        // Create the new mesh we'll act on through the function
        var groupMesh = new KoreMeshData();

        // Return unpopulated mesh if group doesn't exist
        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupMesh;

        // Copy the material for this group into the new mesh
        KoreMeshMaterial groupMaterial = MaterialForGroup(groupName);
        groupMesh.AddGroupWithMaterial(groupName, groupMaterial);

        // Look at the source mesh. Create a hash set of each triangle, vertex and line ID (each indexing value) that is relevant to our named group.
        HashSet<int> relevantVertexIds = new HashSet<int>();
        HashSet<int> relevantLineIds = new HashSet<int>();
        HashSet<int> relevantTriangleIds = new HashSet<int>();

        // Loop through all of the triangles
        KoreMeshTriangleGroup? sourceGroup = NamedGroup(groupName);
        foreach (int triangleId in sourceGroup?.TriangleIds ?? Enumerable.Empty<int>())
        {
            // Simple copy of the triangle IDs
            relevantTriangleIds.Add(triangleId);

            if (Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = Triangles[triangleId];
                relevantVertexIds.Add(triangle.A);
                relevantVertexIds.Add(triangle.B);
                relevantVertexIds.Add(triangle.C);
            }
        }

        // Loop through the lines, adding any line that has both vertices in the point hashset.
        foreach (var lineKvp in Lines)
        {
            int lineID = lineKvp.Key;
            KoreMeshLine line = lineKvp.Value;
            if (relevantVertexIds.Contains(line.A) && relevantVertexIds.Contains(line.B))
            {
                relevantLineIds.Add(lineID);
            }
        }

        // Now we have the list of relevant triangle, vertex, and line IDs - We copy each list of values into the new mesh.

        // copy everything vertex related
        foreach (int currVertexId in relevantVertexIds)
        {
            if (Vertices.ContainsKey(currVertexId))
                groupMesh.Vertices[currVertexId] = Vertices[currVertexId];
            if (Normals.ContainsKey(currVertexId))
                groupMesh.Normals[currVertexId] = Normals[currVertexId];
            if (VertexColors.ContainsKey(currVertexId))
                groupMesh.VertexColors[currVertexId] = VertexColors[currVertexId];
            if (UVs.ContainsKey(currVertexId))
                groupMesh.UVs[currVertexId] = UVs[currVertexId];
        }

        // copy everything line related
        foreach (int currLineId in relevantLineIds)
        {
            if (Lines.ContainsKey(currLineId))
                groupMesh.Lines[currLineId] = Lines[currLineId];
            if (LineColors.ContainsKey(currLineId))
                groupMesh.LineColors[currLineId] = LineColors[currLineId];
        }

        // copy everything triangle related
        foreach (int currTriangleId in relevantTriangleIds)
        {
            if (Triangles.ContainsKey(currTriangleId))
                groupMesh.Triangles[currTriangleId] = Triangles[currTriangleId];
        }

        // Copy the Next*Id values to ensure proper ID space management in the new mesh
        // Since we're preserving original IDs, copy the full ranges to avoid conflicts
        groupMesh.NextVertexId = this.NextVertexId;
        groupMesh.NextLineId = this.NextLineId;
        groupMesh.NextTriangleId = this.NextTriangleId;
        return groupMesh;
    }

    // Create separate meshes for each named triangle group
    // returns Dictionary mapping group names to their extracted meshes
    public Dictionary<string, KoreMeshData> CreateMeshesForAllGroups()
    {
        var groupMeshes = new Dictionary<string, KoreMeshData>();
        
        foreach (var groupKvp in NamedTriangleGroups)
        {
            string groupName = groupKvp.Key;
            KoreMeshData groupMesh = CreateMeshForGroup(groupName);
            
            if (groupMesh.Triangles.Count > 0) // Only add non-empty meshes
            {
                groupMeshes[groupName] = groupMesh;
            }
        }
        
        return groupMeshes;
    }

    // /// <summary>
    // /// Create separate meshes for each unique material
    // /// Groups triangles by material name and creates a mesh for each material
    // /// </summary>
    // /// <returns>Dictionary mapping material names to their extracted meshes</returns>
    // public Dictionary<string, KoreMeshData> CreateMeshesForEachMaterial()
    // {
    //     var materialMeshes = new Dictionary<string, KoreMeshData>();
        
    //     // Group triangles by material name
    //     var trianglesByMaterial = new Dictionary<string, List<int>>();
        
    //     // First collect triangles from named groups
    //     foreach (var groupKvp in NamedTriangleGroups)
    //     {
    //         KoreMeshTriangleGroup group = groupKvp.Value;
    //         string materialName = string.IsNullOrEmpty(group.MaterialName) ? "Default" : group.MaterialName;
            
    //         if (!trianglesByMaterial.ContainsKey(materialName))
    //             trianglesByMaterial[materialName] = new List<int>();
                
    //         trianglesByMaterial[materialName].AddRange(group.TriangleIds);
    //     }
        
    //     // Then collect any ungrouped triangles (assign to default material)
    //     var groupedTriangleIds = new HashSet<int>();
    //     foreach (var group in NamedTriangleGroups.Values)
    //     {
    //         foreach (int triangleId in group.TriangleIds)
    //             groupedTriangleIds.Add(triangleId);
    //     }
        
    //     var ungroupedTriangleIds = Triangles.Keys.Where(id => !groupedTriangleIds.Contains(id)).ToList();
    //     if (ungroupedTriangleIds.Count > 0)
    //     {
    //         string defaultMaterialName = "Default";
    //         if (!trianglesByMaterial.ContainsKey(defaultMaterialName))
    //             trianglesByMaterial[defaultMaterialName] = new List<int>();
    //         trianglesByMaterial[defaultMaterialName].AddRange(ungroupedTriangleIds);
    //     }
        
    //     // Create meshes for each material
    //     foreach (var kvp in trianglesByMaterial)
    //     {
    //         string materialName = kvp.Key;
    //         List<int> triangleIds = kvp.Value;
            
    //         if (triangleIds.Count == 0) continue;
            
    //         // Create a temporary group for this material
    //         string tempGroupName = $"Material_{materialName}";
    //         var tempGroup = new KoreMeshTriangleGroup(materialName, triangleIds);
            
    //         // Temporarily add this group to extract the mesh
    //         var originalGroups = new Dictionary<string, KoreMeshTriangleGroup>(NamedTriangleGroups);
    //         NamedTriangleGroups[tempGroupName] = tempGroup;
            
    //         // Extract the mesh for this material
    //         KoreMeshData materialMesh = CreateMeshForGroup(tempGroupName);
            
    //         // Restore original groups
    //         NamedTriangleGroups.Clear();
    //         foreach (var originalKvp in originalGroups)
    //             NamedTriangleGroups[originalKvp.Key] = originalKvp.Value;
            
    //         if (materialMesh.Triangles.Count > 0)
    //         {
    //             materialMeshes[materialName] = materialMesh;
    //         }
    //     }
        
    //     return materialMeshes;
    // }

    // /// <summary>
    // /// Extract a subset of the mesh containing only specified triangles
    // /// </summary>
    // /// <param name="triangleIds">List of triangle IDs to include in the subset</param>
    // /// <param name="subsetName">Name for the subset (used for group naming)</param>
    // /// <returns>New mesh containing only the specified triangles</returns>
    // public KoreMeshData CreateMeshSubset(List<int> triangleIds, string subsetName = "Subset")
    // {
    //     var subsetMesh = new KoreMeshData();
        
    //     if (triangleIds.Count == 0)
    //         return subsetMesh;
        
    //     // Track vertex ID mapping from original mesh to subset mesh
    //     var vertexIdMapping = new Dictionary<int, int>();
        
    //     // Add all vertices used by the specified triangles
    //     foreach (int triangleId in triangleIds)
    //     {
    //         if (Triangles.ContainsKey(triangleId))
    //         {
    //             KoreMeshTriangle triangle = Triangles[triangleId];
                
    //             // Add vertices if not already added
    //             foreach (int originalVertexId in new[] { triangle.A, triangle.B, triangle.C })
    //             {
    //                 if (!vertexIdMapping.ContainsKey(originalVertexId))
    //                 {
    //                     // Get vertex data from original mesh
    //                     KoreXYZVector position = Vertices[originalVertexId];
    //                     KoreXYZVector? normal = Normals.ContainsKey(originalVertexId) ? Normals[originalVertexId] : null;
    //                     KoreColorRGB? color = VertexColors.ContainsKey(originalVertexId) ? VertexColors[originalVertexId] : null;
    //                     KoreXYVector? uv = UVs.ContainsKey(originalVertexId) ? UVs[originalVertexId] : null;
                        
    //                     // Add vertex to subset mesh and store the mapping
    //                     int newVertexId = subsetMesh.AddVertex(position, normal, color, uv);
    //                     vertexIdMapping[originalVertexId] = newVertexId;
    //                 }
    //             }
    //         }
    //     }
        
    //     // Add triangles using the new vertex IDs
    //     var newTriangleIds = new List<int>();
    //     foreach (int triangleId in triangleIds)
    //     {
    //         if (Triangles.ContainsKey(triangleId))
    //         {
    //             KoreMeshTriangle triangle = Triangles[triangleId];
                
    //             int newVertexA = vertexIdMapping[triangle.A];
    //             int newVertexB = vertexIdMapping[triangle.B];
    //             int newVertexC = vertexIdMapping[triangle.C];
                
    //             int newTriangleId = subsetMesh.AddTriangle(newVertexA, newVertexB, newVertexC);
    //             newTriangleIds.Add(newTriangleId);
    //         }
    //     }
        
    //     // Copy relevant materials
    //     var usedMaterialIds = new HashSet<int>();
    //     foreach (var groupKvp in NamedTriangleGroups)
    //     {
    //         KoreMeshTriangleGroup group = groupKvp.Value;
    //         if (group.TriangleIds.Any(id => triangleIds.Contains(id)))
    //         {
    //             usedMaterialIds.Add(group.MaterialId);
    //         }
    //     }
        
    //     foreach (int materialId in usedMaterialIds)
    //     {
    //         if (Materials.ContainsKey(materialId))
    //         {
    //             subsetMesh.IdForMaterial(Materials[materialId]);
    //         }
    //     }
        
    //     // Add any lines that use vertices in this subset
    //     foreach (var lineKvp in Lines)
    //     {
    //         KoreMeshLine line = lineKvp.Value;
    //         if (vertexIdMapping.ContainsKey(line.A) && vertexIdMapping.ContainsKey(line.B))
    //         {
    //             int newVertexA = vertexIdMapping[line.A];
    //             int newVertexB = vertexIdMapping[line.B];
    //             subsetMesh.AddLine(newVertexA, newVertexB);
    //         }
    //     }
        
    //     return subsetMesh;
    // }
}
