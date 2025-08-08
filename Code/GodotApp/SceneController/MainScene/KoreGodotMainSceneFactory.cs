
// KoreGodotMainSceneFactory is a central global class managing access to internal modelling element.
// KoreGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;
using System.IO;

using Godot;

using KoreCommon;
using KoreSim;

#nullable enable

public static class KoreGodotMainSceneFactory
{
    public static Node3D?                 MainSceneRootNode { get; private set; } = null;
    public static KoreZeroNodeMapManager? MapManagerNode    { get; private set; } = null;
    public static KoreZeroNode?           ZeroNode          { get; private set; } = null; // KoreGodotMainSceneFactory.ZeroNode
    public static KoreWorldMoverNode?     WorldCameraMount  { get; private set; } = null;

    // KoreGodotMainSceneFactory.ViewSize.LatestValue = new KoreXYRect(0, 0, 800, 600);
    // KoreXYRect currViewSize = KoreGodotMainSceneFactory.ViewSize.LatestValue();
    public static KoreLatestHolderStruct<KoreXYRect> ViewSize { get; private set; } = new KoreLatestHolderStruct<KoreXYRect>(KoreXYRect.Zero);
    public static KoreLatestHolderStruct<float>      CameraFOV { get; private set; } = new KoreLatestHolderStruct<float>(60.0f); // Default FOV in degrees

    // -----------------------------------------------------------------------------------------------
    // MARK: Setup Nodes
    // -----------------------------------------------------------------------------------------------

    public static void SetupNodes(Node3D rootNode)
    {
        // Ensure the root node is not null
        if (rootNode == null)
        {
            throw new ArgumentNullException(nameof(rootNode), "Root node cannot be null.");
        }

        MainSceneRootNode = rootNode;

        ZeroNode = new KoreZeroNode();
        rootNode.AddChild(ZeroNode);

        // Map manager hangs off of the ZeroNode, with its own position or 0,0,0.
        // Child tile nodes hang off of the manager and control their own zeronode offset.
        MapManagerNode = new KoreZeroNodeMapManager(ZeroNode);
        KoreGodotMainSceneFactory.ZeroNode?.AddChild(MapManagerNode);

        KoreSimFactory.TriggerInstance();
        KoreAppCommands.RegisterCommands(KoreSimFactory.Instance.ConsoleInterface);

    }

    public static void AddWorldCamera()
    {
        // Create a new camera node
        Camera3D camera = new Camera3D();
        camera.Name = "WorldCamera";
        camera.Fov = 35f;

        // Create the camera mount
        WorldCameraMount = new KoreWorldMoverNode();
        WorldCameraMount.Name = "CameraMount";
        WorldCameraMount.AddChild(camera);
        ZeroNode?.AddChild(WorldCameraMount);

        // Set the camera's position and rotation
        WorldCameraMount.CurrLLA = new KoreLLAPoint(50, 0, 5000);
        
    }

    public static void AddDebugNodes()
    {
        // GodotMeshPrimitives.AddChildDebugSphere(MapManagerNode, 0.1f, KoreColorPalette.Colors["DarkBlue"]);
        //GodotMeshPrimitives.AddChildDebugSphere(ZeroNode, 0.1f, KoreColorPalette.Colors["Yellow"]);

    }
}