// <fileheader>

using System;
using System.Numerics;

namespace KoreCommon;

// Class to define a 2D grid and a position within it:
// - A grid is a number of cells vertically and horizontally.
// - A position is a 0 to n-1 index in each axis

public struct KoreNumeric2DPosition<T> where T : INumber<T>
{
    public T PosX;  // X position within the grid. e.g. 0 to 2 in a 3 cell wide grid
    public T PosY;

    public T ExtentX;  // Size of the grid, e.g. 3 cells wide, from 0.
    public T ExtentY;

    public readonly T Area => ExtentX * ExtentY;

    public readonly double FractionX    => ExtentX == T.One ? 0.0 : Convert.ToDouble(PosX) / Convert.ToDouble(ExtentX - T.One);
    public readonly double FractionY    => ExtentY == T.One ? 0.0 : Convert.ToDouble(PosY) / Convert.ToDouble(ExtentY - T.One);
    public readonly double AspectRatio  => Convert.ToDouble(ExtentX) / Convert.ToDouble(ExtentY);

    // Get the fractions across the box (starting in top left) for the edges of the current position
    public readonly double TopEdgeFraction    => Convert.ToDouble(PosY) / Convert.ToDouble(ExtentY);
    public readonly double BottomEdgeFraction => Convert.ToDouble(PosY + T.One) / Convert.ToDouble(ExtentY);
    public readonly double LeftEdgeFraction   => Convert.ToDouble(PosX) / Convert.ToDouble(ExtentX);
    public readonly double RightEdgeFraction  => Convert.ToDouble(PosX + T.One) / Convert.ToDouble(ExtentX);

    // --------------------------------------------------------------------------------------------

    public KoreNumeric2DPosition(T posX, T posY, T extentX, T extentY)
    {
        PosX = posX;
        PosY = posY;
        ExtentX = extentX;
        ExtentY = extentY;
    }

    public KoreNumeric2DPosition(T posX, T posY, KoreNumeric2DSize<T> size)
    {
        PosX = posX;
        PosY = posY;
        ExtentX = size.Width;
        ExtentY = size.Height;
    }

    // --------------------------------------------------------------------------------------------

    public readonly double CellEdgeFraction(T posX, T posY, KoreXYRectEdge cellEdge)
    {
        return cellEdge switch
        {
            KoreXYRectEdge.Top    => Convert.ToDouble(posY)         / Convert.ToDouble(ExtentY),
            KoreXYRectEdge.Bottom => Convert.ToDouble(posY + T.One) / Convert.ToDouble(ExtentY),
            KoreXYRectEdge.Left   => Convert.ToDouble(posX)         / Convert.ToDouble(ExtentX),
            KoreXYRectEdge.Right  => Convert.ToDouble(posX + T.One) / Convert.ToDouble(ExtentX),
            _ => throw new ArgumentOutOfRangeException(nameof(cellEdge), "Invalid KoreXYRectEdge value")
        };
    }

    public readonly (double leftFraction, double rightFraction, double topFraction, double bottomFraction) CellEdgeFractions()
    {
        return (LeftEdgeFraction, RightEdgeFraction, TopEdgeFraction, BottomEdgeFraction);
    }

    public readonly (double cellLeftFraction, double cellRightFraction, double cellTopFraction, double cellBottomFraction) CellEdgeFractions(T posX, T posY)
    {
        return (CellEdgeFraction(posX, posY, KoreXYRectEdge.Left),
                CellEdgeFraction(posX, posY, KoreXYRectEdge.Right),
                CellEdgeFraction(posX, posY, KoreXYRectEdge.Top),
                CellEdgeFraction(posX, posY, KoreXYRectEdge.Bottom));
    }

    // --------------------------------------------------------------------------------------------

    public override readonly string ToString()
    {
        return $"PosX:{PosX} PosY:{PosY} ExtentX:{ExtentX} ExtentY:{ExtentY}";
    }
}
