using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public partial class KoreMeshData
{
    // --------------------------------------------------------------------------------------------
    // MARK: Validity Ops
    // --------------------------------------------------------------------------------------------

    // Function to fully populate the mesh data with matching normals, UVs, vertex colors, line colors, and triangle colors.
    public void FullyPopulate()
    {
        CreateMissingNormals(); // Points
        CreateMissingUVs();
        CreateMissingVertexColors();
        CreateMissingLineColors(); // Lines
        CreateMissingTriangleColors(); // Triangles
    }

    // Function to examine the vertex list, and remove any orphaned or duplicate lines, triangles, and colors.
    public void MakeValid()
    {
        RemoveOrphanedPoints();
        RemoveDuplicatePoints();

        RemoveBrokenNormals(); // Remove normals that don't have supporting point IDs.

        RemoveBrokenUVs(); // Remove UVs that don't have supporting point IDs.

        RemoveBrokenLines(); // Remove lines that don't have supporting point IDs.
        RemoveDuplicateLines();

        RemoveBrokenLineColors(); // Remove line colors that don't have supporting line IDs.

        RemoveBrokenTriangles(); // Remove triangles that don't have supporting point IDs.
        RemoveDuplicateTriangles();

        RemoveBrokenTriangleColors(); // Remove triangle colors that don't have supporting triangle IDs.
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Populate: Max Ids
    // --------------------------------------------------------------------------------------------

    // Reset the Next IDs, looking for the max values in the current lists - Note that after numerous
    // operations, the IDs can be non-sequential, so we need to find the max value in each list.

    public void ResetMaxIDs()
    {
        // Reset the next IDs based on the current max values in the dictionaries
        NextVertexId   = Vertices.Count  > 0 ? Vertices.Keys.Max()  + 1 : 0;
        NextLineId     = Lines.Count     > 0 ? Lines.Keys.Max()     + 1 : 0;
        NextTriangleId = Triangles.Count > 0 ? Triangles.Keys.Max() + 1 : 0;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Populate: LineId
    // --------------------------------------------------------------------------------------------

        // Functions to fill out the population of the lists based on a line ID.

    public void CreateMissingLineColors(KoreColorRGB? defaultColor = null)
    {
        // Define the default color to pad the LineColors list if it doesn't match the lines count.
        KoreColorRGB fallback = defaultColor ?? new KoreColorRGB(1, 1, 1);

        // Loop through the lines dictionary
        foreach (var kvp in Lines)
        {
            int lineId = kvp.Key;
            KoreMeshLine line = kvp.Value;

            // If the line colors dictionary does not contain this ID, add it with the fallback color
            if (!LineColors.ContainsKey(lineId))
                LineColors[lineId] = new KoreMeshLineColour(fallback, fallback);
        }
    }





    // -----------------------------------------------------------------------------
    // MARK: Vertices
    // -----------------------------------------------------------------------------

    // Create a list of all the point IDs that are used in lines or triangles, then remove any points that are not in that list.

    public void RemoveOrphanedPoints()
    {
        // Determine which vertices are referenced by lines or triangles
        var used = new HashSet<int>();

        // Loop through the lines dictionary
        foreach (var kvp in Lines)
        {
            int lineId = kvp.Key;
            KoreMeshLine line = kvp.Value;

            used.Add(line.A);
            used.Add(line.B);
        }
        // Loop through the triangles dictionary
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            used.Add(triangle.A);
            used.Add(triangle.B);
            used.Add(triangle.C);
        }
        // Loop through the vertex colors dictionary
        foreach (var kvp in VertexColors)
        {
            // Get the vertex ID and its color
            int vertexId = kvp.Key;
            KoreColorRGB color = kvp.Value;

            used.Add(vertexId);
        }

        // Now loop through the vertices and remove any that are not in the used set
        foreach (var key in Vertices.Keys)
        {
            if (!used.Contains(key))
                Vertices.Remove(key);
        }
    }

    // A duplicate point, is one within a close tollerance of distance from another point
    // - We remove the point, and store the ID of the point that was preserved in its place, to renumber in lines and triangles.

    // Point IDs can non-sequential after serval operations, so we can't rely on then beyond being an ID for a thing.

    public void RemoveDuplicatePoints(double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        // A nested loop through the vertices to find any two that are within the tolerance distance
        foreach (var kvp in Vertices)
        {
            int i = kvp.Key;
            KoreXYZVector v = kvp.Value;


        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    public void RemoveBrokenNormals()
    {
        // Loop through the Normals dictionary
        foreach (var kvp in Normals)
        {
            int normalId = kvp.Key;
            //KoreXYZVector normal   = kvp.Value;

            if (!Vertices.ContainsKey(normalId))
            {
                Normals.Remove(normalId);
            }
        }
    }

    // Functions to fill out the population of the lists based on a vertex ID.

    public void CreateMissingNormals(KoreXYZVector? defaultNormal = null)
    {
        // Define the default normal to pad the normals list if it doesn't match the vertices count.
        KoreXYZVector fallback = defaultNormal ?? new KoreXYZVector(0, 1, 0);

        // Loop through the vertices dictionary
        foreach (var kvp in Vertices)
        {
            // Get the vertex ID and its position
            int vertexId = kvp.Key;
            //KoreXYZVector vertex = kvp.Value;

            // If the normals dictionary does not contain this ID, add it with the fallback normal
            if (!Normals.ContainsKey(vertexId))
                Normals[vertexId] = fallback;
        }
    }

    // Create a normal for a vertex based on the first triangle that contains that vertex
    public void SetNormalFromFirstTriangle(int vertexId)
    {
        // Check if the vertex exists
        if (!Vertices.ContainsKey(vertexId))
            return;

        // Find the first triangle that contains this vertex
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            // Check if this triangle contains our vertex
            if (triangle.A == vertexId || triangle.B == vertexId || triangle.C == vertexId)
            {
                // Get the three vertices of this triangle
                if (!Vertices.ContainsKey(triangle.A) ||
                    !Vertices.ContainsKey(triangle.B) ||
                    !Vertices.ContainsKey(triangle.C))
                    continue; // Skip broken triangles

                KoreXYZVector a = Vertices[triangle.A];
                KoreXYZVector b = Vertices[triangle.B];
                KoreXYZVector c = Vertices[triangle.C];

                // Calculate the face normal using cross product (same as AddIsolatedTriangle)
                KoreXYZVector ab = b - a;  // Vector from A to B
                KoreXYZVector ac = c - a;  // Vector from A to C

                // Cross product gives us the face normal (right-hand rule)
                KoreXYZVector faceNormal = KoreXYZVector.CrossProduct(ab, ac);

                // Normalize and invert the face normal (matching AddIsolatedTriangle behavior)
                faceNormal = faceNormal.Normalize();
                faceNormal = faceNormal.Invert();

                // Set the normal for this vertex
                Normals[vertexId] = faceNormal;
                return; // We found the first triangle, so we're done
            }
        }
    }

    public void SetNormalsFromTriangles(List<int> vertexIds) => vertexIds.ForEach(SetNormalFromFirstTriangle);

    // Set normals for all vertices based on the first triangle that contains each vertex
    // Usage: mesh.SetNormalsFromTriangles();
    public void SetNormalsFromTriangles()
    {
        // Loop through the triangles in the mesh and set normals for each vertex
        foreach (var triangle in Triangles.Values)
        {
            SetNormalFromFirstTriangle(triangle.A);
            SetNormalFromFirstTriangle(triangle.B);
            SetNormalFromFirstTriangle(triangle.C);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UVs
    // --------------------------------------------------------------------------------------------

    public void RemoveBrokenUVs()
    {
        // Loop through the UVs dictionary
        foreach (var kvp in UVs)
        {
            int uvId = kvp.Key;
            //KoreXYVector uv = kvp.Value;

            if (!Vertices.ContainsKey(uvId))
            {
                UVs.Remove(uvId);
            }
        }
    }

    public void CreateMissingUVs(KoreXYVector? defaultUV = null)
    {
        // Define the default UV to pad the UVs list if it doesn't match the vertices count.
        KoreXYVector fallback = defaultUV ?? new KoreXYVector(0, 0);

        // Loop through the vertices dictionary
        foreach (var kvp in Vertices)
        {
            // Get the vertex ID and its position
            int vertexId = kvp.Key;
            //KoreXYZVector vertex = kvp.Value;

            // If the UVs dictionary does not contain this ID, add it with the fallback UV
            if (!UVs.ContainsKey(vertexId))
                UVs[vertexId] = fallback;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Vertex Colors
    // --------------------------------------------------------------------------------------------

    public void CreateMissingVertexColors(KoreColorRGB? defaultColor = null)
    {
        // Define the default color to pad the VertexColors list if it doesn't match the vertices count.
        KoreColorRGB fallback = defaultColor ?? new KoreColorRGB(1, 1, 1);

        // Loop through the vertices dictionary
        foreach (var kvp in Vertices)
        {
            // Get the vertex ID and its position
            int vertexId = kvp.Key;
            //KoreXYZVector vertex = kvp.Value;

            // If the vertex colors dictionary does not contain this ID, add it with the fallback color
            if (!VertexColors.ContainsKey(vertexId))
                VertexColors[vertexId] = fallback;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    // Remove lines that don't have supporting point IDs.
    public void RemoveBrokenLines()
    {
        // Loop through the Lines dictionary
        foreach (var kvp in Lines)
        {
            int lineId = kvp.Key;
            KoreMeshLine line = kvp.Value;

            int a = line.A;
            int b = line.B;

            if (!Vertices.ContainsKey(a) || !Vertices.ContainsKey(b))
            {
                Lines.Remove(lineId);
            }
        }
    }

    // As its a dictionary, we have a unique line ID, but the two points within it could be a duplicate, so we need to get rid of them.
    // Loop through the lines, adding each one to a HashSet, and if it already exists, mark it for removal.

    public void RemoveDuplicateLines()
    {
        var uniqueLines = new HashSet<(int, int)>();
        var keysToRemove = new List<int>();

        foreach (var kvp in Lines)
        {
            int lineId = kvp.Key;
            KoreMeshLine line = kvp.Value;
            var pair = (Math.Min(line.A, line.B), Math.Max(line.A, line.B));
            if (!uniqueLines.Add(pair))
            {
                keysToRemove.Add(lineId);
            }
        }

        foreach (var key in keysToRemove)
            Lines.Remove(key);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Line Colors
    // --------------------------------------------------------------------------------------------

    // Remove any line colour reference that does not have a corresponding line

    public void RemoveBrokenLineColors()
    {
        // loop through all the line colours
        foreach (var lineColor in LineColors)
        {
            int lineId = lineColor.Key;
            KoreMeshLineColour lc = lineColor.Value;

            // If the line ID does not exist in the Lines dictionary, remove the line color
            if (!Lines.ContainsKey(lineId))
            {
                LineColors.Remove(lineId);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    public void RemoveBrokenTriangles()
    {
        // Loop through the Triangles dictionary
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            int a = triangle.A;
            int b = triangle.B;
            int c = triangle.C;

            if (!Vertices.ContainsKey(a) || !Vertices.ContainsKey(b) || !Vertices.ContainsKey(c))
            {
                Triangles.Remove(triangleId);
            }
        }
    }

    // As its a dictionary, we have a unique triangle ID, but the points within it could be a duplicate, so we need to get rid of them.
    // Loop through the triangles, adding each three point set to a HashSet, and if it already exists, mark it for removal.

    public void RemoveDuplicateTriangles()
    {
        var uniqueTriangles = new HashSet<(int, int, int)>();
        var keysToRemove = new List<int>();

        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;
            var triplet = (KoreNumericUtils.Min3(triangle.A, triangle.B, triangle.C),
                           KoreNumericUtils.Mid3(triangle.A, triangle.B, triangle.C),
                           KoreNumericUtils.Max3(triangle.A, triangle.B, triangle.C));
            if (!uniqueTriangles.Add(triplet))
            {
                keysToRemove.Add(triangleId);
            }
        }

        foreach (var key in keysToRemove)
            Triangles.Remove(key);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Colour
    // --------------------------------------------------------------------------------------------

    public void CreateMissingTriangleColors(KoreColorRGB? defaultColor = null)
    {
        // Define the default color to pad the TriangleColors list if it doesn't match the triangles count.
        KoreColorRGB fallback = defaultColor ?? new KoreColorRGB(1, 1, 1);

        // Loop through the triangles dictionary
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            // If the triangle colors dictionary does not contain this ID, add it with the fallback color
            if (!TriangleColors.ContainsKey(triangleId))
                TriangleColors[triangleId] = new KoreMeshTriangleColour(fallback);
        }
    }
    public void RemoveBrokenTriangleColors()
    {
        // Loop through the TriangleColors dictionary
        foreach (var kvp in TriangleColors)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangleColour triangleColor = kvp.Value;

            // Remove any referenced trianglecolour that does not have a corresponding triangle
            if (!Triangles.ContainsKey(triangleId))
            {
                TriangleColors.Remove(triangleId);
            }
        }
    }


}
