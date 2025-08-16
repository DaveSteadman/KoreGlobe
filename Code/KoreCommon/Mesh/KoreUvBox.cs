
//using System;
//using System.Numerics;

namespace KoreCommon;


// KoreUVBox: A struct to hold the UV co-oridnates of a box in a texture
// Also includes the functionality of a UV box within a larger texture.



public struct KoreUVBox
{
    public KoreXYVector TopLeft;
    public KoreXYVector BottomRight;

    public KoreUVBox(KoreXYVector topLeft, KoreXYVector bottomRight)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;
    }

    public KoreUVBox(double topLeftX, double topLeftY, double bottomRightX, double bottomRightY)
    {
        TopLeft     = new KoreXYVector(topLeftX, topLeftY);
        BottomRight = new KoreXYVector(bottomRightX, bottomRightY);
    }

    public static KoreUVBox Zero { get { return new KoreUVBox(KoreXYVector.Zero, KoreXYVector.Zero); } }
    public static KoreUVBox Full { get { return new KoreUVBox(KoreXYVector.Zero, KoreXYVector.One); } }

    // --------------------------------------------------------------------------------------------

    public readonly KoreNumericRange<double> UVRangeX { get { return new KoreNumericRange<double>(TopLeft.X, BottomRight.X); } }
    public readonly KoreNumericRange<double> UVRangeY { get { return new KoreNumericRange<double>(TopLeft.Y, BottomRight.Y); } }

    // --------------------------------------------------------------------------------------------

    // Get the UV coordinates within the box based on XY fractions
    // xFraction and yFraction should be in the range [0, 1]

    public KoreXYVector GetUV(double xFraction, double yFraction)
    {
        // Clamp the fractions to the range [0, 1] to ensure valid interpolation
        xFraction = KoreValueUtils.ClampD(xFraction, 0.0f, 1.0f);
        yFraction = KoreValueUtils.ClampD(yFraction, 0.0f, 1.0f);

        // Interpolate between the TopLeft and BottomRight coordinates
        double u = KoreValueUtils.Interpolate(TopLeft.X, BottomRight.X, xFraction);
        double v = KoreValueUtils.Interpolate(TopLeft.Y, BottomRight.Y, yFraction);

        return new KoreXYVector(u, v);
    }

    // --------------------------------------------------------------------------------------------

    // Get a 2D grid of UV coordinates based on the dimensions of a destination point grid
    // Quick UV generation method.
    // Usage: KoreXYVector[,] uvGrid = uvBox.GetUVGrid(10, 10);
    public KoreXYVector[,] GetUVGrid(int horizSize, int vertSize)
    {
        var uvGrid = new KoreXYVector[horizSize, vertSize];

        // Get 2 1D arrays to define the values in the range
        KoreNumeric1DArray<double> uRange = KoreNumeric1DArrayOps<double>.ListForRange(TopLeft.X, BottomRight.X, horizSize);
        KoreNumeric1DArray<double> vRange = KoreNumeric1DArrayOps<double>.ListForRange(TopLeft.Y, BottomRight.Y, vertSize);

        if (TopLeft.X > BottomRight.X)
            KoreCentralLog.AddEntry($"ERROR: UVBox X backwards");
        if (TopLeft.Y > BottomRight.Y)
            KoreCentralLog.AddEntry($"ERROR: UVBox Y backwards");

        for (int x = 0; x < horizSize; x++)
        {
            for (int y = 0; y < vertSize; y++)
            {
                uvGrid[x, y] = new KoreXYVector(uRange[x], vRange[y]);
            }
        }

        return uvGrid;
    }

    // --------------------------------------------------------------------------------------------

    // create a UV box from a grid
    // (0,0) is top left box, (size-1, size-1) is bottom right box.

    // Creates a UV box for a subtile within the main tile
    public static KoreUVBox BoxFromGrid(KoreXYVector topLeft, KoreXYVector bottomRight, int horizSize, int vertSize, int horizIndex, int vertIndex)
    {
        double horizStep = 1.0f / horizSize;
        double vertStep  = 1.0f / vertSize;

        double leftValue  = topLeft.X + horizIndex * horizStep * (bottomRight.X - topLeft.X);
        double rightValue = topLeft.X + (horizIndex + 1) * horizStep * (bottomRight.X - topLeft.X);
        double topValue   = topLeft.Y + vertIndex * vertStep * (bottomRight.Y - topLeft.Y);
        double botValue   = topLeft.Y + (vertIndex + 1) * vertStep * (bottomRight.Y - topLeft.Y);

        return new KoreUVBox(new KoreXYVector(leftValue, topValue), new KoreXYVector(rightValue, botValue));
    }

    public KoreUVBox BoxFromGrid(KoreNumeric2DPosition<int> innerBoxPos)
    {
        // Calculate the horizontal and vertical step sizes
        double horizStep = (BottomRight.X - TopLeft.X) / innerBoxPos.ExtentX;
        double vertStep  = (BottomRight.Y - TopLeft.Y) / innerBoxPos.ExtentY;

        // Calculate the UV coordinates for the top-left corner of the inner box
        double leftValue   = TopLeft.X + innerBoxPos.PosX * horizStep;
        double rightValue  = TopLeft.X + (innerBoxPos.PosX + 1) * horizStep;
        double topValue    = TopLeft.Y + innerBoxPos.PosY * vertStep;
        double bottomValue = TopLeft.Y + (innerBoxPos.PosY + 1) * vertStep;

        // Return a new KoreUVBox using the calculated values
        return new KoreUVBox(new KoreXYVector(leftValue, topValue), new KoreXYVector(rightValue, bottomValue));
    }

}



