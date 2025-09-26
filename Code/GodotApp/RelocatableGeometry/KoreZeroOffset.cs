
using Godot;

using KoreCommon;
using KoreSim;

// KoreZeroOffset:
// - A static class (accessible from everywhere) that provides the mapping between a real-world position and a game engine position.
// - The translation is based on a moving XYZ point, to game-engine coordinates are closer to zero AND a scaling from real-world meters to game-engine units.
// - We have a position that is "set" at any point in time, and a position that is "applied" to synchronise with game engine frame updates.
// - Obviously we only ever consume an "applied" value.
// - The ZeroNode Node3 object holds no data, just the functionality to drive the move from "set" to "applied".

public static class KoreZeroOffset
{
    // ZeroNode: A node always at 0,0,0 for objects (platforms) to be parented to.

    // SET values
    private static KoreLLAPoint SetRwZeroPosLLA = KoreLLAPoint.Zero; // KoreZeroOffset.SetRwZeroPosLLA
    public static bool ZeroPosChangePending = false; // KoreZeroOffset.ZeroPosChangePending

    // APPLIED values
    public static KoreLLAPoint AppliedZeroPosLLA = KoreLLAPoint.Zero; // KoreZeroOffset.AppliedZeroPosLLA
    public static KoreXYZVector AppliedZeroPosXYZ = KoreXYZVector.Zero; // KoreZeroOffset.AppliedZeroPosXYZ


    public static bool IsPosChangeCycle = false; // KoreZeroOffset.IsPosChangeCycle




    // // Real World Earth Center is 0,0,0. We create an offset 0,0,0 for the purposes og focussing the
    // // game engine view within the range of its floating point precision.
    // public static KoreLLAPoint RwZeroPointLLA = new KoreLLAPoint();

    // // Offset "FROM real-world Earth center TO game engine center". We use the inverse of this to place the earth center.
    // public static KoreXYZVector RwZeroPointXYZ = new KoreXYZVector(0, 0, 0);

    // Game engine earth radius and conversion around it.
    public static double GeEarthRadius = 60; // Earth radius in Game Engine units // KoreZeroOffset.GeEarthRadius
    public static double RwToGeDistanceMultiplier = GeEarthRadius / KoreWorldConsts.EarthRadiusM;
    public static double GeToRwDistanceMultiplier = 1 / RwToGeDistanceMultiplier;

    // --------------------------------------------------------------------------------------------
    // MARK: SET
    // --------------------------------------------------------------------------------------------

    // Set the zero point for the game engine.
    // Usage: KoreZeroOffset.SetLLA(pos);

    public static void SetLLA(KoreLLAPoint rwLLA)
    {
        SetRwZeroPosLLA = rwLLA;
        ZeroPosChangePending = true;

        //GD.Print($"KoreZeroOffset.SetLLA: RwZeroPointLLA:{RwZeroPointLLA} RwZeroPointXYZ:{RwZeroPointXYZ}");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: APPLY
    // --------------------------------------------------------------------------------------------

    // NOTE: Expect these to be called in Deferred calls, so that the ZeroNode can apply the position at the end of the frame.

    // Usage: KoreZeroOffset.ApplyLLA();
    public static void ApplyLLA()
    {
        if (ZeroPosChangePending)
        {
            // Set the applied position to the set position.
            AppliedZeroPosLLA = SetRwZeroPosLLA;
            AppliedZeroPosXYZ = AppliedZeroPosLLA.ToXYZ();

            // Clear the change pending flag.
            ZeroPosChangePending = false;

            // Set the applied flag, so consumers know to update positions
            IsPosChangeCycle = true;
        }
    }

    // Usage: KoreZeroOffset.ClearUpdate();
    public static void ClearUpdate()
    {
        IsPosChangeCycle = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Convert
    // --------------------------------------------------------------------------------------------

    // The real-world XYZ we have from the model in A. the Earth centre offset is B, and we need the game engine
    // zero-offset C: C = A - B

    public static KoreXYZVector RwZeroPointOffset(KoreXYZVector RwXYZ)
    {
        KoreXYZVector offset = AppliedZeroPosXYZ.XYZTo(RwXYZ);
        return new KoreXYZVector(offset.X, offset.Y, offset.Z);
    }

    public static KoreXYZVector RwZeroPointOffset(KoreLLAPoint RwLLA)
    {
        KoreXYZVector RwXYZ = RwLLA.ToXYZ();
        KoreXYZVector offset = AppliedZeroPosXYZ.XYZTo(RwXYZ);
        return new KoreXYZVector(offset.X, offset.Y, offset.Z);
    }

    // ---------------------------------------------------------------------------------------------

    // To convert from an RW XYZ to a GE XYZ, we need to:
    // 1 - Subtract the zero point offset to get the offset XYZ.
    // 2 - Invert the Z axis to match the Godot engine orientation.
    // 3 - Scale the XYZ by the GE distance multiplier.
    // 4 - Return the vector3.

    // Usage: Vector3 GePos = KoreZeroOffset.GeZeroPointOffset(RwXYZPos);

    public static Vector3 GeZeroPointOffset(KoreXYZVector RwXYZ)
    {
        // 1 - Subtract the zero point offset to get the offset XYZ.
        KoreXYZVector rwOffsetXYZ = AppliedZeroPosXYZ.XYZTo(RwXYZ);

        // 2 - Invert the Z axis to match the Godot engine orientation.
        double x = rwOffsetXYZ.X;
        double y = rwOffsetXYZ.Y;
        double z = rwOffsetXYZ.Z;

        // 3 - Scale the XYZ by the GE distance multiplier.
        x = x * RwToGeDistanceMultiplier;
        y = y * RwToGeDistanceMultiplier;
        z = z * RwToGeDistanceMultiplier;

        // 4 - Return the vector3 (now floats)
        return new Vector3((float)x, (float)y, (float)z);
    }

    public static Vector3 GeZeroPointOffset(KoreLLAPoint rwLLA) => GeZeroPointOffset(rwLLA.ToXYZ());

    // --------------------------------------------------------------------------------------------
    // MARK: Utils
    // --------------------------------------------------------------------------------------------

    // Report the constants for debugging.
    // Usage: KoreZeroOffset.ReportConsts();
    public static void ReportConsts()
    {
        string constReport = $"KoreZeroOffset.ReportConsts:\n- GeEarthRadius:{GeEarthRadius}\n- RwToGeDistanceMultiplier:{RwToGeDistanceMultiplier}\n- GeToRwDistanceMultiplier:{GeToRwDistanceMultiplier}";

        GD.Print(constReport);

        KoreCentralLog.AddEntry(constReport);
    }

}
