
using Godot;

using KoreCommon;
using KoreGIS;
using KoreSim;

public struct KorePointOrientationV3
{
    public Vector3 Pos;
    public Vector3 PosAbove;
    public Vector3 PosAhead;
    public Vector3 PosNorth;
    public Vector3 VecUp;
    public Vector3 VecForward;
    public Vector3 VecNorth;
}

public static class KoreMovingOriginOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Init
    // --------------------------------------------------------------------------------------------

    // Set the zero point for the game engine.
    // Usage: KoreMovingOriginOps.InitialiseMovingOffset(MO, posLLA, geWorldRadius);

    public static void InitialiseMovingOffset(KoreLLAPoint rwLLA, double geWorldRadius)
    {
        GD.Print($"KoreMovingOriginOps.InitialiseMovingOffset: Setting initial zero LLA to {rwLLA}");

        KoreMovingOrigin.RwOrigin = rwLLA.ToXYZ();
        KoreMovingOrigin.RwToGeScaleMultiplier = geWorldRadius / KoreWorldConsts.EarthRadiusM;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Conv + Godot
    // --------------------------------------------------------------------------------------------


    // --------------------------------------------------------------------------------------------
    // MARK: LLA Conv
    // --------------------------------------------------------------------------------------------

    // Usage: Vector3 pos = KoreMovingOriginOps.RwLLAToGeOffset(posLLA);
    public static Vector3 RwLLAToGeOffset(KoreLLAPoint rwLLA)
    {
        return KoreMovingOrigin.RWtoGodotOffset(rwLLA.ToXYZ());
    }

    // Usage: Vector3 pos = KoreMovingOriginOps.RwLLAToGeOffset(radiusM, latDegs, lonDegs);
    public static Vector3 RwLLAToGeOffset(double radiusM, double latDegs, double lonDegs)
    {
        KoreLLAPoint rwLLA = new KoreLLAPoint() { RadiusM = radiusM, LatDegs = latDegs, LonDegs = lonDegs };
        return RwLLAToGeOffset(rwLLA);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Angles / Vectors
    // --------------------------------------------------------------------------------------------

    // Get the orientation at a given LLA point and AzEl angles, for setting up a transform and LookAt().
    // Usage: var orientation = KoreMovingOriginOps.OrientationAtPoint(llaPoint, az

    public static KorePointOrientationV3 OrientationAtPoint(KoreLLAPoint llaPoint, KoreAzEl azEl)
    {
        KorePointOrientationV3 orientation = new KorePointOrientationV3();


        KoreAzElRange offset = new KoreAzElRange(azEl, 1000.0);

        // Define points around the LLA point
        KoreLLAPoint abovePoint = new KoreLLAPoint() { RadiusM = llaPoint.RadiusM + 100.0, LatDegs = llaPoint.LatDegs, LonDegs = llaPoint.LonDegs };
        KoreLLAPoint northPoint = new KoreLLAPoint() { RadiusM = llaPoint.RadiusM, LatDegs = llaPoint.LatDegs + 0.1, LonDegs = llaPoint.LonDegs };
        KoreLLAPoint aheadPoint = llaPoint.PlusPolarOffset(offset);

        // Calculate the XYZ offset positions of each point
        orientation.Pos      = KoreMovingOrigin.RWtoGodotOffset(llaPoint.ToXYZ());
        orientation.PosAbove = KoreMovingOrigin.RWtoGodotOffset(abovePoint.ToXYZ());
        orientation.PosNorth = KoreMovingOrigin.RWtoGodotOffset(northPoint.ToXYZ());
        orientation.PosAhead = KoreMovingOrigin.RWtoGodotOffset(aheadPoint.ToXYZ());
        // Vectors
        orientation.VecUp      = orientation.PosAbove - orientation.Pos;
        orientation.VecForward = orientation.PosAhead - orientation.Pos;
        orientation.VecNorth   = orientation.PosNorth - orientation.Pos;

        return orientation;
    }


}
