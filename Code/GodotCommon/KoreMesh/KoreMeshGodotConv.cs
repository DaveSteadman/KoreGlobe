using System;
using KoreCommon;
using Godot;

#nullable enable

/// <summary>
/// Conversion utilities between KoreMeshData coordinate system and Godot coordinate system.
///
/// COORDINATE SYSTEMS:
/// - KoreMeshData: X+ right, Y+ up, Z+ forward (right-handed)
/// - Godot: X+ right, Y+ up, Z+ backward (right-handed, Z flipped)
///
/// CONVERSIONS NEEDED:
/// - Position/Normals: Z component needs to be negated
/// - UVs: Already handled in KoreGodotSurfaceMesh (V flipped from top-left to bottom-left)
/// - Triangle winding: No change needed (both use CCW when viewed from outside)
/// </summary>
public static class KoreMeshGodotConv
{
    // --------------------------------------------------------------------------------------------
    // MARK: Position & Vector Conversions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Convert KoreXYZVector position to Godot Vector3.
    /// Flips Z axis: KoreMeshData Z+ forward → Godot Z+ backward
    /// </summary>
    public static Vector3 PositionKoreToGodot(KoreXYZVector pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, -(float)pos.Z);
    }

    /// <summary>
    /// Convert KoreXYZVector normal to Godot Vector3.
    /// Flips Z axis to match coordinate system conversion.
    /// </summary>
    public static Vector3 NormalKoreToGodot(KoreXYZVector normal)
    {
        return new Vector3((float)normal.X, (float)normal.Y, -(float)normal.Z);
    }

    /// <summary>
    /// Convert Godot Vector3 position back to KoreXYZVector.
    /// Flips Z axis: Godot Z+ backward → KoreMeshData Z+ forward
    /// </summary>
    public static KoreXYZVector PositionGodotToKore(Vector3 pos)
    {
        return new KoreXYZVector(pos.X, pos.Y, -pos.Z);
    }

    /// <summary>
    /// Convert Godot Vector3 normal back to KoreXYZVector.
    /// Flips Z axis to match coordinate system conversion.
    /// </summary>
    public static KoreXYZVector NormalGodotToKore(Vector3 normal)
    {
        return new KoreXYZVector(normal.X, normal.Y, -normal.Z);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UV Conversions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Convert KoreXYVector UV to Godot Vector2.
    /// Flips V axis: KoreMeshData top-left (0,0) → Godot bottom-left (0,0)
    /// </summary>
    public static Vector2 UVKoreToGodot(KoreXYVector uv)
    {
        return new Vector2((float)uv.X, 1.0f - (float)uv.Y);
    }

    /// <summary>
    /// Convert Godot Vector2 UV back to KoreXYVector.
    /// Flips V axis: Godot bottom-left (0,0) → KoreMeshData top-left (0,0)
    /// </summary>
    public static KoreXYVector UVGodotToKore(Vector2 uv)
    {
        return new KoreXYVector(uv.X, 1.0f - uv.Y);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Color Conversions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Convert KoreColorRGB to Godot Color.
    /// Direct conversion, no coordinate system changes needed.
    /// </summary>
    public static Color ColorKoreToGodot(KoreColorRGB color)
    {
        return new Color(color.Rf, color.Gf, color.Bf, color.Af);
    }

    /// <summary>
    /// Convert Godot Color back to KoreColorRGB.
    /// Direct conversion, no coordinate system changes needed.
    /// </summary>
    public static KoreColorRGB ColorGodotToKore(Color color)
    {
        return new KoreColorRGB(color.R, color.G, color.B, color.A);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle Winding
    // --------------------------------------------------------------------------------------------

    // /// <summary>
    // /// Convert triangle indices for Godot.
    // /// No change needed - both KoreMeshData and Godot use CCW winding when viewed from outside.
    // /// The Z-flip doesn't affect winding direction since we're flipping all vertices uniformly.
    // /// </summary>
    // public static (int, int, int) ConvertTriangleWinding(int a, int b, int c)
    // {
    //     return (a, b, c); // No change needed
    // }

    public static KoreMeshTriangle TriangleKoreToGodot(KoreMeshTriangle tri)
    {
        // Flip the 2nd and 3rd vertex indices
        return new KoreMeshTriangle(tri.A, tri.C, tri.B);
    }

    public static KoreMeshTriangle TriangleGodotToKore(KoreMeshTriangle tri)
    {
        // Flip the 2nd and 3rd vertex indices
        return new KoreMeshTriangle(tri.A, tri.C, tri.B);
    }
}