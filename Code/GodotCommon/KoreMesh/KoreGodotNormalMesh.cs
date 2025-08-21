// KoreGodotNormalMesh : Class to visualize normal vectors from KoreMeshData as colored lines.
// - Creates lines extending from each vertex in the direction of its normal
// - Helps debug mesh orientation and normal calculations
// - Uses different colors for different normal directions

using System;

using KoreCommon;
using Godot;

public partial class KoreGodotNormalMesh : MeshInstance3D
{
    private SurfaceTool _surfaceTool;
    private float _normalLength = 0.2f; // Length of normal lines
    private bool _showVertexNormals = true;
    private bool _showTriangleNormals = false;

    // Colors for different normal directions
    private readonly Color _positiveXColor = new Color(1.0f, 0.0f, 0.0f, 1.0f); // Red for +X
    private readonly Color _positiveYColor = new Color(0.0f, 1.0f, 0.0f, 1.0f); // Green for +Y
    private readonly Color _positiveZColor = new Color(0.0f, 0.0f, 1.0f, 1.0f); // Blue for +Z
    private readonly Color _negativeColor = new Color(0.8f, 0.8f, 0.0f, 1.0f);  // Yellow for negative directions
    private readonly Color _defaultColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);   // White for mixed/unknown

    // --------------------------------------------------------------------------------------------
    // MARK: Properties
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Length of the normal lines
    /// </summary>
    public float NormalLength
    {
        get => _normalLength;
        set => _normalLength = Mathf.Max(0.01f, value);
    }

    /// <summary>
    /// Whether to show vertex normals
    /// </summary>
    public bool ShowVertexNormals
    {
        get => _showVertexNormals;
        set => _showVertexNormals = value;
    }

    /// <summary>
    /// Whether to show triangle normals (at triangle centers)
    /// </summary>
    public bool ShowTriangleNormals
    {
        get => _showTriangleNormals;
        set => _showTriangleNormals = value;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: MeshInstance3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Disable shadow casting for normal visualization
        CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh Update
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Updates the normal visualization mesh from KoreMeshData
    /// </summary>
    /// <param name="meshData">The mesh data to visualize normals for</param>
    public void UpdateMesh(KoreMeshData meshData)
    {
        _surfaceTool = new SurfaceTool();
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Lines);

        // Draw vertex normals
        if (_showVertexNormals)
        {
            DrawVertexNormals(meshData);
        }

        // Draw triangle normals
        if (_showTriangleNormals)
        {
            DrawTriangleNormals(meshData);
        }

        // Commit the mesh and assign it to this MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        Mesh = mesh;

        // Apply unlit material to make normals always visible
        MaterialOverride = GetUnlitNormalMaterial();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normal Drawing
    // --------------------------------------------------------------------------------------------

    private void DrawVertexNormals(KoreMeshData meshData)
    {
        foreach (var vertexKvp in meshData.Vertices)
        {
            int vertexId = vertexKvp.Key;
            KoreXYZVector vertex = vertexKvp.Value;

            Vector3 godotVertex = KoreMeshGodotConv.PositionKoreToGodot(vertex);

            // Check if this vertex has a normal
            if (!meshData.Normals.ContainsKey(vertexId))
                continue;

            KoreXYZVector normal = meshData.Normals[vertexId];
            Vector3 godotNormal = KoreMeshGodotConv.NormalKoreToGodot(normal);
            godotNormal = godotNormal.Normalized();
            godotNormal *= _normalLength;

            // Convert to Godot coordinates
            Vector3 godotStart = KoreMeshGodotConv.PositionKoreToGodot(vertex);
            Vector3 godotEnd = godotStart + godotNormal;

            // Choose color based on normal direction
            Color normalColor = GetNormalColor(normal);

            // Add the normal line
            _surfaceTool.SetColor(normalColor);
            _surfaceTool.AddVertex(godotStart);
            _surfaceTool.SetColor(normalColor);
            _surfaceTool.AddVertex(godotEnd);
        }
    }

    private void DrawTriangleNormals(KoreMeshData meshData)
    {
        foreach (var triangleKvp in meshData.Triangles)
        {
            int triangleId = triangleKvp.Key;
            KoreMeshTriangle triangle = triangleKvp.Value;

            // Get triangle vertices
            if (!meshData.Vertices.ContainsKey(triangle.A) ||
                !meshData.Vertices.ContainsKey(triangle.B) ||
                !meshData.Vertices.ContainsKey(triangle.C))
                continue;

            KoreXYZVector vertexA = meshData.Vertices[triangle.A];
            KoreXYZVector vertexB = meshData.Vertices[triangle.B];
            KoreXYZVector vertexC = meshData.Vertices[triangle.C];

            // Calculate triangle center
            KoreXYZVector triangleCenter = new KoreXYZVector(
                (vertexA.X + vertexB.X + vertexC.X) / 3.0,
                (vertexA.Y + vertexB.Y + vertexC.Y) / 3.0,
                (vertexA.Z + vertexB.Z + vertexC.Z) / 3.0
            );

            // Calculate triangle normal
            KoreXYZVector ab = vertexB - vertexA;
            KoreXYZVector ac = vertexC - vertexA;
            KoreXYZVector triangleNormal = KoreXYZVector.CrossProduct(ab, ac).Normalize();

            // Calculate end point of normal line
            KoreXYZVector normalEnd = triangleCenter + (triangleNormal * _normalLength);

            // Convert to Godot coordinates
            Vector3 godotStart = KoreMeshGodotConv.PositionKoreToGodot(triangleCenter);
            Vector3 godotEnd = KoreMeshGodotConv.PositionKoreToGodot(normalEnd);

            // Use a different color for triangle normals (magenta)
            Color triangleNormalColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

            // Add the triangle normal line
            _surfaceTool.SetColor(triangleNormalColor);
            _surfaceTool.AddVertex(godotStart);
            _surfaceTool.SetColor(triangleNormalColor);
            _surfaceTool.AddVertex(godotEnd);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Color Mapping
    // --------------------------------------------------------------------------------------------

    private Color GetNormalColor(KoreXYZVector normal)
    {
        // Determine primary direction and return appropriate color
        double absX = Math.Abs(normal.X);
        double absY = Math.Abs(normal.Y);
        double absZ = Math.Abs(normal.Z);

        // Find the dominant axis
        if (absX >= absY && absX >= absZ)
        {
            // X is dominant
            return normal.X > 0 ? _positiveXColor : _negativeColor;
        }
        else if (absY >= absX && absY >= absZ)
        {
            // Y is dominant
            return normal.Y > 0 ? _positiveYColor : _negativeColor;
        }
        else if (absZ >= absX && absZ >= absY)
        {
            // Z is dominant
            return normal.Z > 0 ? _positiveZColor : _negativeColor;
        }

        return _defaultColor;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Materials
    // --------------------------------------------------------------------------------------------

    private StandardMaterial3D GetUnlitNormalMaterial()
    {
        var material = new StandardMaterial3D();

        // Make it unlit so normals are always bright
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;

        // Enable vertex colors so normal colors work
        material.VertexColorUseAsAlbedo = true;

        // Make normals slightly brighter
        material.AlbedoColor = new Color(1.3f, 1.3f, 1.3f, 1.0f);

        // Optional: Make normals always show on top
        // material.NoDepthTest = true;

        return material;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Convenience method to update mesh with specified normal length
    /// </summary>
    public void UpdateMesh(KoreMeshData meshData, float normalLength)
    {
        NormalLength = normalLength;
        UpdateMesh(meshData);
    }

    /// <summary>
    /// Convenience method to update mesh with specific visualization options
    /// </summary>
    public void UpdateMesh(KoreMeshData meshData, bool showVertexNormals, bool showTriangleNormals, float normalLength = 0.2f)
    {
        ShowVertexNormals = showVertexNormals;
        ShowTriangleNormals = showTriangleNormals;
        NormalLength = normalLength;
        UpdateMesh(meshData);
    }
}
