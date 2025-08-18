using System;
using System.Collections.Generic;
using System.IO;
using KoreCommon;
using KoreCommon.SkiaSharp;

namespace KoreCommon.UnitTest;

public static partial class KoreTestMeshUvOps
{
    /// <summary>
    /// Creates an oil barrel mesh with proper UV mapping for texture application.
    /// UV Layout:
    /// - Top circle: Center at (0.25, 0.75) with 0.20 radius
    /// - Bottom circle: Center at (0.75, 0.75) with 0.20 radius
    /// - Cylinder sides: Rectangle in lower half (0.0-1.0, 0.0-0.5)
    /// </summary>
    /// <param name="segments">Number of segments around the cylinder (default 16)</param>
    /// <param name="radius">Radius of the barrel (default 1.0)</param>
    /// <param name="height">Height of the barrel (default 2.0)</param>
    /// <returns>KoreMeshData representing the oil barrel</returns>
    public static KoreMeshData CreateOilBarrelWithUV(int segments = 16, double radius = 1.0, double height = 2.0)
    {
        var mesh = new KoreMeshData();

        // UV layout parameters matching the example image
        var topCenterU = 0.25;
        var topCenterV = 0.75;  // Top of image
        var bottomCenterU = 0.75;
        var bottomCenterV = 0.75;  // Top of image
        var uvRadius = 0.24;  // Slightly smaller than 0.25 to avoid edge bleeding

        // Create center vertices for top and bottom caps
        int topCenterVertex = mesh.AddVertex(
            new KoreXYZVector(0, height/2, 0),
            null, null,
            new KoreXYVector(topCenterU, topCenterV)
        );

        int bottomCenterVertex = mesh.AddVertex(
            new KoreXYZVector(0, -height/2, 0),
            null, null,
            new KoreXYVector(bottomCenterU, bottomCenterV)
        );

        // Create ring vertices for top and bottom caps
        var topRingVertices = new int[segments];
        var bottomRingVertices = new int[segments];

        for (int i = 0; i < segments; i++)
        {
            double angle = (double)i / segments * Math.PI * 2;
            double x = Math.Cos(angle) * radius;
            double z = Math.Sin(angle) * radius;

            // UV coordinates for circular caps
            double topU = topCenterU + Math.Cos(angle) * uvRadius;
            double topV = topCenterV + Math.Sin(angle) * uvRadius;
            double bottomU = bottomCenterU + Math.Cos(angle) * uvRadius;
            double bottomV = bottomCenterV + Math.Sin(angle) * uvRadius;

            // Top ring vertex
            topRingVertices[i] = mesh.AddVertex(
                new KoreXYZVector(x, height/2, z),
                null, null,
                new KoreXYVector(topU, topV)
            );

            // Bottom ring vertex
            bottomRingVertices[i] = mesh.AddVertex(
                new KoreXYZVector(x, -height/2, z),
                null, null,
                new KoreXYVector(bottomU, bottomV)
            );
        }

        // Create cylinder side vertices (duplicate positions but different UVs)
        var sideTopVertices = new int[segments + 1]; // +1 for closing the seam
        var sideBottomVertices = new int[segments + 1]; // +1 for closing the seam

        for (int i = 0; i <= segments; i++) // <= to include the closing vertex
        {
            double angle = (double)(i % segments) / segments * Math.PI * 2; // Wrap the angle for last vertex
            double x = Math.Cos(angle) * radius;
            double z = -1 * Math.Sin(angle) * radius;

            // UV coordinates for cylinder sides - wrap horizontally, span vertically
            double u = (double)i / segments; // This will go from 0 to 1 (including 1.0 for closing)

            // Side vertices (same 3D position as ring vertices but different UV)
            sideTopVertices[i] = mesh.AddVertex(
                new KoreXYZVector(x, height/2, z),
                null, null,
                new KoreXYVector(u, 0.5) // Top of cylinder rectangle
            );

            sideBottomVertices[i] = mesh.AddVertex(
                new KoreXYZVector(x, -height/2, z),
                null, null,
                new KoreXYVector(u, 0.0) // Bottom of cylinder rectangle
            );
        }

        // Create triangles for top cap (fan pattern)
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            mesh.AddTriangle(topCenterVertex, topRingVertices[next], topRingVertices[i]);
        }

        // Create triangles for bottom cap (fan pattern)
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            mesh.AddTriangle(bottomCenterVertex, bottomRingVertices[i], bottomRingVertices[next]);
        }

        // Create triangles for cylinder sides
        for (int i = 0; i < segments; i++)
        {
            int next = i + 1; // No modulo needed since we have segments+1 vertices

            // Two triangles per side segment
            mesh.AddTriangle(sideBottomVertices[i], sideTopVertices[i], sideTopVertices[next]);
            mesh.AddTriangle(sideBottomVertices[i], sideTopVertices[next], sideBottomVertices[next]);
        }

        // Add material
        var barrelMaterial = new KoreMeshMaterial("OilBarrelMaterial", new KoreColorRGB(200, 10, 10));
        mesh.AddMaterial(barrelMaterial);

        // Create named triangle group for all barrel geometry
        var allTriangleIds = new List<int>();
        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            allTriangleIds.Add(i);
        }

        var triangleGroup = new KoreMeshTriangleGroup("OilBarrelMaterial", allTriangleIds);
        mesh.NamedTriangleGroups["OilBarrel"] = triangleGroup;

        return mesh;
    }

    /// <summary>
    /// Test method for creating and visualizing an oil barrel with UV mapping
    /// </summary>
    public static void TestOilBarrelUVLayout(KoreTestLog testLog)
    {
        var mesh = CreateOilBarrelWithUV(16, 1.0, 3.0);

        // Save UV layout images
        string debugPath = "UnitTestArtefacts/oil_barrel_uv_debug.png";
        string cleanPath = "UnitTestArtefacts/oil_barrel_uv_clean.png";

        KoreFileOps.CreateDirectoryForFile(debugPath);
        KoreMeshDataUvOps.SaveUVLayout(mesh, debugPath, 2048, true, true);
        KoreMeshDataUvOps.SaveUVLayout(mesh, cleanPath, 1024, false, true);

        testLog.AddResult("Oil barrel UV layout", true,
            $"Created oil barrel with {mesh.Vertices.Count} vertices, {mesh.Triangles.Count} triangles");
        testLog.AddComment($"Debug image: {debugPath}");
        testLog.AddComment($"Clean image: {cleanPath}");

        // Assign UV texture to material for visual verification
        var material = mesh.GetMaterial("OilBarrelMaterial");
        // if (material != null)
        // {
        var updatedMaterial = new KoreMeshMaterial(
            material.Name,
            material.BaseColor,
            material.Metallic,
            material.Roughness,
            "oil_barrel_uv_clean.png"
        );
        mesh.AddMaterial(updatedMaterial);
        // }

        // Export OBJ/MTL files
        var (objContent, mtlContent) = KoreMeshDataIO.ToObjMtl(mesh, "TestOilBarrel", "TestOilBarrelMats");
        File.WriteAllText("UnitTestArtefacts/TestOilBarrel.obj", objContent);
        File.WriteAllText("UnitTestArtefacts/TestOilBarrelMats.mtl", mtlContent);
        testLog.AddComment("OBJ/MTL files created for oil barrel UV layout with UV texture assignment");

        // Export the JSON representation
        string jsonPath = "UnitTestArtefacts/TestOilBarrel.json";
        KoreFileOps.CreateDirectoryForFile(jsonPath);
        File.WriteAllText(jsonPath, KoreMeshDataIO.ToJson(mesh));

        testLog.AddComment($"JSON file created: {jsonPath}");
    }
}
