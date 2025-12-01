
using System.Collections.Generic;

using KoreCommon;
using KoreGIS;
using Godot;

public static class KoreConvPos
{
    // ---------------------------------------------------------------------------------------------------
    // MARK: KoreCommon To Godot
    // ---------------------------------------------------------------------------------------------------

    // Converts KoreXYZVector to Godot Vector3
    // Usage: Vector3 godotPos = KoreConvPos.VecToV3(koreVect);
    public static Vector3 VecToV3(KoreXYZVector pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
    }

    // Convert a polar coordinate (azimuth, elevation, distance) to a Godot Vector3
    // Usage: Vector3 godotPos = KoreConvPos.PolarToV3(azimuth, elevation, distance);
    public static Vector3 PolarToV3(float azRads, float elRads, float distanceGE)
    {
        float x = distanceGE * Mathf.Cos(elRads) * Mathf.Sin(azRads);
        float y = distanceGE * Mathf.Sin(elRads);
        float z = distanceGE * Mathf.Cos(elRads) * Mathf.Cos(azRads);
        return new Vector3(x, y, z);
    }

    public static Vector3 PolarToV3(KoreAzElRange polarOffset)
    {
        return PolarToV3(
            (float)polarOffset.AzRads,
            (float)polarOffset.ElRads,
            (float)polarOffset.RangeM);
    }

    // ---------------------------------------------------------------------------------------------------
    // MARK: Godot To KoreCommon
    // ---------------------------------------------------------------------------------------------------

    // Converts Godot Vector3 to KoreXYZVector
    // Usage: KoreXYZVector koreVec = KoreConvPos.V3ToVec(godotVec);
    public static KoreXYZVector V3ToVec(Vector3 vec)
    {
        return new KoreXYZVector { X = vec.X, Y = vec.Y, Z = vec.Z };
    }

    // ---------------------------------------------------------------------------------------------------
    // MARK: KoreXYZBox to Vector3 List
    // ---------------------------------------------------------------------------------------------------

    // usage: List<Vector3> points = KoreConvPos.KoreXYZBoxToV3List(koreBox);
    public static List<Vector3> KoreXYZBoxToV3List(KoreXYZBox box)
    {
        List<Vector3> points = new List<Vector3>
        {
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.TopLeftFront)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.TopRightFront)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.BottomRightFront)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.BottomLeftFront)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.TopLeftBack)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.TopRightBack)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.BottomRightBack)),
            VecToV3(box.Corner(KoreXYZBox.EnumCorner.BottomLeftBack))
        };
        return points;
    }
}