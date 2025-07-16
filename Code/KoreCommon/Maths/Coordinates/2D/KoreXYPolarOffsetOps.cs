using System;

namespace KoreCommon;

// Design Decisions:
// - Performs KoreXYPolarOffset functionality thats not prt of the core class.

public static class KoreXYPolarOffsetOps
{
    public static KoreXYVector ToXY(KoreXYPolarOffset o)
    {
        double x = o.Distance * Math.Cos(o.AngleRads);
        double y = o.Distance * Math.Sin(o.AngleRads);
        return new KoreXYVector(x, y);
    }

    // Turn the XY position into an offset from 0,0

    public static KoreXYPolarOffset FromXYVector(KoreXYVector p)
    {
        double distance = Math.Sqrt(p.X * p.X + p.Y * p.Y);
        double angleRads = Math.Atan2(p.Y, p.X);
        return new KoreXYPolarOffset(angleRads, distance);
    }

    // public static KoreXYPolarOffset ToPolarOffset(KoreXYPoint fromPoint, KoreXYPoint toPoint)
    // {
    //     double xDiff = toPoint.X - fromPoint.X;
    //     double yDiff = toPoint.Y - fromPoint.Y;
    //     double distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    //     double angleRads = Math.Atan2(yDiff, xDiff);
    //     return new KoreXYPolarOffset(angleRads, distance);
    // }
}
