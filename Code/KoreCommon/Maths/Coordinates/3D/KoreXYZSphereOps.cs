// <fileheader>

using System;

#nullable enable

namespace KoreCommon;

public static class KoreXYZSphereOps
{
    // Determine if a line intersects with the boundary of a sphere. A line entirely within the sphere is not considered an intersection.

    public static bool LineIntersectsSphere(
        KoreXYZLine line,
        KoreXYZSphere sphere,
        out KoreXYZVector? intersection1,
        out KoreXYZVector? intersection2)
    {
        intersection1 = null;
        intersection2 = null;

        // Compute line direction and the vector from sphere center to line start
        KoreXYZVector lineDir = line.DirectionVector;
        KoreXYZVector sphereToLineStart = line.P1.XYZTo(sphere.Center);

        // Calculate coefficients of the quadratic equation
        double a = KoreXYZVector.DotProduct(lineDir, lineDir);
        double b = 2 * KoreXYZVector.DotProduct(sphereToLineStart, lineDir);
        double c = KoreXYZVector.DotProduct(sphereToLineStart, sphereToLineStart) - (sphere.Radius * sphere.Radius);

        // Calculate the discriminant
        double discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No intersection
            return false;
        }
        else
        {
            // Calculate the two intersection t values
            double t1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double t2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

            // Find intersection points
            intersection1 = line.P1 + t1 * lineDir;
            intersection2 = (discriminant == 0) ? intersection1 : line.P1 + t2 * lineDir;

            return true;
        }
    }
}