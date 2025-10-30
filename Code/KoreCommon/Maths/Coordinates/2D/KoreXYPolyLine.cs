// <fileheader>

using System;
using System.Collections.Generic;

namespace KoreCommon;

public class KoreXYPolyLine
{
    public List<KoreXYVector> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      KoreXYPolyLine polyLine = new KoreXYPolyLine (new List<KoreXYVector> {
    //          new KoreXYVector(0, 0),
    //          new KoreXYVector(1, 1),
    //          new KoreXYVector(2, 2) });

    public KoreXYPolyLine(List<KoreXYVector> points)
    {
        Points = points;
    }

    public KoreXYPolyLine(KoreXYPolyLine polyLine)
    {
        Points = new List<KoreXYVector>(polyLine.Points);
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public double Length()
    {
        double length = 0;
        for (int i = 0; i < Points.Count - 1; i++)
        {
            length += Points[i].DistanceTo(Points[i + 1]);
        }
        return length;
    }

    // --------------------------------------------------------------------------------------------
    // Sublines
    // --------------------------------------------------------------------------------------------

    public int NumSubLines()
    {
        return Points.Count - 1;
    }

    public KoreXYLine SubLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= NumSubLines())
            throw new ArgumentOutOfRangeException("lineIndex", "lineIndex must be between 0 and NumSubLines() - 1");

        return new KoreXYLine(Points[lineIndex], Points[lineIndex + 1]);
    }

    // return a list of line objects, for a caller to then handle separately

    public List<KoreXYLine> SubLines()
    {
        List<KoreXYLine> lines = new List<KoreXYLine>();
        for (int i = 0; i < NumSubLines(); i++)
        {
            lines.Add(SubLine(i));
        }
        return lines;
    }

    // --------------------------------------------------------------------------------------------
    // Edits
    // --------------------------------------------------------------------------------------------

    public KoreXYPolyLine Offset(double x, double y)
    {
        List<KoreXYVector> newPoints = new List<KoreXYVector>();
        foreach (KoreXYVector point in Points)
        {
            newPoints.Add(point.Offset(x, y));
        }
        return new KoreXYPolyLine(newPoints);
    }

    public KoreXYPolyLine Offset(KoreXYVector xy)
    {
        List<KoreXYVector> newPoints = new List<KoreXYVector>();
        foreach (KoreXYVector point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new KoreXYPolyLine(newPoints);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utilities
    // --------------------------------------------------------------------------------------------

    public KoreXYRect AABB()
    {
        if (Points.Count == 0) return KoreXYRect.Zero;

        double minX = Points[0].X;
        double maxX = Points[0].X;
        double minY = Points[0].Y;
        double maxY = Points[0].Y;

        foreach (var point in Points)
        {
            if (point.X < minX) minX = point.X;
            if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.Y > maxY) maxY = point.Y;
        }

        return new KoreXYRect(minX, minY, maxX, maxY);
    }
}