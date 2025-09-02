using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

public static partial class KoreMiniMeshOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Vertices
    // --------------------------------------------------------------------------------------------



    // --------------------------------------------------------------------------------------------
    // MARK: Colors
    // --------------------------------------------------------------------------------------------

    public static bool HasColor(KoreMiniMesh mesh, int colorId) { return mesh.Colors.ContainsKey(colorId); }
    public static bool HasColor(KoreMiniMesh mesh, KoreColorRGB color) { return mesh.Colors.ContainsValue(color); }
    

    public static int GetColorId(KoreMiniMesh mesh, KoreColorRGB color)
    {
        foreach (var kvp in mesh.Colors)
        {
            if (kvp.Value.Equals(color))
                return kvp.Key;
        }
        return -1;
    }
    
    public static int GetOrCreateColorId(KoreMiniMesh mesh, KoreColorRGB color)
    {
        int id = GetColorId(mesh, color);
        if (id >= 0)
            return id;

        return mesh.AddColor(color);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lines
    // --------------------------------------------------------------------------------------------



    // --------------------------------------------------------------------------------------------
    // MARK: Triangles
    // --------------------------------------------------------------------------------------------


    // --------------------------------------------------------------------------------------------
    // MARK: Group
    // --------------------------------------------------------------------------------------------

    public static KoreMiniMeshGroup GetOrCreateGroup(KoreMiniMesh mesh, string groupName)
    {
        if (!mesh.HasGroup(groupName))
            mesh.AddGroup(groupName, new KoreMiniMeshGroup(-1, new List<int>()));

        return mesh.GetGroup(groupName);
    }

    public static void SetGroupColor(KoreMiniMesh mesh, string groupName, int newColorId)
    {
        if (mesh.HasGroup(groupName))
        {
            KoreMiniMeshGroup group = mesh.GetGroup(groupName);
            group = group with { ColorId = newColorId };
            mesh.Groups[groupName] = group; // re-assign the modified group back to the dictionary
        }
    }

    public static void SetGroupColor(KoreMiniMesh mesh, string groupName, KoreColorRGB color)
    {
        if (mesh.HasGroup(groupName))
        {
            int newColorId = GetOrCreateColorId(mesh, color);
            SetGroupColor(mesh, groupName, newColorId);
        }
    }

    public static void AddTrianglesToGroup(KoreMiniMesh mesh, string groupName)
    {
        // create the group if it doesn't exist
        if (!mesh.HasGroup(groupName))
            mesh.AddGroup(groupName, new KoreMiniMeshGroup(-1, new List<int>()));

        // get the group
        KoreMiniMeshGroup group = mesh.GetGroup(groupName);

        // Add the whole triangle list to the group
        foreach (int currTriId in mesh.Triangles.Keys)
            group.TriIdList.Add(currTriId);
    }

}
