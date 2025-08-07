using System;
using System.Collections.Generic;

using KoreCommon;
using KoreSim;

using Godot;

#nullable enable

public partial class KoreWorldPosNode : Node3D
{
    private KoreLLAPoint CurrLLA  = KoreLLAPoint.Zero;
    private bool         PosMoved = false;

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
        if (KoreZeroOffset.IsPosChangeCycle)
        {
            UpdateOffsetPosition();
        }
                    
        // if (Timer1Hz < GloCentralTime.RuntimeSecs)
        // {
        //     Timer1Hz = GloCentralTime.RuntimeSecs + 1.0f;

        //     UpdateRwPosition();
        //     ApplySmoothAttitude();

        // if (ChaseCam.IsCurrent())
        // {
        //     // Get the Camera Polar Offset - flip the azimuth so we create the LLA correctly.
        //     GloAzElRange camPO = ChaseCam.RwCamOffset;

        //     // Get the platform heading and add the camera offset to get the chase cam LLA
        //     //GloLLAPoint? pos    = GloAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
        //     GloCourse? course = GloAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

        //     if (course != null)
        //         camPO.AzDegs += course?.HeadingDegs ?? 0.0;

        //     GloLLAPoint chaseCamLLA  = CurrentPosition.PlusPolarOffset(camPO);

        //     KoreZeroNodeMapManager.SetLoadRefLLA(chaseCamLLA);

        //     string strCamLLA = chaseCamLLA.ToString();
        //     GD.Print($"Camera LLA: Lat:{chaseCamLLA.LatDegs:F6} Lon:{chaseCamLLA.LonDegs:F6} Alt:{chaseCamLLA.AltMslM:F2}");
        // }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Position
    // --------------------------------------------------------------------------------------------

    public void SetPos(KoreLLAPoint newLLA)
    {
        CurrLLA = newLLA;
    }

    public void UpdateOffsetPosition()
    {
        // Set the local position from the parent object
        Vector3 newPos = KoreGeoConvOps.RwToOffsetGe(CurrLLA);

        // Set the local position from the parent object
        var transform    = GlobalTransform;
        transform.Origin = newPos;
        GlobalTransform  = transform;

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------


}


