// <fileheader>

using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

namespace KoreCommon;

public static partial class KoreValueUtils
{
    public const double Deg2Rad = Math.PI / 180.0;
    public const double Rad2Deg = 180.0 / Math.PI;

    // KoreValueUtils.RadsToDegs() // KoreValueUtils.DegsToRads()
    public static double RadsToDegs(double rad) { return rad * (180.0 / Math.PI); }
    public static double DegsToRads(double deg) { return deg * (Math.PI / 180.0); }

    // --------------------------------------------------------------------------------------------

    // Normalizes any angle to the [0, 360) range in degrees
    public static double NormalizeAngle360(double angleDegrees)
    {
        angleDegrees = angleDegrees % 360.0;
        if (angleDegrees < 0)
            angleDegrees += 360.0;
        return angleDegrees;
    }

    // Normalizes any angle to the [-180, 180) range in degrees
    public static double NormalizeAngle180(double angleDegrees)
    {
        angleDegrees = NormalizeAngle360(angleDegrees);
        if (angleDegrees >= 180.0)
            angleDegrees -= 360.0;
        return angleDegrees;
    }

    // Normalizes any angle to the [0, 2π) range in radians
    // Usage: angleRadians = KoreValueUtils.NormalizeAngle2Pi(angleRadians)
    public static double NormalizeAngle2Pi(double angleRadians)
    {
        angleRadians = angleRadians % (2 * Math.PI);
        if (angleRadians < 0)
            angleRadians += (2 * Math.PI);
        return angleRadians;
    }

    // Normalizes any angle to the [-π, π) range in radians
    public static double NormalizeAnglePi(double angleRadians)
    {
        angleRadians = NormalizeAngle2Pi(angleRadians);
        if (angleRadians >= Math.PI)
            angleRadians -= (2 * Math.PI);
        return angleRadians;
    }

    // --------------------------------------------------------------------------------------------

    // Calculates the shortest difference between two angles in degrees
    // Angle "From" 1 "to" 2 in terms of sign.
    public static double AngleDiffDegs(double angle1Degrees, double angle2Degrees)
    {
        double diff = NormalizeAngle180(angle2Degrees) - NormalizeAngle180(angle1Degrees);
        return NormalizeAngle180(diff);
    }

    // Calculates the shortest difference between two angles in radians
    // Angle "From" 1 "to" 2 in terms of sign.
    // Tries to return a smaller value.
    public static double AngleDiffRads(double angle1Radians, double angle2Radians)
    {
        double diff = NormalizeAnglePi(angle1Radians) - NormalizeAnglePi(angle2Radians);
        return NormalizeAnglePi(diff);
    }

    // --------------------------------------------------------------------------------------------

    // Adds a delta to an angle in degrees and normalizes the result
    public static double AnglePlusDeltaDegs(double angleDegrees, double deltaDegrees)
    {
        double newAngle = angleDegrees + deltaDegrees;
        return NormalizeAngle360(newAngle);
    }

    // Adds a delta to an angle in radians and normalizes the result
    public static double AnglePlusDeltaRads(double angleRadians, double deltaRadians)
    {
        double newAngle = angleRadians + deltaRadians;
        return NormalizeAngle2Pi(newAngle);
    }

    // --------------------------------------------------------------------------------------------

    // Check if an angle is within a given range, accounting for wrap-around at 2π.
    public static bool IsAngleInRangeRads(double angleRads, double startAngleRads, double endAngleRads)
    {
        // Normalize angles to be within 0 to 2π
        angleRads = NormalizeAngle2Pi(angleRads);
        startAngleRads = NormalizeAngle2Pi(startAngleRads);
        endAngleRads = NormalizeAngle2Pi(endAngleRads);

        // If startAngle is less than or equal to endAngle, it's a simple range check.
        if (startAngleRads <= endAngleRads)
        {
            return angleRads >= startAngleRads && angleRads <= endAngleRads;
        }
        // If startAngle is greater than endAngle, the range wraps around 2π.
        else
        {
            return angleRads >= startAngleRads || angleRads <= endAngleRads;
        }
    }

    // Check if an angle is within a given range, accounting for wrap-around at 2xPi.

    public static bool IsAngleInRangeRadsDelta(double angleRads, double startAngleRads, double deltaAngleRads)
    {
        // Zero start angle is "east" (0). The Delta is the angle span and direction: positive is anti-clockwise.

        // If the delta is > 2Pi, then the range is the whole circle or greater, return true.
        if (deltaAngleRads >= (2 * Math.PI))
            return true;

        double normalizedAngle = NormalizeAngle2Pi(angleRads);
        double normalizedStartAngle = NormalizeAngle2Pi(startAngleRads);
        double normalizedEndAngle = NormalizeAngle2Pi(startAngleRads + deltaAngleRads);

        if (deltaAngleRads > 0) // Anti-clockwise direction
        {
            if (normalizedEndAngle <= normalizedStartAngle)
            {
                normalizedEndAngle += 2 * Math.PI; // Adjust end angle for wrap-around
                if (normalizedAngle < normalizedStartAngle) normalizedAngle += 2 * Math.PI; // Adjust angle for comparison
            }
            return normalizedAngle >= normalizedStartAngle && normalizedAngle <= normalizedEndAngle;
        }
        else // Clockwise direction
        {
            if (normalizedEndAngle >= normalizedStartAngle)
            {
                normalizedEndAngle -= 2 * Math.PI; // Adjust end angle for wrap-around
                if (normalizedAngle > normalizedStartAngle) normalizedAngle -= 2 * Math.PI; // Adjust angle for comparison
            }
            return normalizedAngle <= normalizedStartAngle && normalizedAngle >= normalizedEndAngle;
        }
    }

}



