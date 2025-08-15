using System;
using System.Collections.Generic;
using System.IO;

using Godot;
using KoreCommon;


public static class KoreGodotMaterialFactory
{
    // --------------------------------------------------------------------------------------------
    // MARK: Mature / fixed functions
    // --------------------------------------------------------------------------------------------

    // Function to create a simple colored material
    // Usage: KoreGodotMaterialFactory.SimpleColoredMaterial(Color.Red);
    public static StandardMaterial3D SimpleColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Function to create a standard colored material (opaque with proper lighting)
    // This is the basic material that casts shadows properly
    // Usage: KoreGodotMaterialFactory.StandardColoredMaterial(new Color(0.8f, 0.2f, 0.2f));
    public static StandardMaterial3D StandardColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        material.Roughness = 0.7f;
        material.Metallic = 0.0f;

        // Settings to help with smooth shading
        material.ShadingMode  = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;

        // Ensure material is completely opaque and properly culled
        material.Transparency = BaseMaterial3D.TransparencyEnum.Disabled;
        material.CullMode = BaseMaterial3D.CullModeEnum.Back;
        material.NoDepthTest = false;
        material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;

        return material;
    }


    // // Function to create a flat-shaded material - proper lighting but uniform across each triangle
    // // Each triangle face gets one lighting value, not interpolated across vertices
    // // Usage: KoreGodotMaterialFactory.FlatShadedMaterial(new Color(0.8f, 0.2f, 0.2f));
    // public static StandardMaterial3D FlatShadedMaterial(Color color)
    // {
    //     StandardMaterial3D material = new StandardMaterial3D();
    //     material.AlbedoColor = color;
    //     material.Roughness = 0.7f;
    //     material.Metallic = 0.0f;

    //     // Use per-vertex shading (not per-pixel) for flat appearance
    //     // The key is to ensure each triangle has identical normals on all 3 vertices
    //     material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex;
    //     material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;

    //     // Standard opaque material settings
    //     material.Transparency = BaseMaterial3D.TransparencyEnum.Disabled;
    //     material.CullMode = BaseMaterial3D.CullModeEnum.Back;
    //     material.NoDepthTest = false;
    //     material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;

    //     return material;
    // }


    // --------------------------------------------------------------------------------------------

    // Function to create a transparent colored material
    // Color Alpha value sets the extent of transparency
    public static StandardMaterial3D TransparentColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;

        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: ShaderMaterial vertexColorMaterial = KoreGodotMaterialFactory.VertexColorMaterial();
    public static ShaderMaterial VertexColorMaterial()
    {
        // Create a 3D spatial shader inline - no file dependency
        string shaderCode = @"
shader_type spatial;

varying vec4 vertex_color;

void vertex() {
    vertex_color = COLOR;
}

void fragment() {
    ALBEDO = vertex_color.rgb;
    ALPHA = vertex_color.a;
}
";

        Shader shader = new Shader();
        shader.Code = shaderCode;

        ShaderMaterial material = new ShaderMaterial();
        material.Shader = shader;

        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Function to create a vertex-colored material (opaque with proper lighting)
    // Uses vertex colors but with StandardMaterial3D for proper shadow casting
    public static StandardMaterial3D VertexColorStandardMaterial()
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 1, 1, 1); // White base color
        material.VertexColorUseAsAlbedo = true; // Use vertex colors as the albedo
        material.Roughness = 0.7f;
        material.Metallic = 0.0f;

        // Settings to help with smooth shading
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;

        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Function to create a vertex-colored transparent material (with proper lighting)
    // Uses vertex colors with transparency and StandardMaterial3D for proper shadow casting
    public static StandardMaterial3D VertexColorTransparentStandardMaterial()
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 1, 1, 1); // White base color
        material.VertexColorUseAsAlbedo = true; // Use vertex colors as the albedo
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        material.Roughness = 0.7f;
        material.Metallic = 0.0f;
        material.NoDepthTest = false; // Keep depth testing for proper sorting
        material.CullMode = BaseMaterial3D.CullModeEnum.Back; // Standard back-face culling

        // Settings to help with smooth shading
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;

        return material;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Image
    // --------------------------------------------------------------------------------------------

    // Create a material from a texture2D 
    // Usage: KoreGodotMaterialFactory.Texture2DMaterial(myTexture);
    public static StandardMaterial3D Texture2DMaterial(Texture2D texture)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoTexture = texture;
        material.Roughness = 0.7f;
        material.Metallic = 0.0f;

        // Settings to help with smooth shading
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;

        return material;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: KoreMeshMaterial Conversion
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Converts a KoreMeshMaterial to Godot's StandardMaterial3D.
    /// Usage: StandardMaterial3D material = KoreGodotMaterialFactory.FromKoreMaterial(koreMaterial);
    /// </summary>
    public static StandardMaterial3D FromKoreMaterial(KoreMeshMaterial koreMaterial)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        
        // Convert base color
        Color godotColor = KoreConvColor.ToGodotColor(koreMaterial.BaseColor);
        material.AlbedoColor = godotColor;
        
        // Set metallic and roughness properties
        material.Metallic = koreMaterial.Metallic;
        material.Roughness = koreMaterial.Roughness;
        
        // Handle transparency
        if (koreMaterial.IsTransparent)
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        }
        else
        {
            material.Transparency = BaseMaterial3D.TransparencyEnum.Disabled;
        }
        
        // Set up proper shading
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.PerPixel;
        material.SpecularMode = BaseMaterial3D.SpecularModeEnum.SchlickGgx;
        
        // Standard settings for proper rendering
        material.CullMode = BaseMaterial3D.CullModeEnum.Back;
        material.NoDepthTest = false;
        
        if (!koreMaterial.IsTransparent)
        {
            material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.OpaqueOnly;
        }
        
        return material;
    }



}
    