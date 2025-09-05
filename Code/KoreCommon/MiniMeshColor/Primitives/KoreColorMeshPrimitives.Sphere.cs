// <fileheader>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreColorMeshPrimitives
{
    // Create a sphere mesh for KoreColorMesh, with a given center, radius, and colormap
    // - We pick the number of lat/long segments from the color list dimensions
    // Usage: KoreColorMesh sphereMesh = KoreColorMeshPrimitives.BasicSphere(center, radius, colormap);

    public static KoreColorMesh BasicSphere(
        KoreXYZVector center,
        double radius,
        KoreColorRGB[,] colormap)
    {
        var mesh = new KoreColorMesh();

        int lonSegments = colormap.GetLength(1); // longitude segments (horizontal divisions)
        int latSegments = colormap.GetLength(0); // latitude segments (vertical divisions)

        var allTriangles = new List<int>();

        // Create single vertices for the poles
        KoreXYZVector topPole    = center + new KoreXYZVector(0, (float)radius, 0);
        KoreXYZVector bottomPole = center + new KoreXYZVector(0, -(float)radius, 0);
        int topPoleId    = mesh.AddVertex(topPole);
        int bottomPoleId = mesh.AddVertex(bottomPole);

        // Create vertex grid for intermediate latitudes only (excluding poles)
        var vertexGrid = new List<List<int>>();

        for (int lat = 1; lat < latSegments; lat++) // Skip poles (lat=0 and lat=latSegments)
        {
            var latRow = new List<int>();

            float a1 = (float)Math.PI * lat / latSegments; // latitude angle (0 to π)
            float sin1 = (float)Math.Sin(a1);
            float cos1 = (float)Math.Cos(a1);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float a2 = 2f * (float)Math.PI * lon / lonSegments; // longitude angle (0 to 2π)
                float sin2 = (float)Math.Sin(a2);
                float cos2 = (float)Math.Cos(a2);

                // Spherical to Cartesian conversion for Godot (right-handed, -Z forward)
                float x = (float)(radius * sin1 * cos2);
                float y = (float)(radius * cos1);
                float z = -(float)(radius * sin1 * sin2); // Negate Z for Godot coordinate system

                KoreXYZVector vertex = center + new KoreXYZVector(x, y, z);
                int vertexId = mesh.AddVertex(vertex);
                latRow.Add(vertexId);
            }

            vertexGrid.Add(latRow);
        }

        // Create triangles connecting to top pole
        if (vertexGrid.Count > 0)
        {
            var firstRow = vertexGrid[0];
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int v1 = topPoleId;
                int v2 = firstRow[lon];
                int v3 = firstRow[lon + 1];
                KoreColorRGB col = colormap[0, lon % colormap.GetLength(1)];

                allTriangles.Add(mesh.AddTriangle(new KoreColorMeshTri(v1, v2, v3, col)));
            }
        }

        // Create triangles between intermediate latitude rows
        for (int lat = 0; lat < vertexGrid.Count - 1; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                // Get the four vertices of the current quad to match original sphere winding
                int v1 = vertexGrid[lat][lon];         // current lat, current lon
                int v2 = vertexGrid[lat + 1][lon];     // next lat, current lon
                int v3 = vertexGrid[lat + 1][lon + 1]; // next lat, next lon
                int v4 = vertexGrid[lat][lon + 1];     // current lat, next lon
                KoreColorRGB col = colormap[lat, lon % colormap.GetLength(1)];

                // Add the quad as two triangles using AddFace helper
                allTriangles.AddRange(KoreColorMeshOps.AddFace(mesh, v1, v4, v3, v2, col));
            }
        }

        // Create triangles connecting to bottom pole
        if (vertexGrid.Count > 0)
        {
            var lastRow = vertexGrid[vertexGrid.Count - 1];
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int v1 = bottomPoleId;
                int v2 = lastRow[lon + 1];
                int v3 = lastRow[lon];
                KoreColorRGB col = colormap[latSegments - 1, lon % colormap.GetLength(1)];
                allTriangles.Add(mesh.AddTriangle(new KoreColorMeshTri(v1, v2, v3, col)));
            }
        }

        return mesh;
    }

}
