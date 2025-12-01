using System;
using System.Collections.Generic;

using KoreCommon;
using KoreGIS;
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
        if (KoreMovingOrigin.IsChangePeriod() || PosMoved)
        {
            UpdateOffsetPosition();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Position
    // --------------------------------------------------------------------------------------------

    public void SetPos(KoreLLAPoint newLLA)
    {
        CurrLLA = newLLA;
        PosMoved = true;
    }

    public void UpdateOffsetPosition()
    {
        // // Set the local position from the parent object
        // Vector3 newPos = KoreGeoConvOps.RwToOffsetGe(CurrLLA);

        // // Set the local position from the parent object
        // var transform    = GlobalTransform;
        // transform.Origin = newPos;
        // GlobalTransform  = transform;

        // // Clear the moved flag
        // PosMoved = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    public void CreateDebugMarker(float radius = 0.1f)
    {
        // Add a debug marker to the ZeroNode.
        GodotMeshPrimitives.AddChildDebugSphere(this, radius, KoreColorPalette.Colors["LightCyan"]);
    }
}


