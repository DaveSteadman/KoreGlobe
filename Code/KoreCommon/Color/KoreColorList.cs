// <fileheader>


using System;
using System.Collections.Generic;

namespace KoreCommon;

// Usage: KoreColorRGB myCol = KoreColorList

public class KoreColorList
{
    private List<KoreColorRGB> ColorsList = new();

    // Basic operations
    public void AddColor(KoreColorRGB color) => ColorsList.Add(color);
    public void RemoveColor(KoreColorRGB color) => ColorsList.Remove(color);
    public void ClearColors() => ColorsList.Clear();
    public List<KoreColorRGB> GetColors() => new List<KoreColorRGB>(ColorsList);


    // --------------------------------------------------------------------------------------------

    // Only add the color if it is sufficiently different from existing colors 
    // Avoid large search issues if we limit the addition of colors early on.
    // Usage: KoreColorList.AddWithCheck(newColor, 30.0f); // Add newColor only if no similar color within 30 units
    public void AddWithCheck(KoreColorRGB newColor, float distTolerance = 1f)
    {
        foreach (var color in ColorsList)
        {
            if (KoreColorOps.WeightedColorDistance(color, newColor) <= distTolerance)
                return; // Similar color already exists, do not add
        }
        ColorsList.Add(newColor);
    }

    // --------------------------------------------------------------------------------------------

    public KoreColorRGB ClosestColor(KoreColorRGB targetColor)
    {
        if (ColorsList.Count == 0) return KoreColorPalette.DefaultColor; // Default if no colors

        // Initialise our working variables off the first list entry
        float closestDist = KoreColorOps.WeightedColorDistance(ColorsList[0], targetColor);
        int closestIndex = 0;

        // Loop through the rest of the list for a better answer
        for (int i = 1; i < ColorsList.Count; i++)
        {
            float currDist = KoreColorOps.WeightedColorDistance(ColorsList[i], targetColor);
            if (currDist < closestDist)
            {
                closestDist = currDist;
                closestIndex = i;
            }
        }

        return ColorsList[closestIndex];
    }

    // --------------------------------------------------------------------------------------------

    // - Create a destination list of colors, adding the first
    // - Add any subsequent colors only if they are not similar to any already in the destination list

    public void RemoveSimilarColors(float distTolerance)
    {
        if (ColorsList.Count < 2) return; // Nothing to do

        List<KoreColorRGB> uniqueColors = new();
        uniqueColors.Add(ColorsList[0]); // Start with the first color

        for (int i = 1; i < ColorsList.Count; i++)
        {
            KoreColorRGB currentColor = ColorsList[i];
            bool isSimilar = false;

            foreach (var uniqueColor in uniqueColors)
            {
                if (KoreColorOps.WeightedColorDistance(currentColor, uniqueColor) <= distTolerance)
                {
                    isSimilar = true;
                    break;
                }
            }

            if (!isSimilar)
            {
                uniqueColors.Add(currentColor);
            }
        }

        ColorsList = uniqueColors;
    }

    // --------------------------------------------------------------------------------------------

    // - Find the closest two colors in the list, remove them and add their average

    public void MergeClosestColors()
    {
        if (ColorsList.Count < 2) return; // Nothing to do

        // First, we do a nested loop to find the from and to index values for the two closest colors

        // Setup the working variables to a default first entry
        int fromIndex = 0;
        int toIndex = 1;
        float closestDist = KoreColorOps.WeightedColorDistance(ColorsList[fromIndex], ColorsList[toIndex]);

        // Top loop goes through all colors except the last
        for (int i = 0; i < ColorsList.Count - 1; ++i)
        {
            // Second loop goes through all colors after the top loop color
            for (int j = i + 1; j < ColorsList.Count; ++j)
            {
                // get the distance
                float currDist = KoreColorOps.WeightedColorDistance(ColorsList[i], ColorsList[j]);

                // If we're closer, this is the new target
                if (currDist < closestDist)
                {
                    closestDist = currDist;
                    fromIndex = i;
                    toIndex = j;
                }
            }
        }

        // Now we have the two closest colors, we create their average
        KoreColorRGB mergedColor = KoreColorOps.Lerp(ColorsList[fromIndex], ColorsList[toIndex], 0.5f);

        // Remove the two original colors, starting with the higher index to avoid messing up the lower index
        ColorsList.RemoveAt(Math.Max(fromIndex, toIndex));
        ColorsList.RemoveAt(Math.Min(fromIndex, toIndex));
        ColorsList.Add(mergedColor);
    }

    // --------------------------------------------------------------------------------------------

    public void ReduceColorCount(int targetCount)
    {
        if (ColorsList.Count <= targetCount) return; // Nothing to do

        while (ColorsList.Count > targetCount)
        {
            MergeClosestColors();
        }
    }

}