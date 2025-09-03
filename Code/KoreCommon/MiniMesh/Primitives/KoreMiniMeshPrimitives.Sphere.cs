// <fileheader>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMiniMeshPrimitives
{
    // Create a sphere mesh for KoreMiniMesh
    // Usage: KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(center, radius, latSegments, material, lineColor);

    public static KoreMiniMesh BasicSphere(
        KoreXYZVector center,
        double radius,
        int latSegments,
        KoreMiniMeshMaterial material,
        KoreColorRGB lineCol)
    {
        if (latSegments < 3) throw new ArgumentException("Sphere must have at least 3 latitude segments");

        var mesh = new KoreMiniMesh();

        // Add material and line color
        mesh.AddMaterial(material);
        int lineColorId = mesh.AddColor(lineCol);
        string matName = material.Name;

        int lonSegments = latSegments * 2; // longitude segments (horizontal divisions)

        var allTriangles = new List<int>();

        // Create single vertices for the poles
        KoreXYZVector topPole = center + new KoreXYZVector(0, (float)radius, 0);
        KoreXYZVector bottomPole = center + new KoreXYZVector(0, -(float)radius, 0);
        int topPoleId = mesh.AddVertex(topPole);
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

                // Spherical to Cartesian conversion
                float x = (float)(radius * sin1 * cos2);
                float y = (float)(radius * cos1);
                float z = (float)(radius * sin1 * sin2);

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
                allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v2, v3)));
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

                // Add the quad as two triangles using AddFace helper
                allTriangles.AddRange(KoreMiniMeshOps.AddFace(mesh, v1, v2, v3, v4));
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
                allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v2, v3)));
            }
        }

        // Create wireframe lines
        // Horizontal lines (latitude circles) for intermediate latitudes
        foreach (var latRow in vertexGrid)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int v1 = latRow[lon];
                int v2 = latRow[lon + 1];
                mesh.AddLine(new KoreMiniMeshLine(v1, v2, lineColorId));
            }
        }

        // Vertical lines (longitude lines)
        for (int lon = 0; lon <= lonSegments; lon++)
        {
            // Connect from top pole to first latitude ring
            if (vertexGrid.Count > 0)
            {
                mesh.AddLine(new KoreMiniMeshLine(topPoleId, vertexGrid[0][lon], lineColorId));
            }

            // Connect between latitude rings
            for (int lat = 0; lat < vertexGrid.Count - 1; lat++)
            {
                int v1 = vertexGrid[lat][lon];
                int v2 = vertexGrid[lat + 1][lon];
                mesh.AddLine(new KoreMiniMeshLine(v1, v2, lineColorId));
            }

            // Connect from last latitude ring to bottom pole
            if (vertexGrid.Count > 0)
            {
                mesh.AddLine(new KoreMiniMeshLine(vertexGrid[vertexGrid.Count - 1][lon], bottomPoleId, lineColorId));
            }
        }

        // Create groups
        mesh.AddGroup("All", new KoreMiniMeshGroup(matName, allTriangles));

        return mesh;
    }

    /// <summary>
    /// Create a simple sphere with default parameters (legacy interface)
    /// </summary>
    /// <param name="center">Center point of the sphere</param>
    /// <param name="radius">Sphere radius</param>
    /// <param name="material">Material for the sphere surface</param>
    /// <param name="lineCol">Color for wireframe lines</param>
    /// <returns>KoreMiniMesh sphere</returns>
    public static KoreMiniMesh Sphere(KoreXYZVector center, float radius, KoreMiniMeshMaterial material, KoreColorRGB lineCol)
    {
        return BasicSphere(center, radius, 16, material, lineCol);
    }

    /// <summary>
    /// Create an optimized sphere mesh for KoreMiniMesh with reduced vertex duplication
    /// and adaptive longitude segments for more even point distribution
    /// </summary>
    /// <param name="center">Center point of the sphere</param>
    /// <param name="radius">Sphere radius</param>
    /// <param name="latSegments">Number of latitude segments (vertical divisions)</param>
    /// <param name="material">Material for the sphere surface</param>
    /// <param name="lineCol">Color for wireframe lines</param>
    /// <returns>KoreMiniMesh sphere</returns>
    public static KoreMiniMesh OptimizedSphere(
        KoreXYZVector center, 
        double radius, 
        int latSegments,
        KoreMiniMeshMaterial material, 
        KoreColorRGB lineCol)
    {
        if (latSegments < 3) throw new ArgumentException("Sphere must have at least 3 latitude segments");
        
        var mesh = new KoreMiniMesh();

        // Add material and line color
        mesh.AddMaterial(material);
        int lineColorId = mesh.AddColor(lineCol);
        string matName = material.Name;

        var allTriangles = new List<int>();

        // Create single vertices for the poles
        KoreXYZVector topPole = center + new KoreXYZVector(0, (float)radius, 0);
        KoreXYZVector bottomPole = center + new KoreXYZVector(0, -(float)radius, 0);
        int topPoleId = mesh.AddVertex(topPole);
        int bottomPoleId = mesh.AddVertex(bottomPole);

        // Store vertex rings for each latitude (excluding poles)
        var latitudeRings = new List<List<int>>();

        // Generate vertices for intermediate latitude rings
        for (int lat = 1; lat < latSegments; lat++) // Skip poles (lat=0 and lat=latSegments)
        {
            float a1 = (float)Math.PI * lat / latSegments; // latitude angle (0 to π)
            float sin1 = (float)Math.Sin(a1);
            float cos1 = (float)Math.Cos(a1);

            // Adaptive longitude segments based on latitude
            // At the equator (lat = latSegments/2), use full resolution
            // Near poles, reduce segments proportionally to sin(latitude)
            int baseLonSegments = latSegments * 2;
            int adaptiveLonSegments = Math.Max(6, (int)(baseLonSegments * sin1 * 0.8f + baseLonSegments * 0.2f));
            // Ensure it's even for symmetry and at least 6 for reasonable shape
            adaptiveLonSegments = (adaptiveLonSegments / 2) * 2;

            var latRing = new List<int>();

            for (int lon = 0; lon < adaptiveLonSegments; lon++)
            {
                float a2 = 2f * (float)Math.PI * lon / adaptiveLonSegments; // longitude angle (0 to 2π)
                float sin2 = (float)Math.Sin(a2);
                float cos2 = (float)Math.Cos(a2);

                // Spherical to Cartesian conversion
                float x = (float)(radius * sin1 * cos2);
                float y = (float)(radius * cos1);
                float z = (float)(radius * sin1 * sin2);

                KoreXYZVector vertex = center + new KoreXYZVector(x, y, z);
                int vertexId = mesh.AddVertex(vertex);
                latRing.Add(vertexId);
            }

            latitudeRings.Add(latRing);
        }

        // Create triangles connecting to top pole
        if (latitudeRings.Count > 0)
        {
            var firstRing = latitudeRings[0];
            for (int i = 0; i < firstRing.Count; i++)
            {
                int v1 = topPoleId;
                int v2 = firstRing[i];
                int v3 = firstRing[(i + 1) % firstRing.Count];
                allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v2, v3)));
            }
        }

        // Create triangles between latitude rings
        for (int ringIdx = 0; ringIdx < latitudeRings.Count - 1; ringIdx++)
        {
            var currentRing = latitudeRings[ringIdx];
            var nextRing = latitudeRings[ringIdx + 1];

            // Connect rings with different vertex counts using a more sophisticated approach
            ConnectRingsWithDifferentCounts(mesh, currentRing, nextRing, allTriangles);
        }

        // Create triangles connecting to bottom pole
        if (latitudeRings.Count > 0)
        {
            var lastRing = latitudeRings[latitudeRings.Count - 1];
            for (int i = 0; i < lastRing.Count; i++)
            {
                int v1 = bottomPoleId;
                int v2 = lastRing[(i + 1) % lastRing.Count];
                int v3 = lastRing[i];
                allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v2, v3)));
            }
        }

        // Create wireframe lines for latitude rings
        foreach (var ring in latitudeRings)
        {
            for (int i = 0; i < ring.Count; i++)
            {
                int v1 = ring[i];
                int v2 = ring[(i + 1) % ring.Count];
                mesh.AddLine(new KoreMiniMeshLine(v1, v2, lineColorId));
            }
        }

        // Create some longitude lines (fewer than before due to adaptive segments)
        int lonLineCount = Math.Min(12, latSegments); // Reasonable number of longitude lines
        for (int lonLine = 0; lonLine < lonLineCount; lonLine++)
        {
            // Connect pole to pole through rings
            mesh.AddLine(new KoreMiniMeshLine(topPoleId, 
                latitudeRings.Count > 0 ? GetClosestVertexInRing(latitudeRings[0], lonLine, lonLineCount) : bottomPoleId, 
                lineColorId));

            for (int ringIdx = 0; ringIdx < latitudeRings.Count - 1; ringIdx++)
            {
                int v1 = GetClosestVertexInRing(latitudeRings[ringIdx], lonLine, lonLineCount);
                int v2 = GetClosestVertexInRing(latitudeRings[ringIdx + 1], lonLine, lonLineCount);
                mesh.AddLine(new KoreMiniMeshLine(v1, v2, lineColorId));
            }

            if (latitudeRings.Count > 0)
            {
                mesh.AddLine(new KoreMiniMeshLine(
                    GetClosestVertexInRing(latitudeRings[latitudeRings.Count - 1], lonLine, lonLineCount),
                    bottomPoleId, 
                    lineColorId));
            }
        }

        // Create groups
        mesh.AddGroup("All", new KoreMiniMeshGroup(matName, allTriangles));

        return mesh;
    }

    /// <summary>
    /// Helper method to get the closest vertex in a ring for longitude lines
    /// </summary>
    private static int GetClosestVertexInRing(List<int> ring, int lonLine, int totalLonLines)
    {
        float targetAngle = 2f * (float)Math.PI * lonLine / totalLonLines;
        float ringStep = 2f * (float)Math.PI / ring.Count;
        int closestIndex = (int)Math.Round(targetAngle / ringStep) % ring.Count;
        return ring[closestIndex];
    }

    /// <summary>
    /// Connect two rings with potentially different vertex counts using triangulation
    /// </summary>
    private static void ConnectRingsWithDifferentCounts(KoreMiniMesh mesh, List<int> ring1, List<int> ring2, List<int> allTriangles)
    {
        // Simple approach: use the ring with more vertices as the "driver"
        // and create triangles that best preserve the shape
        
        if (ring1.Count == ring2.Count)
        {
            // Same count - simple quad connection
            for (int i = 0; i < ring1.Count; i++)
            {
                int v1 = ring1[i];
                int v2 = ring2[i];
                int v3 = ring2[(i + 1) % ring2.Count];
                int v4 = ring1[(i + 1) % ring1.Count];
                allTriangles.AddRange(KoreMiniMeshOps.AddFace(mesh, v1, v2, v3, v4));
            }
        }
        else
        {
            // Different counts - use proportional mapping
            int maxCount = Math.Max(ring1.Count, ring2.Count);
            
            for (int i = 0; i < maxCount; i++)
            {
                // Map indices proportionally
                int i1 = (i * ring1.Count) / maxCount;
                int i1Next = ((i + 1) * ring1.Count) / maxCount;
                int i2 = (i * ring2.Count) / maxCount;
                int i2Next = ((i + 1) * ring2.Count) / maxCount;

                if (i1 != i1Next && i2 != i2Next)
                {
                    // Both rings advance - create quad
                    int v1 = ring1[i1 % ring1.Count];
                    int v2 = ring2[i2 % ring2.Count];
                    int v3 = ring2[i2Next % ring2.Count];
                    int v4 = ring1[i1Next % ring1.Count];
                    allTriangles.AddRange(KoreMiniMeshOps.AddFace(mesh, v1, v2, v3, v4));
                }
                else if (i1 != i1Next)
                {
                    // Only ring1 advances - create triangle
                    int v1 = ring1[i1 % ring1.Count];
                    int v2 = ring2[i2 % ring2.Count];
                    int v3 = ring1[i1Next % ring1.Count];
                    allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v2, v3)));
                }
                else if (i2 != i2Next)
                {
                    // Only ring2 advances - create triangle
                    int v1 = ring1[i1 % ring1.Count];
                    int v2 = ring2[i2 % ring2.Count];
                    int v3 = ring2[i2Next % ring2.Count];
                    allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v2, v3)));
                }
            }
        }
    }

}
