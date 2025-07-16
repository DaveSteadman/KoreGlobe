using System;

namespace KoreCommon;

public struct KoreXYLine : IEquatable<KoreXYLine>
{
    public KoreXYPoint P1 { get; }
    public KoreXYPoint P2 { get; }

    // --------------------------------------------------------------------------------------------
    // MARK: attributes
    // --------------------------------------------------------------------------------------------

    public double Length => P1.DistanceTo(P2);
    public KoreXYPoint Direction => P2 - P1; // return a position as the vector from 0,0

    public KoreXYPoint DirectionUnitVector
    { // return a direction, with a magniude of 1
        get
        {
            if (Length == 0) return new KoreXYPoint(0, 0);
            return Direction * (1 / Length);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: IEquatable implementation
    // --------------------------------------------------------------------------------------------

    public bool Equals(KoreXYLine other)
    {
        return P1.Equals(other.P1) && P2.Equals(other.P2);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYLine(double x1, double y1, double x2, double y2)
    {
        P1 = new KoreXYPoint(x1, y1);
        P2 = new KoreXYPoint(x2, y2);
    }

    public KoreXYLine(KoreXYPoint p1, KoreXYPoint p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public KoreXYLine(KoreXYLine line)
    {
        P1 = line.P1;
        P2 = line.P2;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position methods
    // --------------------------------------------------------------------------------------------

    // Get the centre point of the line

    public KoreXYPoint MidPoint()
    {
        return new KoreXYPoint((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
    }

    public KoreXYPoint Fraction(double fraction)
    {
        // Get the point at a given fraction along the line
        // - 0 = P1, 1 = P2, 0.5 = midpoint
        // - We allow -ve value, backtracking from before P1, along the P1-P2 line.
        // - We allow > 1, to go past P2, along the P1-P2 line.

        double dx = P2.X - P1.X;
        double dy = P2.Y - P1.Y;

        double newX = P1.X + (dx * fraction);
        double newY = P1.Y + (dy * fraction);

        return new KoreXYPoint(newX, newY);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Vector methods
    // --------------------------------------------------------------------------------------------

    public KoreXYVector VectorTo(KoreXYPoint P2)
    {
        // Return a vector from P1 to P2
        return new KoreXYVector(P2.X - P1.X, P2.Y - P1.Y);
    }

    public KoreXYVector UnitVectorTo(KoreXYPoint P2)
    {
        // Return a unit vector from P1 to P2
        KoreXYVector vector = VectorTo(P2);
        if (vector.Length == 0) return new KoreXYVector(0, 0);
        return vector * (1 / vector.Length);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Move methods
    // --------------------------------------------------------------------------------------------

    // Return a new line object, will all points offset by an XY amount.

    public KoreXYLine Offset(double x, double y)
    {
        return new KoreXYLine(P1.Offset(x, y), P2.Offset(x, y));
    }

    public KoreXYLine Offset(KoreXYVector xy)
    {
        return new KoreXYLine(P1.Offset(xy), P2.Offset(xy));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Misc methods
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"({P1.X:F3}, {P1.Y:F3}) -> ({P2.X:F3}, {P2.Y:F3})";
    }
}