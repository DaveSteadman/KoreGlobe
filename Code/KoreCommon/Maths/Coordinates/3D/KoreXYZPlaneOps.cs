// <fileheader>

using System;

#nullable enable

namespace KoreCommon;

public static class KoreXYZPlaneOps
{
    // Check if three points are colinear
    public static bool PointsCollinear(KoreXYZVector p1, KoreXYZVector p2, KoreXYZVector p3)
    {
        // Validate inputs to avoid undefined lines (e.g., same start and end points)
        if (p1.Equals(p2) || p2.Equals(p3) || p1.Equals(p3))
        {
            throw new ArgumentException("Points must be distinct to determine colinearity.");
        }

        // Create two lines, then check if they are parallel
        var line1 = new KoreXYZLine(p1, p2);
        var line2 = new KoreXYZLine(p1, p3);

        return KoreXYZLineOps.IsParallel(line1, line2);
    }

    // Find the intersection point of a line with a plane, if it exists
    // public static KoreXYZVector? IntersectionWithLine(KoreXYZPlane plane, KoreXYZLine line)
    // {
    //     KoreXYZVector planeNormal = plane.VecNormal;

    //     // Calculate the direction vector of the line
    //     KoreXYZVector lineDir = line.P1.VectorTo(line.P2);

    //     // Calculate the dot product of the line direction and the plane normal
    //     double dotProduct = KoreXYZVector.DotProduct(planeNormal, lineDir);

    //     // Check if the line is parallel to the plane
    //     if (Math.Abs(dotProduct) < KoreConsts.ArbitrarySmallDouble)
    //     {
    //         // Special case handling: Check if the line lies in the plane
    //         if (Math.Abs(KoreXYZVector.DotProduct(planeNormal, plane.PntOrigin.VectorTo(line.P1))) < KoreConsts.ArbitrarySmallDouble)
    //         {
    //             // The line lies in the plane
    //             return line.P1; // or any point on the line
    //         }

    //         return null; // No intersection, line is parallel to the plane
    //     }

    //     // Calculate the parameter t at the intersection point
    //     double t = KoreXYZVectorOps.DotProduct(planeNormal, KoreXYZVector.Diff(plane.P1, line.P1)) / dotProduct;

    //     // Calculate and return the intersection point
    //     return KoreXYZVector.Sum(line.P1, KoreXYZVector.Scale(lineDir, t));
    // }
}
