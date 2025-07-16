using System;

// KoreXYZPolarOffset: Class represents an angle defined offset in the abstract XYZ space.

namespace KoreCommon;

public struct KoreXYZPolarOffset
{
    public double Range { get; set; }
    public double AzRads { get; set; }
    public double ElRads { get; set; }

    // Add properties to convert to/from degrees
    public double AzDegs
    {
        get { return AzRads * KoreConsts.RadsToDegsMultiplier; }
        set { AzRads = value * KoreConsts.DegsToRadsMultiplier; }
    }

    public double ElDegs
    {
        get { return ElRads * KoreConsts.RadsToDegsMultiplier; }
        set { ElRads = value * KoreConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Constructor accepts radians
    public KoreXYZPolarOffset(double a, double e, double r)
    {
        this.AzRads = a;
        this.ElRads = e;
        this.Range = r;
    }

    public KoreXYZPolarOffset(KoreXYZVector xyz)
    {
        this = FromXYZ(xyz);
    }

    // Static property for a zero offset
    public static KoreXYZPolarOffset Zero
    {
        get { return new KoreXYZPolarOffset { AzRads = 0.0, ElRads = 0.0, Range = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Functions
    // --------------------------------------------------------------------------------------------

    public KoreXYZPolarOffset Scale(double scaleFactor)
    {
        return new KoreXYZPolarOffset(AzRads, ElRads, Range * scaleFactor);
    }

    public KoreXYZPolarOffset WithRange(double newRange)
    {
        return new KoreXYZPolarOffset(AzRads, ElRads, newRange);
    }

    public KoreXYZPolarOffset Inverse()
    {
        return new KoreXYZPolarOffset
        {
            AzRads = this.AzRads + Math.PI, // 180 degrees in radians
            ElRads = -this.ElRads,
            Range = this.Range
        };
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Conversion
    // --------------------------------------------------------------------------------------------

    public KoreXYZVector ToXYZ()
    {
        double x = Range * Math.Cos(ElRads) * Math.Sin(AzRads);
        double y = Range * Math.Sin(ElRads);
        double z = Range * Math.Cos(ElRads) * Math.Cos(AzRads);

        return new KoreXYZVector(x, y, z);
    }

    public static KoreXYZPolarOffset FromXYZ(KoreXYZVector xyz)
    {
        double mag = xyz.Magnitude;

        KoreXYZPolarOffset newOffset = new KoreXYZPolarOffset()
        {
            Range = mag,
            AzRads = Math.Atan2(xyz.X, xyz.Z),
            ElRads = Math.Asin(xyz.Y / mag)
        };
        return newOffset;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"(AzDegs:{AzDegs:F2}, ElDegs:{ElDegs:F2}, RangeM:{Range:F2})");
    }

}
