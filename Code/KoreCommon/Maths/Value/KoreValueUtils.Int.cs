// <fileheader>

using System;

// KoreValueUtils: A static class for common (double precision) math routines, useful as helper routines for higher-level functionality.

namespace KoreCommon;

public static partial class KoreValueUtils
{
    // KoreValueUtils.Clamp

    static public int Clamp(int val, int min, int max)
    {
        if (min > max) (min, max) = (max, min);
        return (val < min) ? min : (val > max) ? max : val;
    }

    static public int Wrap(int val, int min, int max)
    {
        int range = max - min + 1;
        if (val < min)
        {
            val += range * ((min - val) / range + 1);
        }
        return min + (val - min) % range;
    }

    // --------------------------------------------------------------------------------------------

    // Take an input fraction (0..1) and return the index of the value in the range minval..maxval
    // that corresponds to that fraction.

    public static int IndexFromFraction(float fraction, int minval, int maxval)
    {
        float limitedFraction = LimitToRange(fraction, 0, 1);
        int diff = maxval - minval;
        return (int)(limitedFraction * diff) + minval;
    }

    public static int IndexFromIncrement(float minLimit, float increment, float val)
    {
        int retInc = 0;
        float workingVal = (minLimit + increment);

        while (workingVal < val)
        {
            workingVal += increment;
            retInc++;
        }
        return retInc;
    }

}
