#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;

using KoreCommon;
using Godot;


// KoreGodotImageOperations: Static class holding functions to load images, and export them to textures and materials.
// - Class does not deal with anything async, just bare functions.

public static class KoreGodotImageOps
{
    // -----------------------------------------------------------------------------------
    // MARK: Images
    // -----------------------------------------------------------------------------------

    // Load image, useful for .webp
    // Usage: KoreGodotImageOps.LoadImage("res://images/texture.webp");

    public static Image? LoadImage(string filePath)
    {
        // Load image synchronously
        var image = new Image();
        var err = image.Load(filePath);

        if (err != Error.Ok) return null;
        if (image.IsEmpty()) return null;

        return image;
    }

    // -----------------------------------------------------------------------------------
    // MARK: Textures
    // -----------------------------------------------------------------------------------

    public static Texture2D? LoadTexture(string filePath)
    {
        // Load image synchronously
        var image = LoadImage(filePath);
        if (image == null)
        {
            KoreCentralLog.AddEntry($"LoadTexture: Failed to load image: {filePath}");
            return null;
        }

        // Create the texture synchronously - Note the create then set is thought to be required for webp
        var texture = new ImageTexture();
        texture.SetImage(image);

        // Free CPU memory used by the image, it was only ever a vehicle for creating the texture
        image.Dispose();

        return texture;
    }

    // -----------------------------------------------------------------------------------
    // MARK: Materials
    // -----------------------------------------------------------------------------------

    // Usage: KoreGodotImageOps.LoadMaterial("res://materials/my_material.tres");
    public static StandardMaterial3D? LoadMaterial(string filePath)
    {
        // Create the texture
        var texture = LoadTexture(filePath);
        if (texture == null)
        {
            KoreCentralLog.AddEntry($"LoadMaterial: Failed to load texture: {filePath}");
            return null;
        }

        // Create the material synchronously
        var material = new StandardMaterial3D
        {
            AlbedoTexture = texture,
            ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
        };

        return material;
    }


    // Usage: KoreGodotImageOps.LoadMaterial2("res://materials/my_material.tres");
    public static StandardMaterial3D? LoadMaterial2(string filePath)
    {
        // Create the texture
        var texture = LoadTexture(filePath);
        if (texture == null)
        {
            KoreCentralLog.AddEntry($"LoadMaterial2: Failed to load texture: {filePath}");
            return null;
        }

        // Create a material that responds to lighting
        var material = new StandardMaterial3D
        {
            AlbedoTexture = texture,
            ShadingMode = StandardMaterial3D.ShadingModeEnum.PerPixel, // Default lighting mode
            Metallic = 0.0f,        // Non-metallic for most textures
            Roughness = 0.8f,       // Slightly rough surface (0.0 = mirror, 1.0 = completely rough)
            //Specular = 0.5f,        // Moderate specular reflection
            
            // Enable back face culling for better performance
            CullMode = StandardMaterial3D.CullModeEnum.Back,
            
            // Ensure the material can receive shadows
            //FlagsDoNotReceiveShadows = false,
            
            // Optional: Add some ambient light contribution
            // Remove this line if you want pure lighting-based shading
            // Emission = new Color(0.05f, 0.05f, 0.05f) // Very slight self-illumination
        };

        return material;
    }
}


