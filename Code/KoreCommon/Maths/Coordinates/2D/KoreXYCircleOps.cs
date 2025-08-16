using System;
using System.Collections.Generic;

namespace KoreCommon;

public static class KoreXYCircleOps
{
    public static double ClosestDistanceTo(KoreXYCircle circle, KoreXYVector xy)
    {
        return circle.Center.DistanceTo(xy) - circle.Radius;
    }

    public static double ClosestDistanceTo(KoreXYCircle circle, KoreXYLine line)
    {
        double closestDistance = double.MaxValue;

        List<KoreXYVector> intersectionPoints = IntersectionPoints(circle, line);

        foreach (KoreXYVector intersectionPoint in intersectionPoints)
        {
            double distance = ClosestDistanceTo(circle, intersectionPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        return closestDistance;
    }

    // --------------------------------------------------------------------------------------------
    // Circumference points
    // --------------------------------------------------------------------------------------------

    public static KoreXYVector PointAtAngleRads(KoreXYCircle circle, double angleRads)
    {
        double x = circle.Center.X + circle.Radius * Math.Cos(angleRads);
        double y = circle.Center.Y + circle.Radius * Math.Sin(angleRads);
        return new KoreXYVector(x, y);
    }

    public static KoreXYPolyLine ToPolyLine(KoreXYCircle circle, int numPoints)
    {
        List<KoreXYVector> points = new List<KoreXYVector>();

        double angleIncrement = 2 * Math.PI / numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            double angle = i * angleIncrement;
            points.Add(PointAtAngleRads(circle, angle));
        }

        return new KoreXYPolyLine(points);
    }

    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    public static bool DoesLineIntersect(KoreXYCircle circle, KoreXYLine line)
    {
        return KoreXYCircleOps.ClosestDistanceTo(circle, line) <= circle.Radius;
    }

    public static List<KoreXYVector> IntersectionPoints(KoreXYCircle circle, KoreXYLine line)
    {
        List<KoreXYVector> intersectionPoints = new List<KoreXYVector>();

        double x1 = line.P1.X - circle.Center.X; // Translate line to circle origin
        double y1 = line.P1.Y - circle.Center.Y;
        double x2 = line.P2.X - circle.Center.X;
        double y2 = line.P2.Y - circle.Center.Y;
        double dx = x2 - x1;
        double dy = y2 - y1;
        double dr = Math.Sqrt(dx * dx + dy * dy);
        double D = x1 * y2 - x2 * y1;

        double discriminant = circle.Radius * circle.Radius * dr * dr - D * D;

        if (discriminant < 0)
        {
            return intersectionPoints; // No intersection
        }

        // Calculate intersection points
        double sqrtDiscriminant = Math.Sqrt(discriminant);
        double signDy = dy < 0 ? -1 : 1;

        double xIntersect1 = (D * dy + signDy * dx * sqrtDiscriminant) / (dr * dr) + circle.Center.X;
        double yIntersect1 = (-D * dx + Math.Abs(dy) * sqrtDiscriminant) / (dr * dr) + circle.Center.Y;

        KoreXYVector p1 = new(xIntersect1, yIntersect1);
        if (KoreXYLineOps.IsPointOnLine(line, p1))
            intersectionPoints.Add(p1);

        if (discriminant != 0) // Two intersections
        {
            double xIntersect2 = (D * dy - signDy * dx * sqrtDiscriminant) / (dr * dr) + circle.Center.X;
            double yIntersect2 = (-D * dx - Math.Abs(dy) * sqrtDiscriminant) / (dr * dr) + circle.Center.Y;

            KoreXYVector p2 = new(xIntersect2, yIntersect2);
            if (KoreXYLineOps.IsPointOnLine(line, p2))
                intersectionPoints.Add(p2);
        }

        return intersectionPoints;
    }

    public static List<KoreXYVector> IntersectionPoints(KoreXYCircle circle1, KoreXYCircle circle2)
    {
        List<KoreXYVector> intersectionPoints = new List<KoreXYVector>();

        double x0 = circle1.Center.X;
        double y0 = circle1.Center.Y;
        double r0 = circle1.Radius;
        double x1 = circle2.Center.X;
        double y1 = circle2.Center.Y;
        double r1 = circle2.Radius;

        double dx = x1 - x0;
        double dy = y1 - y0;
        double d = Math.Sqrt(dx * dx + dy * dy);

        // Check if circles do not intersect or one circle contains the other
        if (d > r0 + r1 || d < Math.Abs(r0 - r1) || (d == 0 && r0 == r1))
        {
            return intersectionPoints;
        }

        double a = (r0 * r0 - r1 * r1 + d * d) / (2 * d);
        double h = Math.Sqrt(r0 * r0 - a * a);

        double x2 = x0 + a * dx / d;
        double y2 = y0 + a * dy / d;

        double x3 = x2 + h * dy / d;
        double y3 = y2 - h * dx / d;
        intersectionPoints.Add(new KoreXYVector(x3, y3));

        // Check for the case when circles intersect at two points
        if (d != r0 + r1)
        {
            double x4 = x2 - h * dy / d;
            double y4 = y2 + h * dx / d;
            intersectionPoints.Add(new KoreXYVector(x4, y4));
        }

        return intersectionPoints;
    }


    // --------------------------------------------------------------------------------------------
    // Tangent points
    // --------------------------------------------------------------------------------------------

    // Return the list of (most of the time) two tangent points from a point to a circle. Will be zero
    // if the point is insider the circle.
    public static List<KoreXYVector> TangentPoints(KoreXYCircle circle, KoreXYVector pos)
    {
        List<KoreXYVector> tangentPoints = new List<KoreXYVector>();

        double x1 = circle.Center.X;
        double y1 = circle.Center.Y;
        double x2 = pos.X;
        double y2 = pos.Y;
        double r = circle.Radius;

        double dx = x2 - x1;
        double dy = y2 - y1;
        double dr = Math.Sqrt(dx * dx + dy * dy);

        if (dr < r)
        {
            // Point is inside the circle, no tangent points
            return tangentPoints;
        }

        double theta = Math.Atan2(dy, dx);
        double phi = Math.Acos(r / dr);

        double x3 = x1 + r * Math.Cos(theta + phi);
        double y3 = y1 + r * Math.Sin(theta + phi);

        double x4 = x1 + r * Math.Cos(theta - phi);
        double y4 = y1 + r * Math.Sin(theta - phi);

        tangentPoints.Add(new KoreXYVector(x3, y3));
        tangentPoints.Add(new KoreXYVector(x4, y4));

        return tangentPoints;
    }

}