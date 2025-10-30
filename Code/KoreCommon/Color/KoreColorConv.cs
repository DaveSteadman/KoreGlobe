// <fileheader>

using System;

// Static class to convert color structs

namespace KoreCommon;

public static class KoreColorConv
{
    // --------------------------------------------------------------------------------------------
    // MARK: HSV RGB
    // --------------------------------------------------------------------------------------------

    public static KoreColorHSV HSVtoRGB(KoreColorRGB rgb)
    {
        float r = rgb.R / 255f;
        float g = rgb.G / 255f;
        float b = rgb.B / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        float delta = max - min;

        float h = 0f;
        if (delta == 0f)
            h = 0f; // achromatic
        else if (max == r)
            h = (g - b) / delta % 6;
        else if (max == g)
            h = (b - r) / delta + 2;
        else
            h = (r - g) / delta + 4;

        h *= 60; // degrees

        if (h < 0f)
            h += 360f;

        float s = max == 0 ? 0 : delta / max;
        float v = max;

        return new KoreColorHSV(h, s, v, rgb.A);
    }

    // --------------------------------------------------------------------------------------------

    public static KoreColorRGB HSVtoRGB(KoreColorHSV hsv)
    {
        float r, g, b;

        float h = hsv.H / 60f;
        int i = (int)Math.Floor(h);
        float f = h - i;
        float p = hsv.V * (1 - hsv.S);
        float q = hsv.V * (1 - f * hsv.S);
        float t = hsv.V * (1 - (1 - f) * hsv.S);

        switch (i % 6)
        {
            case 0: r = hsv.V; g = t; b = p; break;
            case 1: r = q; g = hsv.V; b = p; break;
            case 2: r = p; g = hsv.V; b = t; break;
            case 3: r = p; g = q; b = hsv.V; break;
            case 4: r = t; g = p; b = hsv.V; break;
            case 5: r = hsv.V; g = p; b = q; break;
            default: r = g = b = 0f; break;
        }

        return new KoreColorRGB((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(hsv.A * 255));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: RGB Greyscale
    // --------------------------------------------------------------------------------------------

    public static KoreColorGreyscale RGBtoGreyscale(KoreColorRGB rgb)
    {
        // Using the luminosity method for greyscale conversion
        byte v = (byte)(0.299 * rgb.R + 0.587 * rgb.G + 0.114 * rgb.B);
        return new KoreColorGreyscale(v);
    }

    public static KoreColorRGB GreyscaleToRGB(KoreColorGreyscale greyscale)
    {
        // Convert greyscale to RGB by setting R, G, B to the same value
        return new KoreColorRGB(greyscale.V, greyscale.V, greyscale.V, 255);
    }

}

