// using System;
// using System.Collections.Generic;

// using Godot;

// using KoreCommon;
// using KoreSim;

// #nullable enable

// public partial class KoreGodotEntity : Node3D
// {
//     public string EntityName { get; set; }

//     // Setup default model info, so we always have something to work with.
//     public Kore3DModelInfo ModelInfo { get; set; } = Kore3DModelInfo.Default();

//     public Node3D AttitudeNode = new Node3D() { Name = "Attitude" };

//     private KoreElementContrail ElementContrail;

//     // private KoreAttitude CurrentAttitude = new KoreAttitude();
//     private KoreLLAPoint CurrentPosition = new KoreLLAPoint();
//     private KoreCameraPolarOffset ChaseCam = new KoreCameraPolarOffset();

//     private float Timer1Hz = 0.0f;

//     // --------------------------------------------------------------------------------------------
//     // MARK: Node Functions
//     // --------------------------------------------------------------------------------------------

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         CreateEntity();

//         AddChild(AttitudeNode);

//         // ElementContrail = new KoreElementContrail();
//         // ElementContrail.InitElement(EntityName);
//         // ElementContrail.SetModel(EntityName);
//         // KoreGodotFactory.Instance.GodotEntityManager.ElementRootNode.AddChild(ElementContrail);
//     }

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//         UpdateEntityPosition();

//         if (KoreCentralTime.CheckTimer(ref Timer1Hz, 1.0f))
//         {
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
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Create
//     // --------------------------------------------------------------------------------------------

//     public void CreateEntity()
//     {
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
//     }

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

//     public void UpdateEntityPosition()
//     {
//         // Update the position and orientation of the entity.
//         // This is done by the parent node.

//         KoreLLAPoint? pos = KoreAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
//         KoreCourse? course = KoreAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

//         if (pos != null)
//             CurrentPosition = (KoreLLAPoint)pos;

//         if (pos == null || course == null)
//         {
//             GD.Print($"EC0-0025: Platform {EntityName} not found.");
//             return;
//         }

//         KoreEntityV3 entityVecs = KoreGeoConvOperations.RwToGeStruct((KoreLLAPoint)pos, (KoreCourse)course);

//         //GD.Print($"Name: {EntityName} PosLLA:{pos} Ahead:{entityVecs.PosAhead} up:{entityVecs.VecUp}");

//         Position = entityVecs.Pos;
//         //LookAtFromPosition(entityVecs.Pos, entityVecs.PosAhead, entityVecs.VecUp, true);

//         LookAt(entityVecs.PosAhead, entityVecs.VecUp);

//         KoreAttitude? att = KoreAppFactory.Instance.EventDriver.GetPlatformAttitude(EntityName);
//         if (att != null)
//             UpdateAttitude((KoreAttitude)att);
//     }

//     public void UpdateAttitude(KoreAttitude attitude)
//     {
//         CurrentAttitude.PitchUpDegs = KoreValueUtils.AdjustWithinBounds(CurrentAttitude.PitchUpDegs, attitude.PitchUpDegs, 1);
//         CurrentAttitude.RollClockwiseDegs = KoreValueUtils.AdjustWithinBounds(CurrentAttitude.RollClockwiseDegs, attitude.RollClockwiseDegs, 1);
//         CurrentAttitude.YawClockwiseDegs = KoreValueUtils.AdjustWithinBounds(CurrentAttitude.YawClockwiseDegs, attitude.YawClockwiseDegs, 1);

//         double pitchUpRads = CurrentAttitude.PitchUpRads;
//         double rollClockwiseRads = CurrentAttitude.RollClockwiseRads;
//         double yawClockwiseRads = CurrentAttitude.YawClockwiseRads;

//         float gePitchRads = (float)pitchUpRads;
//         float geRollRads = (float)rollClockwiseRads;
//         float geYawRads = (float)yawClockwiseRads;

//         AttitudeNode.Rotation = new Vector3(gePitchRads, geYawRads, geRollRads);
//     }

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



// }


