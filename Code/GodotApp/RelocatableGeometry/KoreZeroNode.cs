using System;

using Godot;

using KoreCommon;

#nullable enable

// ------------------------------------------------------------------------------------------------
// KoreZeroNode:
// - Point off of which all the relocatable geometry is ANCHORED in the game engine.
// - Holds MINIMAL FUNCTIONALITY/TIMING to drive the move from "pending" to "applied" position in KoreRelocateOps.
// - Class maintains the concept of the frame in which a new offset is applied.
// ------------------------------------------------------------------------------------------------

public partial class KoreZeroNode : Node3D
{
    // Internal timer to check for a position change - avoid running check every frame
    private float CheckTimer              = 0.0f;
    private float CheckTimerIntervalsSecs = 0.5f;

    // An extra trigger to update expedite the application of a new zero position.
    // Usage: KoreZeroNode.UpdateTrigger
    static public bool UpdateTrigger = false;

    //public KoreMovingOrigin MovingOrigin = new KoreMovingOrigin();

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "ZeroNode";
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        // On a timer, or explicit trigger, assess the ZeroNode position.
        if (KoreCentralTime.CheckTimer(ref CheckTimer, CheckTimerIntervalsSecs) || (UpdateTrigger))
        {
            // Clear any manual trigger (even if there is no position change).
            // Likely a camera or scene change requires a quicker update, this is the purpose of the trigger.
            if (UpdateTrigger)
            {
                UpdateTrigger = false;
                KoreCentralTime.ResetTimer(ref CheckTimer, CheckTimerIntervalsSecs);
            }

            // If the ZeroPosChangePending is set, apply the new position, with a deferred call to the end-of-frame.
            if (KoreMovingOrigin.IsNewOffsetPending())
            {
                GD.Print($"KoreZeroNode: Applying new offset");
                CallDeferred(nameof(ApplyOffsetDeferred));
            }
        }

        // Clear down a change cycle after one frame - not its set in a deferred call, and cleared in a deferred call.
        if (KoreMovingOrigin.IsChangePeriod())
        {
            GD.Print($"KoreZeroNode: Clearing ChangePeriod");
            CallDeferred(nameof(ClearUpdateDeferred));
        }
    }

    // --------------------------------------------------------------------------------------------

    // Package the calls as Deferred (ie call at end of frame) to help with frame sync issues.

    private void ApplyOffsetDeferred() => KoreMovingOrigin.ApplyOffset();
    private void ClearUpdateDeferred() => KoreMovingOrigin.ClearChangePeriod();

    // --------------------------------------------------------------------------------------------

}
