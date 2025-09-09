using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;
using SkiaSharp;

using KoreCommon;
using KoreCommon.SkiaSharp;
using KoreSim;


#nullable enable

// Tile Image related functions

public partial class KoreZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Image
    // --------------------------------------------------------------------------------------------

    private KoreColorRGB[,] TileImage(int azCount, int elCount)
    {
        KoreColorRGB[,] colorMap = new KoreColorRGB[azCount, elCount];

        // Loop up the tile structure until we find an image to use

        var currTileCode = TileCode;
        KoreMapTileFilepaths filepaths = new KoreMapTileFilepaths(currTileCode);

        while (!filepaths.WebpFileExists && (currTileCode.MapLvl > 0))
        {
            currTileCode = currTileCode.ParentCode();
            filepaths = new KoreMapTileFilepaths(currTileCode);
        }

        if (filepaths.WebpFileExists)
        {
            // Load the image (SkiaSharp) with proper disposal
            using (SKBitmap tileImage = KoreSkiaSharpBitmapOps.LoadBitmap(filepaths.WebpFilepath))
            {
                // determine the difference in angle ranges between this tile, and the tile for the image, so
                // we can subsample the image to this tile's range.

                if (TileCode.TileCode != currTileCode.TileCode)
                {
                    // We need to subsample the image to this tile's range
                    KoreLLBox currTileBox = TileCode.LLBox;
                    KoreLLBox imgTileBox = currTileCode.LLBox;

                    // Get the fraction ranges for the image tile
                    double currTileWidthDegs = currTileBox.DeltaLonDegs;
                    double currTileHeightDegs = currTileBox.DeltaLatDegs;
                    double imgTileWidthDegs = imgTileBox.DeltaLonDegs;
                    double imgTileHeightDegs = imgTileBox.DeltaLatDegs;

                    // X coordinates (longitude) - normal mapping
                    double minXFrac = (currTileBox.MinLonDegs - imgTileBox.MinLonDegs) / imgTileWidthDegs;
                    double maxXFrac = (currTileBox.MaxLonDegs - imgTileBox.MinLonDegs) / imgTileWidthDegs;

                    // Y coordinates (latitude) - FLIPPED because image Y goes down but lat goes up
                    double minYFrac = (imgTileBox.MaxLatDegs - currTileBox.MaxLatDegs) / imgTileHeightDegs;
                    double maxYFrac = (imgTileBox.MaxLatDegs - currTileBox.MinLatDegs) / imgTileHeightDegs;

                    // Create subsection with proper disposal
                    using (SKBitmap newTileImage = KoreSkiaSharpBitmapOps.BitmapSubsection(tileImage, (float)minXFrac, (float)minYFrac, (float)maxXFrac, (float)maxYFrac))
                    {
                        colorMap = KoreSkiaSharpBitmapOps.SampleBitmapColors(newTileImage, azCount, elCount);
                    }
                }
                else
                {
                    colorMap = KoreSkiaSharpBitmapOps.SampleBitmapColors(tileImage, azCount, elCount);
                }
            }
        }
        return colorMap;
    }
}
