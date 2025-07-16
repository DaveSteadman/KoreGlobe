// using System;

namespace KoreCommon;

// public struct KoreNumericRange<float>
// {
//     // Attributes, read-only after creation, and calc a range.
//     public float Min   { get; private set; }
//     public float Max   { get; private set; }
//     public float Range { get { return Max - Min; } }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Constructors and standard ranges
//     // --------------------------------------------------------------------------------------------

//     public KoreNumericRange<float>(float min, float max)
//     {
//         if (min > max) { (min, max) = (max, min); }

//         Min = min;
//         Max = max;
//     }

//     // Predefined constant ranges
//     public static readonly KoreNumericRange<float> ZeroToOne            = new KoreNumericRange<float>(0f, 1f);
//     public static readonly KoreNumericRange<float> ZeroTo360Degrees     = new KoreNumericRange<float>(0f, 360f);
//     public static readonly KoreNumericRange<float> Minus180To180Degrees = new KoreNumericRange<float>(-180f, 180f);
//     public static readonly KoreNumericRange<float> ZeroToTwoPiRadians   = new KoreNumericRange<float>(0f, (float)(Math.PI * 2));
//     public static readonly KoreNumericRange<float> MinusPiToPiRadians   = new KoreNumericRange<float>((float)(-Math.PI), (float)Math.PI);

//     // --------------------------------------------------------------------------------------------

//     public bool IsInRange(float val)
//     {
//         if (val > Max) return false;
//         if (val < Min) return false;
//         return true;
//     }

//     public float FractionInRange(float val)
//     {
//         if (val < Min) return 0f;
//         if (val > Max) return 1f;
//         return (val - Min) / Range;
//     }

//     // --------------------------------------------------------------------------------------------

//     // Returns a subrange from teh current object's range, given the "GridElem"th element within GridCount elements.
//     public KoreNumericRange<float> SubRangeGrid(int GridCount, int GridElem)
//     {
//         if (GridCount <= 0)
//         {
//             throw new ArgumentException("GridCount must be greater than zero.");
//         }
//         if (GridElem < 0 || GridElem >= GridCount)
//         {
//             throw new ArgumentOutOfRangeException("GridElem must be within the range of 0 to GridCount - 1.");
//         }

//         float subRangeSize = Range / GridCount;
//         float newMin       = Min + (subRangeSize * GridElem);
//         float newMax       = newMin + subRangeSize;

//         return new KoreNumericRange<float>(newMin, newMax);
//     }

//     // --------------------------------------------------------------------------------------------

//     public override string ToString()
//     {
//         return $"Range: [{Min:F3}, {Max:F3}]";
//     }
// }
