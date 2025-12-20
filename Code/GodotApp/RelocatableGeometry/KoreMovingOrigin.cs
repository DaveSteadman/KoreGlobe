using System;
using System.Numerics;

using KoreCommon;
using Godot;

// KoreMovingOrigin: Relocatable geometry
// - We have "RW" real world co-ordinates, and "GE" game engine co-ordinates.
// - We have an RwOrigin point, which is the real-world position that maps to the GE origin (0,0,0).
// - A scale factor exists between RW and GE units.

// ------------------------------------------------------------------------------------------------

// Conversions:

// Processing Steps:
// 1 - RW to RWOffset: Add the RwOrigin offset to get RWOffset position, from any RW XYZ position.
// 2 - RWOffset to GE: Scale (multiply) the RWOffset position by the scale factor to get GE position.
// 3 - GE to GE Types: Convert GE position to Godot Vector3 or other types as needed.

// Reverse processing Steps:
// 1 - GE Types to GE: Convert Godot Vector3 or other types to GE position.
// 2 - GE to RWOffset: Reverse scale (multiply by the inverse) the GE position by the scale factor to get an RW unit
// 3 - RWOffset to RW: Reverse (subtract) the RwOrigin offset to get RW position from RWOffset position.

// ------------------------------------------------------------------------------------------------

// Origin Update:

// 1 - A caller requests a new RwOrigin position by calling QueueNewOffset().
// 2 - The new RwOrigin is stored as PendingRwOrigin, and a ChangePending flag is set.
// 3 - At the start of the next frame/update cycle, ApplyOffset() is called.
// 4 - The RwOrigin is updated to the PendingRwOrigin, and ChangePending is cleared.
// 5 - A ChangePeriod flag is set to indicate that a change has just occurred.
// 6 - Consumers can check IsChangePeriod() to see if a change has occurred this frame, and respond accordingly.
// 7 - At the end of the frame/update cycle, ClearChangePeriod() is called.

// ------------------------------------------------------------------------------------------------

public static class KoreMovingOrigin
{
    // Define the RW Origin and the scale factor - everything else is derived from these.
    public static KoreXYZVector RwOrigin = KoreXYZVector.Zero;
    public static double RwToGeScaleMultiplier = 1.0;

    // Create a couple of private attributes to allow a random caller to set the next offset position
    private static KoreXYZVector PendingRwOrigin = KoreXYZVector.Zero;
    private static bool ChangePending = false;

    // There is a cycle or frame period in which the change is made and consumers catch up
    private static bool ChangePeriod = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Apply Offset
    // --------------------------------------------------------------------------------------------

    public static void QueueNewOffset(KoreXYZVector newOrigin)
    {
        PendingRwOrigin = newOrigin;
        ChangePending   = true;
    }

    // --------------------------------------------------------------------------------------------

    public static bool IsNewOffsetPending() => ChangePending;

    // --------------------------------------------------------------------------------------------

    public static void ApplyOffset()
    {
        if (ChangePending)
        {
            RwOrigin = PendingRwOrigin;
            ChangePending = false;
            ChangePeriod = true;
        }
    }

    // --------------------------------------------------------------------------------------------

    // Usage: if (KoreMovingOrigin.IsChangePeriod()) { ... }
    public static bool IsChangePeriod() => ChangePeriod;
    public static void ClearChangePeriod() => ChangePeriod = false;

    // --------------------------------------------------------------------------------------------
    // MARK: RW GE Conversion
    // --------------------------------------------------------------------------------------------

    // Translation only - no scale

    // Convert real-world position to offset position, applying the zero offset.
    // Usage: var gePos = KoreMovingOrigin.RWtoOffset(vecPosXYZ);
    public static KoreXYZVector RWtoRWOffset(KoreXYZVector rwPos)
    {
        // Apply the geo rw offset (still in double precision)
        return rwPos + RwOrigin;
    }

    public static KoreXYZVector RWOffsetToRW(KoreXYZVector rwOffset)
    {
        // Reverse the geo rw offset (still in double precision)
        return rwOffset - RwOrigin;
    }

    // --------------------------------------------------------------------------------------------

    // Scaling applied

    public static KoreXYZVector RWOffsettoGeOffset(KoreXYZVector rwOffset)
    {
        // Apply the geo rw offset (still in double precision)
        return rwOffset * RwToGeScaleMultiplier;
    }

    public static KoreXYZVector GeOffsetToRWOffset(KoreXYZVector gePos)
    {
        // Reverse the scale
        KoreXYZVector rwOffset = gePos.Scale(1.0 / RwToGeScaleMultiplier);

        // Reverse the geo rw offset (still in double precision)
        return rwOffset;
    }

    // --------------------------------------------------------------------------------------------

    // XYZ To Ge Units

    public static Godot.Vector3 XYZtoGodot(KoreXYZVector koreXYZ)
    {
        return new Godot.Vector3(
            (float)(koreXYZ.X + RwOrigin.X),
            (float)(koreXYZ.Y + RwOrigin.Y),
            (float)(koreXYZ.Z + RwOrigin.Z)
        );
    }

    public static KoreXYZVector GodottoXYZ(Godot.Vector3 godotVec)
    {
        return new KoreXYZVector(
            (double)godotVec.X,
            (double)godotVec.Y,
            (double)godotVec.Z
        );
    }

    // --------------------------------------------------------------------------------------------

    // Combined RW to GE

    // Usage: var gePos = KoreMovingOrigin.RWtoGodotOffset(vecPosXYZ);
    public static Godot.Vector3 RWtoGodotOffset(KoreXYZVector rwPos)
    {
        // Step 1 - RW to RWOffset
        KoreXYZVector rwOffset = RWtoRWOffset(rwPos);

        // Step 2 - RWOffset to GE Offset (scaling)
        KoreXYZVector geOffset = RWOffsettoGeOffset(rwOffset);

        // Step 3 - GE Offset to Godot Vector3
        return new Godot.Vector3(
            (float)geOffset.X,
            (float)geOffset.Y,
            (float)geOffset.Z
        );
    }

    // --------------------------------------------------------------------------------------------
    // MARK: RW GE Godot
    // --------------------------------------------------------------------------------------------


}