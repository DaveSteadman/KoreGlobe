// <fileheader>

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public static partial class KoreMiniMeshOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Circle Points
    // --------------------------------------------------------------------------------------------

    // Function to create a circle of points, returning the list of Ids

    public static List<int> AddCirclePoints(
        KoreMiniMesh mesh,
        KoreXYZVector center, 
        KoreXYZVector normal, 
        double radius, 
        int numSides,
        KoreXYZVector? referenceDirection = null)
    {
        if (numSides < 3) throw new ArgumentException("Circle must have at least 3 sides");
        
        List<int> pointIds = new List<int>();

        // Create a plane for the circle using KoreXYZPlane
        KoreXYZPlane plane;

        if (referenceDirection.HasValue)
        {
            // Use provided reference direction as the plane's Y-axis
            plane = KoreXYZPlane.MakePlane(center, normal, referenceDirection.Value);
        }
        else
        {
            // Use automatic reference direction selection
            KoreXYZVector autoReference = FindPerpendicularVector(normal.Normalize());
            plane = KoreXYZPlane.MakePlane(center, normal, autoReference);
        }

        // Generate circle points using the plane's 2D->3D projection
        double angleStep = Math.Tau / numSides;

        for (int i = 0; i < numSides; i++)
        {
            double angle = i * angleStep;

            // Create 2D point in the plane's coordinate system
            var point2D = new KoreXYVector(
                radius * Math.Cos(angle),
                radius * Math.Sin(angle)
            );

            // Project to 3D using the plane
            KoreXYZVector point3D = plane.Project2DTo3D(point2D);

            int vertexId = mesh.AddVertex(point3D);
            pointIds.Add(vertexId);
        }

        return pointIds;
    }

    /// <summary>
    /// Find a vector perpendicular to the given vector using a consistent strategy
    /// </summary>
    private static KoreXYZVector FindPerpendicularVector(KoreXYZVector vector)
    {
        // Strategy: Try standard basis vectors and pick the one that's most perpendicular
        KoreXYZVector[] candidates = { KoreXYZVector.Right, KoreXYZVector.Up, KoreXYZVector.Forward };

        double minDot = double.MaxValue;
        KoreXYZVector bestCandidate = KoreXYZVector.Right;

        foreach (var candidate in candidates)
        {
            double dot = Math.Abs(KoreXYZVector.DotProduct(vector, candidate));
            if (dot < minDot)
            {
                minDot = dot;
                bestCandidate = candidate;
            }
        }

        // Return the most perpendicular candidate (will be made orthogonal by MakePlane)
        return bestCandidate;
    }



    // --------------------------------------------------------------------------------------------
    // MARK: #
    // --------------------------------------------------------------------------------------------


}
