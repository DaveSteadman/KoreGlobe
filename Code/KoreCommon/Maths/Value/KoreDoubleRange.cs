// using System;

namespace KoreCommon;

// #nullable enable

// public enum RangeBehavior { Wrap, Limit }

// public classKoreNumericRange<double>
// {
//     public double        Min      { get; private set; }
//     public double        Max      { get; private set; }
//     public double        Range    { get { return Max - Min; } }
//     public RangeBehavior Behavior { get; private set; }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Constructors
//     // --------------------------------------------------------------------------------------------

//     publicKoreNumericRange<double>(double min, double max, RangeBehavior behavior = RangeBehavior.Limit)
//     {
//         if (min > max) (min, max) = (max, min); // Flip values to ensure min <= max
//         Min      = min;
//         Max      = max;
//         Behavior = behavior;
//     }

//     // Predefined constant ranges
//     public static readonlyKoreNumericRange<double> ZeroToOne            = newKoreNumericRange<double>(0.0, 1.0);
//     public static readonlyKoreNumericRange<double> ZeroTo360Degrees     = newKoreNumericRange<double>(0.0, 360.0);
//     public static readonlyKoreNumericRange<double> Minus180To180Degrees = newKoreNumericRange<double>(-180.0, 180.0);
//     public static readonlyKoreNumericRange<double> ZeroToTwoPiRadians   = newKoreNumericRange<double>(0.0, Math.PI * 2);
//     public static readonlyKoreNumericRange<double> MinusPiToPiRadians   = newKoreNumericRange<double>(-Math.PI, Math.PI);

//     // --------------------------------------------------------------------------------------------
//     // MARK: Range Checking
//     // --------------------------------------------------------------------------------------------

//     public bool IsInRange(double val)
//     {
//         return val >= Min && val <= Max;
//     }
//     public bool Contains(double val) => IsInRange(val);

//     public double FractionInRange(double val)
//     {
//         if (val < Min) return 0.0;
//         if (val > Max) return 1.0;
//         return (val - Min) / Range;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Range Application
//     // --------------------------------------------------------------------------------------------

//     public double Apply(double value)
//     {
//         if (IsInRange(value))
//             return value;

//         switch (Behavior)
//         {
//             case RangeBehavior.Wrap:
//                 double offset = (value - Min) % Range;
//                 return (offset >= 0) ? Min + offset : Max + offset;
//             case RangeBehavior.Limit:
//                 return (value < Min) ? Min : Max;
//             default:
//                 throw new InvalidOperationException("Unsupported range behavior.");
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: General functions
//     // --------------------------------------------------------------------------------------------

//     public override bool Equals(object? obj)
//     {
//         if (ReferenceEquals(this, obj))
//         {
//             return true;
//         }

//         if (obj isKoreNumericRange<double> other)
//         {
//             return this.Min.Equals(other.Min) &&
//                 this.Max.Equals(other.Max) &&
//                 this.Behavior == other.Behavior;
//         }

//         return false;
//     }

//     public override string ToString()
//     {
//         return $"[Min:{Min:F3}, Max:{Max:F3}, Behavior:{Behavior}]";
//     }

//     public override int GetHashCode()
//     {
//         throw new NotImplementedException();
//     }
// }
