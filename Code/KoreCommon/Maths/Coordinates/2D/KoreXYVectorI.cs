using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

// Point = a position, absolute.
// Vector = an offset, relative, scalable.

namespace KoreCommon;

public struct KoreXYVectorI
{
    public int X { get; }
    public int Y { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYVectorI(int x, int y)
    {
        X = x;
        Y = y;
    }

    public KoreXYVectorI(KoreXYVectorI xy)
    {
        X = xy.X;
        Y = xy.Y;
    }

    // Zero default constructor
    public static KoreXYVectorI Zero => new KoreXYVectorI(0, 0);

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public KoreXYVectorI Offset(int x, int y) => new KoreXYVectorI(X + x, Y + y);
    public KoreXYVectorI Offset(KoreXYVectorI xy) => new KoreXYVectorI(X + xy.X, Y + xy.Y);

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static KoreXYVectorI Sum(KoreXYVectorI a, KoreXYVectorI b) => new KoreXYVectorI(a.X + b.X, a.Y + b.Y);
    public static KoreXYVectorI Diff(KoreXYVectorI a, KoreXYVectorI b) => new KoreXYVectorI(a.X - b.X, a.Y - b.Y);
    public static KoreXYVectorI Scale(KoreXYVectorI a, int b) => new KoreXYVectorI(a.X * b, a.Y * b);

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static KoreXYVectorI operator +(KoreXYVectorI a, KoreXYVectorI b) { return new KoreXYVectorI(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static KoreXYVectorI operator -(KoreXYVectorI a, KoreXYVectorI b) { return new KoreXYVectorI(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    public static KoreXYVectorI operator *(KoreXYVectorI a, int b) { return new KoreXYVectorI(a.X * b, a.Y * b); }
    public static KoreXYVectorI operator *(int b, KoreXYVectorI a) { return new KoreXYVectorI(a.X * b, a.Y * b); }

    // / operator overload for scaling point in relation to origin
    public static KoreXYVectorI operator /(KoreXYVectorI a, int b) { return new KoreXYVectorI(a.X / b, a.Y / b); }
    public static KoreXYVectorI operator /(int b, KoreXYVectorI a) { return new KoreXYVectorI(a.X / b, a.Y / b); }

}