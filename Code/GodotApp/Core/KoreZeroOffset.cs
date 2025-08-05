
using Godot;

using KoreCommon;
using KoreSim;

// A static (non-Node3D class for globally managing the zero offset)

public static class KoreZeroOffset
{
    // ZeroNode: A node always at 0,0,0 for objects (platforms) to be parented to.


    // Real World Earth Center is 0,0,0. We create an offset 0,0,0 for the purposes og focussing the
    // game engine view within the range of its floating point precision.
    public static KoreLLAPoint RwZeroPointLLA = new KoreLLAPoint();

    // Offset "FROM real-world Earth center TO game engine center". We use the inverse of this to place the earth center.
    public static KoreXYZPoint RwZeroPointXYZ = new KoreXYZPoint(0, 0, 0);

    // Game engine earth radius and conversion around it.
    public static double GeEarthRadius            = 5; // Earth radius in Game Engine units
    public static double RwToGeDistanceMultiplier = GeEarthRadius / KoreWorldConsts.EarthRadiusM;
    public static double GeToRwDistanceMultiplier = 1 / RwToGeDistanceMultiplier;

    // Define a reasonable "Up Distance" (Real World Meters) that still works when scales to the GE ranges.
    public static double UpDistRwM    = 2 * GeToRwDistanceMultiplier;
    public static double AheadDistGeM = 2 * GeToRwDistanceMultiplier;

    // --------------------------------------------------------------------------------------------

    // Report the constants for debugging.
    // Usage: KoreZeroOffset.ReportConsts();
    public static void ReportConsts()
    {
        string constReport = $"KoreZeroOffset.ReportConsts:\n- GeEarthRadius:{GeEarthRadius}\n- RwToGeDistanceMultiplier:{RwToGeDistanceMultiplier}\n- GeToRwDistanceMultiplier:{GeToRwDistanceMultiplier}\n- AheadDistGeM:{AheadDistGeM}\n- UpDistRwM:{UpDistRwM}";

        GD.Print(constReport);

        KoreCentralLog.AddEntry(constReport);
    }

    // --------------------------------------------------------------------------------------------

    // Set the zero point for the game engine.
    // Usage: KoreZeroOffset.SetLLA(pos);

    public static void SetLLA(KoreLLAPoint rwLLA)
    {
        RwZeroPointLLA = rwLLA;
        RwZeroPointXYZ = rwLLA.ToXYZ();

        //GD.Print($"KoreZeroOffset.SetLLA: RwZeroPointLLA:{RwZeroPointLLA} RwZeroPointXYZ:{RwZeroPointXYZ}");
    }

    // --------------------------------------------------------------------------------------------

    // The real-world XYZ we have from the model in A. the Earth centre offset is B, and we need the game engine
    // zero-offset C: C = A - B

    public static KoreXYZPoint RwZeroPointOffset(KoreXYZPoint RwXYZ)
    {
        KoreXYZVector offset = RwZeroPointXYZ.XYZTo(RwXYZ);
        return new KoreXYZPoint(offset.X, offset.Y, offset.Z);
    }

    public static KoreXYZPoint RwZeroPointOffset(KoreLLAPoint RwLLA)
    {
        KoreXYZPoint RwXYZ = RwLLA.ToXYZ();
        KoreXYZVector offset = RwZeroPointXYZ.XYZTo(RwXYZ);
        return new KoreXYZPoint(offset.X, offset.Y, offset.Z);
    }

    // ---------------------------------------------------------------------------------------------

    // To convert from an RW XYZ to a GE XYZ, we need to:
    // 1 - Subtract the zero point offset to get the offset XYZ.
    // 2 - Invert the Z axis to match the Godot engine orientation.
    // 3 - Scale the XYZ by the GE distance multiplier.
    // 4 - Return the vector3.

    // Usage: Vector3 GePos = KoreZeroOffset.GeZeroPointOffset(RwXYZPos);

    public static Vector3 GeZeroPointOffset(KoreXYZPoint RwXYZ)
    {
        // 1 - Subtract the zero point offset to get the offset XYZ.
        KoreXYZVector rwOffsetXYZ = RwZeroPointXYZ.XYZTo(RwXYZ);

        // 2 - Invert the Z axis to match the Godot engine orientation.
        double x = rwOffsetXYZ.X;
        double y = rwOffsetXYZ.Y;
        double z = rwOffsetXYZ.Z * -1;

        // 3 - Scale the XYZ by the GE distance multiplier.
        x = x * RwToGeDistanceMultiplier;
        y = y * RwToGeDistanceMultiplier;
        z = z * RwToGeDistanceMultiplier;

        // 4 - Return the vector3.
        return new Vector3((float)x, (float)y, (float)z);
    }

    public static Vector3 GeZeroPointOffset(KoreLLAPoint rwLLA) => GeZeroPointOffset(rwLLA.ToXYZ());

    // ---------------------------------------------------------------------------------------------

    // Usage: KoreZeroOffset.GeZeroPoint()
    public static Vector3 GeZeroPoint()
    {
        // return new Vector3((float)RwZeroPointXYZ.X, (float)RwZeroPointXYZ.Y, (float)RwZeroPointXYZ.Z);
        return new Vector3(0, 0, 0);
    }

    // RwZeroPointXYZ is the offset from the real-world core to the zero point, so we need to apply the invese of this
    // to get the now-offset core point.
    // Note that we always need to be inverting the Z axis to match the Godot engine orientation, so the -- = + here.
    // Usage: Vector3 GeCorePos = KoreZeroOffset.GeCorePoint();
    public static Vector3 GeCorePoint()
    {
        // The Z axis is inverted in the Godot engine, but as we're creating an iverted vector, the inverse of this pplies (ie to X and Y)
        double x = RwZeroPointXYZ.X * RwToGeDistanceMultiplier * -1;
        double y = RwZeroPointXYZ.Y * RwToGeDistanceMultiplier * -1;
        double z = RwZeroPointXYZ.Z * RwToGeDistanceMultiplier;

        return new Vector3((float)x, (float)y, (float)z);
    }
}
