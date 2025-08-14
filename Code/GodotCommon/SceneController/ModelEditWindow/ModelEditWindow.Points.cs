using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

public partial class ModelEditWindow
{

    // Create a basic model and output the JSON into the code edit control
    private void OutputInitialJSON()
    {
        string jsonStr = KoreMeshDataIO.ToJson(WindowMeshData, dense: false);

        // Output the JSON string to the code edit control
        MeshJsonEdit!.SetText(jsonStr);

        // Also visualize the points and lines in the 3D scene
        DeleteAllDebug();
        DrawPoints(WindowMeshData);
        DrawLines(WindowMeshData);
    }

    // Text the text and update the model
    private void UpdateModelFromText()
    {
        if (MeshJsonEdit == null) return;

        string jsonStr = MeshJsonEdit.GetText();
        WindowMeshData = KoreMeshDataIO.FromJson(jsonStr);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Point
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Add a single debug sphere at the specified position in the 3D scene
    /// </summary>
    /// <param name="position">World position to place the debug sphere</param>
    /// <param name="radius">Radius of the debug sphere (default: 0.05)</param>
    /// <param name="color">Color of the debug sphere (default: yellow)</param>
    public void MarkPoint(Vector3 position, float radius = 0.05f, KoreColorRGB? color = null)
    {
        if (MountRoot == null)
        {
            GD.PrintErr("ModelEditWindow.MarkPoint: MountRoot is null");
            return;
        }

        // Use yellow as default color
        var sphereColor = color ?? KoreColorPalette.Colors["Yellow"];

        // Create a container node for this point
        Node3D pointNode = new Node3D() { Name = $"DebugPoint_{position.X:F2}_{position.Y:F2}_{position.Z:F2}" };
        pointNode.Position = position;

        // Add the debug sphere to the point node
        GodotMeshPrimitives.AddChildDebugSphere(pointNode, radius, sphereColor);

        // Add the point node to the mount root
        MountRoot.AddChild(pointNode);
    }

    /// <summary>
    /// Clear all existing debug points and add a sphere for each vertex in the mesh
    /// </summary>
    /// <param name="meshData">The mesh data to visualize points for</param>
    /// <param name="radius">Radius of each debug sphere (default: 0.05)</param>
    /// <param name="color">Color of the debug spheres (default: cyan)</param>
    public void DrawPoints(KoreMeshData meshData, float radius = 0.05f, KoreColorRGB? color = null)
    {
        if (MountRoot == null)
        {
            GD.PrintErr("ModelEditWindow.DrawPoints: MountRoot is null");
            return;
        }

        // Clear existing debug points
        ClearDebugPoints();

        // Use cyan as default color for point clouds
        var sphereColor = color ?? KoreColorPalette.Colors["Cyan"];

        // Add a sphere for each vertex in the mesh
        foreach (var kvp in meshData.Vertices)
        {
            int vertexId = kvp.Key;
            KoreXYZVector vertexPos = kvp.Value;
            Vector3 godotPos = new Vector3((float)vertexPos.X, (float)vertexPos.Y, (float)vertexPos.Z);

            MarkPoint(godotPos, radius, sphereColor);
        }

        GD.Print($"ModelEditWindow.DrawPoints: Added {meshData.Vertices.Count} debug spheres");
    }

    /// <summary>
    /// Clear all existing debug point spheres from the scene
    /// </summary>
    public void ClearDebugPoints()
    {
        if (MountRoot == null) return;

        // Remove all children whose names start with "DebugPoint_"
        var children = MountRoot.GetChildren();
        foreach (Node child in children)
        {
            if (child.Name.ToString().StartsWith("DebugPoint_"))
            {
                child.QueueFree();
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Line
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Add a single debug cylinder representing a line between two points in the 3D scene
    /// </summary>
    /// <param name="start">Start position of the line</param>
    /// <param name="end">End position of the line</param>
    /// <param name="radius">Radius of the debug cylinder (default: 0.02)</param>
    /// <param name="color">Color of the debug cylinder (default: orange)</param>
    public void MarkLine(Vector3 start, Vector3 end, float radius = 0.02f, KoreColorRGB? color = null)
    {
        if (MountRoot == null)
        {
            GD.PrintErr("ModelEditWindow.MarkLine: MountRoot is null");
            return;
        }

        // Use orange as default color for lines
        var cylinderColor = color ?? KoreColorPalette.Colors["Orange"];

        // Calculate line properties
        Vector3 direction = end - start;
        float length = direction.Length();
        Vector3 center = (start + end) * 0.5f;

        // Create a container node for this line
        Node3D lineNode = new Node3D() { Name = $"DebugLine_{start.X:F2}_{start.Y:F2}_{start.Z:F2}_to_{end.X:F2}_{end.Y:F2}_{end.Z:F2}" };
        lineNode.Position = center;

        // Create the cylinder mesh data
        KoreXYZVector p1 = new KoreXYZVector(0, -length * 0.5, 0);
        KoreXYZVector p2 = new KoreXYZVector(0, length * 0.5, 0);
        KoreMeshData cylinderMesh = KoreMeshDataPrimitives.Cylinder(p1, p2, radius, radius, 8, false);

        // Set all vertices to the cylinder color
        cylinderMesh.SetAllVertexColors(cylinderColor);

        // Create and add the mesh nodes
        KoreGodotLineMesh lineMeshNode = new KoreGodotLineMesh() { Name = "CylinderLines" };
        lineMeshNode.UpdateMesh(cylinderMesh);
        lineNode.AddChild(lineMeshNode);

        KoreGodotSurfaceMesh surfaceMeshNode = new KoreGodotSurfaceMesh() { Name = "CylinderSurface" };
        surfaceMeshNode.UpdateMesh(cylinderMesh);
        lineNode.AddChild(surfaceMeshNode);

        // Rotate the cylinder to align with the line direction
        if (direction.Length() > 0.001f)
        {
            Vector3 up = Vector3.Up;
            Vector3 normalizedDirection = direction.Normalized();

            // Calculate rotation to align Y-axis (cylinder default) with line direction
            if (Mathf.Abs(normalizedDirection.Dot(up)) < 0.999f)
            {
                Vector3 rotationAxis = up.Cross(normalizedDirection).Normalized();
                float rotationAngle = Mathf.Acos(up.Dot(normalizedDirection));
                lineNode.RotateObjectLocal(rotationAxis, rotationAngle);
            }
            else if (normalizedDirection.Dot(up) < 0)
            {
                // Handle the case where direction is opposite to up
                lineNode.RotationDegrees = new Vector3(180, 0, 0);
            }
        }

        // Add the line node to the mount root
        MountRoot.AddChild(lineNode);
    }

    /// <summary>
    /// Clear all existing debug lines and add a cylinder for each line in the mesh
    /// </summary>
    /// <param name="meshData">The mesh data to visualize lines for</param>
    /// <param name="radius">Radius of each debug cylinder (default: 0.02)</param>
    /// <param name="color">Color of the debug cylinders (default: orange)</param>
    public void DrawLines(KoreMeshData meshData, float radius = 0.02f, KoreColorRGB? color = null)
    {
        if (MountRoot == null)
        {
            GD.PrintErr("ModelEditWindow.DrawLines: MountRoot is null");
            return;
        }

        // Clear existing debug lines
        ClearDebugLines();

        // Use orange as default color for line visualization
        var cylinderColor = color ?? KoreColorPalette.Colors["Orange"];

        // Add a cylinder for each line in the mesh
        foreach (var kvp in meshData.Lines)
        {
            int lineId = kvp.Key;
            KoreMeshLine line = kvp.Value;

            // Get vertex positions
            if (meshData.Vertices.ContainsKey(line.A) && meshData.Vertices.ContainsKey(line.B))
            {
                KoreXYZVector startPos = meshData.Vertices[line.A];
                KoreXYZVector endPos = meshData.Vertices[line.B];

                Vector3 godotStart = new Vector3((float)startPos.X, (float)startPos.Y, (float)startPos.Z);
                Vector3 godotEnd = new Vector3((float)endPos.X, (float)endPos.Y, (float)endPos.Z);

                MarkLine(godotStart, godotEnd, radius, cylinderColor);
            }
        }

        GD.Print($"ModelEditWindow.DrawLines: Added {meshData.Lines.Count} debug cylinders");
    }

    /// <summary>
    /// Clear all existing debug line cylinders from the scene
    /// </summary>
    public void ClearDebugLines()
    {
        if (MountRoot == null) return;

        // Remove all children whose names start with "DebugLine_"
        var children = MountRoot.GetChildren();
        foreach (Node child in children)
        {
            if (child.Name.ToString().StartsWith("DebugLine_"))
            {
                child.QueueFree();
            }
        }
    }

    /// <summary>
    // Iteratively get and delete any node parented from MountRoot
    /// </summary>
    public void DeleteAllDebug()
    {
        if (MountRoot == null) return;

        // Remove all children whose names start with "Debug_"
        var children = MountRoot.GetChildren();
        foreach (Node child in children)
        {
            child.QueueFree();
        }
    }

    /// <summary>
    /// Clear all debug visualization elements (both points and lines)
    /// </summary>
    public void ClearAllDebugGeometry()
    {
        ClearDebugPoints();
        ClearDebugLines();
    }

    // Function to get a named triangle grouping out of the mesh and extract the
    // triangles as a new meshinstance3d.

    public void DrawDebugTriangles(string triangleGroupName)
    {
        // Fail if no mount point control
        if (MountRoot == null) return;

        // Fail if no mesh data or named group
        if (!WindowMeshData?.HasNamedGroup(triangleGroupName) ?? true) return;

        // Get the triangle group from the mesh data
    }

}
