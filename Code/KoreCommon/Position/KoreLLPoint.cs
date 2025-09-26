using System;

namespace KoreCommon;


// The struct KoreLLPoint stores Lat Long values from a bottom-left origin, which is useful for map tiles.
// Will convert between this and a KoreLLAPoint where the origin is the "center".

public struct KoreLLPoint
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }

    // --------------------------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // --------------------------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * KoreConsts.RadsToDegsMultiplier; }
        set { LatRads = value * KoreConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * KoreConsts.RadsToDegsMultiplier; }
        set { LonRads = value * KoreConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------
    // Constructors - different options and units
    // --------------------------------------------------------------------------------------------

    // Note that fields can be set:
    //   KoreLLPoint pos = new KoreLLPoint() { latDegs = X, LonDegs = Y };

    public KoreLLPoint(double laRads, double loRads)
    {
        LatRads = laRads;
        LonRads = loRads;
    }

    public KoreLLPoint(KoreLLAPoint llPos)
    {
        this.LatRads = llPos.LatRads;
        this.LonRads = llPos.LonRads;
    }

    public static KoreLLPoint Zero
    {
        get { return new KoreLLPoint { LatRads = 0.0, LonRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------

    // Convert To/From XYZ coordinates
    // - Where X = right to longitude 90� (East)
    // -       Y = up to North Pole (lat 90�) and Y-ve to South Pole (lat -90�)
    // -       Z = forward to zero lat/long and Z-ve to longitude 180 (date line)

    // Usage: KoreXYZVector xyzpos = llpos.ToXYZ(radius);
    public KoreXYZVector ToXYZ(double radius)
    {
        // Protect against div0 radius
        if (radius < KoreConsts.ArbitrarySmallDouble)
            return KoreXYZVector.Zero;

        KoreXYZVector retXYZ = new KoreXYZVector(
            radius * Math.Cos(LatRads) * Math.Sin(LonRads),   // X = r.cos(lat).sin(lon) - to match +X=lon90
            radius * Math.Sin(LatRads),                       // Y = r.sin(lat)
            radius * Math.Cos(LatRads) * Math.Cos(LonRads));  // Z = r.cos(lat).cos(lon) - to match +Z=lon0
        return retXYZ;
    }

    // Usage: KoreLLPoint pos = KoreLLPoint.FromXYZ(xyz);
    public static KoreLLPoint FromXYZ(KoreXYZVector inputXYZ)
    {
        double radius = inputXYZ.Magnitude;

        // Protect against div0 radius
        if (radius < KoreConsts.ArbitrarySmallDouble)
            return KoreLLPoint.Zero;

        double latRads = Math.Asin(inputXYZ.Y / radius);
        double lonRads = Math.Atan2(inputXYZ.X, inputXYZ.Z);
        return new KoreLLPoint(latRads, lonRads);
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"({LatDegs:F3}, {LonDegs:F3})");
    }

}
