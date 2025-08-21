using System;
using KoreCommon;
using Godot;

#nullable enable

// Conversion utilities between KoreMeshData coordinate system and Godot coordinate system.
//
// COORDINATE SYSTEMS:
// - KoreMeshData: X+ right, Y+ up, Z+ forward (right-handed)
// - Godot: X+ right, Y+ up, Z+ backward (right-handed, Z flipped)
//
// CONVERSIONS NEEDED:
// - Position/Normals: Z component needs to be negated
// - UVs: Already handled in KoreGodotSurfaceMesh (V flipped from top-left to bottom-left)
// - Triangle winding: No change needed (both use CCW when viewed from outside)
public static class KoreMeshGodotConv
{
    // --------------------------------------------------------------------------------------------
    // MARK: Position 
    // --------------------------------------------------------------------------------------------

    // Convert KoreXYZVector position to Godot Vector3.
    // Flips Z axis: KoreMeshData Z+ forward → Godot Z+ backward
    public static Vector3 PositionKoreToGodot(KoreXYZVector pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, -(float)pos.Z);
    }

    // Convert Godot Vector3 position back to KoreXYZVector.
    // Flips Z axis: Godot Z+ backward → KoreMeshData Z+ forward
    public static KoreXYZVector PositionGodotToKore(Vector3 pos)
    {
        return new KoreXYZVector(pos.X, pos.Y, -pos.Z);
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Normal
    // --------------------------------------------------------------------------------------------

    // Convert KoreXYZVector normal to Godot Vector3.
    // Flips Z axis to match coordinate system conversion.
    public static Vector3 NormalKoreToGodot(KoreXYZVector normal)
    {
        normal = normal.Invert();
        
        //return new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z);
        return new Vector3((float)normal.X, (float)normal.Y, -(float)normal.Z);
    }


    // Convert Godot Vector3 normal back to KoreXYZVector.
    // Flips Z axis to match coordinate system conversion.
    public static KoreXYZVector NormalGodotToKore(Vector3 normal)
    {
        KoreXYZVector koreNormal = new KoreXYZVector(normal.X, normal.Y, normal.Z);

        koreNormal = koreNormal.Invert();

        return koreNormal;
        
        //return new KoreXYZVector(normal.X, normal.Y, normal.Z);
        //return new KoreXYZVector(normal.X, normal.Y, -normal.Z);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UV Conversions
    // --------------------------------------------------------------------------------------------

    // Convert KoreXYVector UV to Godot Vector2.
    // Flips V axis: 
    // - KoreMeshData top-left is (0,0)
    // - Godot bottom-left is (0,0)
    public static Vector2 UVKoreToGodot(KoreXYVector uv)
    {
        return new Vector2((float)uv.X, -(float)uv.Y);
    }

    // Convert Godot Vector2 UV back to KoreXYVector.
    // Flips V axis: 
    // - KoreMeshData top-left is (0,0)
    // - Godot bottom-left is (0,0)
    public static KoreXYVector UVGodotToKore(Vector2 uv)
    {
        return new KoreXYVector(uv.X, -uv.Y);
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

    // Flipping the triangles

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




