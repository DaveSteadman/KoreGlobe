using System;

// KoreEntityKinetics: Class for the physical position and representation of a Entity.
// Design Decisions:
// - Immutable: New objects are created on the fly and highly likely to be copied around the rest of
//   the system, so readonly to avoid confusion.

// The kinetics only have a "Current" position. The concept of a start position will need to be maintained
// in a route element and re-applied to the current position by external logic.

using KoreCommon;
namespace KoreSim;

public class KoreEntityKinetics
{
    // Clamp to ground is a flag with no function in the model, it is maintained, but up to the
    // rendering side to lookup the elevation of the current map tile and apply it.
    public bool ClampToGround { set; get; } = false;

    // --------------------------------------------------------------------------------------------

    // Current position and orentation
    public KoreLLAPoint CurrPosition { set; get; } = KoreLLAPoint.Zero;
    public KoreAttitude CurrAttitude { set; get; } = KoreAttitude.Zero;

    // Changes being applied
    public KoreAttitudeDelta CurrAttitudeDelta { set; get; } = KoreAttitudeDelta.Zero;
    public KoreCourse CurrCourse { set; get; } = KoreCourse.Zero;
    public KoreCourseDelta CurrCourseDelta { set; get; } = KoreCourseDelta.Zero;

    // Outline of the Entity (judging intersections and extremities)
    public KoreXYZBox BoundingBox { set; get; } = KoreXYZBox.One;

    // --------------------------------------------------------------------------------------------

    // log of positions for the last 100 seconds
    KoreLimitedConcurrentQueue<KoreLLAPoint> PositionLog = new KoreLimitedConcurrentQueue<KoreLLAPoint>(100);

    // --------------------------------------------------------------------------------------------

    public KoreEntityKinetics()
    {
    }

    // --------------------------------------------------------------------------------------------

    public void ResetPosition()
    {
        // CurrPosition = StartPosition;
        // CurrAttitude = StartAttitude;
        // CurrCourse = StartCourse;
    }

    public void SetStart(KoreLLAPoint pos, KoreAttitude att, KoreCourse course)
    {
        CurrPosition = pos;
        CurrAttitude = att;
        CurrCourse = course;
    }


    public void UpdateForDuration(float elapsedSeconds)
    {
        // Update from the highest derivative (acceleration) to the lowest (position)



        // AttitudeDelta
        // CourseDelta
        CurrCourse = CurrCourse.PlusDeltaForTime(CurrCourseDelta, elapsedSeconds);
        CurrAttitude = CurrAttitude.PlusDeltaForTime(CurrAttitudeDelta, elapsedSeconds);
        CurrPosition = CurrPosition.PlusDeltaForTime(CurrCourse, elapsedSeconds);

        // Log the current position, if it has moved more than a nominal amount.
        double minDistanceSeparationM = 10;

        if (PositionLog.Count == 0)
            PositionLog.Enqueue(CurrPosition);
        else
        {
            KoreLLAPoint lastPos = PositionLog.GetMostRecent();
            if (CurrPosition.StraightLineDistanceToM(lastPos) > minDistanceSeparationM)
                PositionLog.Enqueue(CurrPosition);
        }

    }

    // --------------------------------------------------------------------------------------------

    // Output a roll value in Rads.

    public static double EntityRollFromCourseDelta(KoreCourse course, KoreCourseDelta delta, float scaleFactor = 1f)
    {
        // This function provides a basic roll angle value to apply to an aircraft in a turn.
        // - We want to exract the angle change per second from the source delta.
        // - We need the aircraft speed to factor in the energy in the turn.
        // - We have a scale factor to amplify or reduce the effect anecdotally.
        // - We need to output a roll value of 0 to 90 degrees, with a sign matching to the clockwise/anticlockwise orientation. The likely
        //   destination KoreAttitude attribute is "RollClockwiseRads".

        // Extract the rate of heading change in degrees per second, as provided by the delta.
        double headingChangeRateDegsSec = delta.HeadingChangeClockwiseDegsSec;

        // The speed of the aircraft in meters per second is used directly from the course information.
        double speedMps = course.SpeedMps;

        // The roll angle calculation incorporates the heading change rate, the speed as an energy factor, and the scale factor.
        // This combination reflects the dynamics of the turn and allows for empirical adjustment through the scale factor.
        double adjustedHeadingChange = headingChangeRateDegsSec * scaleFactor * speedMps;

        // Clamp the adjusted heading change to a maximum of 90 degrees to maintain realistic roll limits.
        // The direction (sign) of the roll matches the clockwise or anticlockwise direction of the turn.
        double rollDegrees = Math.Clamp(adjustedHeadingChange, -90, 90);

        // Since the desired output is in radians and within the range of -π/2 to π/2,
        // convert the clamped roll degrees to radians for the return value.
        double rollRads = rollDegrees * (Math.PI / 180);

        // Ensure the rollRadians is within the specified range of -π/2 to π/2 radians.
        rollRads = Math.Clamp(rollRads, -Math.PI / 2, Math.PI / 2);

        return rollRads;
    }
}
