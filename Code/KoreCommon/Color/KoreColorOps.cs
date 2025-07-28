using System;

// Static class to convert color structs
namespace KoreCommon;

public static class KoreColorOps
{
    private static Random rnd = new Random();

    // --------------------------------------------------------------------------------------------
    // MARK: Blend
    // --------------------------------------------------------------------------------------------

    // Usage: KoreColorOps.Lerp(col1, col2, t)
    // 0 = col1, 1 = col2, 0.5 = halfway between col1 and col2

    public static KoreColorRGB Lerp(KoreColorRGB col1, KoreColorRGB col2, float col2fraction)
    {
        if (col2fraction < 0.0f) col2fraction = 0.0f;
        if (col2fraction > 1.0f) col2fraction = 1.0f;

        float newR = col1.R + (col2.R - col1.R) * col2fraction;
        float newG = col1.G + (col2.G - col1.G) * col2fraction;
        float newB = col1.B + (col2.B - col1.B) * col2fraction;
        float newA = col1.A + (col2.A - col1.A) * col2fraction;

        return new KoreColorRGB(newR, newG, newB, newA);
    }

    // --------------------------------------------------------------------------------------------

    public static KoreColorRGB ReplaceColorWithTolerance(
        KoreColorRGB pixelColor,
        KoreColorRGB sourceColor,
        KoreColorRGB destinationColor,
        float tolerance)
    {
        // Calculate the Euclidean distance between the pixel color and the source color
        float distance = MathF.Sqrt(
            MathF.Pow(pixelColor.R - sourceColor.R, 2) +
            MathF.Pow(pixelColor.G - sourceColor.G, 2) +
            MathF.Pow(pixelColor.B - sourceColor.B, 2)
        );

        // If the distance is greater than the tolerance, return the original color
        if (distance > tolerance)
            return pixelColor;

        // Calculate the blend factor (1.0 means a perfect match, 0.0 means no match)
        float blendFactor = 1.0f - (distance / tolerance);

        // Blend the source and destination colors proportionally
        return KoreColorOps.Lerp(pixelColor, destinationColor, blendFactor);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Noise
    // --------------------------------------------------------------------------------------------

    // Function to output a new color with a random noise factor on each RGB channel

    // KoreColorRGB noiseCol = KoreColorOps.ColorWithRGBNoise(originalColor, 0.1f);
    public static KoreColorRGB ColorWithRGBNoise(KoreColorRGB color, float fractionNoise)
    {
        float rfnoise = (float)(rnd.NextDouble() - 0.5f) * fractionNoise;
        float gfnoise = (float)(rnd.NextDouble() - 0.5f) * fractionNoise;
        float bfnoise = (float)(rnd.NextDouble() - 0.5f) * fractionNoise;

        float rf = KoreValueUtils.Clamp(color.Rf + rfnoise, 0f, 1f);
        float gf = KoreValueUtils.Clamp(color.Gf + gfnoise, 0f, 1f);
        float bf = KoreValueUtils.Clamp(color.Bf + bfnoise, 0f, 1f);


        KoreColorRGB newColor = new KoreColorRGB(rf, gf, bf, color.Af);

        return newColor;
    }

    // --------------------------------------------------------------------------------------------

    // Function to output a new color with a random noise factor on the overall brightness of th RGB.
    public static KoreColorRGB ColorwithBrightnessNoise(KoreColorRGB color, float fractionNoise)
    {
        // Determine the adjustment multiplier
        float brightnessAdj = (float)(rnd.NextDouble() - 0.5f) * fractionNoise;

        // Apply the adjustment
        return new KoreColorRGB(
            color.R * brightnessAdj,
            color.G * brightnessAdj,
            color.B * brightnessAdj,
            color.A);
    }

}

