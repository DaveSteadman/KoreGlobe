


using Godot;

using KoreGIS;
using KoreCommon;

#nullable enable

public class KoreFlatMapTile
{
    KoreMapTileCode TileCode;
    Node3D? TileNode = null;

    public KoreFlatMapTile(KoreMapTileCode tileCode)
    {
        TileCode = tileCode;
    }

    // Get a 2D array of LLA points for the tile
    // - pointsPerSide: Number of points along each side of the tile

    public KoreLLAPoint[,] TilePoints(int pointsPerSide)
    {
        // Get the LL box for the tile
        KoreLLBox boundsbox = TileCode.LLBox;

        // Create the output array of points
        // Y, X / top to bottom lat, left to right lon, so 0,0 is top-left
        KoreLLAPoint[,] points = new KoreLLAPoint[pointsPerSide, pointsPerSide];

        // Get the corner values and step sizes
        double latMinDegs = boundsbox.MinLatDegs; // Southern edge
        double latMaxDegs = boundsbox.MaxLatDegs; // Northern edge
        double lonMinDegs = boundsbox.MinLonDegs; // Western edge
        double lonMaxDegs = boundsbox.MaxLonDegs; // Eastern edge

        double latStepDegs = (latMaxDegs - latMinDegs) / (pointsPerSide - 1);
        double lonStepDegs = (lonMaxDegs - lonMinDegs) / (pointsPerSide - 1);

        // Create 1D arrays of latitudes and longitudes
        KoreNumeric1DArray<double> latVals = KoreNumeric1DArrayOps<double>.ListForRange(latMinDegs, latMaxDegs, pointsPerSide);
        KoreNumeric1DArray<double> lonVals = KoreNumeric1DArrayOps<double>.ListForRange(lonMinDegs, lonMaxDegs, pointsPerSide);

        // Iterate over the points, getting the LLA for each point
        for (int y = 0; y < pointsPerSide; y++)
        {
            for (int x = 0; x < pointsPerSide; x++)
            {
                points[y, x] = new KoreLLAPoint() { LatDegs = latVals[y], LonDegs = lonVals[x], AltMslM = 0.0 };
            }
        }

        return points;
    }

}



