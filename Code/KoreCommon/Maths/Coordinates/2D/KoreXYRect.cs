using System;

// Class defining a simple 2D rectangle, with no rotation

namespace KoreCommon;

public struct KoreXYRect
{
    // Main attributes
    public KoreXYPoint TopLeft { get; }
    public KoreXYPoint BottomRight { get; }

    // Derived attributes
    public double Width => BottomRight.X - TopLeft.X;
    public double Height => BottomRight.Y - TopLeft.Y;
    public double Left => TopLeft.X;
    public double Right => BottomRight.X;
    public double Top => TopLeft.Y;
    public double Bottom => BottomRight.Y;
    public double Area => Width * Height;

    public KoreXYPoint TopRight => new KoreXYPoint(Right, Top);
    public KoreXYPoint BottomLeft => new KoreXYPoint(Left, Bottom);

    public KoreXYPoint Center => new KoreXYPoint((Left + Right) / 2, (Top + Bottom) / 2);
    public KoreXYPoint TopCenter => new KoreXYPoint((Left + Right) / 2, Top);
    public KoreXYPoint BottomCenter => new KoreXYPoint((Left + Right) / 2, Bottom);
    public KoreXYPoint LeftCenter => new KoreXYPoint(Left, (Top + Bottom) / 2);
    public KoreXYPoint RightCenter => new KoreXYPoint(Right, (Top + Bottom) / 2);

    public KoreXYLine TopLine => new KoreXYLine(TopLeft, TopRight);
    public KoreXYLine BottomLine => new KoreXYLine(BottomLeft, BottomRight);
    public KoreXYLine LeftLine => new KoreXYLine(TopLeft, BottomLeft);
    public KoreXYLine RightLine => new KoreXYLine(TopRight, BottomRight);

    public static KoreXYRect Zero => new KoreXYRect(0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYRect(double x1, double y1, double x2, double y2)
    {
        TopLeft = new(x1, y1);
        BottomRight = new(x2, y2);
    }

    public KoreXYRect(KoreXYPoint topLeft, KoreXYPoint bottomRight)
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
        KoreXYPoint newTL = TopLeft.Offset(xyOffset);
        KoreXYPoint newBR = BottomLeft.Offset(xyOffset);
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

    public KoreXYPoint PointFromFraction(double xFraction, double yFraction)
    {
        // Get the point at a given fraction along the rectangle axis
        // - 0 = top/left, 1 = bottom/right, 0.5 = midpoint
        // - Values beyond 0->1 are allowed, to go outside the rectangle
        return new KoreXYPoint(Left + (Width * xFraction), Top + (Height * yFraction));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Checks
    // --------------------------------------------------------------------------------------------

    public bool Contains(KoreXYPoint xy)
    {
        return xy.X >= Left && xy.X <= Right && xy.Y >= Top && xy.Y <= Bottom;
    }

    public bool Intersects(KoreXYRect other)
    {
        return !(other.Left > Right ||
                 other.Right < Left ||
                 other.Top > Bottom ||
                 other.Bottom < Top);
    }

}
