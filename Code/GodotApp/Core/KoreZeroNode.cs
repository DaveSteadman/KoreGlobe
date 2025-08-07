using System;
using KoreCommon;
using Godot;

#nullable enable

// ------------------------------------------------------------------------------------------------
// KoreZeroNode:
// - Point off of which all the relocatable geometry is ANCHORED.
// - Holds NO DATA, just FUNCTIONALITY/TIMING to drive the move from "set" to "applied" position.
// ------------------------------------------------------------------------------------------------

public partial class KoreZeroNode : Node3D
{
    // Internal timer to check for a position change - avoid running check every frame
    private float CheckTimer              = 0.0f;
    private float CheckTimerIntervalsSecs = 1.0f;

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
        if ((CheckTimer < KoreCentralTime.RuntimeSecs) || (UpdateTrigger))
        {
            // Clear any manual trigger (even if there is no position change).
            // Likely a camera of scene change requires a quicker update, this is the purpose of the trigger.
            UpdateTrigger = false;

            // Update the timer.
            CheckTimer = KoreCentralTime.RuntimeSecs + CheckTimerIntervalsSecs;

            // If the ZeroPosChangePending is set, apply the new position, with a deferred call to the end-of-frame.
            if (KoreZeroOffset.ZeroPosChangePending)
            {
                CallDeferred(nameof(ApplyLLADeferred));
            }
        }

        // Clear down a change cycle after one frame - not its set in a deferred call, and cleared in a deferred call.
        if (KoreZeroOffset.IsPosChangeCycle)
        {
            CallDeferred(nameof(ClearUpdateDeferred));
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ApplyLLADeferred()
    {
        KoreZeroOffset.ApplyLLA();
    }

    private void ClearUpdateDeferred()
    {
        KoreZeroOffset.ClearUpdate();
    }
    
    // --------------------------------------------------------------------------------------------
    // MARK: Internals
    // --------------------------------------------------------------------------------------------

    private void CreateDebugMarker()
    {
        // Add a debug marker to the ZeroNode.
        GodotMeshPrimitives.AddChildDebugSphere(this, 0.1f, KoreColorPalette.Colors["Yellow"]);
    }
}
