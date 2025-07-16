using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

// Point = a position, absolute.
// Vector = an offset, relative, scalable.

namespace KoreCommon;

public struct KoreXYPoint
{
    public double X { get; }
    public double Y { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public KoreXYPoint(KoreXYPoint xy)
    {
        X = xy.X;
        Y = xy.Y;
    }

    public static KoreXYPoint Zero
    {
        get { return new KoreXYPoint(0, 0); }
    }

    public override string ToString()
    {
        return $"KoreXYPoint(X: {X:F3}, Y: {Y:F3})";
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Absolute value of the magnitude of the distance from this point, to the specified point.

    public double DistanceTo(double x, double y)
    {
        return Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
    }

    public double DistanceTo(KoreXYPoint xy)
    {
        return Math.Sqrt(Math.Pow(X - xy.X, 2) + Math.Pow(Y - xy.Y, 2));
    }

    // --------------------------------------------------------------------------------------------

    // return the angle FROM this point TO the given point - East / Positve X axis is zero
    public double AngleToRads(double x, double y)
    {
        if (KoreValueUtils.EqualsWithinTolerance(x, X) && KoreValueUtils.EqualsWithinTolerance(y, Y))
            return 0;

        return Math.Atan2(y - Y, x - X);
    }

    public double AngleToRads(KoreXYPoint xy)
    {
        if (EqualsWithinTolerance(this, xy))
            return 0;

        // return the angle FROM this point TO the given point
        return Math.Atan2(xy.Y - Y, xy.X - X);
    }

    public double AngleToDegs(double x, double y) => KoreValueUtils.RadsToDegs(AngleToRads(x, y));
    public double AngleToDegs(KoreXYPoint xy) => KoreValueUtils.RadsToDegs(AngleToRads(xy));

    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public KoreXYPoint Offset(double x, double y) => new KoreXYPoint(X + x, Y + y);
    public KoreXYPoint Offset(KoreXYVector xy) => new KoreXYPoint(X + xy.X, Y + xy.Y);
    public KoreXYPoint Offset(KoreXYPolarOffset o) => Offset(KoreXYPolarOffsetOps.ToXY(o));

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static KoreXYPoint Sum(KoreXYPoint a, KoreXYPoint b) => new KoreXYPoint(a.X + b.X, a.Y + b.Y);
    public static KoreXYPoint Diff(KoreXYPoint a, KoreXYPoint b) => new KoreXYPoint(a.X - b.X, a.Y - b.Y);
    //public static KoreXYPoint Scale(KoreXYPoint a, double b)    => new KoreXYPoint(a.X * b, a.Y * b);

    public static bool EqualsWithinTolerance(KoreXYPoint a, KoreXYPoint b, double tolerance = KoreConsts.ArbitraryMinDouble)
    {
        return KoreValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance) && KoreValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static KoreXYPoint operator +(KoreXYPoint a, KoreXYPoint b) { return new KoreXYPoint(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static KoreXYPoint operator -(KoreXYPoint a, KoreXYPoint b) { return new KoreXYPoint(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    public static KoreXYPoint operator *(KoreXYPoint a, double b) { return new KoreXYPoint(a.X * b, a.Y * b); }
    public static KoreXYPoint operator *(double b, KoreXYPoint a) { return new KoreXYPoint(a.X * b, a.Y * b); }

    // / operator overload for scaling point in relation to origin
    public static KoreXYPoint operator /(KoreXYPoint a, double b) { return new KoreXYPoint(a.X / b, a.Y / b); }
    public static KoreXYPoint operator /(double b, KoreXYPoint a) { return new KoreXYPoint(a.X / b, a.Y / b); }

}