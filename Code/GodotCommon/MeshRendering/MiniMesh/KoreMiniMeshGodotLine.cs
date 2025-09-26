// <fileheader>

using KoreCommon;
using System.Collections.Generic;

using Godot;

#nullable enable

public partial class KoreMiniMeshGodotLine : MeshInstance3D
{
    private SurfaceTool _surfaceTool = new SurfaceTool();
    private bool _meshNeedsUpdate = false;

    // --------------------------------------------------------------------------------------------
    // MARK: MeshInstance3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh
    // --------------------------------------------------------------------------------------------

    public static Godot.Vector3 XYZtoV3(KoreXYZVector v) => new Godot.Vector3((float)v.X, (float)v.Y, (float)v.Z);
    public static KoreXYZVector V3ToXYZ(Godot.Vector3 v) => new KoreXYZVector(v.X, v.Y, v.Z);

    public void UpdateMesh(KoreMiniMesh newMesh, string groupName)
    {
        //GD.Print("Updating KoreGodotSurfaceMesh with groupName:", groupName);

        // Basic validation
        if (string.IsNullOrEmpty(groupName)) return;
        if (!newMesh.HasGroup(groupName)) return;

        KoreMiniMeshGroup currGrp = newMesh.GetGroup(groupName);

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Lines);

        // Loop through each of the triangles, adding each vertex and normal in turn
        foreach (var kvp in newMesh.Lines)
        {
            int currLineId = kvp.Key;
            KoreMiniMeshLine currLine = kvp.Value;

            // Get and convert each point
            Godot.Vector3 pA = XYZtoV3(newMesh.GetVertex(currLine.A));
            Godot.Vector3 pB = XYZtoV3(newMesh.GetVertex(currLine.B));

            // Get the color
            Godot.Color lineColor = KoreMeshGodotConv.ColorKoreToGodot(newMesh.GetColor(currLine.ColorId));

            // Add the line vertices
            _surfaceTool.SetColor(lineColor);
            _surfaceTool.AddVertex(pA);
            _surfaceTool.SetColor(lineColor);
            _surfaceTool.AddVertex(pB);
        }

        // Generate normals if they weren't provided (Needs to be on main thread)
        Mesh = _surfaceTool.Commit();

        // Apply unlit material to make lines always bright and visible
        MaterialOverride = KoreMiniMeshGodotMaterialFactory.GetUnlitLineMaterial();

        // Disable shadow casting for line meshes (lines typically shouldn't cast shadows)
        CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        _meshNeedsUpdate = false;
    }
}