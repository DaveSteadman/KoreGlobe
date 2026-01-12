// <fileheader>

using System;


// Class to define a 2D grid and a position within it:
// - A grid is a number of cells vertically and horizontally.
// - A position is a 0 to n-1 index in each axis

namespace KoreCommon;

public struct Kore2DGridPos
{
    public int Width; // Size of the grid. EG 3 cells wide
    public int Height;
    public int PosX;  // Position in the grid. EG 0 to 2
    public int PosY;

    // Get the fractions across the box (starting in top left) for the edges of the Position
    public float TopEdgeFraction    => (float)(PosY)     / (float)Height;
    public float BottomEdgeFraction => (float)(PosY + 1) / (float)Height;
    public float LeftEdgeFraction   => (float)(PosX)    / (float)Width;
    public float RightEdgeFraction  => (float)(PosX + 1) / (float)Width;

    public Kore2DGridPos(int width, int height, int posX, int posY)
    {
        Width  = width;
        Height = height;
        PosX   = posX;
        PosY   = posY;
    }

    public float CellEdgeFraction(int posX, int posY, KoreXYRectEdge cellEdge)
    {
        return cellEdge switch
        {
            KoreXYRectEdge.Top    => (float)(posY)     / (float)Height,
            KoreXYRectEdge.Bottom => (float)(posY + 1) / (float)Height,
            KoreXYRectEdge.Left   => (float)(posX)     / (float)Width,
            KoreXYRectEdge.Right  => (float)(posX + 1) / (float)Width,
            _ => throw new ArgumentOutOfRangeException(nameof(cellEdge), "Invalid KoreXYRectEdge value")
        };
    }

    public (float leftFraction, float rightFraction, float topFraction, float bottomFraction) CellEdgeFractions()
    {
        return (LeftEdgeFraction, RightEdgeFraction, TopEdgeFraction, BottomEdgeFraction);
    }

    public (float cellLeftFraction, float cellRightFraction, float cellTopFraction, float cellBottomFraction) CellEdgeFractions(int posX, int posY)
    {
        return (CellEdgeFraction(posX, posY, KoreXYRectEdge.Left),
                CellEdgeFraction(posX, posY, KoreXYRectEdge.Right),
                CellEdgeFraction(posX, posY, KoreXYRectEdge.Top),
                CellEdgeFraction(posX, posY, KoreXYRectEdge.Bottom));
    }

    // Override ToString to report the object content
    public override string ToString()
    {
        return $"Width:{Width} Height:{Height} PosX:{PosX} PosY:{PosY}";
    }
}


