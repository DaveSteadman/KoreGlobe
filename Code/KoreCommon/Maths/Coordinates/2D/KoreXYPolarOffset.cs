using System;

namespace KoreCommon;

// KoreXYPolarOffset: Class representing an angle and distance in 2D space.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Object is immutable

public struct KoreXYPolarOffset
{
    // Main attributes
    public double AngleRads { get; }
    public double Distance { get; }

    // Derived attributes
    public double AngleDegs => AngleRads * KoreConsts.RadsToDegsMultiplier;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public KoreXYPolarOffset(double angleRads, double distance)
    {
        this.AngleRads = angleRads;
        this.Distance = distance;
    }
}
