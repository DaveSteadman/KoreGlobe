

using KoreCommon;
using KoreCommon.SkiaSharp;

using SkiaSharp;

namespace KoreSim;

public class KoreTerrainImageTile
{
    public KoreLLBox LLBox = KoreLLBox.Zero;
    public string ImagePath = string.Empty;

    // --------------------------------------------------------------------------------------------

    // Zero tile
    public static KoreTerrainImageTile Zero
    {
        get { return new KoreTerrainImageTile(); }
    }

    // --------------------------------------------------------------------------------------------

    public void LoadImage()
    {
        
    }

    // --------------------------------------------------------------------------------------------

    public void UnloadImage()
    {

    }

    // --------------------------------------------------------------------------------------------

    public KoreColorRGB GetTerrainColor(double latDegs, double lonDegs, string mapLibraryPath)
    {
        KoreLLPoint pos = new() { LatDegs = latDegs, LonDegs = lonDegs };

        if (LLBox.Contains(pos))
        {
            // Standardise and create the single path
            string imagePath = KoreFileOps.JoinPaths(mapLibraryPath, ImagePath);

            // Load the file, with the using clause so its correctly disposed of afterwards
            // Load the image (SkiaSharp) with proper disposal
            using (SKBitmap tileImage = KoreSkiaSharpBitmapOps.LoadBitmap(imagePath))
            {
                // Use the lat/lon values to get fractions across the image
                double fracBottomToTop = LLBox.LatRangeDegs.FractionInRange(pos.LatDegs);
                double fracLeftToRight = LLBox.LonRangeDegs.FractionInRange(pos.LonDegs);
                double fracTopToBottom = 1 - fracBottomToTop;

                // Determine the pixel position we want to query
                int tileImageWidth = tileImage.Width;
                int tileImageHeight = tileImage.Height;
                int pixelX = (int)(fracLeftToRight * (tileImageWidth - 1));
                int pixelY = (int)(fracTopToBottom * (tileImageHeight - 1));

                // Check we're in-bounds on the pixel positions
                if (pixelX < 0) pixelX = 0;
                if (pixelX >= tileImageWidth) pixelX = tileImageWidth - 1;
                if (pixelY < 0) pixelY = 0;
                if (pixelY >= tileImageHeight) pixelY = tileImageHeight - 1;

                // Get and return the pixel color
                SKColor pixelColor = tileImage.GetPixel(pixelX, pixelY);
                KoreColorRGB returnColor = KoreSkiaSharpConv.ToKoreColorRGB(pixelColor);
                return returnColor;
            }
        }
        return KoreColorRGB.Zero;
    }


}

