using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public partial class KoreMeshData2
{
    // --------------------------------------------------------------------------------------------
    // MARK: Vertices
    // --------------------------------------------------------------------------------------------

    public int AddVertex(KoreXYZVector vertex)
    {
        Vertices[NextVertexId] = vertex;
        return NextVertexId++; // post-increment, we return the value used, then increase it
    }

    // Check if a vertex exists with the given ID
    public bool HasVertex(int vertexId) { return Vertices.ContainsKey(vertexId); }
    public KoreXYZVector GetVertex(int vertexId) { return Vertices[vertexId]; }
    public void RemoveVertexA(int vertexId) { Vertices.Remove(vertexId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    public int AddNormal(KoreXYZVector normal)
    {
        Normals[NextNormalId] = normal;
        return NextNormalId++;
    }

    public bool HasNormal(int normalId) { return Normals.ContainsKey(normalId); }
    public KoreXYZVector GetNormal(int normalId) { return Normals[normalId]; }
    public void RemoveNormal(int normalId) { Normals.Remove(normalId); }

    // --------------------------------------------------------------------------------------------
    // MARK: UVs
    // --------------------------------------------------------------------------------------------

    public int AddUV(KoreXYVector uv)
    {
        UVs[NextUVId] = uv;
        return NextUVId++;
    }

    public bool HasUV(int uvId) { return UVs.ContainsKey(uvId); }
    public KoreXYVector GetUV(int uvId) { return UVs[uvId]; }
    public void RemoveUV(int uvId) { UVs.Remove(uvId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Colors
    // --------------------------------------------------------------------------------------------

    public int AddColor(KoreColorRGB color)
    {
        Colors[NextColorId] = color;
        return NextColorId++;
    }

    public bool HasColor(int colorId) { return Colors.ContainsKey(colorId); }
    public KoreColorRGB GetColor(int colorId) { return Colors[colorId]; }
    public void RemoveColor(int colorId) { Colors.Remove(colorId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    public int AddLine(KoreMeshLineRef line)
    {
        Lines[NextLineId] = line;
        return NextLineId++;
    }

    public bool HasLine(int lineId) { return Lines.ContainsKey(lineId); }
    public KoreMeshLineRef GetLine(int lineId) { return Lines[lineId]; }
    public void RemoveLine(int lineId) { Lines.Remove(lineId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    public int AddTriangle(KoreMeshTriRef triangle)
    {
        Triangles[NextTriangleId] = triangle;
        return NextTriangleId++;
    }

    public bool HasTriangle(int triangleId) { return Triangles.ContainsKey(triangleId); }
    public KoreMeshTriRef GetTriangle(int triangleId) { return Triangles[triangleId]; }
    public void RemoveTriangle(int triangleId) { Triangles.Remove(triangleId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Material
    // --------------------------------------------------------------------------------------------

    public void AddMaterial(KoreMeshMaterial material)
    {
        Materials.Add(material);
    }

    public bool HasMaterial(string matName)
    {
        return Materials.Any(m => m.Name == matName);
    }

    public KoreMeshMaterial GetMaterial(string matName)
    {
        var material = Materials.FirstOrDefault(m => m.Name == matName);
        return string.IsNullOrEmpty(material.Name) ? KoreMeshMaterialPalette.DefaultMaterial : material;
    }

    public void RemoveMaterial(string materialName)
    {
        var materialToRemove = Materials.FirstOrDefault(m => m.Name == materialName);
        if (!string.IsNullOrEmpty(materialToRemove.Name))
            Materials.Remove(materialToRemove);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: MaterialGroup
    // --------------------------------------------------------------------------------------------

    public void AddMaterialGroup(string groupName, List<int> triangleIds)
    {
        MaterialGroups[groupName] = new KoreMesh2MaterialGroup(groupName, triangleIds);
    }

    public bool HasMaterialGroup(string groupName)
    {
        return MaterialGroups.ContainsKey(groupName);
    }

    public KoreMesh2MaterialGroup GetMaterialGroup(string groupName)
    {
        return MaterialGroups.TryGetValue(groupName, out var group) ? group : default;
    }

    public void RemoveMaterialGroup(string groupName)
    {
        MaterialGroups.Remove(groupName);
    }

}
