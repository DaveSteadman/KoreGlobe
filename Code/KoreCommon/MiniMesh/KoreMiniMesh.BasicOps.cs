using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public partial class KoreMiniMesh
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

    public int AddLine(KoreMiniMeshLine line)
    {
        Lines[NextLineId] = line;
        return NextLineId++;
    }

    public bool HasLine(int lineId) { return Lines.ContainsKey(lineId); }
    public KoreMiniMeshLine GetLine(int lineId) { return Lines[lineId]; }
    public void RemoveLine(int lineId) { Lines.Remove(lineId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    public int AddTriangle(KoreMiniMeshTri triangle)
    {
        Triangles[NextTriangleId] = triangle;
        return NextTriangleId++;
    }

    public bool HasTriangle(int triangleId) { return Triangles.ContainsKey(triangleId); }
    public KoreMiniMeshTri GetTriangle(int triangleId) { return Triangles[triangleId]; }
    public void RemoveTriangle(int triangleId) { Triangles.Remove(triangleId); }

    // --------------------------------------------------------------------------------------------
    // MARK: Group
    // --------------------------------------------------------------------------------------------

    public void AddGroup(string groupName, KoreMiniMeshGroup group) { Groups[groupName] = group; }
    public bool HasGroup(string groupName) { return Groups.ContainsKey(groupName); }
    public KoreMiniMeshGroup GetGroup(string groupName) { return Groups.TryGetValue(groupName, out var group) ? group : default; }
    public void RemoveGroup(string groupName) { Groups.Remove(groupName); }

}
