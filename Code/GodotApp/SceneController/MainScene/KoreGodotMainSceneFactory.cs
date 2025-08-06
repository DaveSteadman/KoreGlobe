
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
    public static KoreZeroNode?           ZeroNode          { get; private set; } = null;

    // KoreGodotMainSceneFactory.ViewSize.LatestValue = new KoreXYRect(0, 0, 800, 600);
    // KoreXYRect currViewSize = KoreGodotMainSceneFactory.ViewSize.LatestValue();
    public static KoreLatestHolderStruct<KoreXYRect> ViewSize  { get; private set; } = new KoreLatestHolderStruct<KoreXYRect>(KoreXYRect.Zero);
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

        MapManagerNode = new KoreZeroNodeMapManager(ZeroNode);
        rootNode.AddChild(MapManagerNode);
    }
}