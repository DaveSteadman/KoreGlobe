// <fileheader>

using System;
using System.Collections.Generic;
using System.IO;

using Godot;
using KoreCommon;

#nullable enable


public static class KoreMiniMeshGodotMaterialFactory
{

    // Usage: StandardMaterial3D mat = KoreMiniMeshGodotMaterialFactory.MiniMeshMaterial(koreMaterial);
    public static StandardMaterial3D MiniMeshMaterial(KoreMiniMeshMaterial mat)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = KoreMeshGodotConv.ColorKoreToGodot(mat.BaseColor);
        material.Roughness   = mat.Roughness;
        material.Metallic    = mat.Metallic;

        GD.Print($"MiniMeshMaterial: Name: {mat.Name}, Color: {KoreColorIO.RBGtoHexString(mat.BaseColor)}, M:{mat.Metallic:F1}, R:{mat.Roughness:F1}");

        // Handle transparency based on alpha
        if (mat.BaseColor.A < KoreColorIO.MaxByte)
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

    // Usage: StandardMaterial3D mat = KoreMiniMeshGodotMaterialFactory.GetUnlitLineMaterial();
    public static StandardMaterial3D GetUnlitLineMaterial()
    {
        var material = new StandardMaterial3D();

        // Make it unlit so lines are always bright
        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;

        // Enable vertex colors so line colors work
        material.VertexColorUseAsAlbedo = true;

        // Optional: Add some brightness boost
        material.AlbedoColor = new Color(1.2f, 1.2f, 1.2f, 1.0f); // Slightly brighter than normal

        // Disable depth testing if you want lines to always show on top (optional)
        // material.NoDepthTest = true;

        return material;
    }

    // --------------------------------------------------------------------------------------------

    // When we color each triangle differently, and want to see this with just one mesh, we use the
    // VertexColorUseAsAlbedo option in the material.
    // Usage: StandardMaterial3D mat = KoreMiniMeshGodotMaterialFactory.FromVertexColors(transparent: true);

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
}


