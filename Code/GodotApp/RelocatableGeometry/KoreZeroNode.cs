using System;
using KoreCommon;
using Godot;

#nullable enable

// ------------------------------------------------------------------------------------------------
// KoreZeroNode:
// - Point off of which all the relocatable geometry is ANCHORED.
// - Holds MINIMAL FUNCTIONALITY/TIMING to drive the move from "set" to "applied" position in KoreRelocateOps.
// - Class maintains the concept of the frame in which a new offset is applied.
// ------------------------------------------------------------------------------------------------

public partial class KoreZeroNode : Node3D
{
    // Internal timer to check for a position change - avoid running check every frame
    private float CheckTimer = 0.0f;
    private float CheckTimerIntervalsSecs = 0.1f;

    // An extra trigger to update expedite the application of a new zero position.
    // Usage: KoreZeroNode.UpdateTrigger
    static public bool UpdateTrigger = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "ZeroNode";

        CreateDebugMarker();
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        // On a timer, or explicit trigger, assess the ZeroNode position.
        if (KoreCentralTime.CheckTimer(ref CheckTimer, CheckTimerIntervalsSecs) || (UpdateTrigger))
        {
            // Clear any manual trigger (even if there is no position change).
            // Likely a camera of scene change requires a quicker update, this is the purpose of the trigger.
            UpdateTrigger = false;

            // If the ZeroPosChangePending is set, apply the new position, with a deferred call to the end-of-frame.
            if (KoreRelocateOps.IsNewOffsetPending())
            {
                CallDeferred(nameof(ApplyOffsetDeferred));
            }
        }

        // Clear down a change cycle after one frame - not its set in a deferred call, and cleared in a deferred call.
        if (KoreZeroOffset.IsPosChangeCycle)
        {
            CallDeferred(nameof(ClearUpdateDeferred));
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ApplyOffsetDeferred() => KoreRelocateOps.ApplyOffset();
    private void ClearUpdateDeferred() => KoreRelocateOps.ClearChangePeriod();

    // --------------------------------------------------------------------------------------------
    // MARK: Internals
    // --------------------------------------------------------------------------------------------

    private void CreateDebugMarker()
    {
        float debugRadius = 0.25f;
        // Core Sphere
        {
            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattYellow");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(KoreXYZVector.Zero, debugRadius, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = "ZoneNodeMarker - Yellow" };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            AddChild(coloredMeshNode);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine() { Name = "ZoneNodeMarker - Wire" };
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }
    }
}
