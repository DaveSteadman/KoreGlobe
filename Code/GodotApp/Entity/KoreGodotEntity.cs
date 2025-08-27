using System;
using System.Collections.Generic;

using Godot;

using KoreCommon;
using KoreSim;
using SharpGLTF.Geometry.VertexTypes;

#nullable enable

public partial class KoreGodotEntity : Node3D
{
    public string EntityName { get; set; }

    //     // Setup default model info, so we always have something to work with.
    //     public Kore3DModelInfo ModelInfo { get; set; } = Kore3DModelInfo.Default();

    public Node3D AttitudeNode = new Node3D() { Name = "Attitude" };

    //     private KoreElementContrail ElementContrail;

    private KoreLLAPoint CurrentPosition = new KoreLLAPoint();
    private KoreAttitude CurrentModelAttitude = new KoreAttitude();
    private KoreCourse CurrentCourse = new KoreCourse();
    //     private KoreCameraPolarOffset ChaseCam = new KoreCameraPolarOffset();

    private KoreAttitude CurrentSmoothedAttitude = new KoreAttitude();


    private float TimerPollModel = 0.0f;
    private float TimerPollModelInterval = 0.05f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddChild(AttitudeNode);

        AddDebugSphere();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // The model can be updated asynchronously, so check before updating.
        // Other nodes can QueueFree the node, but will do so once the model is deleted and on the same main-thread, so conflict is avoided there.
        if (!KoreEventDriver.HasEntity(EntityName))
            return;
            
        if (KoreCentralTime.CheckTimer(ref TimerPollModel, TimerPollModelInterval) || KoreZeroOffset.IsPosChangeCycle)
        {

            UpdateModelData();
            UpdateEntityPosition();

            //             UpdateZeroNode();

            //             if (ChaseCam.IsCurrent())
            //             {
            //                 // Get the Camera Polar Offset - flip the azimuth so we create the LLA correctly.
            //                 KorePolarOffset camPO = ChaseCam.RwCamOffset;

            //                 // Get the platform heading and add the camera offset to get the chase cam LLA
            //                 //KoreLLAPoint? pos    = KoreAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
            //                 KoreCourse? course = KoreAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

            //                 if (course != null)
            //                     camPO.AzDegs += course?.HeadingDegs ?? 0.0;

            //                 KoreLLAPoint chaseCamLLA = CurrentPosition.PlusPolarOffset(camPO);

            //                 KoreZeroNodeMapManager.SetLoadRefLLA(chaseCamLLA);

            //                 string strCamLLA = chaseCamLLA.ToString();
            //                 GD.Print($"Camera LLA: Lat:{chaseCamLLA.LatDegs:F6} Lon:{chaseCamLLA.LonDegs:F6} Alt:{chaseCamLLA.AltMslM:F2}");
            //             }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    public void CreateEntity()
    {
        //         // Set this node name. - This gets the position and earth orinetation set on it.
        //         Name = EntityName;

        //         // Create a marker for the entity
        //         Node3D marker = new Node3D() { Name = "AxisMarker" };
        //         AddChild(marker);
        //         //KorePrimitiveFactory.AddAxisMarkers(marker, 0.003f, 0.001f);

        //         // Setup the chase camera and default position
        //         ChaseCam.Name = "ChaseCam";
        //         AddChild(ChaseCam);
        //         ChaseCam.SetCameraPosition(300, 20, 20); // 300m, 20 degs up, 20 degs right
    }

    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Chase Cam
    //     // --------------------------------------------------------------------------------------------

    //     public void EnableChaseCam()
    //     {
    //         ChaseCam.CamNode.Current = true;
    //     }

    //     // Report the current state of the camer

    //     public void UpdateZeroNode()
    //     {
    //         // GD.Print("EntityName:{EntityName}");

    //         // Only drive the zero node to match the entity position if the chase cam is the current camera
    //         if (ChaseCam.IsCurrent())
    //         {
    //             KoreZeroNode.SetZeroNodePosition(CurrentPosition);
    //             // GD.Print($"ZERO NODE UPDATE: EntityName:{EntityName} CurrentPosition:{CurrentPosition}");
    //         }
    //     }

    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Position and Attitude
    //     // --------------------------------------------------------------------------------------------

    //     // Note that position here is a polar offset for the rotating chase cam

    public void UpdateModelData()
    {
        // Get the entity positional info, and update if valid
        KoreLLAPoint? pos = KoreEventDriver.GetEntityPosition(EntityName);
        KoreCourse? course = KoreEventDriver.GetEntityCourse(EntityName);
        KoreAttitude? att = KoreEventDriver.GetEntityAttitude(EntityName);
        if (pos != null) CurrentPosition = pos.Value;
        if (course != null) CurrentCourse = course.Value;
        if (att != null) CurrentModelAttitude = att.Value;

        if (pos == null || course == null || att == null)
        {
            KoreCentralLog.AddEntry($"EC0-0025: Entity data {EntityName} not found.");
            return;
        }
    }

    public void UpdateEntityPosition()
    {
        // Convert the position
        KoreEntityV3 entityVecs = KoreGeoConvOps.RwToGeStruct(CurrentPosition, CurrentCourse);

        // Position
        Position = entityVecs.Pos;
        LookAt(entityVecs.PosAhead, entityVecs.VecUp);

        // Attitude smoothing - 1 degree per frame
        CurrentSmoothedAttitude.PitchUpDegs =
            KoreValueUtils.AdjustWithinBounds(CurrentSmoothedAttitude.PitchUpDegs, CurrentModelAttitude.PitchUpDegs, 1);
        CurrentSmoothedAttitude.RollClockwiseDegs =
            KoreValueUtils.AdjustWithinBounds(CurrentSmoothedAttitude.RollClockwiseDegs, CurrentModelAttitude.RollClockwiseDegs, 1);
        CurrentSmoothedAttitude.YawClockwiseDegs =
            KoreValueUtils.AdjustWithinBounds(CurrentSmoothedAttitude.YawClockwiseDegs, CurrentModelAttitude.YawClockwiseDegs, 1);

        double pitchUpRads = CurrentSmoothedAttitude.PitchUpRads;
        double rollClockwiseRads = CurrentSmoothedAttitude.RollClockwiseRads;
        double yawClockwiseRads = CurrentSmoothedAttitude.YawClockwiseRads;

        float gePitchRads = (float)pitchUpRads;
        float geRollRads = (float)rollClockwiseRads;
        float geYawRads = (float)yawClockwiseRads;

        AttitudeNode.Rotation = new Vector3(gePitchRads, geYawRads, geRollRads);
    }


    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Update Elements: Route
    //     // --------------------------------------------------------------------------------------------

    //     // Function to check if the entity has a route, and update the game-engine route to match.

    //     public void UpdateRoute()
    //     {
    //         // Get the route
    //         List<KoreLLAPoint> routePoints = KoreAppFactory.Instance.EventDriver.PlatformGetRoutePoints(EntityName);

    //         if (routePoints.Count > 0)
    //         {
    //             return;
    //         }

    //         // Update the route
    //     }

    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Update Elements: Contrail
    //     // --------------------------------------------------------------------------------------------

    //     public void UpdateContrail()
    //     {
    //         //ElementContrail.UpdateElement();
    //     }


    private void AddDebugSphere()
    {
        // Create a simple sphere mesh for debugging
        SphereMesh sphereMesh = new SphereMesh
        {
            Radius = 0.3f,
            Height = 0.6f,
            RadialSegments = 16,
            Rings = 8
        };

        MeshInstance3D sphereInstance = new MeshInstance3D
        {
            Mesh = sphereMesh,
            Name = "DebugSphere"
        };

        AttitudeNode.AddChild(sphereInstance);
    }



}


