// <fileheader>

using System;
using System.Collections.Generic;
using System.IO;

using Godot;
using KoreCommon;

#nullable enable


public static class KoreColorMeshGodotMaterialFactory
{

    // When we color each triangle differently, and want to see this with just one mesh, we use the 
    // VertexColorUseAsAlbedo option in the material.
    // Usage: StandardMaterial3D mat = KoreColorMeshGodotMaterialFactory.FromVertexColors(transparent: true);

    public static StandardMaterial3D FromVertexColors(bool transparent = false)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 1, 1, 1); // White base
        material.VertexColorUseAsAlbedo = true;
        material.Roughness = 0.8f;
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


