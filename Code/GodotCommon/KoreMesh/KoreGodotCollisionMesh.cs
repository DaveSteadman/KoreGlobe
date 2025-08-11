using System.Collections.Generic;

using KoreCommon;
using Godot;

public partial class KoreGodotCollisionMesh : CollisionShape3D
{
    private bool _shapeNeedsUpdate = false;

    // --------------------------------------------------------------------------------------------
    // MARK: CollisionShape3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Set a default name
        Name = "CollisionMesh";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Collision Shape
    // --------------------------------------------------------------------------------------------

    public void UpdateCollisionMesh(KoreMeshData newMeshData, bool useConvex = false)
    {
        if (newMeshData.Triangles.Count == 0)
        {
            // No triangles, clear the shape
            Shape = null;
            return;
        }

        // Create a Godot mesh from the KoreMeshData
        var surfaceTool = new SurfaceTool();
        surfaceTool.Clear();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // Create vertex mapping
        var vertexIdToSurfaceIndex = new Dictionary<int, int>();
        int surfaceVertexIndex = 0;

        // Add vertices
        foreach (var kvp in newMeshData.Vertices)
        {
            Vector3 godotPos = KoreConvPos.VecToV3(kvp.Value);
            surfaceTool.AddVertex(godotPos);
            vertexIdToSurfaceIndex[kvp.Key] = surfaceVertexIndex++;
        }

        // Add triangles
        foreach (var kvp in newMeshData.Triangles)
        {
            var triangle = kvp.Value;
            int indexA = vertexIdToSurfaceIndex[triangle.A];
            int indexB = vertexIdToSurfaceIndex[triangle.B];
            int indexC = vertexIdToSurfaceIndex[triangle.C];

            surfaceTool.AddIndex(indexA);
            surfaceTool.AddIndex(indexB);
            surfaceTool.AddIndex(indexC);
        }

        // Create the mesh
        var mesh = surfaceTool.Commit();

        // Create collision shape from mesh
        if (useConvex)
        {
            // Convex shape - better performance, less accurate
            Shape = mesh.CreateConvexShape();
        }
        else
        {
            // Trimesh shape - exact collision, slower
            Shape = mesh.CreateTrimeshShape();
        }

        _shapeNeedsUpdate = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------

    public void ClearCollisionMesh()
    {
        Shape = null;
    }

    public bool HasCollisionMesh()
    {
        return Shape != null;
    }
}



