using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

/// <summary>
/// Static validity and cleanup operations for KoreMeshData
/// Contains methods for mesh validation, cleanup, and population of missing data
/// </summary>
public static partial class KoreMeshDataEditOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: ID Management
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Reset the Next IDs, looking for the max values in the current lists
    /// Note that after numerous operations, the IDs can be non-sequential, so we need to find the max value in each list.
    /// </summary>
    public static void ResetMaxIDs(KoreMeshData mesh)
    {
        // Reset the next IDs based on the current max values in the dictionaries
        mesh.NextVertexId   = (mesh.Vertices.Count > 0 ? mesh.Vertices.Keys.Max() + 1 : 0);
        mesh.NextLineId     = (mesh.Lines.Count > 0 ? mesh.Lines.Keys.Max() + 1 : 0);
        mesh.NextTriangleId = (mesh.Triangles.Count > 0 ? mesh.Triangles.Keys.Max() + 1 : 0);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: High-Level Validity Operations
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Fully populate the mesh data with matching normals, UVs, vertex colors, line colors, and triangle colors.
    /// </summary>
    public static void FullyPopulate(KoreMeshData mesh)
    {
        CreateMissingNormals(mesh); // Points
        CreateMissingUVs(mesh);
        CreateMissingVertexColors(mesh);
        CreateMissingLineColors(mesh); // Lines
    }

    /// <summary>
    /// Examine the vertex list, and remove any orphaned or duplicate lines, triangles, and colors.
    /// </summary>
    public static void MakeValid(KoreMeshData mesh)
    {
        RemoveOrphanedPoints(mesh);
        RemoveDuplicatePoints(mesh);

        RemoveBrokenNormals(mesh); // Remove normals that don't have supporting point IDs.

        RemoveBrokenUVs(mesh); // Remove UVs that don't have supporting point IDs.

        RemoveBrokenLines(mesh); // Remove lines that don't have supporting point IDs.
        RemoveDuplicateLines(mesh);

        RemoveBrokenLineColors(mesh); // Remove line colors that don't have supporting line IDs.

        RemoveBrokenTriangles(mesh); // Remove triangles that don't have supporting point IDs.
        RemoveDuplicateTriangles(mesh);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Bounding Box
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Loop through the vertices, recording the max/min X, Y, Z values. Then return a KoreXYZBox
    /// </summary>
    public static KoreXYZBox GetBoundingBox(KoreMeshData mesh)
    {
        if (mesh.Vertices.Count == 0)
            return KoreXYZBox.Zero;

        double minX = double.MaxValue, maxX = double.MinValue;
        double minY = double.MaxValue, maxY = double.MinValue;
        double minZ = double.MaxValue, maxZ = double.MinValue;

        foreach (var kvp in mesh.Vertices)
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

    /// <summary>
    /// Create a list of all the point IDs that are used in lines or triangles, then remove any points that are not in that list.
    /// </summary>
    public static void RemoveOrphanedPoints(KoreMeshData mesh)
    {
        // Determine which vertices are referenced by lines or triangles
        HashSet<int> referencedVertices = new HashSet<int>();

        // Add vertices referenced by lines
        foreach (var line in mesh.Lines.Values)
        {
            referencedVertices.Add(line.A);
            referencedVertices.Add(line.B);
        }

        // Add vertices referenced by triangles
        foreach (var triangle in mesh.Triangles.Values)
        {
            referencedVertices.Add(triangle.A);
            referencedVertices.Add(triangle.B);
            referencedVertices.Add(triangle.C);
        }

        // Remove unreferenced vertices
        var verticesToRemove = mesh.Vertices.Keys.Where(id => !referencedVertices.Contains(id)).ToList();

        foreach (int vertexId in verticesToRemove)
        {
            mesh.Vertices.Remove(vertexId);
            mesh.Normals.Remove(vertexId);
            mesh.VertexColors.Remove(vertexId);
            mesh.UVs.Remove(vertexId);
        }
    }

    /// <summary>
    /// Remove duplicate points within tolerance
    /// </summary>
    public static void RemoveDuplicatePoints(KoreMeshData mesh, double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        var verticesArray = mesh.Vertices.ToArray();
        var toRemove = new List<int>();

        for (int i = 0; i < verticesArray.Length; i++)
        {
            for (int j = i + 1; j < verticesArray.Length; j++)
            {
                var vertex1 = verticesArray[i];
                var vertex2 = verticesArray[j];

                // Check if vertices are within tolerance
                if (Math.Abs(vertex1.Value.X - vertex2.Value.X) < tolerance &&
                    Math.Abs(vertex1.Value.Y - vertex2.Value.Y) < tolerance &&
                    Math.Abs(vertex1.Value.Z - vertex2.Value.Z) < tolerance)
                {
                    // Mark the second vertex for removal and update references
                    toRemove.Add(vertex2.Key);

                    // Update line references
                    foreach (var lineKvp in mesh.Lines)
                    {
                        var line = lineKvp.Value;
                        if (line.A == vertex2.Key) line.A = vertex1.Key;
                        if (line.B == vertex2.Key) line.B = vertex1.Key;
                        mesh.Lines[lineKvp.Key] = line;
                    }

                    // Update triangle references
                    foreach (var triangleKvp in mesh.Triangles)
                    {
                        var triangle = triangleKvp.Value;
                        if (triangle.A == vertex2.Key) triangle.A = vertex1.Key;
                        if (triangle.B == vertex2.Key) triangle.B = vertex1.Key;
                        if (triangle.C == vertex2.Key) triangle.C = vertex1.Key;
                        mesh.Triangles[triangleKvp.Key] = triangle;
                    }
                }
            }
        }

        // Remove duplicate vertices
        foreach (int vertexId in toRemove)
        {
            mesh.Vertices.Remove(vertexId);
            mesh.Normals.Remove(vertexId);
            mesh.VertexColors.Remove(vertexId);
            mesh.UVs.Remove(vertexId);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Normals
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove normals that don't have supporting vertex IDs
    /// </summary>
    public static void RemoveBrokenNormals(KoreMeshData mesh)
    {
        var invalidNormalIds = mesh.Normals.Keys.Where(id => !mesh.Vertices.ContainsKey(id)).ToList();
        foreach (int normalId in invalidNormalIds)
        {
            mesh.Normals.Remove(normalId);
        }
    }

    /// <summary>
    /// Create missing normals for vertices
    /// </summary>
    public static void CreateMissingNormals(KoreMeshData mesh, KoreXYZVector? defaultNormal = null)
    {
        KoreXYZVector normal = defaultNormal ?? new KoreXYZVector(0, 1, 0); // Default to up vector

        foreach (int vertexId in mesh.Vertices.Keys)
        {
            if (!mesh.Normals.ContainsKey(vertexId))
            {
                mesh.Normals[vertexId] = normal;
            }
        }
    }

    /// <summary>
    /// Calculate normal for a triangle
    /// </summary>
    public static KoreXYZVector NormalForTriangle(KoreMeshData mesh, int triangleId)
    {
        if (!mesh.Triangles.ContainsKey(triangleId))
            return new KoreXYZVector(0, 1, 0);

        var triangle = mesh.Triangles[triangleId];

        if (!mesh.Vertices.ContainsKey(triangle.A) ||
            !mesh.Vertices.ContainsKey(triangle.B) ||
            !mesh.Vertices.ContainsKey(triangle.C))
            return new KoreXYZVector(0, 1, 0);

        var vertexA = mesh.Vertices[triangle.A];
        var vertexB = mesh.Vertices[triangle.B];
        var vertexC = mesh.Vertices[triangle.C];

        // Calculate cross product for normal
        var edge1 = vertexB - vertexA;
        var edge2 = vertexC - vertexA;

        // Manual cross product calculation: edge1 × edge2
        var crossX = edge1.Y * edge2.Z - edge1.Z * edge2.Y;
        var crossY = edge1.Z * edge2.X - edge1.X * edge2.Z;
        var crossZ = edge1.X * edge2.Y - edge1.Y * edge2.X;

        var normal = new KoreXYZVector(crossX, crossY, crossZ);

        // Normalize the normal vector
        var length = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);
        if (length > 0.0001) // Avoid division by zero
        {
            normal = new KoreXYZVector(normal.X / length, normal.Y / length, normal.Z / length);
        }
        else
        {
            normal = new KoreXYZVector(0, 1, 0); // Default up vector
        }

        return normal;
    }

    /// <summary>
    /// Set normal from first triangle that uses this vertex
    /// </summary>
    public static void SetNormalFromFirstTriangle(KoreMeshData mesh, int vertexId)
    {
        foreach (var triangleKvp in mesh.Triangles)
        {
            var triangle = triangleKvp.Value;
            if (triangle.A == vertexId || triangle.B == vertexId || triangle.C == vertexId)
            {
                var normal = NormalForTriangle(mesh, triangleKvp.Key);
                mesh.Normals[vertexId] = normal;
                return;
            }
        }

        // If no triangle found, set default normal
        mesh.Normals[vertexId] = new KoreXYZVector(0, 1, 0);
    }

    /// <summary>
    /// Set normals from triangles for all vertices
    /// </summary>
    public static void SetNormalsFromTriangles(KoreMeshData mesh)
    {
        // Clear existing normals
        mesh.Normals.Clear();

        // Calculate normals for each vertex based on triangles
        foreach (int vertexId in mesh.Vertices.Keys)
        {
            SetNormalFromFirstTriangle(mesh, vertexId);
        }
    }

    /// <summary>
    /// Calculate normals for each triangle and assign to vertices
    /// Usage: KoreMeshDataEditOps.CalcNormalsForAllTriangles(mesh);
    /// </summary>
    public static void CalcNormalsForAllTriangles(KoreMeshData mesh)
    {
        foreach (var kvp in mesh.Triangles)
        {
            CalcNormalsForTriangle(mesh, kvp.Key);
        }
    }

    /// <summary>
    /// Calculate normal for a specific triangle and assign to its vertices
    /// </summary>
    public static KoreXYZVector CalcNormalsForTriangle(KoreMeshData mesh, int triangleId)
    {
        if (!mesh.Triangles.ContainsKey(triangleId))
            return KoreXYZVector.Zero;

        // Get the vertices
        KoreMeshTriangle triangle = mesh.Triangles[triangleId];
        KoreXYZVector a = mesh.Vertices[triangle.A];
        KoreXYZVector b = mesh.Vertices[triangle.B];
        KoreXYZVector c = mesh.Vertices[triangle.C];

        // Calculate the face normal using cross product
        KoreXYZVector ab = b - a;  // Vector from A to B
        KoreXYZVector ac = c - a;  // Vector from A to C
        KoreXYZVector faceNormal = KoreXYZVector.CrossProduct(ab, ac).Normalize();

        // Normalize and invert the face normal
        faceNormal = faceNormal.Normalize();
        faceNormal = faceNormal.Invert(); // Required Step - no explanation

        // Set the normals
        mesh.Normals[triangle.A] = faceNormal;
        mesh.Normals[triangle.B] = faceNormal;
        mesh.Normals[triangle.C] = faceNormal;

        return faceNormal;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UVs
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove UVs that don't have supporting vertex IDs
    /// </summary>
    public static void RemoveBrokenUVs(KoreMeshData mesh)
    {
        var invalidUVIds = mesh.UVs.Keys.Where(id => !mesh.Vertices.ContainsKey(id)).ToList();
        foreach (int uvId in invalidUVIds)
        {
            mesh.UVs.Remove(uvId);
        }
    }

    /// <summary>
    /// Create missing UVs for vertices
    /// </summary>
    public static void CreateMissingUVs(KoreMeshData mesh, KoreXYVector? defaultUV = null)
    {
        KoreXYVector uv = defaultUV ?? new KoreXYVector(0, 0);

        foreach (int vertexId in mesh.Vertices.Keys)
        {
            if (!mesh.UVs.ContainsKey(vertexId))
            {
                mesh.UVs[vertexId] = uv;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Vertex Colors
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Create missing vertex colors
    /// </summary>
    public static void CreateMissingVertexColors(KoreMeshData mesh, KoreColorRGB? defaultColor = null)
    {
        KoreColorRGB color = defaultColor ?? KoreColorRGB.White;

        foreach (int vertexId in mesh.Vertices.Keys)
        {
            if (!mesh.VertexColors.ContainsKey(vertexId))
            {
                mesh.VertexColors[vertexId] = color;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove lines that don't have supporting vertex IDs
    /// </summary>
    public static void RemoveBrokenLines(KoreMeshData mesh)
    {
        var invalidLineIds = mesh.Lines.Where(kvp =>
            !mesh.Vertices.ContainsKey(kvp.Value.A) ||
            !mesh.Vertices.ContainsKey(kvp.Value.B))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (int lineId in invalidLineIds)
        {
            mesh.Lines.Remove(lineId);
            mesh.LineColors.Remove(lineId);
        }
    }

    /// <summary>
    /// Remove duplicate lines
    /// </summary>
    public static void RemoveDuplicateLines(KoreMeshData mesh)
    {
        var linesToRemove = new List<int>();
        var linesArray = mesh.Lines.ToArray();

        for (int i = 0; i < linesArray.Length; i++)
        {
            for (int j = i + 1; j < linesArray.Length; j++)
            {
                var line1 = linesArray[i].Value;
                var line2 = linesArray[j].Value;

                // Check if lines are the same (either direction)
                if ((line1.A == line2.A && line1.B == line2.B) ||
                    (line1.A == line2.B && line1.B == line2.A))
                {
                    linesToRemove.Add(linesArray[j].Key);
                }
            }
        }

        foreach (int lineId in linesToRemove)
        {
            mesh.Lines.Remove(lineId);
            mesh.LineColors.Remove(lineId);
        }
    }

    /// <summary>
    /// Remove line colors that don't have supporting line IDs
    /// </summary>
    public static void RemoveBrokenLineColors(KoreMeshData mesh)
    {
        var invalidLineColorIds = mesh.LineColors.Keys.Where(id => !mesh.Lines.ContainsKey(id)).ToList();
        foreach (int lineColorId in invalidLineColorIds)
        {
            mesh.LineColors.Remove(lineColorId);
        }
    }

    /// <summary>
    /// Create missing line colors
    /// </summary>
    public static void CreateMissingLineColors(KoreMeshData mesh, KoreColorRGB? defaultColor = null)
    {
        KoreColorRGB color = defaultColor ?? KoreColorRGB.White;

        foreach (int lineId in mesh.Lines.Keys)
        {
            if (!mesh.LineColors.ContainsKey(lineId))
            {
                mesh.LineColors[lineId] = new KoreMeshLineColour(color, color);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Remove triangles that don't have supporting vertex IDs
    /// </summary>
    public static void RemoveBrokenTriangles(KoreMeshData mesh)
    {
        var invalidTriangleIds = mesh.Triangles.Where(kvp =>
            !mesh.Vertices.ContainsKey(kvp.Value.A) ||
            !mesh.Vertices.ContainsKey(kvp.Value.B) ||
            !mesh.Vertices.ContainsKey(kvp.Value.C))
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (int triangleId in invalidTriangleIds)
        {
            mesh.Triangles.Remove(triangleId);
        }
    }

    /// <summary>
    /// Remove duplicate triangles
    /// </summary>
    public static void RemoveDuplicateTriangles(KoreMeshData mesh)
    {
        var trianglesToRemove = new List<int>();
        var trianglesArray = mesh.Triangles.ToArray();

        for (int i = 0; i < trianglesArray.Length; i++)
        {
            for (int j = i + 1; j < trianglesArray.Length; j++)
            {
                var tri1 = trianglesArray[i].Value;
                var tri2 = trianglesArray[j].Value;

                // Check if triangles have the same vertices (any permutation)
                var vertices1 = new HashSet<int> { tri1.A, tri1.B, tri1.C };
                var vertices2 = new HashSet<int> { tri2.A, tri2.B, tri2.C };

                if (vertices1.SetEquals(vertices2))
                {
                    trianglesToRemove.Add(trianglesArray[j].Key);
                }
            }
        }

        foreach (int triangleId in trianglesToRemove)
        {
            mesh.Triangles.Remove(triangleId);
        }
    }
}
