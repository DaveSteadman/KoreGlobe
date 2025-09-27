using System;
using System.Numerics;

using KoreCommon;
using Godot;

// KoreRelocateOps: Relocatable geometry
// - Runs entirely off of real world positions, as a last point where double precision is involved.
// - Maintains an offset position, and converts between common and Godot types.
// Note:
// - We need to apply a new offset at a defined point in time, to be frame synchronised. Thats the job for the
//   zero node in a deferred call, to apply the new value.

public static class KoreRelocateOps
{
    // Game Engine Units for an offset to apply to geometry to keep it near the origin.
    public static KoreXYZVector GeoOffset = KoreXYZVector.Zero;

    // Create a couple of private attributes to allow a random caller to set the next offset position
    private static KoreXYZVector PendingGeoOffset = KoreXYZVector.Zero;
    private static bool ChangePending = false;

    // There is a cycle or frame period in which the change is made and consumers catch up
    private static bool ChangePeriod = false;

    // The scale factor between real-world (RW) and game-engine (GE) units.
    // - RW is in metres, GE is in "units" (usually 1 unit = 1 metre, but can be changed to 1km etc).
    public static double RwToGeScaleMultiplier = 1.0;

    // --------------------------------------------------------------------------------------------
    // MARK: Offset
    // --------------------------------------------------------------------------------------------

    public static void QueueNewOffset(KoreXYZVector newOffset)
    {
        PendingGeoOffset = newOffset;
        ChangePending = true;
    }

    // --------------------------------------------------------------------------------------------

    public static bool IsNewOffsetPending() => ChangePending;

    // --------------------------------------------------------------------------------------------

    public static void ApplyOffset()
    {
        if (ChangePending)
        {
            GeoOffset = PendingGeoOffset;
            ChangePending = false;
            ChangePeriod = true;
        }
    }

    // --------------------------------------------------------------------------------------------

    // Usage: if (KoreRelocateOps.IsChangePeriod()) { ... }
    public static bool IsChangePeriod() => ChangePeriod;
    public static void ClearChangePeriod() => ChangePeriod = false;

    // --------------------------------------------------------------------------------------------
    // MARK: RW GE Conversion
    // --------------------------------------------------------------------------------------------

    // Convert real-world position to game-engine position, applying the zero offset.
    // Usage: var gePos = KoreRelocateOps.RWtoGE(vecPosXYZ);
    public static Godot.Vector3 RWtoGE(KoreXYZVector rwPos)
    {
        // 1 - Apply the geo rw offset (still in double precision)
        KoreXYZVector rwOffset = GeoOffset.XYZTo(rwPos);

        // 1 - Scale the real-world position to game-engine units
        KoreXYZVector gePos = rwOffset * RwToGeScaleMultiplier;

        // 3 - Convert to Vector3 (float precision)
        return new Godot.Vector3((float)gePos.X, (float)gePos.Y, (float)gePos.Z);
    }

    // --------------------------------------------------------------------------------------------

    // Convert game-engine position to real-world position, removing the zero offset.
    // Inverts the RWtoGE operations
    // Usage: var rwPosXYZ = KoreRelocateOps.GEtoRW(vec3Pos)
    public static KoreXYZVector GEtoRW(Godot.Vector3 gePos)
    {
        // 1 - Convert to double precision
        KoreXYZVector gePosD = new KoreXYZVector(gePos.X, gePos.Y, gePos.Z);

        // 2 - Scale the game-engine position to real-world units
        KoreXYZVector gePosRWScaled = gePosD / RwToGeScaleMultiplier;

        // 3 - Remove the geo offset (still in double precision)
        KoreXYZVector rwPos = gePosRWScaled + GeoOffset;

        return rwPos;
    }

}