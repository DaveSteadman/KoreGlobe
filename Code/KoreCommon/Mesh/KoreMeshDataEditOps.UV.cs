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

}
