// <fileheader>

using System;

// Class defining a simple 2D rectangle, with no rotation

namespace KoreCommon;

// enum to define rect positions
public enum KoreXYRectPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center,
    TopCenter,
    BottomCenter,
    LeftCenter,
    RightCenter
};

public struct KoreXYRect
{
    // Main attributes
    public KoreXYVector TopLeft { get; }
    public KoreXYVector BottomRight { get; }

    // Derived attributes
    public double Left   => TopLeft.X;
    public double Right  => BottomRight.X;
    public double Top    => TopLeft.Y;
    public double Bottom => BottomRight.Y;
    public double Width  => BottomRight.X - TopLeft.X;
    public double Height => BottomRight.Y - TopLeft.Y;
    public double Area   => Width * Height;

    // Derive the other corners and centers
    public KoreXYVector TopRight => new KoreXYVector(Right, Top);
    public KoreXYVector TopCenter    => new KoreXYVector((Left + Right) / 2, Top);
    public KoreXYVector LeftCenter   => new KoreXYVector(Left, (Top + Bottom) / 2);
    public KoreXYVector Center       => new KoreXYVector((Left + Right) / 2, (Top + Bottom) / 2);
    public KoreXYVector RightCenter  => new KoreXYVector(Right, (Top + Bottom) / 2);
    public KoreXYVector BottomLeft   => new KoreXYVector(Left, Bottom);
    public KoreXYVector BottomCenter => new KoreXYVector((Left + Right) / 2, Bottom);

    // Edges
    public KoreXYLine TopLine       => new KoreXYLine(TopLeft, TopRight);
    public KoreXYLine BottomLine    => new KoreXYLine(BottomLeft, BottomRight);
    public KoreXYLine LeftLine      => new KoreXYLine(TopLeft, BottomLeft);
    public KoreXYLine RightLine     => new KoreXYLine(TopRight, BottomRight);

    public static KoreXYRect Zero => new KoreXYRect(0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYRect(double x1, double y1, double x2, double y2)
    {
        TopLeft = new(x1, y1);
        BottomRight = new(x2, y2);
    }

    public KoreXYRect(KoreXYVector topLeft, KoreXYVector bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public KoreXYRect(KoreXYRect rect)
    {
        TopLeft = rect.TopLeft;
        BottomRight = rect.BottomRight;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Rect
    // --------------------------------------------------------------------------------------------

    public KoreXYRect Offset(KoreXYVector xyOffset)
    {
        KoreXYVector newTL = TopLeft.Offset(xyOffset);
        KoreXYVector newBR = BottomRight.Offset(xyOffset);
        return new KoreXYRect(newTL, newBR);
    }

    public KoreXYRect Inset(double inset)
    {
        // Return a Zero rect if we would turn the rectangle inside out
        if (Width < inset * 2 || Height < inset * 2)
            return Zero;

        // Inset the rectangle by a given amount
        // - inset > 0 = smaller rectangle
        // - inset < 0 = larger rectangle
        return new KoreXYRect(Left + inset, Top + inset, Right - inset, Bottom - inset);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Point
    // --------------------------------------------------------------------------------------------

    public KoreXYVector PointFromFraction(double xFraction, double yFraction)
    {
        // Get the point at a given fraction along the rectangle axis
        // - 0 = top/left, 1 = bottom/right, 0.5 = midpoint
        // - Values beyond 0->1 are allowed, to go outside the rectangle
        return new KoreXYVector(Left + (Width * xFraction), Top + (Height * yFraction));
    }

    public KoreXYVector PointFromPosition(KoreXYRectPosition position)
    {
        // Get a point at a given position within the rectangle
        return position switch
        {
            KoreXYRectPosition.TopLeft      => TopLeft,
            KoreXYRectPosition.TopRight     => TopRight,
            KoreXYRectPosition.BottomLeft   => BottomLeft,
            KoreXYRectPosition.BottomRight  => BottomRight,
            KoreXYRectPosition.Center       => Center,
            KoreXYRectPosition.TopCenter    => TopCenter,
            KoreXYRectPosition.BottomCenter => BottomCenter,
            KoreXYRectPosition.LeftCenter   => LeftCenter,
            KoreXYRectPosition.RightCenter  => RightCenter,
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Checks
    // --------------------------------------------------------------------------------------------

    public bool Contains(KoreXYVector xy)
    {
        return xy.X >= Left && xy.X <= Right &&
               xy.Y >= Top  && xy.Y <= Bottom;
    }

    public bool Intersects(KoreXYRect other)
    {
        return !(other.Left   > Right  ||
                 other.Right  < Left   ||
                 other.Top    > Bottom ||
                 other.Bottom < Top);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Misc
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"KoreXYRect(TopLeft: {TopLeft}, BottomRight: {BottomRight})";
    }
}
