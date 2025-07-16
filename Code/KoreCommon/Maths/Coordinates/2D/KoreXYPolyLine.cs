using System;
using System.Collections.Generic;

namespace KoreCommon;

public class KoreXYPolyLine
{
    public List<KoreXYPoint> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      KoreXYPolyLine polyLine = new KoreXYPolyLine (new List<KoreXYPoint> {
    //          new KoreXYPoint(0, 0),
    //          new KoreXYPoint(1, 1),
    //          new KoreXYPoint(2, 2) });

    public KoreXYPolyLine(List<KoreXYPoint> points)
    {
        Points = points;
    }

    public KoreXYPolyLine(KoreXYPolyLine polyLine)
    {
        Points = new List<KoreXYPoint>(polyLine.Points);
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
        List<KoreXYPoint> newPoints = new List<KoreXYPoint>();
        foreach (KoreXYPoint point in Points)
        {
            newPoints.Add(point.Offset(x, y));
        }
        return new KoreXYPolyLine(newPoints);
    }

    public KoreXYPolyLine Offset(KoreXYVector xy)
    {
        List<KoreXYPoint> newPoints = new List<KoreXYPoint>();
        foreach (KoreXYPoint point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new KoreXYPolyLine(newPoints);
    }



}