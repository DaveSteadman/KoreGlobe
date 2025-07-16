// KoreGodotLineMesh : Class to take a KoreCommon/KoreMeshData and create a Godot LineMesh from it.
// - Will use the lines list and the Godot SurfaceTool.

using KoreCommon;

using Godot;

public partial class KoreGodotLineMesh : Node3D
{
    private MeshInstance3D _meshInstance;
    private SurfaceTool _surfaceTool;
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
        _surfaceTool  = new SurfaceTool();
        _meshInstance = new MeshInstance3D();
        
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Lines);

        // Traverse the list using Id, so we have the index to look stuff up.
        int numLines = newMeshData.Lines.Count;
        for (int i = 0; i < numLines; i++)
        {
            // Get the line details
            var line = newMeshData.Lines[i];
            int pointAId = line.A;
            int pointBId = line.B;
            KoreXYZVector vecA = newMeshData.Vertices[pointAId];
            KoreXYZVector vecB = newMeshData.Vertices[pointBId];
            KoreMeshLineColour lineColour = newMeshData.LineColors[i];

            // Convert the line details to Godot terms
            Vector3 godotPosA = KoreConvPos.VecToV3(vecA);
            Vector3 godotPosB = KoreConvPos.VecToV3(vecB);
            Color colStart = KoreConvColor.ToGodotColor(lineColour.StartColor);
            Color colEnd = KoreConvColor.ToGodotColor(lineColour.EndColor);

            // Add the vertices to the SurfaceTool
            _surfaceTool.SetColor(colStart);
            _surfaceTool.AddVertex(godotPosA);
            _surfaceTool.SetColor(colEnd);
            _surfaceTool.AddVertex(godotPosB);
        }

        // Commit the mesh and assign it to the MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        _meshInstance.Mesh = mesh;

        _meshNeedsUpdate = false;
    }

}
