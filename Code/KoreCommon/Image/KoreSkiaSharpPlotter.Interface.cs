// <fileheader>

using SkiaSharp;
using System;
using System.Collections.Generic;

namespace KoreCommon.SkiaSharp;

// KoreSkiaSharpPlotter.Interface: Conversion functions between Kore types and SK types
// - Provides seamless integration between KoreCommon types and SkiaSharp plotting
// - Extension methods for easy conversion and plotting with Kore data structures

public partial class KoreSkiaSharpPlotter
{
    // --------------------------------------------------------------------------------------------
    // MARK: Point
    // --------------------------------------------------------------------------------------------

    public void DrawPoint(KoreXYVector v, int crossSize = 3)        => DrawPointAsCross(KoreSkiaSharpConv.ToSKPoint(v), crossSize);
    public void DrawPointAsCross(KoreXYVector v, int crossSize = 3) => DrawPointAsCross(KoreSkiaSharpConv.ToSKPoint(v), crossSize);
    public void DrawPointAsCircle(KoreXYVector v, int radius = 3)   => DrawPointAsCircle(KoreSkiaSharpConv.ToSKPoint(v), radius);

    // --------------------------------------------------------------------------------------------
    // MARK: Line
    // --------------------------------------------------------------------------------------------

    public void DrawLine(KoreXYVector v1, KoreXYVector v2) => DrawLine(KoreSkiaSharpConv.ToSKPoint(v1), KoreSkiaSharpConv.ToSKPoint(v2));

    // --------------------------------------------------------------------------------------------
    // MARK: Triangle
    // --------------------------------------------------------------------------------------------

    public void DrawTriangle(KoreXYVector p1, KoreXYVector p2, KoreXYVector p3) => DrawTriangle(KoreSkiaSharpConv.ToSKPoint(p1), KoreSkiaSharpConv.ToSKPoint(p2), KoreSkiaSharpConv.ToSKPoint(p3));

    // --------------------------------------------------------------------------------------------
    // MARK: Rect
    // --------------------------------------------------------------------------------------------

    public void DrawRect(KoreXYRect rect, SKPaint fillPaint)
    {
        SKRect skRect = KoreSkiaSharpConv.ToSKRect(rect);
        DrawRect(skRect, fillPaint);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Color
    // --------------------------------------------------------------------------------------------

    public void SetColor(KoreColorRGB color)
    {
        DrawSettings.Color = KoreSkiaSharpConv.ToSKColor(color);
    }

    public void SetColor(KoreColorRGB color, float alpha)
    {
        DrawSettings.Color = KoreSkiaSharpConv.ToSKColor(color, alpha);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Text
    // --------------------------------------------------------------------------------------------

    public void DrawText(string text, KoreXYVector pos, int fontSize = 12)             => DrawText(text, KoreSkiaSharpConv.ToSKPoint(pos), fontSize);
    public void DrawTextCentered(string text, KoreXYVector pos, float fontSize = 12) => DrawTextCentered(text, KoreSkiaSharpConv.ToSKPoint(pos), fontSize);

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh Data Drawing
    // --------------------------------------------------------------------------------------------

    public void DrawMeshWireframe(KoreMeshData meshData, KoreColorRGB? lineColor = null)
    {
        if (lineColor.HasValue)
        {
            DrawSettings.Color = KoreSkiaSharpConv.ToSKColor(lineColor.Value);
        }

        // Draw all lines in the mesh
        foreach (var line in meshData.Lines.Values)
        {
            if (meshData.Vertices.TryGetValue(line.A, out var vertexA) &&
                meshData.Vertices.TryGetValue(line.B, out var vertexB))
            {
                DrawLine(KoreSkiaSharpConv.ToSKPoint(vertexA), KoreSkiaSharpConv.ToSKPoint(vertexB));
            }
        }
    }

    public void DrawMeshPoints(KoreMeshData meshData, KoreColorRGB? pointColor = null, int pointSize = 3)
    {
        if (pointColor.HasValue)
        {
            DrawSettings.Color = KoreSkiaSharpConv.ToSKColor(pointColor.Value);
        }

        // Draw all vertices as points
        foreach (var vertex in meshData.Vertices.Values)
        {
            DrawPointAsCross(KoreSkiaSharpConv.ToSKPoint(vertex), pointSize);
        }
    }

    public void DrawMeshTriangles(KoreMeshData meshData, KoreColorRGB? fillColor = null)
    {
        if (fillColor.HasValue)
        {
            DrawSettings.Color = KoreSkiaSharpConv.ToSKColor(fillColor.Value);
            DrawSettings.Paint.Style = SKPaintStyle.Fill;
        }

        // Draw all triangles as filled shapes
        foreach (var triangle in meshData.Triangles.Values)
        {
            if (meshData.Vertices.TryGetValue(triangle.A, out var vertexA) &&
                meshData.Vertices.TryGetValue(triangle.B, out var vertexB) &&
                meshData.Vertices.TryGetValue(triangle.C, out var vertexC))
            {
                DrawTriangle(KoreSkiaSharpConv.ToSKPoint(vertexA), KoreSkiaSharpConv.ToSKPoint(vertexB), KoreSkiaSharpConv.ToSKPoint(vertexC));
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Path Drawing (for Bezier curves)
    // --------------------------------------------------------------------------------------------

    public void DrawPath(List<KoreXYVector> pathPoints, KoreColorRGB? lineColor = null)
    {
        if (pathPoints.Count < 2) return;

        if (lineColor.HasValue)
        {
            DrawSettings.Color = KoreSkiaSharpConv.ToSKColor(lineColor.Value);
        }

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            DrawLine(KoreSkiaSharpConv.ToSKPoint(pathPoints[i]), KoreSkiaSharpConv.ToSKPoint(pathPoints[i + 1]));
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Godot Integration (In-Memory)
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates a Godot ImageTexture from a SkiaSharp bitmap without file I/O
    /// </summary>
    // public static Godot.ImageTexture CreateGodotTexture(this SKBitmap bitmap)
    // {
    //     // Convert SKBitmap to PNG byte array
    //     using var image = bitmap.Encode(SKEncodedImageFormat.Png, 100);
    //     byte[] pngData = image.ToArray();

    //     // Create Godot Image from PNG data
    //     var godotImage = new Godot.Image();
    //     godotImage.LoadPngFromBuffer(pngData);

    //     // Create ImageTexture from the Image
    //     var texture = new Godot.ImageTexture();
    //     texture.SetImage(godotImage);

    //     return texture;
    // }

    /// <summary>
    /// Creates a Godot ImageTexture from the plotter's current canvas
    /// </summary>
    // public static Godot.ImageTexture CreateGodotTexture(this KoreSkiaSharpPlotter plotter)
    // {
    //     return plotter.GetBitmap().CreateGodotTexture();
    // }

    // /// <summary>
    // /// Applies the plotter's current canvas as a texture to a Sprite2D node
    // /// </summary>
    // public static void ApplyToSprite2D(this KoreSkiaSharpPlotter plotter, Godot.Sprite2D sprite)
    // {
    //     sprite.Texture = plotter.CreateGodotTexture();
    // }

    /// <summary>
    /// Creates a new Sprite2D node with the plotter's current canvas as texture
    /// </summary>
    // public static Godot.Sprite2D CreateSprite2D(this KoreSkiaSharpPlotter plotter)
    // {
    //     var sprite = new Godot.Sprite2D();
    //     sprite.Texture = plotter.CreateGodotTexture();
    //     return sprite;
    // }

    /// <summary>
    /// Gets the PNG byte array from the plotter's current canvas
    /// </summary>
    public byte[] GetPngBytes()
    {
        using var image = GetBitmap().Encode(SKEncodedImageFormat.Png, 100);
        return image.ToArray();
    }

    /// <summary>
    /// Gets the PNG byte array from a SkiaSharp bitmap
    /// </summary>
    // public static byte[] GetPngBytes(this SKBitmap bitmap)
    // {
    //     using var image = bitmap.Encode(SKEncodedImageFormat.Png, 100);
    //     return image.ToArray();
    // }

}
