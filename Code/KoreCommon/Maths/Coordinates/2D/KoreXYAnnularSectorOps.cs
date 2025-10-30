// <fileheader>

using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

public static class KoreXYAnnularSectorOps
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    // public static List<KoreXYVector> IntersectionPoints(KoreXYAnnularSector sector, KoreXYLine line)
    // {
    //     List<KoreXYVector> intersectionPoints = new List<KoreXYVector>();

    //     // Break up the shape into two arcs and two lines, then use the intersection methods for each
    //     List<KoreXYVector> innerArcIntersections = KoreXYArcOps.IntersectionPoints(sector.InnerArc, line);
    //     List<KoreXYVector> outerArcIntersections = KoreXYArcOps.IntersectionPoints(sector.OuterArc, line);
    //     KoreXYVector? startIntersection          = KoreXYLineOps.Intersection(sector.StartInnerOuterLine, line);
    //     KoreXYVector? endIntersection            = KoreXYLineOps.Intersection(sector.EndInnerOuterLine, line);

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
