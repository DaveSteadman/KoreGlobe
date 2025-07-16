using System;
using System.Collections.Generic;

namespace KoreCommon;

/// <summary>
/// Represents an immutable 2D polygon on a Kore. The polygon is considered closed,
/// with an implicit final line connecting the last and first points. There's no need
/// to repeat the first point at the end of the vertices list.
/// </summary>
public struct KoreXYPolygon
{
    public IReadOnlyList<KoreXYPoint> Vertices { get; }

    public KoreXYPolygon(IEnumerable<KoreXYPoint> vertices)
    {
        Vertices = new List<KoreXYPoint>(vertices).AsReadOnly();
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Determine the area of the polygon - for a simple non-intersecting case.

    public double Area()
    {
        double area = 0;
        for (int i = 0; i < Vertices.Count; i++)
        {
            int j = (i + 1) % Vertices.Count;
            area += Vertices[i].X * Vertices[j].Y - Vertices[j].X * Vertices[i].Y;
        }
        return Math.Abs(area / 2); // Absolute value for area
    }

    // offset each point by a given x y

    public KoreXYPolygon Offset(double x, double y)
    {
        List<KoreXYPoint> newVertices = new List<KoreXYPoint>();
        foreach (KoreXYPoint vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(x, y));
        }
        return new KoreXYPolygon(newVertices);
    }

    // offset each point in the polygon by the given vector

    public KoreXYPolygon Offset(KoreXYVector xy)
    {
        List<KoreXYPoint> newVertices = new List<KoreXYPoint>();
        foreach (KoreXYPoint vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(xy));
        }
        return new KoreXYPolygon(newVertices);
    }
}
