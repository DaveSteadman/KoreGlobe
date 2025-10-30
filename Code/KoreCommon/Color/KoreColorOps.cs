// <fileheader>

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

        float newR = col1.Rf + (col2.Rf - col1.Rf) * col2fraction;
        float newG = col1.Gf + (col2.Gf - col1.Gf) * col2fraction;
        float newB = col1.Bf + (col2.Bf - col1.Bf) * col2fraction;
        float newA = col1.Af + (col2.Af - col1.Af) * col2fraction;

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
            MathF.Pow(pixelColor.Rf - sourceColor.Rf, 2) +
            MathF.Pow(pixelColor.Gf - sourceColor.Gf, 2) +
            MathF.Pow(pixelColor.Bf - sourceColor.Bf, 2)
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
    // Usage: KoreColorRGB noiseCol = KoreColorOps.ColorwithBrightnessNoise(originalColor, 0.1f);
    public static KoreColorRGB ColorwithBrightnessNoise(KoreColorRGB color, float fractionNoise)
    {
        // Determine the adjustment multiplier
        float brightnessAdj = (float)(rnd.NextDouble() - 0.5f) * fractionNoise;

        // Apply the adjustment
        return new KoreColorRGB(
            color.Rf * brightnessAdj,
            color.Gf * brightnessAdj,
            color.Bf * brightnessAdj,
            color.Af);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Distance
    // --------------------------------------------------------------------------------------------

    // When comparing colors, we often want to know how "far apart" they are. We'll use sum of differences
    // in the RGBA values.

    // Note: The human eye is more sensitive to green, then red, then blue, so this "geometric" distance,
    // also called Euclidean or Manhattan distance, is not ideal for perceptual color matching. Images will
    // have green areas with fewer available colours than (perceived) red or blue areas.

    // Usage: int dist = KoreColorOps.ColorDistance(col1, col2);
    public static float ColorDistance(KoreColorRGB col1, KoreColorRGB col2)
    {
        // Calculate differences (sign doesn't matter as we square them)
        float rDiff = col1.R - col2.R;
        float gDiff = col1.G - col2.G;
        float bDiff = col1.B - col2.B;

        // If we equate RGB with XYZ, the distance defines a radius around the RGB value, so we'll use
        // pythagoras to get a single distance value
        float dist = MathF.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
        return dist;
    }

    // A weighted version of the color distance, that does a better job of matching human perception.

    // Usage: int dist = KoreColorOps.WeightedColorDistance(col1, col2);
    public static float WeightedColorDistance(KoreColorRGB col1, KoreColorRGB col2)
    {
        // Calculate differences (sign doesn't matter as we square them)
        float rDiff = col1.R - col2.R;
        float gDiff = col1.G - col2.G;
        float bDiff = col1.B - col2.B;

        // Weighting factors based on human eye sensitivity to different colors, biasing green.
        // Experimenting with the weights has lead to a color-cast being added to images.
        float rWeight = 0.2126f;
        float gWeight = 0.7152f;
        float bWeight = 0.0722f;

        // If we equate RGB with XYZ, the distance defines a radius around the RGB value, so we'll use
        // pythagoras to get a single distance value
        float dist = MathF.Sqrt(
            rWeight * rDiff * rDiff +
            gWeight * gDiff * gDiff +
            bWeight * bDiff * bDiff);

        return dist;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Color Name
    // --------------------------------------------------------------------------------------------

    // Return a name for any given color, starting with the names from the system palette, suffixes with the short hex code
    // Usage: string name = KoreColorOps.ColorName(color) => "Red_#FF0104"

    public static string ColorName(KoreColorRGB color)
    {
        (string name, KoreColorRGB col) = KoreColorPalette.ClosestColor(color);

        string hexString = KoreColorIO.RBGtoHexStringShort(color);

        return $"{name}_{hexString}";
    }

}


