// KoreGodotSurfaceMesh : Class to take a KoreCommon/KoreMeshData and create a Godot SurfaceMesh from it.
// - Will use the vertices/triangles list and the Godot SurfaceTool.

using KoreCommon;
using System.Collections.Generic;

using Godot;

public partial class KoreGodotSurfaceMesh : Node3D
{
    private MeshInstance3D _meshInstance;
    private SurfaceTool _surfaceTool = new SurfaceTool();
    private bool _meshNeedsUpdate = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Create a MeshInstance3D to hold the generated line mesh
        //_meshInstance = new MeshInstance3D();
        AddChild(_meshInstance);

        // Initialize the SurfaceTool
        //_surfaceTool = new SurfaceTool();

        // Apply the shared Vertex Color Material to the MeshInstance3D
        // _meshInstance.MaterialOverride = GetSharedVertexColorMaterial();

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
        _meshInstance = new MeshInstance3D();
        
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

        // // Check if any vertex colors have transparency
        // bool hasTransparency = false;
        // foreach (var vertexColor in newMeshData.VertexColors.Values)
        // {
        //     if (vertexColor.A < 1.0f)
        //     {
        //         hasTransparency = true;
        //         break;
        //     }
        // }

        // Choose material based on whether transparency is needed
        // StandardMaterial3D material;
        // if (hasTransparency)
        // {
            // Use the new vertex color transparent material
        StandardMaterial3D material = KoreGodotMaterialFactory.TransparentColoredMaterial(new Color(1, 0, 0, 0.5f));
        ShaderMaterial material2 = KoreGodotMaterialFactory.VertexColorMaterial();
        StandardMaterial3D material3 = KoreGodotMaterialFactory.VertexColorTransparentMaterial();
        // }
        // else
        // {
        //     // Use vertex color shader for opaque colors
        //     material = KoreGodotMaterialFactory.VertexColorMaterial();
        // }

        // Commit the mesh and assign it to the MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        _meshInstance.Mesh = mesh;
        _meshInstance.MaterialOverride = material2;

        _meshNeedsUpdate = false;
    }

}
