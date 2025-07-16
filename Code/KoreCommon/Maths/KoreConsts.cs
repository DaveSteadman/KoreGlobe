using System;

namespace KoreCommon;

public static class KoreConsts
{
    public const double kPi = Math.PI;
    public const double kTwoPi = 2 * Math.PI;

    public const double DegsInRads90 = (90 * DegsToRadsMultiplier);

    // --------------------------------------------------------------------------------------------

    // Basic Conversion Consts

    public const double RadsToDegsMultiplier = 180 / Math.PI;
    public const double DegsToRadsMultiplier = Math.PI / 180;

    // DEPRICATED - Use better named "small" over "min"
    public const double ArbitraryMinDouble = 0.00001;  // Used to check for values too close to 0.
    public const float ArbitraryMinFloat = 0.00001f; // Used to check for values too close to 0.

    public const double ArbitrarySmallDouble = 0.0000001; // Used to check for values too close to 0.
    public const float ArbitrarySmallFloat = 0.00001f; // Used to check for values too close to 0.
}