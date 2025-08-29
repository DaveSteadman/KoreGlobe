// using System;
// using System.Collections.Generic;
// using System.Linq;

// #nullable enable

// namespace KoreCommon;

// /// <summary>
// /// Static validity and cleanup operations for KoreMeshData
// /// Contains methods for mesh validation, cleanup, and population of missing data
// /// </summary>
// public static partial class KoreMeshDataEditOps
// {
//     // --------------------------------------------------------------------------------------------
//     // MARK: ID Management
//     // --------------------------------------------------------------------------------------------

//     /// <summary>
//     /// Reset the Next IDs, looking for the max values in the current lists
//     /// Note that after numerous operations, the IDs can be non-sequential, so we need to find the max value in each list.
//     /// </summary>
//     public static void ResetMaxIDs(KoreMeshData mesh)
//     {
//         // Reset the next IDs based on the current max values in the dictionaries
//         mesh.NextVertexId   = (mesh.Vertices.Count > 0 ? mesh.Vertices.Keys.Max() + 1 : 0);
//         mesh.NextLineId     = (mesh.Lines.Count > 0 ? mesh.Lines.Keys.Max() + 1 : 0);
//         mesh.NextTriangleId = (mesh.Triangles.Count > 0 ? mesh.Triangles.Keys.Max() + 1 : 0);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: High-Level Validity Operations
//     // --------------------------------------------------------------------------------------------

//     /// <summary>
//     /// Fully populate the mesh data with matching normals, UVs, vertex colors, line colors, and triangle colors.
//     /// </summary>
//     public static void FullyPopulate(KoreMeshData mesh)
//     {
//         CreateMissingNormals(mesh); // Points
//         CreateMissingUVs(mesh);
//         CreateMissingVertexColors(mesh);
//         CreateMissingLineColors(mesh); // Lines
//     }

//     /// <summary>
//     /// Examine the vertex list, and remove any orphaned or duplicate lines, triangles, and colors.
//     /// </summary>
//     public static void MakeValid(KoreMeshData mesh)
//     {
//         RemoveOrphanedPoints(mesh);
//         RemoveDuplicatePoints(mesh);

//         RemoveBrokenNormals(mesh); // Remove normals that don't have supporting point IDs.

//         RemoveBrokenUVs(mesh); // Remove UVs that don't have supporting point IDs.

//         RemoveBrokenLines(mesh); // Remove lines that don't have supporting point IDs.
//         RemoveDuplicateLines(mesh);

//         RemoveBrokenLineColors(mesh); // Remove line colors that don't have supporting line IDs.

//         RemoveBrokenTriangles(mesh); // Remove triangles that don't have supporting point IDs.
//         RemoveDuplicateTriangles(mesh);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Bounding Box
//     // --------------------------------------------------------------------------------------------

//     /// <summary>
//     /// Loop through the vertices, recording the max/min X, Y, Z values. Then return a KoreXYZBox
//     /// </summary>
//     public static KoreXYZBox GetBoundingBox(KoreMeshData mesh)
//     {
//         if (mesh.Vertices.Count == 0)
//             return KoreXYZBox.Zero;

//         double minX = double.MaxValue, maxX = double.MinValue;
//         double minY = double.MaxValue, maxY = double.MinValue;
//         double minZ = double.MaxValue, maxZ = double.MinValue;

//         foreach (var kvp in mesh.Vertices)
//         {
//             KoreXYZVector vertex = kvp.Value;
//             if (vertex.X < minX) minX = vertex.X;
//             if (vertex.X > maxX) maxX = vertex.X;
//             if (vertex.Y < minY) minY = vertex.Y;
//             if (vertex.Y > maxY) maxY = vertex.Y;
//             if (vertex.Z < minZ) minZ = vertex.Z;
//             if (vertex.Z > maxZ) maxZ = vertex.Z;
//         }

//         KoreXYZVector center = new KoreXYZVector((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
//         double width = maxX - minX;
//         double height = maxY - minY;
//         double length = maxZ - minZ;

//         return new KoreXYZBox(center, width, height, length);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Vertices
//     // --------------------------------------------------------------------------------------------




//     // --------------------------------------------------------------------------------------------
//     // MARK: UVs
//     // --------------------------------------------------------------------------------------------

//     /// <summary>
//     /// Remove UVs that don't have supporting vertex IDs
//     /// </summary>
//     public static void RemoveBrokenUVs(KoreMeshData mesh)
//     {
//         var invalidUVIds = mesh.UVs.Keys.Where(id => !mesh.Vertices.ContainsKey(id)).ToList();
//         foreach (int uvId in invalidUVIds)
//         {
//             mesh.UVs.Remove(uvId);
//         }
//     }

//     /// <summary>
//     /// Create missing UVs for vertices
//     /// </summary>
//     public static void CreateMissingUVs(KoreMeshData mesh, KoreXYVector? defaultUV = null)
//     {
//         KoreXYVector uv = defaultUV ?? new KoreXYVector(0, 0);

//         foreach (int vertexId in mesh.Vertices.Keys)
//         {
//             if (!mesh.UVs.ContainsKey(vertexId))
//             {
//                 mesh.UVs[vertexId] = uv;
//             }
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Vertex Colors
//     // --------------------------------------------------------------------------------------------

//     /// <summary>
//     /// Create missing vertex colors
//     /// </summary>
//     public static void CreateMissingVertexColors(KoreMeshData mesh, KoreColorRGB? defaultColor = null)
//     {
//         KoreColorRGB color = defaultColor ?? KoreColorRGB.White;

//         foreach (int vertexId in mesh.Vertices.Keys)
//         {
//             if (!mesh.VertexColors.ContainsKey(vertexId))
//             {
//                 mesh.VertexColors[vertexId] = color;
//             }
//         }
//     }



//     /// <summary>
//     /// Remove line colors that don't have supporting line IDs
//     /// </summary>
//     public static void RemoveBrokenLineColors(KoreMeshData mesh)
//     {
//         var invalidLineColorIds = mesh.LineColors.Keys.Where(id => !mesh.Lines.ContainsKey(id)).ToList();
//         foreach (int lineColorId in invalidLineColorIds)
//         {
//             mesh.LineColors.Remove(lineColorId);
//         }
//     }

//     /// <summary>
//     /// Create missing line colors
//     /// </summary>
//     public static void CreateMissingLineColors(KoreMeshData mesh, KoreColorRGB? defaultColor = null)
//     {
//         KoreColorRGB color = defaultColor ?? KoreColorRGB.White;

//         foreach (int lineId in mesh.Lines.Keys)
//         {
//             if (!mesh.LineColors.ContainsKey(lineId))
//             {
//                 mesh.LineColors[lineId] = new KoreMeshLineColour(color, color);
//             }
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Triangles
//     // --------------------------------------------------------------------------------------------

//     /// <summary>
//     /// Remove triangles that don't have supporting vertex IDs
//     /// </summary>
//     public static void RemoveBrokenTriangles(KoreMeshData mesh)
//     {
//         var invalidTriangleIds = mesh.Triangles.Where(kvp =>
//             !mesh.Vertices.ContainsKey(kvp.Value.A) ||
//             !mesh.Vertices.ContainsKey(kvp.Value.B) ||
//             !mesh.Vertices.ContainsKey(kvp.Value.C))
//             .Select(kvp => kvp.Key)
//             .ToList();

//         foreach (int triangleId in invalidTriangleIds)
//         {
//             mesh.Triangles.Remove(triangleId);
//         }
//     }

//     /// <summary>
//     /// Remove duplicate triangles
//     /// </summary>
//     public static void RemoveDuplicateTriangles(KoreMeshData mesh)
//     {
//         var trianglesToRemove = new List<int>();
//         var trianglesArray = mesh.Triangles.ToArray();

//         for (int i = 0; i < trianglesArray.Length; i++)
//         {
//             for (int j = i + 1; j < trianglesArray.Length; j++)
//             {
//                 var tri1 = trianglesArray[i].Value;
//                 var tri2 = trianglesArray[j].Value;

//                 // Check if triangles have the same vertices (any permutation)
//                 var vertices1 = new HashSet<int> { tri1.A, tri1.B, tri1.C };
//                 var vertices2 = new HashSet<int> { tri2.A, tri2.B, tri2.C };

//                 if (vertices1.SetEquals(vertices2))
//                 {
//                     trianglesToRemove.Add(trianglesArray[j].Key);
//                 }
//             }
//         }

//         foreach (int triangleId in trianglesToRemove)
//         {
//             mesh.Triangles.Remove(triangleId);
//         }
//     }
// }
