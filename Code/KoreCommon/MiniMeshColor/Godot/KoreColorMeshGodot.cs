// <fileheader>

using KoreCommon;
using System.Collections.Generic;

using Godot;

#nullable enable

public partial class KoreColorMeshGodot : MeshInstance3D
{
    private SurfaceTool _surfaceTool = new SurfaceTool();
    private bool _meshNeedsUpdate = false;

    // --------------------------------------------------------------------------------------------
    // MARK: MeshInstance3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh
    // --------------------------------------------------------------------------------------------

    public static Godot.Vector3 XYZtoV3(KoreXYZVector v) => new Godot.Vector3((float)v.X, (float)v.Y, (float)v.Z);
    public static KoreXYZVector V3ToXYZ(Godot.Vector3 v) => new KoreXYZVector(v.X, v.Y, v.Z);

    // --------------------------------------------------------------------------------------------

    public void UpdateMesh(KoreColorMesh newMesh)
    {
        GD.Print("Updating KoreColorMeshGodot");
        Name = $"ColorMeshSurface";

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // Loop through each of the triangles, adding each vertex and normal in turn
        foreach (var kvp in newMesh.Triangles)
        {
            int triId = kvp.Key;
            KoreColorMeshTri currTri = kvp.Value;

            // Get the color for this triangle
            Color godotCol = KoreConvColor.ToGodotColor(currTri.Color);

            // Get the normal for this triangle
            Vector3 triNormal = XYZtoV3(KoreColorMeshOps.CalculateFaceNormal(newMesh, currTri));
            
            // get and convert each point
            Godot.Vector3 pA = XYZtoV3(newMesh.GetVertex(currTri.A));
            Godot.Vector3 pB = XYZtoV3(newMesh.GetVertex(currTri.B));
            Godot.Vector3 pC = XYZtoV3(newMesh.GetVertex(currTri.C));

            // Add the triangle indices
            _surfaceTool.SetColor(godotCol);
            _surfaceTool.SetNormal(triNormal);
            _surfaceTool.AddVertex(pA);

            _surfaceTool.SetColor(godotCol);
            _surfaceTool.SetNormal(triNormal);
            _surfaceTool.AddVertex(pB);

            _surfaceTool.SetColor(godotCol);
            _surfaceTool.SetNormal(triNormal);
            _surfaceTool.AddVertex(pC);
        }
        
        // Generate normals if they weren't provided (Needs to be on main thread)
        Mesh = _surfaceTool.Commit();

        // Apply material from mesh
        MaterialOverride = KoreColorMeshGodotMaterialFactory.FromVertexColors(transparent: true);

        // Enable shadow casting for surface meshes
        CastShadow = GeometryInstance3D.ShadowCastingSetting.On;

        _meshNeedsUpdate = false;
    }

}



