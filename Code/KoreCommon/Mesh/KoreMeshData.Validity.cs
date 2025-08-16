using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public partial class KoreMeshData
{
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
    // MARK: Validity Ops
    // --------------------------------------------------------------------------------------------

    // Function to fully populate the mesh data with matching normals, UVs, vertex colors, line colors, and triangle colors.
    public void FullyPopulate()
    {
        CreateMissingNormals(); // Points
        CreateMissingUVs();
        CreateMissingVertexColors();
        CreateMissingLineColors(); // Lines
    }

    // Function to examine the vertex list, and remove any orphaned or duplicate lines, triangles, and colors.
    public void MakeValid()
    {
        // Delegate to the static EditOps version for consistency
        KoreMeshDataEditOps.MakeValid(this);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Bounding Box
    // --------------------------------------------------------------------------------------------

    // Loop through the vertices, recording the max/min X, Y, Z values. Then return a KoreXYZBox

    public KoreXYZBox GetBoundingBox()
    {
        if (Vertices.Count == 0)
            return KoreXYZBox.Zero;

        double minX = double.MaxValue, maxX = double.MinValue;
        double minY = double.MaxValue, maxY = double.MinValue;
        double minZ = double.MaxValue, maxZ = double.MinValue;

        foreach (var kvp in Vertices)
        {
            KoreXYZVector vertex = kvp.Value;
            if (vertex.X < minX) minX = vertex.X;
            if (vertex.X > maxX) maxX = vertex.X;
            if (vertex.Y < minY) minY = vertex.Y;
            if (vertex.Y > maxY) maxY = vertex.Y;
            if (vertex.Z < minZ) minZ = vertex.Z;
            if (vertex.Z > maxZ) maxZ = vertex.Z;
        }

        KoreXYZVector center = new KoreXYZVector((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        double width = maxX - minX;
        double height = maxY - minY;
        double length = maxZ - minZ;

        return new KoreXYZBox(center, width, height, length);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Vertices
    // --------------------------------------------------------------------------------------------

    // Create a list of all the point IDs that are used in lines or triangles, then remove any points that are not in that list.

    public void RemoveOrphanedPoints()
    {
        // Delegate to the static EditOps version for consistency
        KoreMeshDataEditOps.RemoveOrphanedPoints(this);
    }

    // A duplicate point, is one within a close tollerance of distance from another point
    // - We remove the point, and store the ID of the point that was preserved in its place, to renumber in lines and triangles.

    // Point IDs can non-sequential after serval operations, so we can't rely on then beyond being an ID for a thing.

    public void RemoveDuplicatePoints(double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        // Create a mapping from duplicate point ID to canonical (first found) point ID
        var remapping = new Dictionary<int, int>();
        var processedVertices = new List<int>();

        // Find duplicates by comparing each vertex with all previously processed vertices
        foreach (var kvp in Vertices)
        {
            int currentId = kvp.Key;
            KoreXYZVector currentVertex = kvp.Value;
            bool foundDuplicate = false;

            // Check against all previously processed vertices
            foreach (int earlierId in processedVertices)
            {
                KoreXYZVector earlierVertex = Vertices[earlierId];

                if (currentVertex.IsEqualTo(earlierVertex, tolerance))
                {
                    // Found a duplicate - map current ID to the earlier ID
                    remapping[currentId] = earlierId;
                    foundDuplicate = true;
                    break;
                }
            }

            // If not a duplicate, add to processed list
            if (!foundDuplicate)
            {
                processedVertices.Add(currentId);
            }
        }

        // Update all references to use canonical vertex IDs
        RemapVertexReferences(remapping);

        // Remove the duplicate vertices
        foreach (int duplicateId in remapping.Keys)
        {
            Vertices.Remove(duplicateId);
            // Also remove associated data for the duplicate vertex
            Normals.Remove(duplicateId);
            UVs.Remove(duplicateId);
            VertexColors.Remove(duplicateId);
        }
    }

    // Helper method to update all vertex ID references in lines and triangles
    private void RemapVertexReferences(Dictionary<int, int> remapping)
    {
        // Update line references
        var updatedLines = new Dictionary<int, KoreMeshLine>();
        foreach (var kvp in Lines)
        {
            int lineId = kvp.Key;
            KoreMeshLine line = kvp.Value;

            int newA = remapping.ContainsKey(line.A) ? remapping[line.A] : line.A;
            int newB = remapping.ContainsKey(line.B) ? remapping[line.B] : line.B;

            updatedLines[lineId] = new KoreMeshLine(newA, newB);
        }
        Lines = updatedLines;

        // Update triangle references
        var updatedTriangles = new Dictionary<int, KoreMeshTriangle>();
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            int newA = remapping.ContainsKey(triangle.A) ? remapping[triangle.A] : triangle.A;
            int newB = remapping.ContainsKey(triangle.B) ? remapping[triangle.B] : triangle.B;
            int newC = remapping.ContainsKey(triangle.C) ? remapping[triangle.C] : triangle.C;

            updatedTriangles[triangleId] = new KoreMeshTriangle(newA, newB, newC);
        }
        Triangles = updatedTriangles;
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

            // If we don't have a matching vertex ID, remove the normal
            if (!Vertices.ContainsKey(normalId))
                Normals.Remove(normalId);
        }
    }

    // --------------------------------------------------------------------------------------------

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

    // --------------------------------------------------------------------------------------------

    public KoreXYZVector NormalForTriangle(int triangleId)
    {
        if (!Triangles.ContainsKey(triangleId))
            return KoreXYZVector.Zero;

        KoreMeshTriangle triangle = Triangles[triangleId];
        KoreXYZVector a = Vertices[triangle.A];
        KoreXYZVector b = Vertices[triangle.B];
        KoreXYZVector c = Vertices[triangle.C];

        // Calculate the face normal using cross product
        KoreXYZVector ab = b - a;  // Vector from A to B
        KoreXYZVector ac = c - a;  // Vector from A to C
        KoreXYZVector faceNormal = KoreXYZVector.CrossProduct(ab, ac).Normalize();

        // Normalize and invert the face normal (matching AddIsolatedTriangle behavior)
        faceNormal = faceNormal.Normalize();
        faceNormal = faceNormal.Invert();

        return faceNormal;
    }

    // --------------------------------------------------------------------------------------------

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
                KoreXYZVector faceNormal = NormalForTriangle(triangleId);

                // Set the normal for this vertex
                Normals[vertexId] = faceNormal;
                return; // We found the first triangle, so we're done
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Set normals for all vertices based on the first triangle that contains each vertex
    // Usage: mesh.SetNormalsFromTriangles();
    public void SetNormalsFromTriangles()
    {
        // Create a dictionary of all the vertex IDs, and the lowest number of triangle ID they appear in
        // We can then iterate through the list once, creating the normals more efficiently.
        var vertexTriangleMap = new Dictionary<int, int>();
        foreach (var kvp in Triangles)
        {
            int triangleId = kvp.Key;
            KoreMeshTriangle triangle = kvp.Value;

            if (!vertexTriangleMap.ContainsKey(triangle.A)) vertexTriangleMap[triangle.A] = triangleId;
            if (!vertexTriangleMap.ContainsKey(triangle.B)) vertexTriangleMap[triangle.B] = triangleId;
            if (!vertexTriangleMap.ContainsKey(triangle.C)) vertexTriangleMap[triangle.C] = triangleId;
        }

        // Now loop through the vertexTriangleMap and set normals for each vertex
        foreach (var kvp in vertexTriangleMap)
        {
            int vertexId = kvp.Key;
            int triangleId = kvp.Value;
            Normals[vertexId] = NormalForTriangle(triangleId);
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
    // MARK: Materials
    // --------------------------------------------------------------------------------------------




}
