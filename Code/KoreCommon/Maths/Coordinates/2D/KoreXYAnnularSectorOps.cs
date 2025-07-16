using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

public static class KoreXYAnnularSectorOps
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    // public static List<KoreXYPoint> IntersectionPoints(KoreXYAnnularSector sector, KoreXYLine line)
    // {
    //     List<KoreXYPoint> intersectionPoints = new List<KoreXYPoint>();

    //     // Break up the shape into two arcs and two lines, then use the intersection methods for each
    //     List<KoreXYPoint> innerArcIntersections = KoreXYArcOps.IntersectionPoints(sector.InnerArc, line);
    //     List<KoreXYPoint> outerArcIntersections = KoreXYArcOps.IntersectionPoints(sector.OuterArc, line);
    //     KoreXYPoint? startIntersection          = KoreXYLineOps.Intersection(sector.StartInnerOuterLine, line);
    //     KoreXYPoint? endIntersection            = KoreXYLineOps.Intersection(sector.EndInnerOuterLine, line);

    //     // Consolidate the results into one list
    //     if (startIntersection != null)
    //         intersectionPoints.Add(startIntersection);
    //     if (endIntersection != null)
    //         intersectionPoints.Add(endIntersection);
    //     intersectionPoints.AddRange(innerArcIntersections);
    //     intersectionPoints.AddRange(outerArcIntersections);

    //     return intersectionPoints;
    // }


}
