using Godot;
using System;
using System.IO;

#nullable enable

/*
Example usage:

    // Basic loading
    var model = Kore3DLoader.LoadGltfFromFile(@"C:\Models\spaceship.gltf");

    // Load and add to scene
    Kore3DLoader.LoadGltfAndAddToParent(@"C:\Models\building.glb", this, "MyBuilding");

    // Load with transform
    var car = Kore3DLoader.LoadGltfWithTransform(
        @"C:\Models\car.gltf", 
        new Vector3(5, 0, 0),      // position
        new Vector3(0, 1.57f, 0),  // rotation (90 degrees Y)
        new Vector3(2, 2, 2)       // scale (2x bigger)
    );
*/

public static class Kore3DLoader
{
    /// <summary>
    /// Load a GLTF model from a file path on disk
    /// </summary>
    /// <param name="filePath">Full path to the .gltf or .glb file (e.g., "C:\\Models\\mymodel.gltf")</param>
    /// <returns>The loaded Node3D scene, or null if loading failed</returns>
    public static Node3D? LoadGltfFromFile(string filePath)
    {
        try
        {
            // Validate file exists
            if (!File.Exists(filePath))
            {
                GD.PrintErr($"GLTF file not found: {filePath}");
                return null;
            }

            // Validate file extension
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".gltf" && extension != ".glb")
            {
                GD.PrintErr($"Invalid file type. Expected .gltf or .glb, got: {extension}");
                return null;
            }

            // Create GLTF document loader
            var gltfDocument = new GltfDocument();
            var gltfState = new GltfState();

            // Load the GLTF file
            Error result = gltfDocument.AppendFromFile(filePath, gltfState);

            if (result != Error.Ok)
            {
                GD.PrintErr($"Failed to load GLTF file: {filePath}, Error: {result}");
                return null;
            }

            // Generate the scene from the GLTF data
            Node? scene = gltfDocument.GenerateScene(gltfState);

            if (scene == null)
            {
                GD.PrintErr($"Failed to generate scene from GLTF file: {filePath}");
                return null;
            }

            // Cast to Node3D (GLTF scenes are typically Node3D)
            if (scene is Node3D node3D)
            {
                GD.Print($"Successfully loaded GLTF model: {Path.GetFileName(filePath)}");
                return node3D;
            }
            else
            {
                GD.PrintErr($"GLTF scene is not a Node3D: {filePath}");
                scene.QueueFree();
                return null;
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Exception loading GLTF file {filePath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Load a GLTF model from a file path and add it as a child to the specified parent node
    /// </summary>
    /// <param name="filePath">Full path to the .gltf or .glb file</param>
    /// <param name="parent">Parent node to add the loaded model to</param>
    /// <param name="modelName">Optional name for the loaded model node</param>
    /// <returns>The loaded Node3D scene, or null if loading failed</returns>
    public static Node3D? LoadGltfAndAddToParent(string filePath, Node parent, string? modelName = null)
    {
        var loadedScene = LoadGltfFromFile(filePath);

        if (loadedScene != null)
        {
            // Set the name if provided
            if (!string.IsNullOrEmpty(modelName))
            {
                loadedScene.Name = modelName;
            }
            else
            {
                loadedScene.Name = Path.GetFileNameWithoutExtension(filePath);
            }

            parent.AddChild(loadedScene);
            GD.Print($"Added GLTF model '{loadedScene.Name}' to parent node");
        }

        return loadedScene;
    }

    /// <summary>
    /// Load a GLTF model with specific transform settings
    /// </summary>
    /// <param name="filePath">Full path to the .gltf or .glb file</param>
    /// <param name="position">Position to place the model</param>
    /// <param name="rotation">Rotation to apply to the model (in radians)</param>
    /// <param name="scale">Scale to apply to the model</param>
    /// <returns>The loaded and transformed Node3D scene, or null if loading failed</returns>
    public static Node3D? LoadGltfWithTransform(string filePath, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
    {
        var loadedScene = LoadGltfFromFile(filePath);

        if (loadedScene != null)
        {
            if (position.HasValue)
                loadedScene.Position = position.Value;

            if (rotation.HasValue)
                loadedScene.Rotation = rotation.Value;

            if (scale.HasValue)
                loadedScene.Scale = scale.Value;
        }

        return loadedScene;
    }
}