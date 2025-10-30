// <fileheader>



// KoreColorRange - A class to contain a list of numeric values associated colors, then provide
// interpolation between them from an input value.

using System;
using System.Collections.Generic;

namespace KoreCommon;

// ------------------------------------------------------------------------------------------------

struct KoreColorRangeEntry
{
    public float value;
    public KoreColorRGB color;

    public KoreColorRangeEntry(float value, KoreColorRGB color)
    {
        this.value = value;
        this.color = color;
    }
}

// ------------------------------------------------------------------------------------------------

public class KoreColorRange
{
    List<KoreColorRangeEntry> colorRangeList = new List<KoreColorRangeEntry>();

    // --------------------------------------------------------------------------------------------
    // MARK: Add
    // --------------------------------------------------------------------------------------------

    public void AddEntry(float value, KoreColorRGB color)
    {
        colorRangeList.Add(new KoreColorRangeEntry(value, color));

        // Sort the list by value
        colorRangeList.Sort((a, b) => a.value.CompareTo(b.value));
    }

    public void Clear()
    {
        colorRangeList.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Get
    // --------------------------------------------------------------------------------------------

    public KoreColorRGB GetColor(float value)
    {
        // Return WHite if no entries
        if (colorRangeList.Count == 0)
        {
            return KoreColorPalette.Colors["Magenta"];
        }

        // Return the first color if the value is less than the first entry
        if (value <= colorRangeList[0].value)
        {
            return colorRangeList[0].color;
        }

        // Return the last color if the value is greater than the last entry
        if (value >= colorRangeList[colorRangeList.Count - 1].value)
        {
            return colorRangeList[colorRangeList.Count - 1].color;
        }

        // Interpolate between the two values in the list (different to the lerp between the two fractions once they're found)
        for (int i = 0; i < colorRangeList.Count - 1; i++)
        {
            if (value >= colorRangeList[i].value && value <= colorRangeList[i + 1].value)
            {
                // Find the fraction between the two values, then lerp the colours
                float fraction = (value - colorRangeList[i].value) / (colorRangeList[i + 1].value - colorRangeList[i].value);
                return KoreColorOps.Lerp(colorRangeList[i].color, colorRangeList[i + 1].color, fraction);
            }
        }

        // Should never get here - return a default color
        return KoreColorPalette.Colors["Magenta"];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Standard color ranges
    // --------------------------------------------------------------------------------------------

    public static KoreColorRange RedToGreen()
    {
        KoreColorRange colorRange = new KoreColorRange();
        colorRange.AddEntry(0f, KoreColorPalette.Colors["Red"]);
        colorRange.AddEntry(1f, KoreColorPalette.Colors["Green"]);
        return colorRange;
    }

    public static KoreColorRange RedYellowGreen()
    {
        KoreColorRange colorRange = new KoreColorRange();
        colorRange.AddEntry(0.0f, KoreColorPalette.Colors["Red"]);
        colorRange.AddEntry(0.5f, KoreColorPalette.Colors["Yellow"]);
        colorRange.AddEntry(1.0f, KoreColorPalette.Colors["Green"]);
        return colorRange;
    }

    public static KoreColorRange BlueGreenYellowOrangeRed()
    {
        KoreColorRange colorRange = new KoreColorRange();
        colorRange.AddEntry(0.00f, KoreColorPalette.Colors["Blue"]);
        colorRange.AddEntry(0.25f, KoreColorPalette.Colors["Green"]);
        colorRange.AddEntry(0.50f, KoreColorPalette.Colors["Yellow"]);
        colorRange.AddEntry(0.75f, KoreColorPalette.Colors["Orange"]);
        colorRange.AddEntry(1.00f, KoreColorPalette.Colors["Red"]);
        return colorRange;
    }

}
