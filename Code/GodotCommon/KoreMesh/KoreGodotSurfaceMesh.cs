// KoreGodotSurfaceMesh : Class to take a KoreCommon/KoreMeshData and create a Godot SurfaceMesh from it.
// - Will use the vertices/triangles list and the Godot SurfaceTool.

using KoreCommon;
using System.Collections.Generic;

using Godot;

public partial class KoreGodotSurfaceMesh : MeshInstance3D
{
    private SurfaceTool _surfaceTool = new SurfaceTool();
    private bool _meshNeedsUpdate = false;

    // --------------------------------------------------------------------------------------------
    // MARK: MeshInstance3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialize the SurfaceTool
        //_surfaceTool = new SurfaceTool();

        // Apply the shared Vertex Color Material to this MeshInstance3D
        // MaterialOverride = GetSharedVertexColorMaterial();

        // Debug Call: Create a cube with colored edges
        //CreateCube();
    }

    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh
    // --------------------------------------------------------------------------------------------

    public void UpdateMesh(KoreMeshData newMeshData)
    {
        _surfaceTool = new SurfaceTool();

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // First, add all unique vertices to the SurfaceTool
        // Create a mapping from KoreMeshData vertex IDs to SurfaceTool indices
        var vertexIdToSurfaceIndex = new Dictionary<int, int>();
        int surfaceVertexIndex = 0;

        foreach (var kvp in newMeshData.Vertices)
        {
            int vertexId = kvp.Key;
            KoreXYZVector vertex = kvp.Value;

            // Convert to Godot format
            Vector3 godotPos = KoreConvPos.VecToV3(vertex);

            // Set vertex color if available
            if (newMeshData.VertexColors.ContainsKey(vertexId))
            {
                Color vertexColor = KoreConvColor.ToGodotColor(newMeshData.VertexColors[vertexId]);
                _surfaceTool.SetColor(vertexColor);
            }

            // Set normal if available
            if (newMeshData.Normals.ContainsKey(vertexId))
            {
                Vector3 normal = KoreConvPos.VecToV3(newMeshData.Normals[vertexId]);
                _surfaceTool.SetNormal(normal);
            }

            // Set UV if available
            if (newMeshData.UVs.ContainsKey(vertexId))
            {
                Vector2 uv = new Vector2((float)newMeshData.UVs[vertexId].X, (float)newMeshData.UVs[vertexId].Y);
                _surfaceTool.SetUV(uv);
            }

            _surfaceTool.AddVertex(godotPos);
            vertexIdToSurfaceIndex[vertexId] = surfaceVertexIndex++;
        }

        // Now add triangles using indices to reference the already-added vertices
        foreach (var kvp in newMeshData.Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;
            
            // Get the SurfaceTool indices for each vertex
            int indexA = vertexIdToSurfaceIndex[triangle.A];
            int indexB = vertexIdToSurfaceIndex[triangle.B];
            int indexC = vertexIdToSurfaceIndex[triangle.C];
            
            // Add the triangle indices
            _surfaceTool.AddIndex(indexA);
            _surfaceTool.AddIndex(indexB);
            _surfaceTool.AddIndex(indexC);
        }

        // Check if any vertex colors have transparency
        bool hasTransparency = false;
        bool hasVertexColors = newMeshData.VertexColors.Count > 0;
        
        foreach (var vertexColor in newMeshData.VertexColors.Values)
        {
            if (vertexColor.IsTransparent) // Check for transparency in byte values
            {
                hasTransparency = true;
                break;
            }
        }

        // Choose material based on vertex colors and transparency
        StandardMaterial3D material;
        if (hasVertexColors && hasTransparency)
        {
            // Use vertex color transparent material
            material = KoreGodotMaterialFactory.VertexColorTransparentStandardMaterial();
        }
        else if (hasVertexColors)
        {
            // Use vertex color opaque material
            material = KoreGodotMaterialFactory.VertexColorStandardMaterial();
        }
        else
        {
            // Use standard colored material with default color
            material = KoreGodotMaterialFactory.StandardColoredMaterial(new Color(0.8f, 0.2f, 0.2f));
        }

        // Commit the mesh and assign it to the MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        
        // Generate normals if they weren't provided
        //_surfaceTool.GenerateNormals();
        //mesh = _surfaceTool.Commit();
        
        Mesh = mesh;
        MaterialOverride = material;
        
        // Enable shadow casting for surface meshes
        CastShadow = GeometryInstance3D.ShadowCastingSetting.On;

        _meshNeedsUpdate = false;
    }

}
