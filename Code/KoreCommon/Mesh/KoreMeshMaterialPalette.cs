using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

// KoreMeshMaterialPalette: A static class containing predefined materials accessible by name.
// Similar to KoreColorPalette and location palettes, provides a named dictionary of common materials.
// Useful for consistent material usage across projects and easy material referencing by string names.

public static class KoreMeshMaterialPalette
{
    public static readonly KoreMeshMaterial DefaultMaterial = new KoreMeshMaterial("MattWhite", KoreColorRGB.White, 0.0f, 0.8f);

    private static readonly List<KoreMeshMaterial> MaterialsList = new List<KoreMeshMaterial>
    {
        // --------------------------------------------------------------------------------------------
        // MARK: Basic Colors
        // --------------------------------------------------------------------------------------------
        
        new KoreMeshMaterial("MattWhite", new KoreColorRGB(255, 255, 255), 0f, 1f),
        new KoreMeshMaterial("MattGray", new KoreColorRGB(128, 128, 128), 0f, 1f),
        new KoreMeshMaterial("MattBlack", new KoreColorRGB(0, 0, 0), 0f, 1f),

        new KoreMeshMaterial("MattRed", new KoreColorRGB(255, 0, 0), 0f, 1f),
        new KoreMeshMaterial("MattGreen", new KoreColorRGB(0, 255, 0), 0f, 1f),
        new KoreMeshMaterial("MattBlue", new KoreColorRGB(0, 0, 255), 0f, 1f),
        new KoreMeshMaterial("MattYellow", new KoreColorRGB(255, 255, 0), 0f, 1f),
        new KoreMeshMaterial("MattMagenta", new KoreColorRGB(255, 0, 255), 0f, 1f),
        new KoreMeshMaterial("MattCyan", new KoreColorRGB(0, 255, 255), 0f, 1f),

        new KoreMeshMaterial("MattDarkRed", new KoreColorRGB(130, 0, 0), 0f, 1f),

        // --------------------------------------------------------------------------------------------
        // MARK: Metallic Materials
        // --------------------------------------------------------------------------------------------

        new KoreMeshMaterial("Metal", new KoreColorRGB(128, 128, 128), 1.0f, 0.2f),
        new KoreMeshMaterial("Gold", new KoreColorRGB(255, 214, 0), 1.0f, 0.1f),
        new KoreMeshMaterial("Silver", new KoreColorRGB(242, 242, 242), 1.0f, 0.1f),
        new KoreMeshMaterial("Copper", new KoreColorRGB(184, 115, 51), 1.0f, 0.2f),
        new KoreMeshMaterial("Bronze", new KoreColorRGB(163, 128, 61), 1.0f, 0.3f),
        new KoreMeshMaterial("Steel", new KoreColorRGB(179, 179, 179), 1.0f, 0.2f),
        new KoreMeshMaterial("Iron", new KoreColorRGB(143, 145, 148), 1.0f, 0.4f),
        new KoreMeshMaterial("Aluminum", new KoreColorRGB(232, 235, 235), 1.0f, 0.1f),

        // --------------------------------------------------------------------------------------------
        // MARK: Plastic Materials
        // --------------------------------------------------------------------------------------------
        
        new KoreMeshMaterial("PlasticRed", new KoreColorRGB(255, 0, 0), 0.0f, 0.8f),
        new KoreMeshMaterial("PlasticBlue", new KoreColorRGB(0, 0, 255), 0.0f, 0.8f),
        new KoreMeshMaterial("PlasticGreen", new KoreColorRGB(0, 255, 0), 0.0f, 0.8f),
        new KoreMeshMaterial("PlasticWhite", KoreColorRGB.White, 0.0f, 0.8f),
        new KoreMeshMaterial("PlasticBlack", new KoreColorRGB(0, 0, 0), 0.0f, 0.8f),
        new KoreMeshMaterial("PlasticYellow", new KoreColorRGB(255, 255, 0), 0.0f, 0.8f),

        // --------------------------------------------------------------------------------------------
        // MARK: Stone Materials
        // --------------------------------------------------------------------------------------------
        
        new KoreMeshMaterial("Marble", new KoreColorRGB(237, 237, 237), 0.0f, 0.1f),
        new KoreMeshMaterial("Granite", new KoreColorRGB(102, 102, 102), 0.0f, 0.8f),
        new KoreMeshMaterial("Sandstone", new KoreColorRGB(194, 179, 128), 0.0f, 0.9f),
        new KoreMeshMaterial("Concrete", new KoreColorRGB(140, 140, 140), 0.0f, 0.9f),

        // --------------------------------------------------------------------------------------------
        // MARK: Transparent Materials
        // --------------------------------------------------------------------------------------------

        new KoreMeshMaterial("Glass", new KoreColorRGB(250, 250, 250, 100), 0.0f, 0.0f), 
        new KoreMeshMaterial("SmokedGlass", new KoreColorRGB(77, 77, 77, 128), 0.0f, 0.1f),
        new KoreMeshMaterial("BlueGlass", new KoreColorRGB(51, 102, 204, 102), 0.0f, 0.0f),
        new KoreMeshMaterial("GreenGlass", new KoreColorRGB(51, 204, 102, 102), 0.0f, 0.0f),
        new KoreMeshMaterial("Water", new KoreColorRGB(0, 102, 204, 179), 0.0f, 0.0f),
        new KoreMeshMaterial("Ice", new KoreColorRGB(230, 242, 255, 204), 0.0f, 0.0f),

        // --------------------------------------------------------------------------------------------
        // MARK: Fabric Materials
        // --------------------------------------------------------------------------------------------
        
        new KoreMeshMaterial("Cotton", new KoreColorRGB(242, 242, 230), 0.0f, 1.0f),
        new KoreMeshMaterial("Silk", new KoreColorRGB(230, 217, 204), 0.0f, 0.2f),
        new KoreMeshMaterial("Leather", new KoreColorRGB(115, 77, 38), 0.0f, 0.7f),
        new KoreMeshMaterial("Rubber", new KoreColorRGB(38, 38, 38), 0.0f, 0.9f),

        // --------------------------------------------------------------------------------------------
        // MARK: Special Effect Materials
        // --------------------------------------------------------------------------------------------
        
        new KoreMeshMaterial("Chrome", new KoreColorRGB(242, 242, 242), 1.0f, 0.0f),
        new KoreMeshMaterial("Mirror", new KoreColorRGB(250, 250, 250), 1.0f, 0.0f),
        new KoreMeshMaterial("Ceramic", KoreColorRGB.White, 0.0f, 0.1f),
        new KoreMeshMaterial("Porcelain", new KoreColorRGB(250, 250, 245), 0.0f, 0.0f),
    };

    // --------------------------------------------------------------------------------------------
    // MARK: Helper Methods
    // --------------------------------------------------------------------------------------------

    // Find material by name, returns MattWhite if not found
    public static KoreMeshMaterial Find(string name)
    {
        foreach (var material in MaterialsList)
        {
            if (material.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return material;
        }

        // Return default MattWhite material if not found
        return DefaultMaterial;
    }

    // Get material by name, returns White if not found (backward compatibility)
    public static KoreMeshMaterial GetMaterial(string name)
    {
        return Find(name);
    }

    // Check if material exists in palette
    public static bool HasMaterial(string name)
    {
        foreach (var material in MaterialsList)
        {
            if (material.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    // --------------------------------------------------------------------------------------------

    // Get all material names
    public static string[] GetMaterialNames()
    {
        var names = new string[MaterialsList.Count];
        for (int i = 0; i < MaterialsList.Count; i++)
        {
            names[i] = MaterialsList[i].Name;
        }
        Array.Sort(names);
        return names;
    }

    // Get materials by category (basic approximation based on naming)
    public static List<KoreMeshMaterial> GetMaterialsByCategory(string category)
    {
        var result = new List<KoreMeshMaterial>();
        var categoryLower = category.ToLowerInvariant();

        foreach (var material in MaterialsList)
        {
            var nameLower = material.Name.ToLowerInvariant();
            bool matches = categoryLower switch
            {
                "metal" or "metallic" => material.IsMetallic,
                "plastic" => nameLower.Contains("plastic"),
                "glass" or "transparent" => material.IsTransparent,
                "wood" => nameLower.Contains("oak") || nameLower.Contains("pine") || 
                         nameLower.Contains("mahogany") || nameLower.Contains("walnut"),
                "stone" => nameLower.Contains("marble") || nameLower.Contains("granite") || 
                          nameLower.Contains("sandstone") || nameLower.Contains("concrete"),
                "fabric" => nameLower.Contains("cotton") || nameLower.Contains("silk") || 
                           nameLower.Contains("leather") || nameLower.Contains("rubber"),
                _ => false
            };

            if (matches)
                result.Add(material);
        }

        return result;
    }
}
