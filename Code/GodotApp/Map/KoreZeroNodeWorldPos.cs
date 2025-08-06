using System;
using System.Collections.Generic;

using Godot;

using KoreCommon;

#nullable enable

public partial class KoreZeroNodeWorldPos : Node3D
{

    private double HeadingDegs = 0.0;
    private KoreLLAPoint CurrPos = new();

    private float Timer1Hz = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // UpdateEntityPosition();

        if (Timer1Hz <  KoreCentralTime.RuntimeSecs)
        {
            Timer1Hz =  KoreCentralTime.RuntimeSecs + 1.0f;
            UpdateZeroNode();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------



    // --------------------------------------------------------------------------------------------
    // MARK: Chase Cam
    // --------------------------------------------------------------------------------------------



    // Report the current state of the camer

    public void UpdateZeroNode()
    {
        // GD.Print("EntityName:{EntityName}");

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position and Attitude
    // --------------------------------------------------------------------------------------------

    // Note that position here is a polar offset for the rotating chase cam

    // public void UpdateEntityPosition()
    // {
    //     // Update the position and orientation of the entity.
    //     // This is done by the parent node.

    //      KoreLLAPoint? pos  = KoreAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
    //      KoreCourse? course = KoreAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

    //     if (pos != null)
    //         CurrentPosition = ( KoreLLAPoint)pos;

    //     if (pos == null || course == null)
    //     {
    //         GD.Print($"EC0-0025: Platform {EntityName} not found.");
    //         return;
    //     }

    //      KoreEntityV3 entityVecs =  KoreGeoConvOperations.RwToGeStruct(( KoreLLAPoint)pos, ( KoreCourse)course);

    //     //GD.Print($"Name: {EntityName} PosLLA:{pos} Ahead:{entityVecs.PosAhead} up:{entityVecs.VecUp}");

    //     Position = entityVecs.Pos;
    //     //LookAtFromPosition(entityVecs.Pos, entityVecs.PosAhead, entityVecs.VecUp, true);

    //     LookAt(entityVecs.PosAhead, entityVecs.VecUp);

    //      KoreAttitude? att =  KoreAppFactory.Instance.EventDriver.GetPlatformAttitude(EntityName);
    //     if (att != null)
    //         UpdateAttitude(( KoreAttitude)att);
    // }

    // public void UpdateAttitude( KoreAttitude attitude)
    // {
    //     CurrentAttitude.PitchUpDegs =  KoreValueUtils.AdjustWithinBounds(CurrentAttitude.PitchUpDegs, attitude.PitchUpDegs, 1);
    //     CurrentAttitude.RollClockwiseDegs =  KoreValueUtils.AdjustWithinBounds(CurrentAttitude.RollClockwiseDegs, attitude.RollClockwiseDegs, 1);
    //     CurrentAttitude.YawClockwiseDegs =  KoreValueUtils.AdjustWithinBounds(CurrentAttitude.YawClockwiseDegs, attitude.YawClockwiseDegs, 1);

    //     double pitchUpRads = CurrentAttitude.PitchUpRads;
    //     double rollClockwiseRads = CurrentAttitude.RollClockwiseRads;
    //     double yawClockwiseRads = CurrentAttitude.YawClockwiseRads;

    //     float gePitchRads = (float)pitchUpRads;
    //     float geRollRads = (float)rollClockwiseRads;
    //     float geYawRads = (float)yawClockwiseRads;

    //     AttitudeNode.Rotation = new Vector3(gePitchRads, geYawRads, geRollRads);
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Elements: Route
    // --------------------------------------------------------------------------------------------

    // Function to check if the entity has a route, and update the game-engine route to match.

    // public void UpdateRoute()
    // {
    //     // Get the route
    //     List< KoreLLAPoint> routePoints =  KoreAppFactory.Instance.EventDriver.PlatformGetRoutePoints(EntityName);

    //     if (routePoints.Count > 0)
    //     {
    //         return;
    //     }

    //     // Update the route
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Elements: Contrail
    // // --------------------------------------------------------------------------------------------

    // public void UpdateContrail()
    // {
    //     //ElementContrail.UpdateElement();
    // }



}


