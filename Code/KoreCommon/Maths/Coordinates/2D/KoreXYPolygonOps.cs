using System;

#nullable enable

namespace KoreCommon;

public static class KoreXYPolygonOps
{
    // public static KoreXYLine? ClipLineToPolygon(this KoreXYLine line, KoreXYPolygon polygon)
    // {
    //     KoreXYLine? clippedLine = null;
    //     KoreXYVector? p1 = null;
    //     KoreXYVector? p2 = null;

    //     for (int i = 0; i < polygon.Vertices.Count; i++)
    //     {
    //         // Check if the line intersects with the polygon edge
    //         KoreXYLine curredge = new KoreXYLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.Vertices.Count]);

    //         if (KoreXYLineOps.TryIntersect(line, curredge, out KoreXYVector intersection))
    //         {
    //             // If we found an intersection, we can use it
    //             if (p1 == null)
    //             {
    //                 p1 = intersection;
    //             }
    //             else if (p2 == null)
    //             {
    //                 p2 = intersection;
    //             }
    //             else
    //             {
    //                 if (p1.DistanceTo(line.P1) > p2.DistanceTo(line.P1))
    //                 {
    //                     p1 = p2;
    //                     p2 = intersection;
    //                 }
    //                 else
    //                 {
    //                     p2 = intersection;
    //                 }
    //             }
    //         }
    //     }

    //     if (p1 != null && p2 != null)
    //     {
    //         clippedLine = new KoreXYLine(p1, p2);
    //     }

    //     return clippedLine;
    // }
}