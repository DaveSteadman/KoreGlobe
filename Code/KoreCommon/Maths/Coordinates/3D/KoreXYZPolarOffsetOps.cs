// <fileheader>

using System;

// KoreXYZPolarOffsetOps: Common operations on KoreXYZPolarOffset objects, but outside of direct class responsibilities.

namespace KoreCommon;

public static class KoreXYZPolarOffsetOps
{
    // Deterine the angle between two 3D polar offsets
    public static double AngleDegsBetween(KoreXYZPolarOffset a, KoreXYZPolarOffset b)
    {
        // Convert both offsets to XYZ coordinates
        KoreXYZVector pointA = a.ToXYZ();
        KoreXYZVector pointB = b.ToXYZ();

        // Calculate the dot product of the two vectors
        double dotProduct = (pointA.X * pointB.X) + (pointA.Y * pointB.Y) + (pointA.Z * pointB.Z);

        // Calculate the magnitudes of the two vectors
        double magnitudeA = Math.Sqrt((pointA.X * pointA.X) + (pointA.Y * pointA.Y) + (pointA.Z * pointA.Z));
        double magnitudeB = Math.Sqrt((pointB.X * pointB.X) + (pointB.Y * pointB.Y) + (pointB.Z * pointB.Z));

        // Calculate the cosine of the angle
        double cosTheta = dotProduct / (magnitudeA * magnitudeB);

        // Ensure the cosine value is clamped between -1 and 1 to handle floating-point inaccuracies
        cosTheta = Math.Max(-1.0, Math.Min(1.0, cosTheta));

        // Calculate the angle in radians and convert to degrees
        double angleRads = Math.Acos(cosTheta);
        double angleDegs = angleRads * KoreConsts.RadsToDegsMultiplier;

        return angleDegs;
    }

    public static KoreXYZPolarOffset Lerp(KoreXYZPolarOffset fromOffset, KoreXYZPolarOffset toOffset, double fraction)
    {
        fraction = KoreValueUtils.LimitToRange(fraction, 0, 1);
        double invFraction = 1 - fraction;

        KoreXYZPolarOffset newOffset = new KoreXYZPolarOffset();
        newOffset.AzRads = (fromOffset.AzRads * invFraction) + (toOffset.AzRads * fraction);
        newOffset.ElRads = (fromOffset.ElRads * invFraction) + (toOffset.ElRads * fraction);
        newOffset.Range = (fromOffset.Range * invFraction) + (toOffset.Range * fraction);

        return newOffset;
    }
}
