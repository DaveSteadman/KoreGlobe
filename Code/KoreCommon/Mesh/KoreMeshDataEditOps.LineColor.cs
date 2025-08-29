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
    // MARK: Vertex Colors
    // --------------------------------------------------------------------------------------------




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

}
