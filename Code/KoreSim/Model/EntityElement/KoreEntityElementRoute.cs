using System;
using System.Collections.Generic;

using KoreCommon;

public class KoreEntityElementRoute : KoreEntityElement
{
    // A simple route being a list of KoreLLAPoint values.
    public List<KoreLLAPoint> RoutePoints { set; get; } = new List<KoreLLAPoint>();

    public override string Type => "Route";

    // --------------------------------------------------------------------------------------------
    // MARK Report
    // --------------------------------------------------------------------------------------------

    public override string Report()
    {
        return $"Route: {RoutePoints.Count} points // Start:{RoutePoints[0]} // End:{RoutePoints[RoutePoints.Count - 1]}";
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Update
    // ---------------------------------------------------------------------------------------------

    public void AddPoint(KoreLLAPoint point)
    {
        RoutePoints.Add(point);
    }

    public void AddPoints(List<KoreLLAPoint> points)
    {
        RoutePoints.AddRange(points);
    }

    public void Clear() => RoutePoints.Clear();

    // ---------------------------------------------------------------------------------------------
    // MARK: Query
    // ---------------------------------------------------------------------------------------------

    public List<KoreLLAPoint> Points => RoutePoints;


    public int PointCount() => RoutePoints.Count;
    public KoreLLAPoint Point(int index) => RoutePoints[index];



}

