using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

/// <summary>
/// Mesh division and extraction operations for KoreMeshData
/// Contains methods for splitting meshes into smaller parts based on groups, materials, etc.
/// </summary>
public partial class KoreMeshData
{
    /// <summary>
    /// Create a new KoreMeshData containing only the geometry and material for a specific named group
    /// </summary>
    /// <param name="groupName">Name of the triangle group to extract</param>
    /// <returns>New mesh containing only the specified group's data</returns>
    public KoreMeshData CreateMeshForGroup(string groupName)
    {
        var groupMesh = new KoreMeshData();
        
        // Return empty mesh if group doesn't exist
        if (!NamedTriangleGroups.ContainsKey(groupName))
            return groupMesh;
        
        KoreMeshTriangleGroup sourceGroup = NamedTriangleGroups[groupName];

        // Copy the material for this group into the new mesh
        int newMaterialId = 0;
        if (Materials.ContainsKey(sourceGroup.MaterialId))
        {
            KoreMeshMaterial material = Materials[sourceGroup.MaterialId];
            newMaterialId = groupMesh.IdForMaterial(material);
        }

        // Add the named group to the new mesh
        groupMesh.AddNamedGroup(groupName);
        groupMesh.SetGroupMaterialId(groupName, newMaterialId);

        // Look at the source mesh. Create a hash set of each triangle, vertex and line ID (each indexing value) that is relevant to our named group.
        HashSet<int> relevantVertexIds   = new HashSet<int>();
        HashSet<int> relevantLineIds     = new HashSet<int>();
        HashSet<int> relevantTriangleIds = new HashSet<int>();

        foreach (int triangleId in sourceGroup.TriangleIds)
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
        groupMesh.NextMaterialId = this.NextMaterialId;

        return groupMesh;
    }

    /// <summary>
    /// Create separate meshes for each named triangle group
    /// </summary>
    /// <returns>Dictionary mapping group names to their extracted meshes</returns>
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

    /// <summary>
    /// Create separate meshes for each unique material
    /// Groups triangles by material ID and creates a mesh for each material
    /// </summary>
    /// <returns>Dictionary mapping material names to their extracted meshes</returns>
    public Dictionary<string, KoreMeshData> CreateMeshesForEachMaterial()
    {
        var materialMeshes = new Dictionary<string, KoreMeshData>();
        
        // Group triangles by material
        var trianglesByMaterial = new Dictionary<int, List<int>>();
        
        // First collect triangles from named groups
        foreach (var groupKvp in NamedTriangleGroups)
        {
            KoreMeshTriangleGroup group = groupKvp.Value;
            int materialId = group.MaterialId;
            
            if (!trianglesByMaterial.ContainsKey(materialId))
                trianglesByMaterial[materialId] = new List<int>();
                
            trianglesByMaterial[materialId].AddRange(group.TriangleIds);
        }
        
        // Then collect any ungrouped triangles (assign to default material)
        var groupedTriangleIds = new HashSet<int>();
        foreach (var group in NamedTriangleGroups.Values)
        {
            foreach (int triangleId in group.TriangleIds)
                groupedTriangleIds.Add(triangleId);
        }
        
        var ungroupedTriangleIds = Triangles.Keys.Where(id => !groupedTriangleIds.Contains(id)).ToList();
        if (ungroupedTriangleIds.Count > 0)
        {
            int defaultMaterialId = 0; // Use 0 as default material ID
            if (!trianglesByMaterial.ContainsKey(defaultMaterialId))
                trianglesByMaterial[defaultMaterialId] = new List<int>();
            trianglesByMaterial[defaultMaterialId].AddRange(ungroupedTriangleIds);
        }
        
        // Create meshes for each material
        foreach (var kvp in trianglesByMaterial)
        {
            int materialId = kvp.Key;
            List<int> triangleIds = kvp.Value;
            
            if (triangleIds.Count == 0) continue;
            
            // Create a temporary group for this material
            string tempGroupName = $"Material_{materialId}";
            var tempGroup = new KoreMeshTriangleGroup(materialId, triangleIds);
            
            // Temporarily add this group to extract the mesh
            var originalGroups = new Dictionary<string, KoreMeshTriangleGroup>(NamedTriangleGroups);
            NamedTriangleGroups[tempGroupName] = tempGroup;
            
            // Extract the mesh for this material
            KoreMeshData materialMesh = CreateMeshForGroup(tempGroupName);
            
            // Restore original groups
            NamedTriangleGroups.Clear();
            foreach (var originalKvp in originalGroups)
                NamedTriangleGroups[originalKvp.Key] = originalKvp.Value;
            
            // Get material name
            string materialName = Materials.ContainsKey(materialId) ? Materials[materialId].Name : $"Material_{materialId}";
            
            if (materialMesh.Triangles.Count > 0)
            {
                materialMeshes[materialName] = materialMesh;
            }
        }
        
        return materialMeshes;
    }

    /// <summary>
    /// Extract a subset of the mesh containing only specified triangles
    /// </summary>
    /// <param name="triangleIds">List of triangle IDs to include in the subset</param>
    /// <param name="subsetName">Name for the subset (used for group naming)</param>
    /// <returns>New mesh containing only the specified triangles</returns>
    public KoreMeshData CreateMeshSubset(List<int> triangleIds, string subsetName = "Subset")
    {
        var subsetMesh = new KoreMeshData();
        
        if (triangleIds.Count == 0)
            return subsetMesh;
        
        // Track vertex ID mapping from original mesh to subset mesh
        var vertexIdMapping = new Dictionary<int, int>();
        
        // Add all vertices used by the specified triangles
        foreach (int triangleId in triangleIds)
        {
            if (Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = Triangles[triangleId];
                
                // Add vertices if not already added
                foreach (int originalVertexId in new[] { triangle.A, triangle.B, triangle.C })
                {
                    if (!vertexIdMapping.ContainsKey(originalVertexId))
                    {
                        // Get vertex data from original mesh
                        KoreXYZVector position = Vertices[originalVertexId];
                        KoreXYZVector? normal = Normals.ContainsKey(originalVertexId) ? Normals[originalVertexId] : null;
                        KoreColorRGB? color = VertexColors.ContainsKey(originalVertexId) ? VertexColors[originalVertexId] : null;
                        KoreXYVector? uv = UVs.ContainsKey(originalVertexId) ? UVs[originalVertexId] : null;
                        
                        // Add vertex to subset mesh and store the mapping
                        int newVertexId = subsetMesh.AddVertex(position, normal, color, uv);
                        vertexIdMapping[originalVertexId] = newVertexId;
                    }
                }
            }
        }
        
        // Add triangles using the new vertex IDs
        var newTriangleIds = new List<int>();
        foreach (int triangleId in triangleIds)
        {
            if (Triangles.ContainsKey(triangleId))
            {
                KoreMeshTriangle triangle = Triangles[triangleId];
                
                int newVertexA = vertexIdMapping[triangle.A];
                int newVertexB = vertexIdMapping[triangle.B];
                int newVertexC = vertexIdMapping[triangle.C];
                
                int newTriangleId = subsetMesh.AddTriangle(newVertexA, newVertexB, newVertexC);
                newTriangleIds.Add(newTriangleId);
            }
        }
        
        // Copy relevant materials
        var usedMaterialIds = new HashSet<int>();
        foreach (var groupKvp in NamedTriangleGroups)
        {
            KoreMeshTriangleGroup group = groupKvp.Value;
            if (group.TriangleIds.Any(id => triangleIds.Contains(id)))
            {
                usedMaterialIds.Add(group.MaterialId);
            }
        }
        
        foreach (int materialId in usedMaterialIds)
        {
            if (Materials.ContainsKey(materialId))
            {
                subsetMesh.IdForMaterial(Materials[materialId]);
            }
        }
        
        // Add any lines that use vertices in this subset
        foreach (var lineKvp in Lines)
        {
            KoreMeshLine line = lineKvp.Value;
            if (vertexIdMapping.ContainsKey(line.A) && vertexIdMapping.ContainsKey(line.B))
            {
                int newVertexA = vertexIdMapping[line.A];
                int newVertexB = vertexIdMapping[line.B];
                subsetMesh.AddLine(newVertexA, newVertexB);
            }
        }
        
        return subsetMesh;
    }
}
