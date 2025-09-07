// <fileheader>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Godot;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreColorMeshPrimitives
{
    // Create a sphere mesh for KoreColorMesh, with a given center, radius, and colormap
    // - We pick the number of lat/long segments from the color list dimensions
    // Usage: KoreColorMesh sphereMesh = KoreColorMeshPrimitives.SphereSection(center, azElRange, radius, colormap);

    public static KoreColorMesh SphereSection(
        KoreXYZVector center,
        KoreLLBox llBox,
        double radius,
        KoreColorRGB[,] colormap,
        KoreNumeric2DArray<float> tileEleData)
    {
        var mesh = new KoreColorMesh();

        int lonSegments = colormap.GetLength(1); // longitude segments (horizontal divisions)
        int latSegments = colormap.GetLength(0); // latitude segments (vertical divisions)

        // Create a simple double-list that includes poles as duplicated vertices
        var vertexIdGrid = new List<List<int>>();

        for (int lat = 0; lat <= latSegments; lat++) // Include both poles
        {
            // flip the lat, to go from top to bottom
            int usedLat = latSegments - lat;
            double latDegs = llBox.MinLatDegs + (llBox.DeltaLatDegs * usedLat / latSegments);
            float latFraction = (float)lat / latSegments;

            var latRow = new List<int>();

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                double lonDegs = llBox.MinLonDegs + (llBox.DeltaLonDegs * lon / lonSegments);
                float lonFraction = (float)lon / lonSegments;

                double ele = radius + tileEleData.InterpolatedValue(lonFraction, latFraction);

                //GD.Print($"lat: {latDegs:F2}, lon: {lonDegs:F2}, rad: {radius:F2}, ele: {tileEleData.InterpolatedValue(lonFraction, latFraction)}");

                KoreLLAPoint rwLLAPointPos = new KoreLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, RadiusM = ele };
                KoreXYZVector rwXYZPointPos = rwLLAPointPos.ToXYZ();

                // ---- convert from real-world to game engine ----
                rwXYZPointPos = new KoreXYZVector(rwXYZPointPos.X, rwXYZPointPos.Y, -rwXYZPointPos.Z);

                KoreXYZVector vertex = rwXYZPointPos;
                int vertexId = mesh.AddVertex(vertex);
                latRow.Add(vertexId);
            }

            vertexIdGrid.Add(latRow);
        }

        // Create triangles between all adjacent latitude rows
        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                // Get the four vertices of the current quad
                int v1 = vertexIdGrid[lat][lon];         // current lat, current lon
                int v2 = vertexIdGrid[lat + 1][lon];     // next lat, current lon
                int v3 = vertexIdGrid[lat + 1][lon + 1]; // next lat, next lon
                int v4 = vertexIdGrid[lat][lon + 1];     // current lat, next lon

                // Use colormap coordinates
                KoreColorRGB col = colormap[lat % colormap.GetLength(0), lon % colormap.GetLength(1)];

                // Add the quad as two triangles using AddFace helper
                KoreColorMeshOps.AddFace(mesh, v1, v4, v3, v2, col);
            }
        }

        return mesh;
    }

    // --------------------------------------------------------------------------------------------

    public static KoreColorMesh CenteredSphereSection(
        KoreLLBox llBox,
        double radius,
        KoreColorRGB[,] colormap,
        KoreNumeric2DArray<float> tileEleData)
    {
        var mesh = new KoreColorMesh();

        int lonSegments = colormap.GetLength(1); // longitude segments (horizontal divisions)
        int latSegments = colormap.GetLength(0); // latitude segments (vertical divisions)

        // define the center point in real-world coordinates
        KoreLLAPoint rwLLAZeroLonCenter = new KoreLLAPoint() { LatDegs = llBox.MidLatDegs, LonDegs = llBox.MidLonDegs, RadiusM = radius };
        KoreXYZVector rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Create a simple double-list that includes poles as duplicated vertices
        var vertexIdGrid = new List<List<int>>();

        for (int lat = 0; lat <= latSegments; lat++) // Include both poles
        {
            // flip the lat, to go from top to bottom
            int usedLat = latSegments - lat;
            double latDegs = llBox.MinLatDegs + (llBox.DeltaLatDegs * usedLat / latSegments);
            float latFraction = (float)lat / latSegments;

            var latRow = new List<int>();

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                double lonDegs = llBox.MinLonDegs + (llBox.DeltaLonDegs * lon / lonSegments);
                float lonFraction = (float)lon / lonSegments;

                double ele = radius + tileEleData.InterpolatedValue(lonFraction, latFraction);

                //GD.Print($"lat: {latDegs:F2}, lon: {lonDegs:F2}, rad: {radius:F2}, ele: {tileEleData.InterpolatedValue(lonFraction, latFraction)}");

                KoreLLAPoint rwLLAPointPos = new KoreLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, RadiusM = ele };
                KoreXYZVector rwXYZPointPos = rwLLAPointPos.ToXYZ();

                KoreXYZVector rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // ---- convert from real-world to game engine ----
                rwXYZPointPos = new KoreXYZVector(rwXYZPointPos.X, rwXYZPointPos.Y, -rwXYZPointPos.Z);
                KoreXYZVector geVector = new KoreXYZVector(rwXYZPointPos.X, rwXYZPointPos.Y, -rwXYZPointPos.Z);

                // KoreXYZVector vertex = rwXYZPointPos;
                int vertexId = mesh.AddVertex(geVector);
                latRow.Add(vertexId);
            }

            vertexIdGrid.Add(latRow);
        }

        // Create triangles between all adjacent latitude rows
        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                // Get the four vertices of the current quad
                int v1 = vertexIdGrid[lat][lon];         // current lat, current lon
                int v2 = vertexIdGrid[lat + 1][lon];     // next lat, current lon
                int v3 = vertexIdGrid[lat + 1][lon + 1]; // next lat, next lon
                int v4 = vertexIdGrid[lat][lon + 1];     // current lat, next lon

                // Use colormap coordinates
                KoreColorRGB col = colormap[lat % colormap.GetLength(0), lon % colormap.GetLength(1)];

                // Add the quad as two triangles using AddFace helper
                KoreColorMeshOps.AddFace(mesh, v1, v4, v3, v2, col);
            }
        }

        return mesh;
    }


}
