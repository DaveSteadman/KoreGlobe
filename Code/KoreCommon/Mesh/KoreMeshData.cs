using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

public record struct KoreMeshLine(int A, int B);
public record struct KoreMeshTriangle(int A, int B, int C);
public record struct KoreMeshLineColour(KoreColorRGB StartColor, KoreColorRGB EndColor);
public record struct KoreMeshTriangleGroup(int MaterialId, List<int> TriangleIds);

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

    // Vertex colors by vertex ID - for when the mesh is colored by vertex
    public Dictionary<int, KoreColorRGB> VertexColors = new();

    // Lines by unique ID, each referencing vertex IDs
    public Dictionary<int, KoreMeshLine> Lines = new();

    // Line colors by line ID
    public Dictionary<int, KoreMeshLineColour> LineColors = new();

    // Triangles by unique ID, each referencing vertex IDs
    public Dictionary<int, KoreMeshTriangle> Triangles = new();

    // Palette of Material for this mesh, by unique ID
    public Dictionary<int, KoreMeshMaterial> Materials = new();
    
    // Named groups, logical/useful sub-divisions of the mesh triangles, with a material.
    // - Non-exclusive inclusion of triangles allows for multiple uses and some manner of hierarchy
    public Dictionary<string, KoreMeshTriangleGroup> NamedTriangleGroups = new(); // Tags for grouping triangles

    // Counters for unique IDs
    public int NextVertexId = 0;
    public int NextLineId = 0;
    public int NextTriangleId = 0;
    public int NextMaterialId = 0;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Empty constructor
    public KoreMeshData() { }

    // Copy constructor
    public KoreMeshData(KoreMeshData mesh)
    {
        this.Vertices = new Dictionary<int, KoreXYZVector>(mesh.Vertices);
        this.Lines = new Dictionary<int, KoreMeshLine>(mesh.Lines);
        this.Triangles = new Dictionary<int, KoreMeshTriangle>(mesh.Triangles);
        this.Normals = new Dictionary<int, KoreXYZVector>(mesh.Normals);
        this.UVs = new Dictionary<int, KoreXYVector>(mesh.UVs);
        this.VertexColors = new Dictionary<int, KoreColorRGB>(mesh.VertexColors);
        this.LineColors = new Dictionary<int, KoreMeshLineColour>(mesh.LineColors);
        this.Materials = new Dictionary<int, KoreMeshMaterial>(mesh.Materials);
        this.NamedTriangleGroups = new Dictionary<string, KoreMeshTriangleGroup>(mesh.NamedTriangleGroups);
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
        Materials.Clear();
        NamedTriangleGroups.Clear();

        NextVertexId = 0;
        NextLineId = 0;
        NextTriangleId = 0;
        NextMaterialId = 0;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Points
    // --------------------------------------------------------------------------------------------

    public int AddVertex(KoreXYZVector vertex)
    {
        int id = NextVertexId++;
        Vertices[id] = vertex;
        return id;
    }

    public void SetVertex(int id, KoreXYZVector vertex)
    {
        Vertices[id] = vertex;
    }

    // Add a vertex and return its ID
    public int AddVertex(KoreXYZVector vertex, KoreXYZVector? normal = null, KoreColorRGB? color = null, KoreXYVector? uv = null)
    {
        int id = NextVertexId++;
        Vertices[id] = vertex;

        if (normal.HasValue) Normals[id] = normal.Value;
        if (color.HasValue) VertexColors[id] = color.Value;
        if (uv.HasValue) UVs[id] = uv.Value;

        return id;
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

    public void SetNormalForId(int vertexId, KoreXYZVector normal)
    {
        // Need to have the normal tied to the vertex ID
        if (!Vertices.ContainsKey(vertexId))
            throw new ArgumentOutOfRangeException(nameof(vertexId), "Vertex ID is not found.");

        Normals[vertexId] = normal;
    }

    // --------------------------------------------------------------------------------------------

    public KoreXYZVector CalcNormalsForTriangle(int triangleId)
    {
        if (!Triangles.ContainsKey(triangleId))
            return KoreXYZVector.Zero;

        // Get the vertices
        KoreMeshTriangle triangle = Triangles[triangleId];
        KoreXYZVector a = Vertices[triangle.A];
        KoreXYZVector b = Vertices[triangle.B];
        KoreXYZVector c = Vertices[triangle.C];

        // Calculate the face normal using cross product
        KoreXYZVector ab = b - a;  // Vector from A to B
        KoreXYZVector ac = c - a;  // Vector from A to C
        KoreXYZVector faceNormal = KoreXYZVector.CrossProduct(ab, ac).Normalize();

        // Normalize and invert the face normal
        faceNormal = faceNormal.Normalize();
        faceNormal = faceNormal.Invert(); // Required Step - no explanation

        // Set the normals
        Normals[triangle.A] = faceNormal;
        Normals[triangle.B] = faceNormal;
        Normals[triangle.C] = faceNormal;

        return faceNormal;
    }

    // --------------------------------------------------------------------------------------------

    // Set normals for all vertices based on the first triangle that contains each vertex
    // Usage: mesh.CalcNormalsForAllTriangles();

    public void CalcNormalsForAllTriangles()
    {
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            CalcNormalsForTriangle(triangleId);
        }
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

    public void SetAllVertexColors(KoreColorRGB color)
    {
        foreach (var vertexId in Vertices.Keys)
            SetVertexColor(vertexId, color);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    public int AddLine(int vertexIdA, int vertexIdB, KoreColorRGB colLine)
    {
        int id = NextLineId++;
        Lines[id] = new KoreMeshLine(vertexIdA, vertexIdB);
        LineColors[id] = new KoreMeshLineColour(colLine, colLine);
        return id;
    }

    // Add a line and return its ID
    public int AddLine(int vertexIdA, int vertexIdB, KoreColorRGB? colStart = null, KoreColorRGB? colEnd = null)
    {
        int id = NextLineId++;
        Lines[id] = new KoreMeshLine(vertexIdA, vertexIdB);

        if (colStart.HasValue && colEnd.HasValue)
            LineColors[id] = new KoreMeshLineColour(colStart.Value, colEnd.Value);
        return id;
    }

    public int AddLine(KoreMeshLine line, KoreColorRGB? colStart = null, KoreColorRGB? colEnd = null)
    {
        return AddLine(line.A, line.B, colStart, colEnd);
    }

    public int AddLine(KoreXYZVector start, KoreXYZVector end, KoreColorRGB colLine) => AddLine(start, end, colLine, colLine);

    public int AddLine(KoreXYZVector start, KoreXYZVector end, KoreColorRGB colStart, KoreColorRGB colEnd)
    {
        int idxA = AddVertex(start, null, colStart);
        int idxB = AddVertex(end, null, colEnd);
        return AddLine(idxA, idxB, colStart, colEnd);
    }

    // Helper method to add a line only if it doesn't already exist
    private int AddLineIfNotExists(int vertexIdA, int vertexIdB, KoreColorRGB lineColor)
    {
        // Check if line already exists (in either direction)
        var targetLine1 = new KoreMeshLine(vertexIdA, vertexIdB);
        var targetLine2 = new KoreMeshLine(vertexIdB, vertexIdA);

        foreach (var kvp in Lines)
        {
            if (kvp.Value.Equals(targetLine1) || kvp.Value.Equals(targetLine2))
            {
                return kvp.Key; // Return existing line ID
            }
        }

        // Line doesn't exist, create it
        return AddLine(vertexIdA, vertexIdB, lineColor);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Outline
    // --------------------------------------------------------------------------------------------

    public void OutlineTriangle(int v0, int v1, int v2, KoreColorRGB linecolor)
    {
        // Outline the triangle by adding lines between its vertices
        AddLineIfNotExists(v0, v1, linecolor);
        AddLineIfNotExists(v1, v2, linecolor);
        AddLineIfNotExists(v2, v0, linecolor);
    }

    public void OutlineTriangle(int triId, KoreColorRGB linecolor)
    {
        if (Triangles.ContainsKey(triId))
        {
            KoreMeshTriangle triangle = Triangles[triId];
            OutlineTriangle(triangle.A, triangle.B, triangle.C, linecolor);
        }
    }

    // --------------------------------------------------------------------------------------------

    public void OutlineFace(int v0, int v1, int v2, int v3, KoreColorRGB linecolor)
    {
        // Outline the face by adding lines between its vertices
        AddLineIfNotExists(v0, v1, linecolor);
        AddLineIfNotExists(v1, v2, linecolor);
        AddLineIfNotExists(v2, v3, linecolor);
        AddLineIfNotExists(v3, v0, linecolor);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Dotted Lines
    // --------------------------------------------------------------------------------------------

    public void AddDottedLineByDistance(KoreXYZVector start, KoreXYZVector end, KoreColorRGB colLine, double dotSpacing)
    {
        int p1 = AddVertex(start, null, colLine);
        int p2 = AddVertex(end, null, colLine);
        AddDottedLineByDistance(p1, p2, colLine, dotSpacing);
    }

    public void AddDottedLineByDistance(int vertexIdA, int vertexIdB, KoreColorRGB colLine, double dotSpacing)
    {
        // Calculate the distance between the two vertices
        KoreXYZPoint pntA = new KoreXYZPoint(Vertices[vertexIdA]);
        KoreXYZPoint pntB = new KoreXYZPoint(Vertices[vertexIdB]);

        double distance = pntA.DistanceTo(pntB);

        double currDist = 0.0;
        double dotLength = dotSpacing * 0.5; // Each dot is half the spacing

        while (currDist < distance)
        {
            // Calculate the start point of this dot
            double tStart = currDist / distance;
            KoreXYZPoint dotStart = new KoreXYZPoint(
                pntA.X + (pntB.X - pntA.X) * tStart,
                pntA.Y + (pntB.Y - pntA.Y) * tStart,
                pntA.Z + (pntB.Z - pntA.Z) * tStart
            );

            // Calculate the end point of this dot
            double dotEndDist = Math.Min(currDist + dotLength, distance);
            double tEnd = dotEndDist / distance;
            KoreXYZPoint dotEnd = new KoreXYZPoint(
                pntA.X + (pntB.X - pntA.X) * tEnd,
                pntA.Y + (pntB.Y - pntA.Y) * tEnd,
                pntA.Z + (pntB.Z - pntA.Z) * tEnd
            );

            // Add the line segment (the dot)
            AddLine(new KoreXYZVector(dotStart), new KoreXYZVector(dotEnd), colLine);

            // Move to the next dot position (including the gap)
            currDist += dotSpacing;
        }

        // Not returning a value since we're not creating a single returnable lineId
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Poly Line
    // --------------------------------------------------------------------------------------------

    // Add a list of points and connect them all with lines.
    public void AddPolyLine(List<KoreXYZVector> points, KoreColorRGB colLine)
    {
        if (points.Count < 2)
            throw new ArgumentException("At least two points are required to create a polyline.");

        // Add all the points and record the ids in a list
        List<int> pointIds = new List<int>();

        foreach (KoreXYZVector pnt in points)
        {
            int id = AddVertex(pnt, null, colLine);
            pointIds.Add(id);
        }

        for (int i = 0; i < pointIds.Count - 1; i++)
        {
            AddLine(pointIds[i], pointIds[i + 1], colLine);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Line Colors
    // --------------------------------------------------------------------------------------------

    public void SetLineColor(int lineId, KoreColorRGB lineColor)
    {
        // We want to throw here, because we have a unique ID concept and random new additions break this
        if (!Lines.ContainsKey(lineId))
            throw new ArgumentOutOfRangeException(nameof(lineId), "Line ID is not found.");

        LineColors[lineId] = new KoreMeshLineColour(lineColor, lineColor);
    }

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
    // MARK: Triangle
    // --------------------------------------------------------------------------------------------

    // Add a triangle and return its ID
    public int AddTriangle(int vertexIdA, int vertexIdB, int vertexIdC)
    {
        int id = NextTriangleId++;
        Triangles[id] = new KoreMeshTriangle(vertexIdA, vertexIdB, vertexIdC);

        return id;
    }

    public int AddTriangle(KoreMeshTriangle triangle)
    {
        return AddTriangle(triangle.A, triangle.B, triangle.C);
    }

    // Add a completely independent triangle with vertices and optional color.
    public int AddTriangle(KoreXYZVector a, KoreXYZVector b, KoreXYZVector c)
    {
        int idxA = AddVertex(a);
        int idxB = AddVertex(b);
        int idxC = AddVertex(c);

        int triId = AddTriangle(idxA, idxB, idxC);

        return triId;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Face
    // --------------------------------------------------------------------------------------------

    // a ---- b
    // |      |
    // d ---- c

    // Add a new isolated face as two triangles with automatically calculated normals for sharp edges.
    // Creates four separate vertices (no sharing) with proper face normals for crisp rendering.
    public void AddFace(KoreXYZVector a, KoreXYZVector b, KoreXYZVector c, KoreXYZVector d)
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
        int idxA = AddVertex(a, faceNormal);
        int idxB = AddVertex(b, faceNormal);
        int idxC = AddVertex(c, faceNormal);
        int idxD = AddVertex(d, faceNormal);

        // Add the triangle
        int triId1 = AddTriangle(idxA, idxB, idxC);
        int triId2 = AddTriangle(idxA, idxC, idxD);
    }

    public void AddFace(int aId, int bId, int cId, int dId)
    {
        // Create two triangles from the face
        AddTriangle(aId, bId, cId);
        AddTriangle(aId, cId, dId);
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Materials
    // --------------------------------------------------------------------------------------------

    // The mesh class will look to add materials given any opportunity, and use a cull orphaned materials function
    // to remove them later if needed.

    public int IdForMaterial(KoreMeshMaterial material)
    {
        // Search the Materials list for a matching material
        foreach (var kvp in Materials)
        {
            if (kvp.Value.Equals(material))
            {
                return kvp.Key; // Return existing material ID if found
            }
        }

        // Material not found - add a new one
        int materialId = NextMaterialId++;
        Materials[materialId] = material;

        return materialId;
    }

    // Get the material, or return the default material if not setup
    public KoreMeshMaterial MaterialForId(int materialId)
    {
        return Materials.ContainsKey(materialId) ? Materials[materialId] : KoreMeshMaterialPalette.DefaultMaterial;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Groups
    // --------------------------------------------------------------------------------------------

    public void AddNamedGroup(string newName)
    {
        if (!NamedTriangleGroups.ContainsKey(newName))
        {
            KoreMeshTriangleGroup newGroup = new KoreMeshTriangleGroup
            {
                MaterialId = -1,
                TriangleIds = new List<int>()
            };
            NamedTriangleGroups[newName] = newGroup;
        }
    }

    public bool HasNamedGroup(string groupName)
    {
        return NamedTriangleGroups.ContainsKey(groupName);
    }

    public void SetGroupMaterialId(string groupName, int materialId)
    {
        if (NamedTriangleGroups.ContainsKey(groupName))
        {
            KoreMeshTriangleGroup currGroup = NamedTriangleGroups[groupName];
            currGroup.MaterialId = materialId;
            NamedTriangleGroups[groupName] = currGroup;
        }
        else
        {
            KoreMeshTriangleGroup newGroup = new KoreMeshTriangleGroup
            {
                MaterialId = materialId,
                TriangleIds = new List<int>()
            };
            NamedTriangleGroups[groupName] = newGroup;
        }
    }

    public void AddGroupWithMaterial(string groupName, KoreMeshMaterial material)
    {
        if (!NamedTriangleGroups.ContainsKey(groupName))
        {
            NamedTriangleGroups[groupName] = new KoreMeshTriangleGroup
            {
                MaterialId = IdForMaterial(material),
                TriangleIds = new List<int>()
            };
        }
    }

    // --------------------------------------------------------------------------------------------


    public void AddTriangleToGroup(int triangleId, string groupName)
    {
        if (NamedTriangleGroups.ContainsKey(groupName))
        {
            NamedTriangleGroups[groupName].TriangleIds.Add(triangleId);
        }
        else
        {
            KoreMeshTriangleGroup newGroup = new KoreMeshTriangleGroup
            {
                MaterialId = -1,
                TriangleIds = new List<int> { triangleId }
            };
            NamedTriangleGroups[groupName] = newGroup;
        }
    }

    public void AddAllTrianglesToGroup(string groupName)
    {
        if (!NamedTriangleGroups.ContainsKey(groupName))
        {
            AddNamedGroup(groupName);
        }

        KoreMeshTriangleGroup group = NamedTriangleGroups[groupName];
        foreach (var triangle in Triangles)
        {
            group.TriangleIds.Add(triangle.Key);
        }
        NamedTriangleGroups[groupName] = group;
    }

    public HashSet<KoreXYZVector> NamedGroupVertices(string groupName)
    {
        HashSet<KoreXYZVector> vertices = new HashSet<KoreXYZVector>();

        if (NamedTriangleGroups.ContainsKey(groupName))
        {
            KoreMeshTriangleGroup group = NamedTriangleGroups[groupName];
            foreach (int triangleId in group.TriangleIds)
            {
                if (Triangles.ContainsKey(triangleId))
                {
                    KoreMeshTriangle triangle = Triangles[triangleId];
                    vertices.Add(Vertices[triangle.A]);
                    vertices.Add(Vertices[triangle.B]);
                    vertices.Add(Vertices[triangle.C]);
                }
            }
        }

        return vertices;
    }
}