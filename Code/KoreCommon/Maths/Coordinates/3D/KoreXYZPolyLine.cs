using System;
using System.Collections.Generic;

namespace KoreCommon;

public class KoreXYZPolyLine
{
    public List<KoreXYZPoint> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      KoreXYPolyLine polyLine = new KoreXYPolyLine (new List<KoreXYPoint> {
    //          new KoreXYPoint(0, 0),
    //          new KoreXYPoint(1, 1),
    //          new KoreXYPoint(2, 2) });

    public KoreXYZPolyLine(List<KoreXYZPoint> points)
    {
        Points = points;
    }

    public KoreXYZPolyLine(KoreXYZPolyLine polyLine)
    {
        Points = new List<KoreXYZPoint>(polyLine.Points);
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

    public KoreXYZLine SubLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= NumSubLines())
            throw new ArgumentOutOfRangeException("lineIndex", "lineIndex must be between 0 and NumSubLines() - 1");

        return new KoreXYZLine(Points[lineIndex], Points[lineIndex + 1]);
    }

    // return a list of line objects, for a caller to then handle separately

    public List<KoreXYZLine> SubLines()
    {
        List<KoreXYZLine> lines = new List<KoreXYZLine>();
        for (int i = 0; i < NumSubLines(); i++)
        {
            lines.Add(SubLine(i));
        }
        return lines;
    }

    // --------------------------------------------------------------------------------------------
    // Edits
    // --------------------------------------------------------------------------------------------

    public KoreXYZPolyLine Offset(double x, double y, double z)
    {
        List<KoreXYZPoint> newPoints = new List<KoreXYZPoint>();
        foreach (KoreXYZPoint point in Points)
        {
            newPoints.Add(point.Offset(x, y, z));
        }
        return new KoreXYZPolyLine(newPoints);
    }

    public KoreXYZPolyLine Offset(KoreXYZPoint xy)
    {
        List<KoreXYZPoint> newPoints = new List<KoreXYZPoint>();
        foreach (KoreXYZPoint point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new KoreXYZPolyLine(newPoints);
    }

}