using System;

// KoreValueUtils: A static class for common (float precision) math routines, useful as helper routines for higher-level functionality.

namespace KoreCommon;

public static partial class KoreValueUtils
{
    // --------------------------------------------------------------------------------------------
    // Function to essentially allow the % operator on float precision numbers.

    public static float Modulo(float value, float rangesize)
    {
        if (Math.Abs(rangesize) < KoreConsts.ArbitrarySmallFloat)
            throw new ArgumentException("rangeSize too small", nameof(rangesize));

        float wrappedvalue = value - rangesize * (float)Math.Floor(value / rangesize);
        if (wrappedvalue < 0) wrappedvalue += rangesize;
        return wrappedvalue;
    }

    // --------------------------------------------------------------------------------------------

    public static float LimitToRange(float val, float rangemin, float rangemax)
    {
        // Fix the min max range
        if (rangemin > rangemax) (rangemin, rangemax) = (rangemax, rangemin);
        return (val < rangemin) ? rangemin : ((val > rangemax) ? rangemax : val);
    }

    public static float WrapToRange(float val, float rangemin, float rangemax)
    {
        if (rangemin > rangemax) (rangemin, rangemax) = (rangemax, rangemin);

        float diff = rangemax - rangemin;
        float wrappedvalue = Modulo(val - rangemin, diff);
        return wrappedvalue + rangemin;
    }

    // Usage: KoreValueUtils.Clamp(val, minval, maxval)
    public static float Clamp(float val, float minval, float maxval) => LimitToRange(val, minval, maxval);

    // --------------------------------------------------------------------------------------------

    // Determine the difference between two values that wrap (ie longitude or angle values).
    // The difference is always the shortest distance between the two values, taking into account

    public static float DiffInWrapRange(float rangemin, float rangemax, float val1, float val2)
    {
        // First, wrap both values to the range.
        float wrappedVal1 = WrapToRange(val1, rangemin, rangemax);
        float wrappedVal2 = WrapToRange(val2, rangemin, rangemax);

        // Compute the difference.
        float diff = wrappedVal2 - wrappedVal1;

        // Here's the key: we want to adjust this difference so that it represents the shortest turn
        // from val1 to val2, taking into account the possibility of wrapping around from rangemax to rangemin.
        // We will first compute the size of the range.
        float rangeSize = rangemax - rangemin;

        // If the absolute difference is greater than half of the range size, we know that it would be
        // shorter to go the other way around the circle to get from val1 to val2.
        if (Math.Abs(diff) > rangeSize / 2)
        {
            // There are two cases to consider.
            if (diff > 0)
            {
                // If diff is positive, then val2 is counterclockwise from val1 and it would be shorter to
                // go clockwise from val1 to get to val2. So we subtract the rangeSize from diff.
                diff -= rangeSize;
            }
            else
            {
                // If diff is negative, then val2 is clockwise from val1 and it would be shorter to go
                // counterclockwise from val1 to get to val2. So we add the rangeSize to diff.
                diff += rangeSize;
            }
        }
        return diff;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: float myVal = KoreValueUtils.ScaleVal(inval, inrangemin, inrangemax, outrangemin, outrangemax)
    public static float ScaleVal(float inval, float inrangemin, float inrangemax, float outrangemin, float outrangemax)
    {
        // Check in the input value is in range
        inval = LimitToRange(inval, inrangemin, inrangemax);

        // determine the different ranges to multiply the values by
        float indiff = inrangemax - inrangemin;
        float outdiff = outrangemax - outrangemin;

        // check in range and out range are not too small to function
        if (Math.Abs(indiff) < KoreConsts.ArbitrarySmallDouble) throw new ArgumentException("ScaleVal input range too small", nameof(indiff));
        if (Math.Abs(outdiff) < KoreConsts.ArbitrarySmallDouble) throw new ArgumentException("ScaleVal output range too small", nameof(outdiff));

        float diffratio = outdiff / indiff;

        float outval = ((inval - inrangemin) * diffratio) + outrangemin;
        return LimitToRange(outval, outrangemin, outrangemax);
    }


    // --------------------------------------------------------------------------------------------

    public static bool IsInRange(float val, float rangemin, float rangemax)
    {
        return val >= rangemin && val <= rangemax;
    }

    // --------------------------------------------------------------------------------------------

    public static float Interpolate(float inrangemin, float inrangemax, float fraction)
    {
        // fraction = LimitToRange(fraction, 0, 1); // Deactivating the limit check, so extrapolations are also available to the caller.
        float diffVal = inrangemax - inrangemin;
        return inrangemin + (diffVal * fraction);
    }

    // --------------------------------------------------------------------------------------------
    // Useful way to test floating point numbers.

    public static bool EqualsWithinTolerance(float val, float matchval, float tolerance = KoreConsts.ArbitrarySmallFloat)
    {
        return Math.Abs(val - matchval) <= tolerance;
    }

    // more shorthand comparisons
    // Usage: KoreValueUtils.IsEqual(val, matchval, tolerance)
    public static bool IsEqual(float val, float matchval, float tolerance = KoreConsts.ArbitrarySmallFloat) => EqualsWithinTolerance(val, matchval, tolerance);

}
