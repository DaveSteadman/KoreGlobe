using System;

#nullable enable

namespace KoreCommon;

// KoreMiniMeshMaterial: A record struct to hold material properties for 3D geometry.
// Designed to be compatible with GLTF material properties and Godot's StandardMaterial3D.
// Keeps materials as simple data containers that rendering layers can interpret.
// Note: Alpha/transparency is handled by the BaseColor's A channel.
// Name field enables GLTF round-trip and semantic material identification.

public struct KoreMiniMeshMaterial : IEquatable<KoreMiniMeshMaterial>
{

    public string       Name      { get; set; }     // Material name for GLTF export/import and identification
    public KoreColorRGB BaseColor { get; set; }     // Includes RGBA - alpha channel handles transparency
    public float        Metallic  { get; set; }     // 0 = dielectric (plastic/wood), 1 = metallic
    public float        Roughness { get; set; }     // 0 = mirror smooth, 1 = completely rough

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public KoreMiniMeshMaterial(string name, KoreColorRGB baseColor, float metallic = 0.0f, float roughness = 0.7f, string? filename = null)
    {
        Name      = name;
        BaseColor = baseColor;
        Metallic  = metallic;
        Roughness = roughness;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Default materials
    // --------------------------------------------------------------------------------------------

    public static KoreMiniMeshMaterial White => KoreMiniMeshMaterialPalette.GetMaterial("MattWhite");

    // --------------------------------------------------------------------------------------------
    // MARK: Helper Methods
    // --------------------------------------------------------------------------------------------

    // Create a material with just a color (using default material properties)
    public static KoreMiniMeshMaterial FromColor(string name, KoreColorRGB color)
    {
        return new KoreMiniMeshMaterial(name, color);
    }

    // Create a material with just a color (anonymous)
    public static KoreMiniMeshMaterial FromColor(KoreColorRGB color)
    {
        return new KoreMiniMeshMaterial("Anonymous", color);
    }

    // Create a material from a filename with fallback color
    // Usage: KoreMiniMeshMaterial.FromTexture("MyTexture", "path/to/texture.png", KoreMiniMeshMaterial.White.BaseColor)
    public static KoreMiniMeshMaterial FromTexture(string name, string filename, KoreColorRGB fallbackColor)
    {
        return new KoreMiniMeshMaterial(name, fallbackColor, filename: filename);
    }

    // Create a transparent version of this material
    public KoreMiniMeshMaterial WithAlpha(float alpha)
    {
        // Create new color with specified alpha
        var newColor = new KoreColorRGB(BaseColor.Rf, BaseColor.Gf, BaseColor.Bf, alpha);
        return this with { BaseColor = newColor };
    }

    // Create a metallic version of this material
    public KoreMiniMeshMaterial AsMetallic(float metallic = 1.0f, float roughness = 0.2f)
    {
        return this with { Metallic = metallic, Roughness = roughness };
    }

    // Create a plastic/matte version of this material
    public KoreMiniMeshMaterial AsPlastic(float roughness = 0.8f)
    {
        return this with { Metallic = 0.0f, Roughness = roughness };
    }

    // Check if this material is transparent
    public bool IsTransparent => BaseColor.IsTransparent;

    // Check if this material is metallic
    public bool IsMetallic => Metallic > 0.5f;

    // --------------------------------------------------------------------------------------------
    // MARK: String Representation
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        if (BaseColor.IsTransparent)
            return $"Material({Name}, {BaseColor}, M:{Metallic:F1}, R:{Roughness:F1})";
        else
            return $"Material({Name}, {BaseColor}, M:{Metallic:F1}, R:{Roughness:F1})";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Equals
    // --------------------------------------------------------------------------------------------

    public bool Equals(KoreMiniMeshMaterial other)
    {
        bool baseColorMatch = BaseColor.Equals(other.BaseColor);
        bool metallicMatch  = KoreValueUtils.EqualsWithinTolerance(Metallic, other.Metallic);
        bool roughnessMatch = KoreValueUtils.EqualsWithinTolerance(Roughness, other.Roughness);

        return Name == other.Name &&
               baseColorMatch &&
               metallicMatch &&
               roughnessMatch;
    }

    public override bool Equals(object? obj)
    {
        return obj is KoreMiniMeshMaterial other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, BaseColor, Metallic, Roughness);
    }

}
