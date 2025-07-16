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

    public override string ToString()
    {
        return string.Format($"({LatDegs:F3}, {LonDegs:F3})");
    }

}
