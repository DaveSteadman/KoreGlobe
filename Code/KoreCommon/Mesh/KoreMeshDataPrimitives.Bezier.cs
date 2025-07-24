using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Godot;

namespace KoreCommon;

// File of static functions to create KoreMeshData primitives around Bezier curves
// - See KoreNumeric1DArrayOps for Bezier calculations

public static partial class KoreMeshDataPrimitives
{
    // Bezier3Line: Creates a line (not triangles) from three XYZ vectors, a color, and a number of points/divisions.

    public static KoreMeshData Bezier3Line(
        KoreXYZVector p1, KoreXYZVector p2, KoreXYZVector p3, KoreColorRGB? lineColor = null, int divisions = 10)
    {

        KoreNumeric1DArray<double> xValues = new KoreNumeric1DArray<double>(3);
        KoreNumeric1DArray<double> yValues = new KoreNumeric1DArray<double>(3);
        KoreNumeric1DArray<double> zValues = new KoreNumeric1DArray<double>(3);

        xValues[0] = p1.X; xValues[1] = p2.X; xValues[2] = p3.X;
        yValues[0] = p1.Y; yValues[1] = p2.Y; yValues[2] = p3.Y;
        zValues[0] = p1.Z; zValues[1] = p2.Z; zValues[2] = p3.Z;

        KoreNumeric1DArray<double> fractionList = KoreNumeric1DArrayOps<double>.ListForRange(0, 1, divisions);

        // debug print the fractions list
        string fractionsStr = string.Join(", ", fractionList);
        GD.Print($"Fraction List: {fractionsStr}");


        List<int> pointIds = new List<int>();

        KoreMeshData newMesh = new KoreMeshData();

        foreach (double currVal in fractionList)
        {
            double currX = KoreNumeric1DArrayOps<double>.CalculateBezierPoint(currVal, xValues);
            double currY = KoreNumeric1DArrayOps<double>.CalculateBezierPoint(currVal, yValues);
            double currZ = KoreNumeric1DArrayOps<double>.CalculateBezierPoint(currVal, zValues);

            KoreXYZVector newPoint = new KoreXYZVector(currX, currY, currZ);

            int currPointId = newMesh.AddVertex(newPoint, null, null, null);
            pointIds.Add(currPointId);
        }


        // Loop through all the points, adding them and making lines.
        for (int i = 0; i < pointIds.Count - 1; i++)
        {
            int startId = pointIds[i];
            int endId = pointIds[i + 1];
            newMesh.AddLine(startId, endId, lineColor);
        }

        KoreColorRGB debugCol = KoreColorPalette.Colors["Red"];

        // Add dotted lines to debug the control points
        newMesh.AddDottedLineByDistance(p1, p2, debugCol, 0.1);
        newMesh.AddDottedLineByDistance(p2, p3, debugCol, 0.1);

        return newMesh;
    }
}
