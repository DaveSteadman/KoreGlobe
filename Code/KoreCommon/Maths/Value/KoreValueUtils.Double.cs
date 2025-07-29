using System;
using System.Collections.Generic;

// KoreValueUtils: A static class for common (double precision) math routines, useful as helper routines for higher-level functionality.

namespace KoreCommon;

public static partial class KoreValueUtils
{
    // --------------------------------------------------------------------------------------------
    // Function to essentially allow the % operator on double precision numbers.

    public static double Modulo(double value, double rangesize)
    {
        if (Math.Abs(rangesize) < KoreConsts.ArbitrarySmallDouble)
            throw new ArgumentException("rangeSize too small", nameof(rangesize));

        double wrappedvalue = value - rangesize * Math.Floor(value / rangesize);
        if (wrappedvalue < 0) wrappedvalue += rangesize;
        return wrappedvalue;
    }

    // --------------------------------------------------------------------------------------------

    // KoreValueUtils.LimitToRange(5, 0, 10) // Returns 5

    public static double LimitToRange(double val, double rangemin, double rangemax)
    {
        if (rangemin > rangemax) (rangemin, rangemax) = (rangemax, rangemin);

        return (val < rangemin) ? rangemin : ((val > rangemax) ? rangemax : val);
    }

    public static double ClampD(double val, double rangemin, double rangemax)
    {
        return LimitToRange(val, rangemin, rangemax);
    }

    public static double WrapToRange(double val, double rangemin, double rangemax)
    {
        double diff = rangemax - rangemin;
        double wrappedvalue = Modulo(val - rangemin, diff);
        return wrappedvalue + rangemin;
    }

    public static double Diff(double val1, double val2)
    {
        double maxval = Math.Max(val1, val2);
        double minval = Math.Min(val1, val2);
        return maxval - minval;
    }

    // --------------------------------------------------------------------------------------------

    // Determine the difference between two values that wrap (ie longitude or angle values).

    public static double DiffInWrapRange(double rangemin, double rangemax, double val1, double val2)
    {
        // First, wrap both values to the range.
        double wrappedVal1 = WrapToRange(val1, rangemin, rangemax);
        double wrappedVal2 = WrapToRange(val2, rangemin, rangemax);

        // Compute the difference.
        double diff = wrappedVal2 - wrappedVal1;

        // Here's the key: we want to adjust this difference so that it represents the shortest turn
        // from val1 to val2, taking into account the possibility of wrapping around from rangemax to rangemin.
        // We will first compute the size of the range.
        double rangeSize = rangemax - rangemin;

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

    // Take an input fraction (0..1) and return the index of the value in the range minval..maxval
    // that corresponds to that fraction.

    public static int IndexFromFraction(double fraction, int minval, int maxval)
    {
        double limitedFraction = LimitToRange(fraction, 0, 1);
        int diff = maxval - minval;
        return (int)(limitedFraction * diff) + minval;
    }

    public static int IndexFromIncrement(double minLimit, double increment, double val)
    {
        int retInc = 0;
        double workingVal = (minLimit + increment);

        while (workingVal < val)
        {
            workingVal += increment;
            retInc++;
        }
        return retInc;
    }

    // --------------------------------------------------------------------------------------------

    public static bool IsInRange(double val, double rangemin, double rangemax)
    {
        return val >= rangemin && val <= rangemax;
    }

    // --------------------------------------------------------------------------------------------
    // Uses a y=mx+c mechanism to convert a value between an input and output range.

    public static double ScaleVal(double inval, double inrangemin, double inrangemax, double outrangemin, double outrangemax)
    {
        // Check in the input value is in range
        inval = LimitToRange(inval, inrangemin, inrangemax);

        // determine the different ranges to multiply the values by
        double indiff = inrangemax - inrangemin;
        double outdiff = outrangemax - outrangemin;

        // check in range and out range are not too small to function
        if (Math.Abs(indiff) < KoreConsts.ArbitrarySmallDouble) throw new ArgumentException("ScaleVal input range too small", nameof(indiff));
        if (Math.Abs(outdiff) < KoreConsts.ArbitrarySmallDouble) throw new ArgumentException("ScaleVal output range too small", nameof(outdiff));

        double diffratio = outdiff / indiff;

        double outval = ((inval - inrangemin) * diffratio) + outrangemin;
        return LimitToRange(outval, outrangemin, outrangemax);
    }

    // --------------------------------------------------------------------------------------------

    // Function to move from the current to the commanded value, but not exceed the maxDelta.

    // Usage: KoreValueUtils.AdjustWithinBounds(5, 10, 3) // Returns 8

    public static double AdjustWithinBounds(double currentValue, double commandedValue, double maxDelta)
    {
        double diff = commandedValue - currentValue;
        double delta = Math.Sign(diff) * Math.Min(Math.Abs(diff), maxDelta);
        return currentValue + delta;
    }

    // --------------------------------------------------------------------------------------------

    public static double Interpolate(double inrangemin, double inrangemax, double fraction)
    {
        // fraction = LimitToRange(fraction, 0, 1); // Deactivating the limit check, so extrapolations are also available to the caller.
        double diffVal = inrangemax - inrangemin;
        return inrangemin + (diffVal * fraction);
    }

    // --------------------------------------------------------------------------------------------
    // Useful way to test floating point numbers.

    // Usage: KoreValueUtils.EqualsWithinTolerance(val, val2, 0.3);

    public static bool EqualsWithinTolerance(double val, double matchval, double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        return Math.Abs(val - matchval) <= tolerance;
    }

    // Usage: KoreValueUtils.IsZero(val)
    public static bool IsZero(double val, double tolerance = KoreConsts.ArbitrarySmallDouble)
    {
        return EqualsWithinTolerance(val, 0, tolerance);
    }

    // --------------------------------------------------------------------------------------------

    // Generates a random float value between minVal and maxVal (inclusive)

    // KoreValueUtils.RandomInRange(1, 10) // Returns a random number between 1 and 10

    public static double RandomInRange(double minVal, double maxVal)
    {
        return ScaleVal(random.NextDouble(), 0, 1, minVal, maxVal);
    }

    // --------------------------------------------------------------------------------------------

    // Create a list of floats from start to end
    public static List<double> CreateRangeList(int length, double start, double end)
    {
        List<double> rangeList = new List<double>();

        for (int i = 0; i < length; i++)
            rangeList.Add(start + (end - start) * i / (length - 1));

        return rangeList;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreValueUtils.Min3(1.0, 2.0, 3.0) // Returns 1.0
    public static double Min3(double a, double b, double c)
    {
        double min = a;
        if (b < min) min = b;
        if (c < min) min = c;
        return min;
    }

    // --------------------------------------------------------------------------------------------

    public static double Max3(double a, double b, double c)
    {
        double max = a;
        if (b > max) max = b;
        if (c > max) max = c;
        return max;
    }

    // --------------------------------------------------------------------------------------------

}
