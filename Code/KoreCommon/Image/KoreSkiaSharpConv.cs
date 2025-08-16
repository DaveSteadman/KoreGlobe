using SkiaSharp;
using System;
using System.IO;
using System.Collections.Generic;

namespace KoreCommon.SkiaSharp;

// KoreSkiaSharpConv: Convert between Kore and SK class types.

public static class KoreSkiaSharpConv
{
    // --------------------------------------------------------------------------------------------
    // MARK: Point/Vector
    // --------------------------------------------------------------------------------------------

    public static SKPoint ToSKPoint(this KoreXYVector vector)
    {
        return new SKPoint((float)vector.X, (float)vector.Y);
    }

    public static SKPoint ToSKPoint(this KoreXYZVector vector)
    {
        return new SKPoint((float)vector.X, (float)vector.Y);
    }

    // --------------------------------------------------------------------------------------------

    public static KoreXYVector ToKoreXYVector(this SKPoint point)
    {
        return new KoreXYVector(point.X, point.Y);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Rect
    // --------------------------------------------------------------------------------------------

    public static SKRect ToSKRect(this Kore2DGridSize size)
    {
        return new SKRect(0, 0, size.Width, size.Height);
    }

    public static SKRect ToSKRect(this KoreXYRect rect)
    {
        return new SKRect(
            (float)rect.Left, (float)rect.Top, (float)rect.Right, (float)rect.Bottom);
    }

    public static SKRect ToSKRect(float x, float y, float width, float height)
    {
        return new SKRect(x, y, x + width, y + height);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Color
    // --------------------------------------------------------------------------------------------

    public static SKColor ToSKColor(this KoreColorRGB color)
    {
        return new SKColor(
            (byte)(color.R * 255),
            (byte)(color.G * 255),
            (byte)(color.B * 255),
            255);
    }

    public static SKColor ToSKColor(this KoreColorRGB color, float alpha)
    {
        return new SKColor(
            (byte)(color.R * 255),
            (byte)(color.G * 255),
            (byte)(color.B * 255),
            (byte)(alpha * 255));
    }

    public static KoreColorRGB ToKoreColorRGB(this SKColor color)
    {
        float colR = (float)color.Red;
        float colG = (float)color.Green;
        float colB = (float)color.Blue;

        return new KoreColorRGB(colR, colG, colB);
    }

}