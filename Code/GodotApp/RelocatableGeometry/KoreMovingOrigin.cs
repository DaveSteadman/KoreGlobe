using System;
using System.Numerics;

using KoreCommon;
using Godot;

// KoreMovingOrigin: Relocatable geometry
// - We have "RW" real world co-ordinates, and "GE" game engine co-ordinates.
// - We have an RwOrigin point, which is the real-world position that maps to the GE origin (0,0,0).
// - A scale factor exists between RW and GE units.

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

    // Convert real-world position to offset position, applying the zero offset.
    // Usage: var gePos = KoreMovingOrigin.RWtoOffset(vecPosXYZ);
    public static KoreXYZVector RWtoRWOffset(KoreXYZVector rwPos)
    {
        // Apply the geo rw offset (still in double precision)
        return RwOrigin.XYZTo(rwPos);
    }

    public static KoreXYZVector RWOffsetToRW(KoreXYZVector rwOffset)
    {
        // Reverse the geo rw offset (still in double precision)
        return rwOffset + RwOrigin;
    }

    public static KoreXYZVector RWtoGeOffset(KoreXYZVector rwPos)
    {
        // Apply the geo rw offset (still in double precision)
        KoreXYZVector rwOffset = RwOrigin.XYZTo(rwPos);
        return rwOffset * RwToGeScaleMultiplier;
    }

    public static KoreXYZVector GeOffsetToRW(KoreXYZVector gePos)
    {
        // Reverse the scale
        KoreXYZVector rwOffset = gePos.Scale(1.0 / RwToGeScaleMultiplier);

        // Reverse the geo rw offset (still in double precision)
        return rwOffset + RwOrigin;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: RW GE Godot
    // --------------------------------------------------------------------------------------------

    // Convert real-world position to offset position, applying the zero offset.
    // Usage: var gePos = KoreMovingOrigin.RWtoRWOffsetG(vecPosXYZ);
    public static Godot.Vector3 RWtoRWOffsetG(KoreXYZVector rwPos)
    {
        // Apply the geo rw offset (still in double precision)
        KoreXYZVector rwOffset = RwOrigin.XYZTo(rwPos);
        return KoreConvPos.VecToV3(rwOffset);
    }
}