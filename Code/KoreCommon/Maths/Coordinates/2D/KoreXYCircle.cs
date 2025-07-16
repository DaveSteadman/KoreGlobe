using System;

namespace KoreCommon;

public struct KoreXYCircle
{
    public KoreXYPoint Center { get; }
    public double Radius { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Area => Math.PI * Radius * Radius;
    public double Circumference => 2 * Math.PI * Radius;
    public double Diameter => 2 * Radius;

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYCircle(double x, double y, double radius)
    {
        Center = new(x, y);
        Radius = radius;
    }

    public KoreXYCircle(KoreXYPoint center, double radius)
    {
        Center = center;
        Radius = radius;
    }

    public KoreXYCircle(KoreXYCircle circle)
    {
        Center = circle.Center;
        Radius = circle.Radius;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public bool Contains(double x, double y)
    {
        return Center.DistanceTo(x, y) <= Radius;
    }

    public bool Contains(KoreXYPoint xy)
    {
        return Center.DistanceTo(xy) <= Radius;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public KoreXYCircle Offset(double x, double y)
    {
        return new KoreXYCircle(Center.Offset(x, y), Radius);
    }

    public KoreXYCircle Offset(KoreXYVector xy)
    {
        return new KoreXYCircle(Center.Offset(xy), Radius);
    }

    public KoreXYCircle Offset(KoreXYPolarOffset o)
    {
        return new KoreXYCircle(Center.Offset(o), Radius);
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    public KoreXYPoint PointAtAngle(double angleDegs)
    {
        double angleRads = angleDegs * KoreConsts.DegsToRadsMultiplier;
        double x = Center.X + Radius * Math.Cos(angleRads);
        double y = Center.Y + Radius * Math.Sin(angleRads);
        return new KoreXYPoint(x, y);
    }

}