using Godot;
using System;
using System.Linq;

using KoreCommon;
using KoreCommon.Mesh;
using System.Collections.Generic;
using System.IO;

#nullable enable

public partial class ModelEditWindow
{

    // Create a basic model and output the JSON into the code edit control
    private void MeshToJSON()
    {
        string jsonStr = KoreMeshDataIO.ToJson(WindowMeshData, dense: false);

        // Output the JSON string to the code edit control
        MeshJsonEdit!.SetText(jsonStr);

        // Also visualize the points and lines in the 3D scene
        DrawPoints(WindowMeshData);
        DrawLines(WindowMeshData);
    }

    private void JSONToMesh()
    {
        // Read the JSON from the code edit control
        string jsonStr = MeshJsonEdit!.GetText();

        // Parse the JSON into a KoreMeshData object
        WindowMeshData = KoreMeshDataIO.FromJson(jsonStr);


        DeleteAllDebug();

        // Update the 3D mesh visualization using material groups

        if (ViewSelection.ShowFaces) DrawMeshWithGroups(WindowMeshData);
        if (ViewSelection.ShowPoints) DrawPoints(WindowMeshData);
        if (ViewSelection.ShowLines) DrawLines(WindowMeshData);
        if (ViewSelection.ShowNormals) DrawNormals(WindowMeshData);
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
    public void MarkPoint(Vector3 position, float radius = 0.03f, KoreColorRGB? color = null)
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
    public void DrawPoints(KoreMeshData meshData, float radius = 0.03f, KoreColorRGB? color = null)
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

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    public void DrawNormals(KoreMeshData meshData)
    {
        if (NormalMeshData == null)
        {
            NormalMeshData = new KoreGodotNormalMesh();
            MountRoot!.AddChild(NormalMeshData);
        }
        NormalMeshData.UpdateMesh(meshData, 0.15f); // Small normals for cube
    }

    public void ClearNormals()
    {
        if (NormalMeshData != null)
        {
            NormalMeshData.ClearMesh();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Debug
    // --------------------------------------------------------------------------------------------


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
        // ClearDebugPoints();
        // ClearDebugLines();
        // ClearNormals();

        ClearMeshInstances();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh Surface Rendering
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Draw the mesh using separate MeshInstance3D nodes for each material group
    /// </summary>
    /// <param name="meshData">The mesh data to render</param>
    public void DrawMeshWithGroups(KoreMeshData? meshData)
    {
        if (MountRoot == null || meshData == null) return;

        // Clear any existing mesh instances (but keep debug objects)
        ClearMeshInstances();

        // Get a mesh for each material
        Dictionary<string, KoreMeshData> groupMeshDict = KoreMeshDataEditOps.MeshForEachGroup(meshData);

        foreach (var kvp in groupMeshDict)
        {
            string groupName = kvp.Key;
            KoreMeshData groupMesh = kvp.Value;

            // Create a MeshInstance3D for this group
            var meshInstance = new KoreGodotSurfaceMesh();
            meshInstance.Name = $"MeshGroup_{groupName}";

            // Update the mesh with the group data, passing source path for texture resolution
            meshInstance.UpdateMesh(groupMesh, groupName, SourceFilePath);

            // Add to the scene
            MountRoot!.AddChild(meshInstance);

            GD.Print($"Drew mesh group '{groupName}' with {groupMesh.Triangles.Count} triangles, {groupMesh.Materials.Count} materials");


            // Write each mesh to a debug json file
            string debugJson = KoreMeshDataIO.ToJson(groupMesh, dense: false);
            File.WriteAllText($"Debug_{groupName}.json", debugJson);
        }

    }



    /// <summary>
    /// Clear all mesh instances (but preserve debug objects like spheres and cylinders)
    /// </summary>
    private void ClearMeshInstances()
    {
        if (MountRoot == null) return;

        // Remove only KoreGodotSurfaceMesh nodes, keep debug objects
        var childrenToRemove = new List<Node>();
        foreach (Node child in MountRoot.GetChildren())
        {
           child.QueueFree();
        }

        // foreach (Node child in childrenToRemove)
        // {
        //     MountRoot.RemoveChild(child);
        //     child.QueueFree();
        // }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Export Testing
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Test OBJ export functionality - prints OBJ and MTL content to console
    /// </summary>
    public void TestObjExport()
    {
        // Create test mesh with multiple materials
        KoreMeshData meshData = KoreMeshDataPrimitives.BasicCube(0.5f, KoreMeshMaterialPalette.DefaultMaterial);

        // Add a second material
        var redMaterial = new KoreMeshMaterial("RedMaterial", new KoreColorRGB(255, 0, 0), 0.1f, 0.3f);
        meshData.AddMaterial(redMaterial);

        // Add a third material
        var blueMaterial = new KoreMeshMaterial("BlueMaterial", new KoreColorRGB(0, 0, 255), 0.8f, 0.1f);
        meshData.AddMaterial(blueMaterial);

        // Create triangle groups with different materials
        var triangleIds = new List<int>(meshData.Triangles.Keys);
        if (triangleIds.Count >= 6) // Cube should have 12 triangles (6 faces * 2 triangles each)
        {
            // Assign first 4 triangles to red material
            var redTriangles = triangleIds.Take(4).ToList();
            meshData.NamedTriangleGroups["RedGroup"] = new KoreMeshTriangleGroup("RedMaterial", redTriangles);

            // Assign next 4 triangles to blue material
            var blueTriangles = triangleIds.Skip(4).Take(4).ToList();
            meshData.NamedTriangleGroups["BlueGroup"] = new KoreMeshTriangleGroup("BlueMaterial", blueTriangles);

            // Leave remaining triangles with default material
        }

        GD.Print($"=== Export Test - Materials: {meshData.Materials.Count}, Groups: {meshData.NamedTriangleGroups.Count} ===");

        // Export to OBJ/MTL
        var (objContent, mtlContent) = KoreMeshDataIO.ToObjMtl(meshData, "TestCube", "cube_materials");

        GD.Print("=== OBJ Content ===");
        GD.Print(objContent);

        GD.Print("=== MTL Content ===");
        GD.Print(mtlContent);

        File.WriteAllLines("TestCube.obj", new[] { objContent });
        File.WriteAllLines("cube_materials.mtl", new[] { mtlContent });

        // Also update the JSON display
        string jsonStr = KoreMeshDataIO.ToJson(meshData, dense: false);
        GD.Print($"OBJ Export Test Complete!\n\n=== JSON ===\n{jsonStr}");
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


    // --------------------------------------------------------------------------------------------
    // MARK: Import Testing
    // --------------------------------------------------------------------------------------------

    // Usage: ModelEditWindow.TestObjImport()
    public void TestObjImport()
    {
        GD.Print("=== OBJ Import Test ===");

        // Load the OBJ file
        var objContent = File.ReadAllText("TestCube.obj");
        var mtlContent = File.ReadAllText("cube_materials.mtl");

        GD.Print("=== MTL Content ===");
        GD.Print(mtlContent);

        // Import the OBJ/MTL
        var meshData = KoreMeshDataIO.FromObjMtl(objContent, mtlContent);
        KoreMeshDataEditOps.FlipAllTriangleWindings(meshData);

        // Update the window mesh data
        // WindowMeshData = meshData;

        // Debug: Print material count and details
        GD.Print($"=== Import Results ===");
        GD.Print($"Materials count: {meshData.Materials.Count}");
        for (int i = 0; i < meshData.Materials.Count; i++)
        {
            var material = meshData.Materials[i];
            GD.Print($"Material {i}: {material.Name} - Color: {material.BaseColor} - Metallic: {material.Metallic} - Roughness: {material.Roughness}");
        }

        string jsonStr = KoreMeshDataIO.ToJson(meshData, dense: false);
        GD.Print($"Imported Mesh Data JSON:\n{jsonStr}");

        // Put the new text in the edit window
        MeshJsonEdit!.SetText(jsonStr);

        GD.Print("=== OBJ Import Test Complete ===");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: glTF Import Testing
    // --------------------------------------------------------------------------------------------

    // Usage: ModelEditWindow.TestGltfImport()
    public void TestGltfImport()
    {
        GD.Print("=== glTF Import Test ===");

        try
        {
            // Load the glTF file from UnitTestArtefacts
            string gltfPath = "UnitTestArtefacts/TestOilBarrel.gltf";

            if (!File.Exists(gltfPath))
            {
                GD.PrintErr($"glTF file not found: {gltfPath}");
                GD.PrintErr("Make sure to run the unit tests first to generate the oil barrel glTF file");
                return;
            }

            // Import the glTF using our import functionality
            var meshData = KoreMeshDataGltfIO.LoadFromGltf(gltfPath);

            //meshData.SetNormalsFromTriangles();

            // Update the window mesh data and source path
            WindowMeshData = meshData;
            SourceFilePath = gltfPath;

            // Debug: Print material count and details
            GD.Print($"=== glTF Import Results ===");
            GD.Print($"Vertices count: {meshData.Vertices.Count}");
            GD.Print($"Triangles count: {meshData.Triangles.Count}");
            GD.Print($"Materials count: {meshData.Materials.Count}");

            for (int i = 0; i < meshData.Materials.Count; i++)
            {
                var material = meshData.Materials[i];
                GD.Print($"Material {i}: {material.Name} - Color: {material.BaseColor} - Filename: {material.Filename}");
            }

            // Convert to JSON and display in editor
            string jsonStr = KoreMeshDataIO.ToJson(meshData, dense: false);
            GD.Print($"Imported glTF Mesh Data JSON:\n{jsonStr}");

            // Put the new text in the edit window
            MeshJsonEdit!.SetText(jsonStr);

            // Clear existing debug visuals
            DeleteAllDebug();

            // Update the 3D mesh visualization using material groups
            DrawMeshWithGroups(meshData);

            // Also visualize the points and lines in the 3D scene
            DrawPoints(meshData);
            DrawLines(meshData);

            GD.Print("=== glTF Import Test Complete ===");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"glTF Import failed: {ex.Message}");
            GD.PrintErr($"Stack trace: {ex.StackTrace}");
        }
    }

}
