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
    private float UITimer = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms

    private float UISlowTimer = 0.0f;
    private float UISlowTimerInterval = 0.1f;

    // ---------------------------------------------------------------------------------------------
    // MARK: Node3D
    // ---------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        ConstructNodes();

        CreateDebugMarker();

        // This method is called when the node is added to the scene.
        GD.Print("MainScene is ready!");
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        // This method is called every frame.
        // You can use 'delta' to make frame-rate independent calculations.
        // GD.Print("Processing frame with delta: " + delta);

        if (KoreCentralTime.CheckTimer(ref UISlowTimer, UISlowTimerInterval))
        {
            // move the zero point a fraction of the way to the camera LLA
            // KoreLLAPoint cameraLLA = KoreGodotMainSceneFactory.WorldCameraMount?.CurrLLA ?? KoreLLAPoint.Zero;
            // KoreLLAPoint zeroPosLLA = KoreZeroOffset.AppliedZeroPosLLA;


            // Get the XYZ of the camera position
            Vector3 geXYZ = SceneObjects.WorldCameraMount!.Position;
            KoreXYZVector geXYZ2 = KoreConvPos.V3ToVec(geXYZ);

            // The camera is at an absolute position, so get the vector to the zero point
            KoreXYZVector vecZeroPToCam = KoreXYZVector.Zero.XYZTo(geXYZ2);

            // Turn that into a lat-long-alt
            KoreLLPoint camLLPos = KoreLLPoint.FromXYZ(vecZeroPToCam);
            KoreLLAPoint camLLA = new KoreLLAPoint() {
                LatDegs = camLLPos.LatDegs,
                LonDegs = camLLPos.LonDegs,
                RadiusM = KoreZeroOffset.GeEarthRadius };

            GD.Print($"XYZOffset:{vecZeroPToCam.X:F2},{vecZeroPToCam.Y:F2},{vecZeroPToCam.Z:F2} // CamLLA:{camLLA.LatDegs:F2},{camLLA.LonDegs:F2},{camLLA.RadiusM:F2}");

            // Lerp 10% of the way to the camera position, but fix the alt at 0
            // KoreLLAPoint newZeroPos = KoreLLAPointOps.GreatCircleInterpolation(
            //     KoreZeroOffset.AppliedZeroPosLLA, // current position
            //     camLLA, // our target position
            //     0.1);   // 10% of the way there
            // newZeroPos.RadiusM = 10;

            // Set the new position
            KoreXYZVector newZeroXYZ = camLLA.ToXYZ();

            //DemoNode?.SetPosition(KoreConvPos.VecToV3(newZeroXYZ));

            KoreRelocateOps.QueueNewOffset(newZeroXYZ);


            //KoreZeroOffset.SetLLA(newZeroPos);

            // Get the camera string and save it to the config
            // string cameraString = KoreGodotMainSceneFactory.WorldCameraMount?.GetMoverString() ?? string.Empty;
            // KoreSimFactory.Instance.KoreConfig.Set("CameraPosition", cameraString);
            // KoreSimFactory.Instance.SaveConfig();

            //AggressiveMemoryCleanup();
        }
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

    // --------------------------------------------------------------------------------------------
    // MARK: Resize
    // --------------------------------------------------------------------------------------------

    // Read basic information about the viewport

    // public void UpdateCurrentViewportSize()
    // {
    //     Godot.Vector2 vSize = GetViewport().GetVisibleRect().Size;
    //     KoreGodotMainSceneFactory.ViewSize.LatestValue = new KoreXYRect(0, 0, vSize.X, vSize.Y);

    //     Camera3D? camera = GetViewport().GetCamera3D();
    //     if (camera != null)
    //         KoreGodotMainSceneFactory.CameraFOV.LatestValue = camera.Fov;

    //     //GD.Print($"MainScene: Viewport size updated to: {KoreGodotMainSceneFactory.ViewSize.LatestValue} // Camera FOV: {KoreGodotMainSceneFactory.CameraFOV.LatestValue:F2}");
    // }


    // --------------------------------------------------------------------------------------------
    // MARK: Debug Marker
    // --------------------------------------------------------------------------------------------

    // Create a debug Magenta sphere at the zero point

    private void CreateDebugMarker()
    {
        float debugRadius = 0.25f;

        DebugMarkerNode = new Node3D() { Name = "ZeroNodeDebugMarker - Magenta" };
        AddChild(DebugMarkerNode);

        // Core Sphere
        {
            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattMagenta");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(KoreXYZVector.Zero, debugRadius, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = "ZoneNodeMarker" };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            DebugMarkerNode.AddChild(coloredMeshNode);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine() { Name = "ZoneNodeMarker - Wire" };
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            DebugMarkerNode.AddChild(lineMeshNode1);
        }
    }

}
