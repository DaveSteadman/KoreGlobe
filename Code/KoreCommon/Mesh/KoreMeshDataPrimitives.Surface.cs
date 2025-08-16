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
    // Usage: var cubeMesh = KoreMeshDataPrimitives.Surface(1.0f, new KoreColorRGB(255, 0, 0));
    public static KoreMeshData Surface(KoreXYZVector[,] vertices, KoreUVBox uvBox)
    {
        var mesh = new KoreMeshData();

        // Basic setup, dimensions and UVs
        int width  = vertices.GetLength(0);
        int height = vertices.GetLength(1);
        KoreXYVector[,] uvGrid = uvBox.GetUVGrid(width, height);

        // Loop through the grid, adding points and UVs. Create a corresponding output grid of the point IDs
        int[,] pointIds = new int[width, height];

        for (int iX = 0; iX < width; iX++)
        {
            for (int iY = 0; iY < height; iY++)
            {
                // Add vertex with position and UV
                pointIds[iX, iY] = mesh.AddVertex(
                    vertices[iX, iY],
                    null, null, uvGrid[iX, iY]);
            }
        }

        // Loop through the point IDs to create triangles and lines
        for (int iX = 0; iX < width - 1; iX++)
        {
            for (int iY = 0; iY < height - 1; iY++)
            {
                // Create two triangles for each quad
                int p00 = pointIds[iX, iY];
                int p01 = pointIds[iX, iY + 1];
                int p10 = pointIds[iX + 1, iY];
                int p11 = pointIds[iX + 1, iY + 1];

                mesh.AddTriangle(p00, p10, p01);
                mesh.AddTriangle(p01, p10, p11);

                // Always add top and left edges
                mesh.AddLine(p00, p10);  // Left edge
                mesh.AddLine(p01, p00);  // Top edge

                // Add right edge only for last column
                if (iX == width - 2)
                    mesh.AddLine(p10, p11);  // Right edge

                // Add bottom edge only for last row
                if (iY == height - 2)
                    mesh.AddLine(p11, p01);  // Bottom edge
            }
        }

        // Create the normals based on the triangles
        mesh.SetNormalsFromTriangles();

        return mesh;
    }

}
