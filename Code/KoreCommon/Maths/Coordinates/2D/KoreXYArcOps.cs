using System;
using System.Collections.Generic;

namespace KoreCommon;

public static class KoreXYArcOps
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    public static List<KoreXYPoint> IntersectionPoints(KoreXYArc arc, KoreXYLine line)
    {
        List<KoreXYPoint> intersectionPoints = new List<KoreXYPoint>();

        // Use the new ToCircle method
        KoreXYCircle circle = arc.Circle;

        // Assume IntersectionPoints(KoreXYCircle, KoreXYLine) is implemented elsewhere
        List<KoreXYPoint> circleIntersections = KoreXYCircleOps.IntersectionPoints(circle, line);

        foreach (var point in circleIntersections)
        {
            double angleToPoint = arc.Center.AngleToRads(point);

            // Use the new IsAngleInRange method
            if (arc.ContainsAngle(angleToPoint))
                intersectionPoints.Add(point);
        }

        return intersectionPoints;
    }

    // --------------------------------------------------------------------------------------------
    // Conversions
    // --------------------------------------------------------------------------------------------

    public static KoreXYPoint PointAtFraction(KoreXYArc arc, double fraction)
    {
        double angle = arc.StartAngleRads + (arc.DeltaAngleRads * fraction);
        return KoreXYPointOps.OffsetPolar(arc.Center, arc.Radius, angle);
    }

    public static KoreXYPolyLine ToPolyLine(KoreXYArc arc, int numPoints)
    {
        List<KoreXYPoint> points = new List<KoreXYPoint>();

        double startAngle = arc.StartAngleRads;
        double endAngle = arc.EndAngleRads;
        double angleIncrement = (endAngle - startAngle) / (numPoints - 1);

        for (int i = 0; i < numPoints; i++)
        {
            double angle = startAngle + (i * angleIncrement);
            points.Add(KoreXYPointOps.OffsetPolar(arc.Center, arc.Radius, angle));
        }

        return new KoreXYPolyLine(points);
    }

}