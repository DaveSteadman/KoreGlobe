using System;
using System.Runtime;
using System.Collections.Generic;

using Godot;

using KoreCommon;
using KoreGIS;
using KoreSim;


#nullable enable

// Kore3DRelocatableSceneNode: Top level node for a scene with the relocatable zero point.

public partial class Kore3DRelocatableSceneNode : Node3D
{
    // MainScene.UIMount
    public Kore3DRelocatableSceneObjects SceneObjects { get; private set; } = new Kore3DRelocatableSceneObjects();

    Node3D? DebugMarkerNode = null;

    // UI Timers
    private float UITimer         = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms

    private float UISlowTimer         = 0.0f;
    private float UISlowTimerInterval = 1f;

    // ---------------------------------------------------------------------------------------------
    // MARK: Node3D
    // ---------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        ConstructNodes();

        SceneObjects.FrameworkManager = new Kore3DFrameworkObjectManager();
        SceneObjects.FrameworkManager.CreateNodes(SceneObjects.ZeroNode!);

        // This method is called when the node is added to the scene.
        GD.Print("MainScene is ready!");
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        if (KoreCentralTime.CheckTimer(ref UISlowTimer, UISlowTimerInterval))
        {
            // Debug placing of the zero offset - just for testing
            // - Get the camera position in GE XYZ
            // - If the magnitude is > 1 GE unit, move the zero offset to zero
            // - else, move the zero offset to a magnitude of 1, in the direction of the camera position
            // -    Note that the RW scale is 1:1 with GE units for now.

            Vector3 geXYZ = SceneObjects.WorldCameraMount!.Position;

            KoreXYZVector newZeroOffsetVec;

            if (geXYZ.Length() > 1.0f)
            {
                newZeroOffsetVec = KoreConvPos.V3ToVec(geXYZ);
                newZeroOffsetVec.Magnitude = 1;

                GD.Print($"posVec: {newZeroOffsetVec}");
            }
            else
            {
                newZeroOffsetVec = KoreXYZVector.Zero;

                GD.Print($"Zeroing origin");
            }
            KoreMovingOrigin.QueueNewOffset(newZeroOffsetVec);

        }
        SceneObjects.FrameworkManager!.UpdateNodes();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Support
    // ---------------------------------------------------------------------------------------------

    public void ConstructNodes()
    {
        // Construct the zero node
        SceneObjects.ZeroNode = new KoreZeroNode();
        AddChild(SceneObjects.ZeroNode);

        // Construct the quad ZN map manager (requires zero node as parameter)
        // SceneObjects.QuadZNMapManager = new KoreQuadZNMapManager(SceneObjects.ZeroNode);
        // SceneObjects.ZeroNode.AddChild(SceneObjects.QuadZNMapManager);

        // Construct the zero node map manager (requires zero node as parameter)
        // SceneObjects.ZeroNodeMapManager = new KoreZeroNodeMapManager(SceneObjects.ZeroNode);
        // SceneObjects.ZeroNode.AddChild(SceneObjects.ZeroNodeMapManager);

        // Construct the world camera mount
        {
            // Create a new camera node
            Camera3D camera = new Camera3D();
            camera.Name = "WorldCamera";
            camera.Fov  = 35f;
            camera.Near = 0.1f;
            camera.Far  = 100_000f;

            // Create the camera mount
            SceneObjects.WorldCameraMount = new KoreRelocatableXYZMoverNode();
            SceneObjects.WorldCameraMount.Name = "CameraMount";
            //SceneObjects.WorldCameraMount.CurrLLA = new KoreLLAPoint(50, 0, 5000);
            SceneObjects.WorldCameraMount.AddChild(camera);
            SceneObjects.ZeroNode.AddChild(SceneObjects.WorldCameraMount);
        }

        KoreAppCommands.RegisterCommands(KoreSimFactory.Instance.ConsoleInterface);
    }

    // --------------------------------------------------------------------------------------------

    private void AggressiveMemoryCleanup()
    {
        // 1. Clean up Godot resources first
        GetTree().CallGroup("nodes", "queue_free"); // Better approach for Godot 4

        // 2. Force cleanup of any queued-for-deletion nodes
        //SceneTree.CurrentScene?.PropagateCall("queue_free");

        // 3. Multiple C# GC passes
        for (int i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        // 4. Compact the heap
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();

        // 5. Log memory usage
        long memoryAfter = GC.GetTotalMemory(false);
        //GD.Print($"Memory after aggressive cleanup: {memoryAfter / 1024 / 1024}MB");
    }

}
