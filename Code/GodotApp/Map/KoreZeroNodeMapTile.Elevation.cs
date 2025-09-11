using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Elevation Methods
    // --------------------------------------------------------------------------------------------

    private KoreNumeric2DArray<float> LoadTileEleArr()
    {
            KoreNumeric2DArray<float> eleData = new KoreNumeric2DArray<float>();

        if (Filepaths.EleArrFileExists)
        {
            KoreElevationTile? eleTile = KoreElevationTileIO.ReadFromTextFile(Filepaths.EleArrFilepath);

            if (eleTile != null)
            {
                eleData = eleTile.ElevationData;

                // if we have a subsurface tile, set it to a lower resolution
                if (eleData.MaxVal() <= 0)
                    eleData = new KoreFloat2DArray(10, 10);

                // Write the simplified data back to the file, to be faster next time.
                // eleTile.ElevationData = TileEleData;
                // KoreElevationTileIO.WriteToTextFile(eleTile, Filepaths.EleArrFilepath);
            }
            else
            {
                KoreCentralLog.AddEntry($"Failed to load: {Filepaths.EleArrFilepath}");
            }
            eleData = eleData.CropValuesToRange(new KoreNumericRange<float>(0f, 10000f));
        }
        else
        {
            eleData = new KoreFloat2DArray(20, 20);
            //eleData.SetAllNoise(2.0f, (float)(KoreWorldConsts.EarthRadiusM / 100.0));            
        }

        return eleData;
    }

    // --------------------------------------------------------------------------------------------

    // private void SubsampleParentTileEle()
    // {
    //     KoreLLPoint tileCenter = TileCode.LLBox.CenterPoint;
    //     int tileResLat = TileSizePointsPerLvl[TileCode.MapLvl];
    //     int tileResLon = KoreElevationUtils.LonResForLat(tileResLat, tileCenter.LatRads);

    //     // Use the latitude resolution and latitude to figure out a longitude resolution the gives the "most square" tiles possible.
    //     // int tileResLon = KoreElevationUtils.LonResForLat(tileResLat, lLBox.CenterPoint.LatDegs);

    //     if (ParentTile != null)
    //     {
    //         Kore2DGridPos tileGridPos = TileCode.GridPos;

    //         // Copy the parent's elevation data - regardless of its resolution, to pass that along to the child tiles
    //         //KoreFloat2DArray RawParentTileEleData = ParentTile.ChildEleData[tileGridPos.PosX, tileGridPos.PosY];

    //         TileEleData = ParentTile.TileEleData.GetInterpolatedSubgrid(tileGridPos, tileResLon, tileResLat);
    //     }
    //     else
    //     {
    //         TileEleData = new KoreFloat2DArray(tileResLon, tileResLat);
    //     }
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Ele
    // --------------------------------------------------------------------------------------------

    public float GetLocalElevation(KoreLLPoint pos)
    {
        KoreLLBox llBounds = TileCode.LLBox;
        if (!llBounds.Contains(pos))
            return KoreElevationUtils.InvalidEle;

        (float latFrac, float lonFrac) = llBounds.GetLatLonFraction(pos);

        return 0f;  ///TileEleData.InterpolatedValue(latFrac, lonFrac);
    }

    public float GetElevation(KoreLLPoint pos)
    {
        KoreLLBox llBounds = TileCode.LLBox;
        if (!llBounds.Contains(pos))
            return KoreElevationUtils.InvalidEle;

        // If we have child tiles all constructed, the look one up and return its elevation
        if (ChildTileDataAvailable)
        {
            foreach (KoreZeroNodeMapTile currTile in ChildTiles)
            {
                if (currTile.IsPointInTile(pos))
                {
                    return currTile.GetElevation(pos);
                }
            }
        }
        // Else, return the local elevation value
        return GetLocalElevation(pos);
    }

}