using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshDataPrimitives
{
    public static KoreMeshData BasicHemisphere(float radius, KoreColorRGB color, int numLatSegments)
    {
        int latSegments = numLatSegments;
        int lonSegments = numLatSegments * 2;

        var mesh = new KoreMeshData();
        var indexMap = new List<int>();

        for (int lat = 0; lat <= latSegments; lat++)
        {
            double a1 = (Math.PI / 2.0) * lat / latSegments; // 0 to PI/2
            double sin1 = Math.Sin(a1);
            double cos1 = Math.Cos(a1);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                double a2 = 2 * Math.PI * lon / lonSegments;
                double sin2 = Math.Sin(a2);
                double cos2 = Math.Cos(a2);

                double x = radius * sin1 * cos2;
                double y = radius * cos1;
                double z = radius * sin1 * sin2;

                var vertex = new KoreXYZVector(x, y, z);
                var normal = (radius != 0) ? vertex / radius : new KoreXYZVector(0, 1, 0);

                int idx = mesh.AddVertex(vertex, normal, color);
                indexMap.Add(idx);
            }
        }

        // Triangles
        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int current = lat * (lonSegments + 1) + lon;
                int next = current + lonSegments + 1;

                mesh.AddTriangle(indexMap[current], indexMap[next], indexMap[current + 1]);
                mesh.AddTriangle(indexMap[current + 1], indexMap[next], indexMap[next + 1]);
            }
        }

        // Horizontal wireframe lines
        for (int lat = 0; lat <= latSegments; lat++)
        {
            int rowStart = lat * (lonSegments + 1);
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int a = indexMap[rowStart + lon];
                int b = indexMap[rowStart + lon + 1];
                mesh.AddLine(a, b, color, color);
            }
        }

        // Vertical wireframe lines
        for (int lon = 0; lon <= lonSegments; lon++)
        {
            for (int lat = 0; lat < latSegments; lat++)
            {
                int a = indexMap[lat * (lonSegments + 1) + lon];
                int b = indexMap[(lat + 1) * (lonSegments + 1) + lon];
                mesh.AddLine(a, b, color, color);
            }
        }

        return mesh;
    }

}
