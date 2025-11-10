// <fileheader>

using System;
using System.Collections.Generic;

namespace KoreCommon;

// Represents an immutable 2D polygon on a Kore. The polygon is considered closed,
// with an implicit final line connecting the last and first points. There's no need
// to repeat the first point at the end of the vertices list.
public struct KoreXYPolygon
{
    public IReadOnlyList<KoreXYVector> Vertices { get; }

    public KoreXYPolygon(IEnumerable<KoreXYVector> vertices)
    {
        Vertices = new List<KoreXYVector>(vertices).AsReadOnly();
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
        List<KoreXYVector> newVertices = new List<KoreXYVector>();
        foreach (KoreXYVector vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(x, y));
        }
        return new KoreXYPolygon(newVertices);
    }

    // offset each point in the polygon by the given vector

    public KoreXYPolygon Offset(KoreXYVector xy)
    {
        List<KoreXYVector> newVertices = new List<KoreXYVector>();
        foreach (KoreXYVector vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(xy));
        }
        return new KoreXYPolygon(newVertices);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utilities
    // --------------------------------------------------------------------------------------------

    public KoreXYRect AABB()
    {
        if (Vertices.Count == 0) return KoreXYRect.Zero;

        double minX = Vertices[0].X;
        double maxX = Vertices[0].X;
        double minY = Vertices[0].Y;
        double maxY = Vertices[0].Y;

        foreach (var vertex in Vertices)
        {
            if (vertex.X < minX) minX = vertex.X;
            if (vertex.X > maxX) maxX = vertex.X;
            if (vertex.Y < minY) minY = vertex.Y;
            if (vertex.Y > maxY) maxY = vertex.Y;
        }

        return new KoreXYRect(minX, minY, maxX, maxY);
    }
}
