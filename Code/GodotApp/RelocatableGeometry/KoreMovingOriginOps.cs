
using Godot;

using KoreCommon;
using KoreGIS;
using KoreSim;

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
        KoreXYZVector rwXYZ = rwLLA.ToXYZ();
        KoreXYZVector gePos = KoreMovingOrigin.RWtoGeOffset(rwXYZ);

        Vector3 godotPos = KoreConvPos.VecToV3(gePos);
        return godotPos;
    }

    // Usage: Vector3 pos = KoreMovingOriginOps.RwLLAToGeOffset(radiusM, latDegs, lonDegs);
    public static Vector3 RwLLAToGeOffset(double radiusM, double latDegs, double lonDegs)
    {
        KoreLLAPoint rwLLA = new KoreLLAPoint() { RadiusM = radiusM, LatDegs = latDegs, LonDegs = lonDegs };
        return RwLLAToGeOffset(rwLLA);
    }

}
