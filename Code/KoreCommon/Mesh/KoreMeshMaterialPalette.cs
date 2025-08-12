using System;
using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

// KoreMeshMaterialPalette: A static class containing predefined materials accessible by name.
// Similar to KoreColorPalette and location palettes, provides a named dictionary of common materials.
// Useful for consistent material usage across projects and easy material referencing by string names.

public static class KoreMeshMaterialPalette
{
    public static readonly Dictionary<string, KoreMeshMaterial> Materials = new Dictionary<string, KoreMeshMaterial>
    {
        // --------------------------------------------------------------------------------------------
        // MARK: Basic Colors
        // --------------------------------------------------------------------------------------------
        
        { "Red",         new KoreMeshMaterial("Red", new KoreColorRGB(255, 0, 0)) },
        { "Green",       new KoreMeshMaterial("Green", new KoreColorRGB(0, 255, 0)) },
        { "Blue",        new KoreMeshMaterial("Blue", new KoreColorRGB(0, 0, 255)) },
        { "White",       new KoreMeshMaterial("White", KoreColorRGB.White) },
        { "Black",       new KoreMeshMaterial("Black", new KoreColorRGB(0, 0, 0)) },
        { "Gray",        new KoreMeshMaterial("Gray", new KoreColorRGB(128, 128, 128)) },
        { "Yellow",      new KoreMeshMaterial("Yellow", new KoreColorRGB(255, 255, 0)) },
        { "Magenta",     new KoreMeshMaterial("Magenta", new KoreColorRGB(255, 0, 255)) },
        { "Cyan",        new KoreMeshMaterial("Cyan", new KoreColorRGB(0, 255, 255)) },

        // --------------------------------------------------------------------------------------------
        // MARK: Metallic Materials
        // --------------------------------------------------------------------------------------------
        
        { "Metal",       new KoreMeshMaterial("Metal", new KoreColorRGB(128, 128, 128), 1.0f, 0.2f) },
        { "Gold",        new KoreMeshMaterial("Gold", new KoreColorRGB(1.0f, 0.84f, 0.0f), 1.0f, 0.1f) },
        { "Silver",      new KoreMeshMaterial("Silver", new KoreColorRGB(0.95f, 0.95f, 0.95f), 1.0f, 0.1f) },
        { "Copper",      new KoreMeshMaterial("Copper", new KoreColorRGB(0.72f, 0.45f, 0.20f), 1.0f, 0.2f) },
        { "Bronze",      new KoreMeshMaterial("Bronze", new KoreColorRGB(0.64f, 0.50f, 0.24f), 1.0f, 0.3f) },
        { "Steel",       new KoreMeshMaterial("Steel", new KoreColorRGB(0.70f, 0.70f, 0.70f), 1.0f, 0.2f) },
        { "Iron",        new KoreMeshMaterial("Iron", new KoreColorRGB(0.56f, 0.57f, 0.58f), 1.0f, 0.4f) },
        { "Aluminum",    new KoreMeshMaterial("Aluminum", new KoreColorRGB(0.91f, 0.92f, 0.92f), 1.0f, 0.1f) },

        // --------------------------------------------------------------------------------------------
        // MARK: Plastic Materials
        // --------------------------------------------------------------------------------------------
        
        { "PlasticRed",    new KoreMeshMaterial("PlasticRed", new KoreColorRGB(255, 0, 0), 0.0f, 0.8f) },
        { "PlasticBlue",   new KoreMeshMaterial("PlasticBlue", new KoreColorRGB(0, 0, 255), 0.0f, 0.8f) },
        { "PlasticGreen",  new KoreMeshMaterial("PlasticGreen", new KoreColorRGB(0, 255, 0), 0.0f, 0.8f) },
        { "PlasticWhite",  new KoreMeshMaterial("PlasticWhite", KoreColorRGB.White, 0.0f, 0.8f) },
        { "PlasticBlack",  new KoreMeshMaterial("PlasticBlack", new KoreColorRGB(0, 0, 0), 0.0f, 0.8f) },
        { "PlasticYellow", new KoreMeshMaterial("PlasticYellow", new KoreColorRGB(255, 255, 0), 0.0f, 0.8f) },

        // --------------------------------------------------------------------------------------------
        // MARK: Wood Materials
        // --------------------------------------------------------------------------------------------
        
        { "Oak",         new KoreMeshMaterial("Oak", new KoreColorRGB(0.65f, 0.50f, 0.39f), 0.0f, 0.9f) },
        { "Pine",        new KoreMeshMaterial("Pine", new KoreColorRGB(0.85f, 0.75f, 0.45f), 0.0f, 0.9f) },
        { "Mahogany",    new KoreMeshMaterial("Mahogany", new KoreColorRGB(0.42f, 0.26f, 0.15f), 0.0f, 0.8f) },
        { "Walnut",      new KoreMeshMaterial("Walnut", new KoreColorRGB(0.40f, 0.28f, 0.18f), 0.0f, 0.9f) },

        // --------------------------------------------------------------------------------------------
        // MARK: Stone Materials
        // --------------------------------------------------------------------------------------------
        
        { "Marble",      new KoreMeshMaterial("Marble", new KoreColorRGB(0.93f, 0.93f, 0.93f), 0.0f, 0.1f) },
        { "Granite",     new KoreMeshMaterial("Granite", new KoreColorRGB(0.40f, 0.40f, 0.40f), 0.0f, 0.8f) },
        { "Sandstone",   new KoreMeshMaterial("Sandstone", new KoreColorRGB(0.76f, 0.70f, 0.50f), 0.0f, 0.9f) },
        { "Concrete",    new KoreMeshMaterial("Concrete", new KoreColorRGB(0.55f, 0.55f, 0.55f), 0.0f, 0.9f) },

        // --------------------------------------------------------------------------------------------
        // MARK: Transparent Materials
        // --------------------------------------------------------------------------------------------
        
        { "Glass",         new KoreMeshMaterial("Glass", new KoreColorRGB(255, 255, 255, 77), 0.0f, 0.0f) }, // ~30% alpha
        { "SmokedGlass",   new KoreMeshMaterial("SmokedGlass", new KoreColorRGB(0.3f, 0.3f, 0.3f, 0.5f), 0.0f, 0.1f) },
        { "BlueGlass",     new KoreMeshMaterial("BlueGlass", new KoreColorRGB(0.2f, 0.4f, 0.8f, 0.4f), 0.0f, 0.0f) },
        { "GreenGlass",    new KoreMeshMaterial("GreenGlass", new KoreColorRGB(0.2f, 0.8f, 0.4f, 0.4f), 0.0f, 0.0f) },
        { "Water",         new KoreMeshMaterial("Water", new KoreColorRGB(0.0f, 0.4f, 0.8f, 0.7f), 0.0f, 0.0f) },
        { "Ice",           new KoreMeshMaterial("Ice", new KoreColorRGB(0.9f, 0.95f, 1.0f, 0.8f), 0.0f, 0.0f) },

        // --------------------------------------------------------------------------------------------
        // MARK: Fabric Materials
        // --------------------------------------------------------------------------------------------
        
        { "Cotton",      new KoreMeshMaterial("Cotton", new KoreColorRGB(0.95f, 0.95f, 0.90f), 0.0f, 1.0f) },
        { "Silk",        new KoreMeshMaterial("Silk", new KoreColorRGB(0.90f, 0.85f, 0.80f), 0.0f, 0.2f) },
        { "Leather",     new KoreMeshMaterial("Leather", new KoreColorRGB(0.45f, 0.30f, 0.15f), 0.0f, 0.7f) },
        { "Rubber",      new KoreMeshMaterial("Rubber", new KoreColorRGB(0.15f, 0.15f, 0.15f), 0.0f, 0.9f) },

        // --------------------------------------------------------------------------------------------
        // MARK: Special Effect Materials
        // --------------------------------------------------------------------------------------------
        
        { "Chrome",      new KoreMeshMaterial("Chrome", new KoreColorRGB(0.95f, 0.95f, 0.95f), 1.0f, 0.0f) },
        { "Mirror",      new KoreMeshMaterial("Mirror", new KoreColorRGB(0.98f, 0.98f, 0.98f), 1.0f, 0.0f) },
        { "Ceramic",     new KoreMeshMaterial("Ceramic", KoreColorRGB.White, 0.0f, 0.1f) },
        { "Porcelain",   new KoreMeshMaterial("Porcelain", new KoreColorRGB(0.98f, 0.98f, 0.96f), 0.0f, 0.0f) },
    };

    // --------------------------------------------------------------------------------------------
    // MARK: Helper Methods
    // --------------------------------------------------------------------------------------------

    // Get material by name, returns White if not found
    public static KoreMeshMaterial GetMaterial(string name)
    {
        return Materials.TryGetValue(name, out var material) ? material : KoreMeshMaterial.White;
    }

    // Check if material exists in palette
    public static bool HasMaterial(string name)
    {
        return Materials.ContainsKey(name);
    }

    // Get all material names
    public static string[] GetMaterialNames()
    {
        var names = new string[Materials.Count];
        Materials.Keys.CopyTo(names, 0);
        Array.Sort(names);
        return names;
    }

    // Get materials by category (basic approximation based on naming)
    public static Dictionary<string, KoreMeshMaterial> GetMaterialsByCategory(string category)
    {
        var result = new Dictionary<string, KoreMeshMaterial>();
        var categoryLower = category.ToLowerInvariant();

        foreach (var kvp in Materials)
        {
            var nameLower = kvp.Key.ToLowerInvariant();
            bool matches = categoryLower switch
            {
                "metal" or "metallic" => kvp.Value.IsMetallic,
                "plastic" => nameLower.Contains("plastic"),
                "glass" or "transparent" => kvp.Value.IsTransparent,
                "wood" => nameLower.Contains("oak") || nameLower.Contains("pine") || 
                         nameLower.Contains("mahogany") || nameLower.Contains("walnut"),
                "stone" => nameLower.Contains("marble") || nameLower.Contains("granite") || 
                          nameLower.Contains("sandstone") || nameLower.Contains("concrete"),
                "fabric" => nameLower.Contains("cotton") || nameLower.Contains("silk") || 
                           nameLower.Contains("leather") || nameLower.Contains("rubber"),
                _ => false
            };

            if (matches)
                result[kvp.Key] = kvp.Value;
        }

        return result;
    }
}
