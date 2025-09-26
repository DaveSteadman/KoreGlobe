using System;
using System.Collections.Generic;
using System.IO;

using Godot;
using KoreCommon;

#nullable enable


public static class KoreGodotMaterialFactory
{
    // --------------------------------------------------------------------------------------------
    // MARK: Color
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a standard material from a color (handles transparency automatically).
    /// Usage: KoreGodotMaterialFactory.SimpleColoredMaterial(Color.Red);
    /// </summary>
    public static StandardMaterial3D SimpleColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        material.Roughness = 0.7f;
        material.Metallic = 0.0f;

        // Handle transparency based on alpha
        if (color.A < 1.0f)
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        }
        else
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Disabled;
            material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;
        }

        // Standard shading settings
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;
        material.CullMode = BaseMaterial3D.CullModeEnum.Back;
        material.NoDepthTest = false;

        return material;
    }

    // Usage: StandardMaterial3D mat = KoreGodotMaterialFactory.MiniMeshMaterial(koreMaterial);
    public static StandardMaterial3D MiniMeshMaterial(KoreMiniMeshMaterial mat)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = KoreMeshGodotConv.ColorKoreToGodot(mat.BaseColor);
        material.Roughness   = mat.Roughness;
        material.Metallic    = mat.Metallic;

        // Handle transparency based on alpha
        if (mat.BaseColor.A < 1.0f)
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        }
        else
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Disabled;
            material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;
        }

        // Standard shading settings
        material.ShadingMode   = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode  = BaseMaterial3D.SpecularModeEnum.SchlickGgx;
        material.CullMode      = BaseMaterial3D.CullModeEnum.Back;
        material.NoDepthTest   = false;

        return material;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Vertex
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a vertex-colored material for gradient effects.
    /// Usage: KoreGodotMaterialFactory.FromVertexColors();
    /// </summary>
    public static StandardMaterial3D FromVertexColors(bool transparent = false)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 1, 1, 1); // White base
        material.VertexColorUseAsAlbedo = true;
        material.Roughness = 0.7f;
        material.Metallic = 0.0f;

        // Handle transparency
        if (transparent)
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        }
        else
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Disabled;
            material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;
        }

        // Standard shading settings
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;
        material.CullMode = BaseMaterial3D.CullModeEnum.Back;
        material.NoDepthTest = false;

        return material;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Texture
    // --------------------------------------------------------------------------------------------

    public static StandardMaterial3D? FromTexture(string filename, string? searchPath = null)
    {
        string? resolvedPath = ResolveTexturePath(filename, searchPath);
        if (string.IsNullOrEmpty(resolvedPath))
        {
            GD.Print($"Failed to find texture: {filename}");
            return null;
        }

        var material = KoreGodotImageOps.LoadMaterial2(resolvedPath);
        if (material != null)
        {
            GD.Print($"Successfully loaded texture material: {resolvedPath}");
            return material;
        }

        GD.Print($"Failed to load texture: {resolvedPath}");
        return null;
    }
 
    // --------------------------------------------------------------------------------------------
    // MARK: Kore Mesh Material 
    // --------------------------------------------------------------------------------------------

    public static StandardMaterial3D FromKoreMaterial(KoreMeshMaterial koreMaterial, string? basePath = null)
    {
        // Try texture first if filename is provided
        if (!string.IsNullOrEmpty(koreMaterial.Filename))
        {
            var textureMaterial = FromTexture(koreMaterial.Filename, basePath);
            if (textureMaterial != null)
            {
                // Apply material properties to the texture material
                textureMaterial.Metallic = koreMaterial.Metallic;
                textureMaterial.Roughness = koreMaterial.Roughness;

                if (koreMaterial.IsTransparent)
                {
                    textureMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
                }

                return textureMaterial;
            }

            KoreCentralLog.AddEntry($"Texture failed, falling back to color for material: {koreMaterial.Name}");
        }

        // Fallback to color-based material
        Color godotColor = KoreConvColor.ToGodotColor(koreMaterial.BaseColor);
        var colorMaterial = SimpleColoredMaterial(godotColor);

        // Apply material properties
        colorMaterial.Metallic  = koreMaterial.Metallic;
        colorMaterial.Roughness = koreMaterial.Roughness;

        return colorMaterial;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Helper Functions
    // --------------------------------------------------------------------------------------------

    private static string? ResolveTexturePath(string filename, string? basePath)
    {
        // If we have a base path, try relative to that first
        if (!string.IsNullOrEmpty(basePath))
        {
            string? baseDirectory = File.Exists(basePath) ? Path.GetDirectoryName(basePath) : basePath;
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                string fullPath = Path.Combine(baseDirectory, filename);
                if (File.Exists(fullPath))
                    return fullPath;
            }
        }

        // Try just the filename (absolute path or current directory)
        if (File.Exists(filename))
            return filename;

        // Nothing found
        return null;
    }

}


