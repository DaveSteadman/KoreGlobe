// <fileheader>

using System;

namespace KoreCommon;

public static class KoreXYZPolyLineOps
{
    // 3 point Bezier curve in 3D
    public static KoreXYZVector Calculate3PointBezierPos(double t, KoreXYZVector start, KoreXYZVector control, KoreXYZVector end)
    {
        double u = 1 - t;
        double tt = t * t;
        double uu = u * u;

        double x = uu * start.X + 2 * u * t * control.X + tt * end.X;
        double y = uu * start.Y + 2 * u * t * control.Y + tt * end.Y;
        double z = uu * start.Z + 2 * u * t * control.Z + tt * end.Z;

        return new KoreXYZVector(x, y, z);
    }

    // The tangent angle concept needs to be adapted for 3D as a tangent vector rather than an angle
    public static KoreXYZVector Calculate3PointBezierFirstDerivative(KoreXYZVector start, KoreXYZVector control, KoreXYZVector end, double t)
    {
        double dx = 2 * (1 - t) * (control.X - start.X) + 2 * t * (end.X - control.X);
        double dy = 2 * (1 - t) * (control.Y - start.Y) + 2 * t * (end.Y - control.Y);
        double dz = 2 * (1 - t) * (control.Z - start.Z) + 2 * t * (end.Z - control.Z);

        return new KoreXYZVector(dx, dy, dz);
    }

    // Second derivative in 3D
    public static KoreXYZVector Calculate3PointBezierSecondDerivative(KoreXYZVector start, KoreXYZVector control, KoreXYZVector end, double t)
    {
        return new KoreXYZVector(2 * (end.X - 2 * control.X + start.X), 2 * (end.Y - 2 * control.Y + start.Y), 2 * (end.Z - 2 * control.Z + start.Z));
    }

    // --------------------------------------------------------------------------------------------

    // 4 point Bezier curve in 3D
    public static KoreXYZVector Calculate4PointBezierPos(double t, KoreXYZVector start, KoreXYZVector control1, KoreXYZVector control2, KoreXYZVector end)
    {
        double u = 1 - t;
        double tt = t * t;
        double uu = u * u;
        double uuu = uu * u;
        double ttt = tt * t;

        double x = uuu * start.X + 3 * uu * t * control1.X + 3 * u * tt * control2.X + ttt * end.X;
        double y = uuu * start.Y + 3 * uu * t * control1.Y + 3 * u * tt * control2.Y + ttt * end.Y;
        double z = uuu * start.Z + 3 * uu * t * control1.Z + 3 * u * tt * control2.Z + ttt * end.Z;

        return new KoreXYZVector(x, y, z);
    }

    public static KoreXYZVector Calculate4PointBezierFirstDerivative(KoreXYZVector start, KoreXYZVector control1, KoreXYZVector control2, KoreXYZVector end, double t)
    {
        double dx = 3 * (1 - t) * (1 - t) * (control1.X - start.X) +
                    6 * (1 - t) * t * (control2.X - control1.X) +
                    3 * t * t * (end.X - control2.X);
        double dy = 3 * (1 - t) * (1 - t) * (control1.Y - start.Y) +
                    6 * (1 - t) * t * (control2.Y - control1.Y) +
                    3 * t * t * (end.Y - control2.Y);
        double dz = 3 * (1 - t) * (1 - t) * (control1.Z - start.Z) +
                    6 * (1 - t) * t * (control2.Z - control1.Z) +
                    3 * t * t * (end.Z - control2.Z);

        return new KoreXYZVector(dx, dy, dz);
    }

    // Second derivative in 3D
    public static KoreXYZVector Calculate4PointBezierSecondDerivative(KoreXYZVector start, KoreXYZVector control1, KoreXYZVector control2, KoreXYZVector end, double t)
    {
        double dx = 6 * (1 - t) * (control2.X - 2 * control1.X + start.X) + 6 * t * (end.X - 2 * control2.X + control1.X);
        double dy = 6 * (1 - t) * (control2.Y - 2 * control1.Y + start.Y) + 6 * t * (end.Y - 2 * control2.Y + control1.Y);
        double dz = 6 * (1 - t) * (control2.Z - 2 * control1.Z + start.Z) + 6 * t * (end.Z - 2 * control2.Z + control1.Z);

        return new KoreXYZVector(dx, dy, dz);
    }

    // Calculate curvature for 3D is more complex and typically involves more advanced geometric calculations.
    // Placeholder for CalculateCurvature method

    // Creating a 3D Bezier polyline remains conceptually similar, just working with KoreXYZVector instead of KoreXYVector
    // Placeholder for Create3PointBezier and Create4PointBezier methods for 3D
}
