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
        // GD.Print("Updating KoreColorMeshGodot");
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

        // Clear the SurfaceTool to free memory - no longer needed
        _surfaceTool.Clear();

        // Apply material from mesh
        MaterialOverride = KoreColorMeshGodotMaterialFactory.FromVertexColors(transparent: false);

        // Enable shadow casting for surface meshes
        CastShadow = GeometryInstance3D.ShadowCastingSetting.On;

        _meshNeedsUpdate = false;
    }

    // --------------------------------------------------------------------------------------------

    // Turn the ColorMesh into a Godot mesh with distinct triangles.

    public void UpdateMeshBackground(KoreColorMesh newMesh)
    {
        // GD.Print("Updating KoreColorMeshGodot");
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
    }

    // --------------------------------------------------------------------------------------------

    // Turn the ColorMesh into a Godot mesh with colors per vertex, not per triangle.
    // This is more memory efficient for large meshes with many triangles, but means that colors
    // are averaged across triangles sharing a vertex.

    public void UpdateMeshBackground2(KoreColorMesh newMesh)
    {
        // GD.Print("Updating KoreColorMeshGodot");
        Name = $"ColorMeshSurface";

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // Dictionary to map mesh vertex ID to SurfaceTool vertex index
        Dictionary<int, int> meshToSurfaceVertexMap = new Dictionary<int, int>();
        int surfaceVertexIndex = 0;

        // Loop through each vertex in turn, adding it and looking up its color.
        foreach (var kvp in newMesh.Vertices)
        {
            int vId = kvp.Key;
            KoreXYZVector currV = kvp.Value;

            // Get the color for this vertex
            KoreColorRGB color = KoreColorMeshOps.FirstColorForVertex(newMesh, vId);
            Color godotCol = KoreConvColor.ToGodotColor(color);

            // get and convert the point
            Godot.Vector3 pV = XYZtoV3(currV);

            // Add the vertex
            _surfaceTool.SetColor(godotCol);
            _surfaceTool.AddVertex(pV);

            // Map the mesh vertex ID to our manually tracked surface index
            meshToSurfaceVertexMap[vId] = surfaceVertexIndex;
            surfaceVertexIndex++;
        }

        // Second pass: Add triangles using AddIndex
        foreach (var kvp in newMesh.Triangles)
        {
            KoreColorMeshTri currTri = kvp.Value;

            // Look up the SurfaceTool indices for this triangle's vertices
            int indexA = meshToSurfaceVertexMap[currTri.A];
            int indexB = meshToSurfaceVertexMap[currTri.B];
            int indexC = meshToSurfaceVertexMap[currTri.C];

            // Add triangle by referencing the vertex indices
            _surfaceTool.AddIndex(indexA);
            _surfaceTool.AddIndex(indexB);
            _surfaceTool.AddIndex(indexC);
        }

        _surfaceTool.GenerateNormals();
    }


    // --------------------------------------------------------------------------------------------

    // Perform the creation activities that must be done on the main thread.

    public void UpdateMeshMainThread()
    {
        // Generate normals if they weren't provided (Needs to be on main thread)
        Mesh = _surfaceTool.Commit();

        // Apply material from mesh
        MaterialOverride = KoreColorMeshGodotMaterialFactory.FromVertexColors(transparent: false);

        // Enable shadow casting for surface meshes
        CastShadow = GeometryInstance3D.ShadowCastingSetting.On;

        _meshNeedsUpdate = false;
    }

    // --------------------------------------------------------------------------------------------

    // After the mesh has been extracted, reset the SurfaceTool to free memory.

    public void PostCreateTidyUp()
    {
        // Clear the SurfaceTool to free memory - no longer needed
        _surfaceTool.Clear();
    }
    
}


