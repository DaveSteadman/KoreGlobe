using System;

using Godot;

using KoreCommon;
using KoreSim;

public class KoreUVBoxDropEdgeTile
{
    // Define the basic corners of the tile (or subtile) in UV co-ordinates of its texture
    public Vector2 TopLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }

    // The edge offset for the tile, to prevent texture bleeding
    public float BoxEdgeOffset { get; set; }
    public float BoxInsetOffset { get; set; }

    public float MinX => TopLeft.X;
    public float MaxX => BottomRight.X;
    public float MinY => TopLeft.Y;
    public float MaxY => BottomRight.Y;

    // The grid of UVs output for the tile, accomodating the drop-edge and insets
    private Vector2[,] UVGrid;

    // GloUVBoxDropEdgeTile.UVTopLeft and GloUVBoxDropEdgeTile.BottomRight
    public static Vector2 UVTopLeft = new Vector2(0.0001f, 0.0001f);
    public static Vector2 UVBottomRight = new Vector2(0.9999f, 0.9999f);

    public bool UseEdgeOffsets { get; set; } = false; // flag to generate a simpler UV grid when false

    public KoreNumericRange<float> UVRangeX { get { return new KoreNumericRange<float>(TopLeft.X, BottomRight.X); } }
    public KoreNumericRange<float> UVRangeY { get { return new KoreNumericRange<float>(TopLeft.Y, BottomRight.Y); } }

    // --------------------------------------------------------------------------------------------

    // Constructor that sets up the corners and initializes the UV grid

    public KoreUVBoxDropEdgeTile(Vector2 topLeft, Vector2 bottomRight, float edgeOffset = 0.0001f, float insetOffset = 0.0001f)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;

        BoxEdgeOffset = edgeOffset;
        BoxInsetOffset = insetOffset;

        //InitializeUvGrid(horizSize, vertSize);
    }

    // Constructor that derives the UV box from a parent tile and subgrid position

    public KoreUVBoxDropEdgeTile(KoreUVBoxDropEdgeTile parentBox, Kore2DGridPos gridPos)
    {
        // Calculate the new UV box from the parent box and grid position
        float minParentX = parentBox.TopLeft.X;
        float minParentY = parentBox.TopLeft.Y;
        float maxParentX = parentBox.BottomRight.X;
        float maxParentY = parentBox.BottomRight.Y;

        float diffParentX = maxParentX - minParentX;
        float diffParentY = maxParentY - minParentY;

        float childMinX = minParentX + gridPos.LeftEdgeFraction * diffParentX;
        float childMaxX = minParentX + gridPos.RightEdgeFraction * diffParentX;
        float childMinY = minParentY + gridPos.TopEdgeFraction * diffParentY;
        float childMaxY = minParentY + gridPos.BottomEdgeFraction * diffParentY;

        TopLeft = new Vector2(childMinX, childMinY);
        BottomRight = new Vector2(childMaxX, childMaxY);

        BoxEdgeOffset = parentBox.BoxEdgeOffset;
        BoxInsetOffset = parentBox.BoxInsetOffset;

        //InitializeUvGrid(horizSize, vertSize);
    }

    // --------------------------------------------------------------------------------------------

    // With no input information, return a default UV box

    public static KoreUVBoxDropEdgeTile Default(int horizSize, int vertSize)
    {
        return new KoreUVBoxDropEdgeTile(UVTopLeft, UVBottomRight, horizSize, vertSize);
    }

    public static KoreUVBoxDropEdgeTile Zero
    {
        get { return new KoreUVBoxDropEdgeTile(Vector2.Zero, Vector2.Zero); }
    }

    // Return a UV box for the full extent of an image KoreUVBoxDropEdgeTile.FullImage()
    public static KoreUVBoxDropEdgeTile FullImage()
    {
        return new KoreUVBoxDropEdgeTile(UVTopLeft, UVBottomRight);
    }

    // --------------------------------------------------------------------------------------------

    // Create a simpler grid that just interpolates across the main UV range.
    // Takes into account the TopLeft BottomRight corners.

    public void InitializeSimpleUvGrid(int horizSize, int vertSize)
    {
        UVGrid = new Vector2[horizSize, vertSize];

        float horizStep = (BottomRight.X - TopLeft.X) / (horizSize - 1);
        float vertStep = (BottomRight.Y - TopLeft.Y) / (vertSize - 1);

        for (int y = 0; y < vertSize; y++)
        {
            for (int x = 0; x < horizSize; x++)
            {
                UVGrid[x, y] = new Vector2(TopLeft.X + (x * horizStep), TopLeft.Y + (y * vertStep));
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Initializes the UV grid with offsets and insets
    public void InitializeUvGrid(int horizSize, int vertSize)
    {
        UVGrid = new Vector2[horizSize + 2, vertSize + 2];

        float horizStep = (1.0f - 2 * BoxEdgeOffset) / horizSize;
        float vertStep = (1.0f - 2 * BoxEdgeOffset) / vertSize;

        for (int y = 0; y < vertSize + 2; y++)
        {
            for (int x = 0; x < horizSize + 2; x++)
            {
                UVGrid[x, y] = CalculateUV(x, y, horizSize, vertSize, horizStep, vertStep, BoxEdgeOffset, BoxInsetOffset);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Calculates individual UV coordinates based on grid position and offsets
    private Vector2 CalculateUV(int x, int y, int horizSize, int vertSize, float horizStep, float vertStep, float edgeOffset, float insetOffset)
    {
        float u = Mathf.Lerp(TopLeft.X, BottomRight.X, GetStepPosition(x, horizSize, horizStep, edgeOffset, insetOffset));
        float v = Mathf.Lerp(TopLeft.Y, BottomRight.Y, GetStepPosition(y, vertSize, vertStep, edgeOffset, insetOffset));

        return new Vector2(u, v);
    }

    // --------------------------------------------------------------------------------------------

    // Determines the position within the step, considering offsets and insets
    private float GetStepPosition(int index, int size, float step, float edgeOffset, float insetOffset)
    {
        if (index == 0)
            return edgeOffset;
        else if (index == size + 1)
            return 1.0f - edgeOffset;
        else if (index == 1)
            return edgeOffset + insetOffset;
        else if (index == size)
            return 1.0f - edgeOffset - insetOffset;
        else
            return edgeOffset + insetOffset + (index - 1) * step;
    }

    // --------------------------------------------------------------------------------------------

    // Provides access to the precomputed UV coordinates
    public Vector2 GetUV(int x, int y)
    {
        if (x < 0 || x >= UVGrid.GetLength(0) || y < 0 || y >= UVGrid.GetLength(1))
            throw new ArgumentOutOfRangeException($"Index out of bounds: x={x}, y={y}");

        return UVGrid[x, y];
    }

    // --------------------------------------------------------------------------------------------

    // ToString override

    public override string ToString()
    {
        return $"Left:{TopLeft.X:0.00}, Right:{BottomRight.X:0.00}, Top:{TopLeft.Y:0.00}, Bottom:{BottomRight.Y:0.00} // res:{UVGrid.GetLength(0)} ";
    }

}
