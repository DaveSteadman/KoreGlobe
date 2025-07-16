using System;

namespace KoreCommon;

public struct KoreXYZSphere
{
    public KoreXYZPoint Center { get; private set; }
    public double Radius { get; private set; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Volume { get { return 4 / 3 * Math.PI * Radius * Radius * Radius; } }
    public double SurfaceArea { get { return 4 * Math.PI * Radius * Radius; } }
    public double Diameter { get { return 2 * Radius; } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYZSphere(KoreXYZPoint center, double radius)
    {
        if (radius <= 0)
        {
            throw new ArgumentException("Radius must be positive.", nameof(radius));
        }

        Center = center;
        Radius = radius;
    }

    public bool ContainsPoint(KoreXYZPoint point)
    {
        return (point - Center).Magnitude <= Radius;
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    public KoreXYZPoint SurfacePoint(double AzDegs, double ElDegs)
    {
        double azRads = AzDegs * KoreConsts.DegsToRadsMultiplier;
        double elRads = ElDegs * KoreConsts.DegsToRadsMultiplier;

        double x = Center.X + Radius * Math.Cos(elRads) * Math.Cos(azRads);
        double y = Center.Y + Radius * Math.Cos(elRads) * Math.Sin(azRads);
        double z = Center.Z + Radius * Math.Sin(elRads);

        return new KoreXYZPoint(x, y, z);
    }

    // Additional methods can be added here as needed
}