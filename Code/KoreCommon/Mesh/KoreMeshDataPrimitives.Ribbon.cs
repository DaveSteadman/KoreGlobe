using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshDataPrimitives
{
    // Ribbon: Creates a ribbon mesh of points, wih the left and right sides from the perspect of looking down the ribbon fro the starting edge
    // The normals are generated automatically from the triangles.
    public static KoreMeshData Ribbon(
        List<KoreXYZVector> leftPoints, List<KoreXYVector> leftUVs,
        List<KoreXYZVector> rightPoints, List<KoreXYVector> rightUVs)
    {
        var mesh = new KoreMeshData();

        if (leftPoints.Count != rightPoints.Count || leftUVs.Count != rightUVs.Count)
            throw new ArgumentException("Left and right points/UVs must have the same count.");

        // Loop through the points list (-1), and use the current position and the next position to create two triangles at a time.
        for (int i = 0; i < leftPoints.Count - 1; i++)
        {
            // Get the current and next positions for both sides
            KoreXYZVector leftCurrent  = leftPoints[i];
            KoreXYZVector leftNext     = leftPoints[i + 1];
            KoreXYZVector rightCurrent = rightPoints[i];
            KoreXYZVector rightNext    = rightPoints[i + 1];

            // Get the UVs for both sides
            KoreXYVector leftUVCurrent  = leftUVs[i];
            KoreXYVector leftUVNext     = leftUVs[i + 1];
            KoreXYVector rightUVCurrent = rightUVs[i];
            KoreXYVector rightUVNext    = rightUVs[i + 1];

            int pntIdL0 = 0;
            int pntIdL1 = 0;
            int pntIdR0 = 0;
            int pntIdR1 = 0;

            // If we are on the first pass, add all the points, otherwise just add the +1 points
            if (i == 0)
            {
                pntIdL0 = mesh.AddVertex(leftCurrent, null, null, leftUVCurrent);
                pntIdR0 = mesh.AddVertex(rightCurrent, null, null, rightUVCurrent);
                pntIdL1 = mesh.AddVertex(leftNext, null, null, leftUVNext);
                pntIdR1 = mesh.AddVertex(rightNext, null, null, rightUVNext);
            }
            else
            {
                pntIdL1 = mesh.AddVertex(leftNext, null, null, leftUVNext);
                pntIdR1 = mesh.AddVertex(rightNext, null, null, rightUVNext);
            }

            // Create two triangles (two faces) for the current segment
            mesh.AddTriangle(pntIdL0, pntIdR0, pntIdR1);
            mesh.AddTriangle(pntIdL0, pntIdR1, pntIdL1);

            // move the IDs back one step
            pntIdL0 = pntIdL1;
            pntIdR0 = pntIdR1;
        }

        // We've added the triangles okay, so we can now loop through them and auto-calculate normals
        mesh.SetNormalsFromTriangles();

        return mesh;
    }

}
