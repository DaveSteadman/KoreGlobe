// <fileheader>

using System;
using System.Collections.Generic;

namespace KoreCommon;

public static class KoreXYZVectorOps
{
    // Dot Product - the cosine of the angle between the two vectors
    // Considering both as lines from 0,0 to the points, this is the cosine of the angle between them
    // near +1 means the angle is near 0 degrees (absolute to remove sign)
    // wiki - https://en.wikipedia.org/wiki/Dot_product

    // Computes the dot product of two 3D points (vectors).
    // The result is a scalar representing the cosine of the angle between the vectors, scaled by their magnitudes.
    // Useful for measuring alignment or projection between directions.
    // Calculates the dot product of two 3D points (vectors).
    // This returns the sum of the products of their corresponding components.
    // The result is proportional to the cosine of the angle between the vectors.
    public static double DotProduct(KoreXYZVector a, KoreXYZVector b)
    {
        return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
    }

    // Cross Product - the vector perpendicular to both input vectors
    // wiki - https://en.wikipedia.org/wiki/Cross_product

    // Computes the cross product of two 3D points (vectors).
    // Returns a new vector perpendicular to both inputs, with magnitude equal to the area of the parallelogram they span.
    // Used to find normals or perpendicular directions in 3D geometry.
    // Calculates the cross product of two 3D points (vectors).
    // Returns a new vector perpendicular to both input vectors, following the right-hand rule.
    // Useful for finding normals or the area of a parallelogram defined by the vectors.
    // Usage: KoreXYZVector normal = KoreXYZVectorOps.CrossProduct(a, b);
    public static KoreXYZVector CrossProduct(KoreXYZVector a, KoreXYZVector b)
    {
        double x = (a.Y * b.Z) - (a.Z * b.Y);
        double y = (a.Z * b.X) - (a.X * b.Z);
        double z = (a.X * b.Y) - (a.Y * b.X);

        return new KoreXYZVector(x, y, z);
    }

    // --------------------------------------------------------------------------------------------

    // Returns the angle in radians between two 3D points (vectors) from the origin.
    // Uses the dot product and magnitudes to compute the cosine of the angle, then clamps for safety.
    // Calculates the angle (in radians) between two 3D vectors from the origin.
    // Uses the dot product and magnitudes to compute the angle via arccosine.
    // Handles edge cases to avoid NaN from floating-point rounding errors.
    public static double AngleBetweenRads(KoreXYZVector a, KoreXYZVector b)
    {
        double dotProduct = DotProduct(a, b);
        double magA = a.Magnitude;
        double magB = b.Magnitude;

        double cosAngle = dotProduct / (magA * magB);

        // Handle edge cases
        if (cosAngle > 1.0)
            cosAngle = 1.0;
        else if (cosAngle < -1.0)
            cosAngle = -1.0;

        return Math.Acos(cosAngle);
    }

    // Get the angle between lines origin-A and origin-B
    public static double AngleBetweenRads(KoreXYZVector origin, KoreXYZVector a, KoreXYZVector b)
    {
        KoreXYZVector vecA = a - origin;
        KoreXYZVector vecB = b - origin;

        return AngleBetweenRads(vecA, vecB);
    }

    // --------------------------------------------------------------------------------------------

    // usage: KoreXYZVector a = new KoreXYZVector(1, 2, 3);
    //        KoreXYZVector c = KoreXYZVectorOps.OffsetAzEl(a, 10, KoreValueUtils.DegsToRads(45), KoreValueUtils.DegsToRads(30));

    // Returns a new point offset from the origin by a given radius, azimuth, and elevation (in radians).
    // Converts spherical coordinates to Cartesian, then adds to the origin.
    // Useful for generating points on a sphere or in polar coordinate systems.
    public static KoreXYZVector OffsetAzEl(KoreXYZVector origin, double radius, double azimuthRads, double elevationRads)
    {
        double x = origin.X + radius * Math.Cos(azimuthRads) * Math.Cos(elevationRads);
        double y = origin.Y + radius * Math.Sin(azimuthRads) * Math.Cos(elevationRads);
        double z = origin.Z + radius * Math.Sin(elevationRads);

        return new KoreXYZVector(x, y, z);
    }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreXYZVector c = KoreXYZVectorOps.Lerp(from, to, 0.1);

    // Linearly interpolates between two 3D points by a given fraction (0.0 to 1.0).
    // Returns a point along the straight line from 'fromPoint' to 'toPoint'.
    // Handles edge cases for fraction <= 0 or >= 1.
    public static KoreXYZVector Lerp(KoreXYZVector fromPoint, KoreXYZVector toPoint, double fraction)
    {
        // Handle edge cases
        if (fraction <= 0.0) return fromPoint;
        if (fraction >= 1.0) return toPoint;

        // Calculate the interpolated point
        double x = fromPoint.X + (toPoint.X - fromPoint.X) * fraction;
        double y = fromPoint.Y + (toPoint.Y - fromPoint.Y) * fraction;
        double z = fromPoint.Z + (toPoint.Z - fromPoint.Z) * fraction;

        return new KoreXYZVector(x, y, z);
    }

    // --------------------------------------------------------------------------------------------

    // Spherically interpolates between two 3D points on a unit sphere by a given fraction.
    // Produces smooth interpolation along the surface of the sphere (great arc), not a straight line.
    // Normalizes inputs, computes the angle, and blends using sine-based weights.
    public static KoreXYZVector Slerp(KoreXYZVector a, KoreXYZVector b, double fraction)
    {
        // Handle edge cases
        if (fraction <= 0.0) return a;
        if (fraction >= 1.0) return b;

        // Normalize input points (assuming they are not already normalized)
        a = a.Normalize();
        b = b.Normalize();

        // Calculate the angle between the two points
        double angle = AngleBetweenRads(a, b);

        if (Math.Abs(angle) < KoreConsts.ArbitrarySmallDouble)
            return a;  // The points are colinear

        // Compute the Slerp
        double sinAngle = Math.Sin(angle);
        double sinFractionAngle = Math.Sin(fraction * angle);
        double sinComplementAngle = Math.Sin((1 - fraction) * angle);

        // Calculate the interpolated point
        KoreXYZVector interpolated = (a * sinComplementAngle + b * sinFractionAngle) / sinAngle;

        return interpolated;
    }

    // --------------------------------------------------------------------------------------------

    // Computes an inset point at vertex B, offset along the angle bisector of segments AB and BC by distance t.
    // Useful for generating smoothed or beveled corners in 3D geometry.
    // Handles all three dimensions for accurate 3D insetting.
    // Usage: KoreXYZVector insetPoint = KoreXYZVectorOps.InsetPoint(a, b, c, insetDistance);
    public static KoreXYZVector InsetPoint(KoreXYZVector a, KoreXYZVector b, KoreXYZVector c, double t)
    {
        // Calculate direction vectors for AB and BC, including the Z dimension
        double dxAB = b.X - a.X;
        double dyAB = b.Y - a.Y;
        double dzAB = b.Z - a.Z; // Include Z dimension
        double dxBC = c.X - b.X;
        double dyBC = c.Y - b.Y;
        double dzBC = c.Z - b.Z; // Include Z dimension

        // Normalize direction vectors
        double magAB = Math.Sqrt(dxAB * dxAB + dyAB * dyAB + dzAB * dzAB); // Include Z dimension
        double magBC = Math.Sqrt(dxBC * dxBC + dyBC * dyBC + dzBC * dzBC); // Include Z dimension
        dxAB /= magAB;
        dyAB /= magAB;
        dzAB /= magAB; // Normalize Z component
        dxBC /= magBC;
        dyBC /= magBC;
        dzBC /= magBC; // Normalize Z component

        // Calculate bisector vector, including the Z dimension
        double bisectorX = dxAB + dxBC;
        double bisectorY = dyAB + dyBC;
        double bisectorZ = dzAB + dzBC; // Include Z dimension
        double magBisector = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY + bisectorZ * bisectorZ); // Include Z dimension
        bisectorX /= magBisector;
        bisectorY /= magBisector;
        bisectorZ /= magBisector; // Normalize Z component

        // Calculate inset point along the bisector, including the Z dimension
        double xInset = b.X + t * bisectorX;
        double yInset = b.Y + t * bisectorY;
        double zInset = b.Z + t * bisectorZ; // Include Z dimension

        return new KoreXYZVector(xInset, yInset, zInset);
    }

    // --------------------------------------------------------------------------------------------

    // Rotates a 3D point around an arbitrary axis by a given angle (in radians).
    // Uses Rodrigues' rotation formula to compute the rotated coordinates.
    // Axis must be nonzero; function normalizes it internally.
    public static KoreXYZVector RotateAboutAxis(KoreXYZVector point, KoreXYZVector axis, double angleRads)
    {
        // Normalize the axis vector
        double axisLength = Math.Sqrt(axis.X * axis.X + axis.Y * axis.Y + axis.Z * axis.Z);
        if (axisLength == 0) throw new ArgumentException("Axis cannot be a zero vector.", nameof(axis));
        double axisX = axis.X / axisLength;
        double axisY = axis.Y / axisLength;
        double axisZ = axis.Z / axisLength;

        // Compute the cosine and sine of the angle
        double cosAngle = Math.Cos(angleRads);
        double sinAngle = Math.Sin(angleRads);

        // Compute the rotated coordinates
        double newX = (cosAngle + (1 - cosAngle) * axisX * axisX) * point.X
                    + ((1 - cosAngle) * axisX * axisY - axisZ * sinAngle) * point.Y
                    + ((1 - cosAngle) * axisX * axisZ + axisY * sinAngle) * point.Z;

        double newY = ((1 - cosAngle) * axisY * axisX + axisZ * sinAngle) * point.X
                    + (cosAngle + (1 - cosAngle) * axisY * axisY) * point.Y
                    + ((1 - cosAngle) * axisY * axisZ - axisX * sinAngle) * point.Z;

        double newZ = ((1 - cosAngle) * axisZ * axisX - axisY * sinAngle) * point.X
                    + ((1 - cosAngle) * axisZ * axisY + axisX * sinAngle) * point.Y
                    + (cosAngle + (1 - cosAngle) * axisZ * axisZ) * point.Z;

        return new KoreXYZVector(newX, newY, newZ);
    }

    // --------------------------------------------------------------------------------------------

    // Computes a point on a circle lying in an arbitrary plane in 3D space.
    // The plane is defined by its normal and a reference direction (zero angle), with the circle centered at 'origin'.
    // Returns the point at the given radius and angle (in radians) from the reference direction.

    public static KoreXYZVector PointOnCirclePlane(
        KoreXYZVector origin,
        KoreXYZVector normal,         // Plane normal
        KoreXYZVector reference,      // Zero-angle direction in the plane (must be perpendicular to normal)
        double radius,
        double angleRads)
    {
        var n = normal.Normalize();
        var u = reference.Normalize(); // Zero-angle direction
        var v = KoreXYZVector.CrossProduct(n, u).Normalize(); // 90° direction

        var offset =
            (u * Math.Cos(angleRads) + v * Math.Sin(angleRads)) * radius;

        return origin + offset;
    }

    // --------------------------------------------------------------------------------------------

    public static bool EqualsWithinTolerance(KoreXYZVector a, KoreXYZVector b, double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        return KoreValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance)
            && KoreValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance)
            && KoreValueUtils.EqualsWithinTolerance(a.Z, b.Z, tolerance);
    }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreXYZVector avg = KoreXYZVectorOps.Average(listOfPoints);
    //        KoreXYZVector avg = KoreXYZVectorOps.Average(new List<KoreXYZVector> { p1, p2, p3 });

    public static KoreXYZVector Average(List<KoreXYZVector> points)
    {
        if (points == null || points.Count == 0)
            throw new ArgumentException("Point list cannot be null or empty.", nameof(points));

        double sumX = 0;
        double sumY = 0;
        double sumZ = 0;

        foreach (var point in points)
        {
            sumX += point.X;
            sumY += point.Y;
            sumZ += point.Z;
        }

        int count = points.Count;
        return new KoreXYZVector(sumX / count, sumY / count, sumZ / count);
    }


}
