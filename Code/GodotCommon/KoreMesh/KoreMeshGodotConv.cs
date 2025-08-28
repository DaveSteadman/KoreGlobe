using System;
using KoreCommon;
using Godot;

#nullable enable

// Conversion utilities between KoreMeshData coordinate system and Godot coordinate system.
//
// COORDINATE SYSTEMS:
// - KoreMeshData: X+ right, Y+ up, Z- forward (right-handed, Godot native)
//   UV: top-left origin (0,0), bottom-right (1,1), U+ Right, V+ Down (Godot native)
// - Godot: X+ right, Y+ up, Z+ backward (right-handed, Z flipped)
//
// CONVERSIONS NEEDED:
// - Position/Normals: Direct copy (coordinate systems now match)
// - UVs: Direct copy (both use top-left origin)
// - Triangle winding: Direct copy (both use CW when viewed from outside)
public static class KoreMeshGodotConv
{
    // --------------------------------------------------------------------------------------------
    // MARK: Position 
    // --------------------------------------------------------------------------------------------

    // Convert KoreXYZVector position to Godot Vector3.
    // Direct conversion - coordinate systems now match (both Z- forward)
    public static Vector3 PositionKoreToGodot(KoreXYZVector pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
    }

    // Convert Godot Vector3 position back to KoreXYZVector.
    // Direct conversion - coordinate systems match
    public static KoreXYZVector PositionGodotToKore(Vector3 pos)
    {
        return new KoreXYZVector(pos.X, pos.Y, pos.Z);
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Normal
    // --------------------------------------------------------------------------------------------

    // Convert KoreXYZVector normal to Godot Vector3.
    // Direct conversion - coordinate systems now match
    public static Vector3 NormalKoreToGodot(KoreXYZVector normal)
    {
        return new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z);
    }

    // Convert Godot Vector3 normal back to KoreXYZVector.
    // Direct conversion - coordinate systems match
    public static KoreXYZVector NormalGodotToKore(Vector3 normal)
    {
        return new KoreXYZVector(normal.X, normal.Y, normal.Z);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UV Conversions
    // --------------------------------------------------------------------------------------------

    // Convert KoreXYVector UV to Godot Vector2.
    // Direct conversion - both use bottom-left origin (0,0)
    public static Vector2 UVKoreToGodot(KoreXYVector uv)
    {
        return new Vector2((float)uv.X, (float)uv.Y);
    }

    // Convert Godot Vector2 UV back to KoreXYVector.
    // Direct conversion - both use bottom-left origin (0,0)
    public static KoreXYVector UVGodotToKore(Vector2 uv)
    {
        return new KoreXYVector(uv.X, uv.Y);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Color Conversions
    // --------------------------------------------------------------------------------------------

    // Convert KoreColorRGB to Godot Color.
    // Direct conversion, no coordinate system changes needed.
    public static Color ColorKoreToGodot(KoreColorRGB color)
    {
        return new Color(color.Rf, color.Gf, color.Bf, color.Af);
    }

    // Convert Godot Color back to KoreColorRGB.
    // Direct conversion, no coordinate system changes needed.
    public static KoreColorRGB ColorGodotToKore(Color color)
    {
        return new KoreColorRGB(color.R, color.G, color.B, color.A);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Winding
    // --------------------------------------------------------------------------------------------

    // Convert triangle winding for Godot.
    // Direct conversion - both systems now use the same coordinate system and winding
    public static KoreMeshTriangle TriangleKoreToGodot(KoreMeshTriangle tri)
    {
        return new KoreMeshTriangle(tri.A, tri.B, tri.C); // No change needed
    }

    public static KoreMeshTriangle TriangleGodotToKore(KoreMeshTriangle tri)
    {
        return new KoreMeshTriangle(tri.A, tri.B, tri.C); // No change needed
    }
}




