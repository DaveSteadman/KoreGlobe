using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

public record struct KoreMeshLine(int A, int B);
public record struct KoreMeshTriangle(int A, int B, int C);
public record struct KoreMeshLineColour(KoreColorRGB StartColor, KoreColorRGB EndColor);
public record struct KoreMeshTriangleColour(KoreColorRGB Color);

// KoreMeshData: A class to hold mesh data for 3D geometry.
// - points, lines, triangles, normals, UVs, vertex colors, line colors, and triangle colors.
// - Information about the larger context, such as the object's name, position, rotation, and scale is handled by a higher level class.

public partial class KoreMeshData
{
    // Vertices by unique ID
    public Dictionary<int, KoreXYZVector> Vertices = new();
    // Normals by vertex ID
    public Dictionary<int, KoreXYZVector> Normals = new();
    // UVs by vertex ID
    public Dictionary<int, KoreXYVector> UVs = new();
    // Vertex colors by vertex ID
    public Dictionary<int, KoreColorRGB> VertexColors = new();

    // Lines by unique ID, each referencing vertex IDs
    public Dictionary<int, KoreMeshLine> Lines = new();
    // Line colors by line ID
    public Dictionary<int, KoreMeshLineColour> LineColors = new();

    // Triangles by unique ID, each referencing vertex IDs
    public Dictionary<int, KoreMeshTriangle> Triangles = new();
    // Triangle colors by triangle ID
    public Dictionary<int, KoreMeshTriangleColour> TriangleColors = new();

    // Internal counters for unique IDs
    private int _nextVertexId = 0;
    private int _nextLineId = 0;
    private int _nextTriangleId = 0;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Empty constructor
    public KoreMeshData() { }

    // Copy constructor
    public KoreMeshData(
        Dictionary<int, KoreXYZVector>          vertices,
        Dictionary<int, KoreMeshLine>           lines,
        Dictionary<int, KoreMeshTriangle>       triangles,
        Dictionary<int, KoreXYZVector>          normals,
        Dictionary<int, KoreXYVector>           uvs,
        Dictionary<int, KoreColorRGB>           vertexColors,
        Dictionary<int, KoreMeshLineColour>     lineColors,
        Dictionary<int, KoreMeshTriangleColour> triangleColors)
    {
        this.Vertices       = vertices;
        this.Lines          = lines;
        this.Triangles      = triangles;
        this.Normals        = normals;
        this.UVs            = uvs;
        this.VertexColors   = vertexColors;
        this.LineColors     = lineColors;
        this.TriangleColors = triangleColors;
    }


    // Copy constructor
    public KoreMeshData(KoreMeshData mesh)
    {
        this.Vertices       = new Dictionary<int, KoreXYZVector>(mesh.Vertices);
        this.Lines          = new Dictionary<int, KoreMeshLine>(mesh.Lines);
        this.Triangles      = new Dictionary<int, KoreMeshTriangle>(mesh.Triangles);
        this.Normals        = new Dictionary<int, KoreXYZVector>(mesh.Normals);
        this.UVs            = new Dictionary<int, KoreXYVector>(mesh.UVs);
        this.VertexColors   = new Dictionary<int, KoreColorRGB>(mesh.VertexColors);
        this.LineColors     = new Dictionary<int, KoreMeshLineColour>(mesh.LineColors);
        this.TriangleColors = new Dictionary<int, KoreMeshTriangleColour>(mesh.TriangleColors);
    }


    // Initialises the mesh data with empty dictionaries
    public void ClearAllData()
    {
        Vertices.Clear();
        Lines.Clear();
        Triangles.Clear();
        Normals.Clear();
        UVs.Clear();
        VertexColors.Clear();
        LineColors.Clear();
        TriangleColors.Clear();
        _nextVertexId = 0;
        _nextLineId = 0;
        _nextTriangleId = 0;
    }



    // --------------------------------------------------------------------------------------------
    // MARK: Points
    // --------------------------------------------------------------------------------------------

    // Add a vertex and return its ID
    public int AddVertex(KoreXYZVector vertex, KoreXYZVector? normal = null, KoreColorRGB? color = null, KoreXYVector? uv = null)
    {
        int id = _nextVertexId++;
        Vertices[id] = vertex;
        
        if (normal.HasValue) Normals[id] = normal.Value;
        if (color.HasValue) VertexColors[id] = color.Value;
        if (uv.HasValue) UVs[id] = uv.Value;
        
        return id;
    }

    public void SetVertex(int id, KoreXYZVector vertex)
    {
        // We want to throw here, because we have a unique ID concept and random new additions break this
        if (!Vertices.ContainsKey(id))
            throw new ArgumentOutOfRangeException(nameof(id), "Vertex ID is not found.");

        Vertices[id] = vertex;
    }

    // function to add a point from a serialised source (ie bypassing some of the id checks)
    public void AddFromData(int vertexId, KoreXYZVector vertex, KoreXYZVector? normal = null, KoreColorRGB? color = null, KoreXYVector? uv = null)
    {
        Vertices[vertexId] = vertex;

        if (normal.HasValue) Normals[vertexId] = normal.Value;
        if (color.HasValue) VertexColors[vertexId] = color.Value;
        if (uv.HasValue) UVs[vertexId] = uv.Value;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    public int AddNormal(KoreXYZVector normal)
    {
        int id = _nextVertexId++;

        Normals[id] = normal;
        return id;
    }

    public void SetNormal(int vertexId, KoreXYZVector normal)
    {
        // Need to have the normal tied to the vertex ID
        if (!Vertices.ContainsKey(vertexId))
            throw new ArgumentOutOfRangeException(nameof(vertexId), "Vertex ID is not found.");

        Normals[vertexId] = normal;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UVs
    // --------------------------------------------------------------------------------------------


    public void SetUV(int vertexId, KoreXYVector uv)
    {
        // We want to throw here, because we have a unique ID concept and random new additions break this
        if (!Vertices.ContainsKey(vertexId))
            throw new ArgumentOutOfRangeException(nameof(vertexId), "Vertex ID is not found.");
        UVs[vertexId] = uv;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Vertex Colors
    // --------------------------------------------------------------------------------------------


    public void SetVertexColor(int vertexId, KoreColorRGB color)
    {
        // Can only set a vertex colour for a valid vertex ID
        if (!Vertices.ContainsKey(vertexId))
            throw new ArgumentOutOfRangeException(nameof(vertexId), "Vertex ID is not found.");
            
        VertexColors[vertexId] = color;
    }
    
    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    // Add a line and return its ID
    public int AddLine(int vertexIdA, int vertexIdB, KoreColorRGB? colStart = null, KoreColorRGB? colEnd = null)
    {
        int id = _nextLineId++;
        Lines[id] = new KoreMeshLine(vertexIdA, vertexIdB);

        if (colStart.HasValue && colEnd.HasValue)
            LineColors[id] = new KoreMeshLineColour(colStart.Value, colEnd.Value);
        return id;
    }

    public int AddLine(KoreMeshLine line, KoreColorRGB? colStart = null, KoreColorRGB? colEnd = null)
    {
        return AddLine(line.A, line.B, colStart, colEnd);
    }

    public int AddLine(KoreXYZVector start, KoreXYZVector end, KoreColorRGB colStart, KoreColorRGB colEnd)
    {
        int idxA = AddVertex(start, null, colStart);
        int idxB = AddVertex(end, null, colEnd);
        return AddLine(idxA, idxB, colStart, colEnd);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Line Colors
    // --------------------------------------------------------------------------------------------

    public void SetLineColor(int lineId, KoreColorRGB startColor, KoreColorRGB endColor)
    {
        if (!Lines.ContainsKey(lineId))
            throw new ArgumentOutOfRangeException(nameof(lineId), "Line ID is not found.");
        LineColors[lineId] = new KoreMeshLineColour(startColor, endColor);
    }

    public void SetAllLineColors(KoreColorRGB startColor, KoreColorRGB endColor)
    {
        foreach (var lineId in Lines.Keys)
        {
            SetLineColor(lineId, startColor, endColor);
        }
    }

    public void SetAllLineColors(KoreColorRGB color) => SetAllLineColors(color, color);

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    // Add a triangle and return its ID
    public int AddTriangle(int vertexIdA, int vertexIdB, int vertexIdC, KoreColorRGB? color = null)
    {
        int id = _nextTriangleId++;
        Triangles[id] = new KoreMeshTriangle(vertexIdA, vertexIdB, vertexIdC);

        if (color.HasValue)
            TriangleColors[id] = new KoreMeshTriangleColour(color.Value);

        return id;
    }

    public int AddTriangle(KoreMeshTriangle triangle, KoreColorRGB? color = null)
    {
        return AddTriangle(triangle.A, triangle.B, triangle.C, color);
    }

    // Add a completely independent triangle with vertices and optional color.
    public int AddTriangle(KoreXYZVector a, KoreXYZVector b, KoreXYZVector c, KoreColorRGB? linecolor = null, KoreColorRGB? fillColor = null)
    {
        int idxA = AddVertex(a);
        int idxB = AddVertex(b);
        int idxC = AddVertex(c);

        // Set the vertex colors if fill color is provided
        if (fillColor.HasValue)
        {
            VertexColors[idxA] = fillColor.Value;
            VertexColors[idxB] = fillColor.Value;
            VertexColors[idxC] = fillColor.Value;
        }

        // Use the line color if provided, otherwise don't add the lines.
        if (linecolor.HasValue)
        {
            KoreColorRGB lineCol = linecolor.Value;
            AddLine(idxA, idxB, lineCol, lineCol);
            AddLine(idxB, idxC, lineCol, lineCol);
            AddLine(idxC, idxA, lineCol, lineCol);
        }

        int triId = AddTriangle(idxA, idxB, idxC, fillColor);
        return triId;
    }

    // Add an isolated triangle with automatically calculated normals for sharp edges.
    // Creates three separate vertices (no sharing) with proper face normals for crisp rendering.
    public int AddIsolatedTriangle(KoreXYZVector a, KoreXYZVector b, KoreXYZVector c, 
        KoreColorRGB? vertexColor = null, KoreColorRGB? triangleColor = null)
    {
        // Calculate the face normal using cross product
        KoreXYZVector ab = b - a;  // Vector from A to B
        KoreXYZVector ac = c - a;  // Vector from A to C
        
        // Cross product gives us the face normal (right-hand rule)
        KoreXYZVector faceNormal = KoreXYZVector.CrossProduct(ab, ac);
        
        // Normalize the face normal using the built-in method
        faceNormal = faceNormal.Normalize();
        faceNormal = faceNormal.Invert();

        // Add three separate vertices with the same face normal for sharp edges
        int idxA = AddVertex(a, faceNormal, vertexColor);
        int idxB = AddVertex(b, faceNormal, vertexColor);
        int idxC = AddVertex(c, faceNormal, vertexColor);

        // Add the triangle
        int triId = AddTriangle(idxA, idxB, idxC, triangleColor);
        return triId;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Colors
    // --------------------------------------------------------------------------------------------


    public void SetTriangleColor(int triangleId, KoreColorRGB color)
    {
        if (!Triangles.ContainsKey(triangleId))
            throw new ArgumentOutOfRangeException(nameof(triangleId), "Triangle ID is not found.");
        TriangleColors[triangleId] = new KoreMeshTriangleColour(color);
    }
}


