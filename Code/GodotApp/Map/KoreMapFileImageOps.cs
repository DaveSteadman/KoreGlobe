using System;
using System.Collections.Generic;

using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

public static class KoreMapFileImageOps
{
    // Function to load all the webp images from the child tiles, combining them into a single image, and then resizing it and
    // saving it out as the parent tile image.

    // We're using Godot image functions, not the standard C# library.

    // --------------------------------------------------------------------------------------------

    // Usage: KoreMapFileImageOperations.CollateChildTileImages("AB");
    public static void CollateChildTileImages(string strParentTileCode)
    {
        // Create the tile code
        // Load the parent tile image as a background to fill in for missing child tiles
        KoreMapTileCode parentTileCode = new KoreMapTileCode(strParentTileCode);
        KoreMapTileFilepaths parentFilepaths = new KoreMapTileFilepaths(parentTileCode);

        // Load the child tile images
        int numChildTilesX = parentTileCode.ChildCodesWidth();
        int numChildTilesY = parentTileCode.ChildCodesHeight();

        // Determine the working tile image size (1024 X each dimension)
        int workingTileSize = 1024;
        int workingImageSizeX = workingTileSize * numChildTilesX;
        int workingImageSizeY = workingTileSize * numChildTilesY;

        // Create the working image
        Image workingImage = Image.CreateEmpty(workingImageSizeX, workingImageSizeY, false, Image.Format.Rgb8);

        // Load any parent tile image, resize it to the full working image size, and paste it into the working image
        if (parentFilepaths.WebpFileExists)
        {
            Image? parentTileImage = KoreGodotImageOps.LoadImage(parentFilepaths.WebpFilepath);
            if (parentTileImage != null)
            {
                parentTileImage!.Resize(workingImageSizeX, workingImageSizeY);
                workingImage.BlitRect(parentTileImage, new Rect2I(0, 0, workingImageSizeX, workingImageSizeY), new Vector2I(0, 0));
            }
        }

        List<KoreMapTileCode> childCodesList = parentTileCode.ChildCodesList();

        foreach (KoreMapTileCode currCode in childCodesList)
        {
            KoreMapTileFilepaths currFilepaths = new KoreMapTileFilepaths(currCode);

            if (currFilepaths.WebpFileExists)
            {
                Image? childTileImage = KoreGodotImageOps.LoadImage(currFilepaths.WebpFilepath);
                if (childTileImage != null)
                {
                    // resize the child image to the working tile size
                    childTileImage!.Resize(workingTileSize, workingTileSize);

                    // Calculate the position to paste the child image
                    Kore2DGridPos tilePos = currCode.ChildPositionInParent();
                    int topLeftX = tilePos.PosX * workingTileSize;
                    int topLeftY = tilePos.PosY * workingTileSize;
                    Vector2I pastePos = new Vector2I(topLeftX, topLeftY);
                    Rect2I pasteRect = new Rect2I(0, 0, workingTileSize, workingTileSize);

                    // Paste the child image into the working image
                    workingImage.BlitRect(childTileImage, pasteRect, pastePos);
                }
            }
        }

        // Save the working image for analysis
        workingImage.Resize(workingTileSize, workingTileSize);
        workingImage.SaveWebp(parentFilepaths.WebpFilepath);
    }

    // --------------------------------------------------------------------------------------------

    // Special case, collating the lvl0 tiles into a single world image, saved to a special case filename
    public static void CollateChildTileImages_Lvl0()
    {
        int numChildTilesX = 12; // 30x30degree tiles
        int numChildTilesY = 6;
        int workingTileSize = 1024;

        int newImageX = numChildTilesX * workingTileSize;
        int newImageY = numChildTilesY * workingTileSize;

        Image workingImage = Image.CreateEmpty(newImageX, newImageY, false, Image.Format.Rgb8);

        // loop through each of the lvl0 tiles.
        for (int currXIndex = 0; currXIndex < numChildTilesX; currXIndex++)
        {
            for (int currYIndex = 0; currYIndex < numChildTilesY; currYIndex++)
            {
                // Create the tile code
                KoreMapTileCode parentTileCode = new KoreMapTileCode(currXIndex, currYIndex);
                KoreMapTileFilepaths parentFilepaths = new KoreMapTileFilepaths(parentTileCode);

                int destXPos = currXIndex * workingTileSize;
                int destYPos = currYIndex * workingTileSize;
                Vector2I destPos = new Vector2I(destXPos, destYPos);

                // Load any parent tile image, resize it to the full working image size, and paste it into the working image
                if (parentFilepaths.WebpFileExists)
                {
                    Image? parentTileImage = KoreGodotImageOps.LoadImage(parentFilepaths.WebpFilepath);

                    if (parentTileImage != null)
                    {
                        parentTileImage!.Resize(workingTileSize, workingTileSize);
                        workingImage.BlitRect(parentTileImage, new Rect2I(0, 0, workingTileSize, workingTileSize), destPos);
                    }
                }
            }
        }

        // Save the working image for analysis
        string worldTileFilepath = KoreMapTileFilepaths.WorldTileFilepath();
        workingImage.SaveWebp(worldTileFilepath);

    }

    // --------------------------------------------------------------------------------------------

    // Function to take one high resolution image, and chreate the images for this tile, and the set of child tiles
    public static void DivideChildTileImages(string strParentTileCode, string parentTileImagePath)
    {
        // Load the source image
        Image? workingImage = KoreGodotImageOps.LoadImage(parentTileImagePath);
        if (workingImage == null) return;

        // Determine the size of the source image, such that we can divide it well.
        KoreMapTileCode parentTileCode = new KoreMapTileCode(strParentTileCode);
        KoreMapTileFilepaths parentFilepaths = new KoreMapTileFilepaths(parentTileCode);

        // Determine the working tile image size (1024 X each dimension)
        int numChildTilesX = parentTileCode.ChildCodesWidth();
        int numChildTilesY = parentTileCode.ChildCodesHeight();
        int workingTileSize = 1024;
        int workingImageSizeX = workingTileSize * numChildTilesX;
        int workingImageSizeY = workingTileSize * numChildTilesY;

        // resize the working image
        workingImage!.Resize(workingImageSizeX, workingImageSizeY);

        // Get the child tiles, and iterate through them saving out the new tiles
        List<KoreMapTileCode> childCodesList = parentTileCode.ChildCodesList();
        foreach (KoreMapTileCode currCode in childCodesList)
        {
            KoreMapTileFilepaths currFilepaths = new KoreMapTileFilepaths(currCode);

            // Calculate the position to paste the child image
            Kore2DGridPos tilePos = currCode.ChildPositionInParent();
            int topLeftX = tilePos.PosX * workingTileSize;
            int topLeftY = tilePos.PosY * workingTileSize;
            Rect2I copyRect = new Rect2I(topLeftX, topLeftY, workingTileSize, workingTileSize);

            // Create the child image
            Image childImage = workingImage.GetRegion(copyRect);

            // Save the child image
            childImage.SaveWebp(currFilepaths.WebpFilepath);
        }

        // Resize the working image and save it out save the parent tile image
        workingImage.Resize(workingTileSize, workingTileSize);
        workingImage.SaveWebp(parentFilepaths.WebpFilepath);
    }

    // --------------------------------------------------------------------------------------------

    // Special case: Takes a new 2:1 aspect ratio world image and divides it into the 12x6 lvl0 tiles
    // Usage: KoreMapFileImageOperations.DivideChildTileImages_Lvl0(worldimagePath);

    public static void DivideChildTileImages_Lvl0(string parentTileImagePath)
    {
        // Load the source image
        Image? workingImage = KoreGodotImageOps.LoadImage(parentTileImagePath);
        if (workingImage == null) return;

        // Determine the size of the source image, such that we can divide it well.
        int numChildTilesX = 12; // 30x30degree tiles
        int numChildTilesY = 6;
        int workingTileSize = 1024;
        int workingImageSizeX = workingTileSize * numChildTilesX;
        int workingImageSizeY = workingTileSize * numChildTilesY;

        // resize the working image, to make the division cleaner
        workingImage!.Resize(workingImageSizeX, workingImageSizeY);

        // Get the child tiles, and iterate through them saving out the new tiles
        for (int currXIndex = 0; currXIndex < numChildTilesX; currXIndex++)
        {
            for (int currYIndex = 0; currYIndex < numChildTilesY; currYIndex++)
            {
                KoreMapTileCode currCode = new KoreMapTileCode(currXIndex, currYIndex);

                // Calculate the position to paste the child image
                Kore2DGridPos tilePos = currCode.ChildPositionInParent();
                int topLeftX = tilePos.PosX * workingTileSize;
                int topLeftY = tilePos.PosY * workingTileSize;
                Rect2I copyRect = new Rect2I(topLeftX, topLeftY, workingTileSize, workingTileSize);

                // Create the child image
                Image childImage = workingImage.GetRegion(copyRect);

                // Save the child image
                KoreMapTileFilepaths currFilepaths = new KoreMapTileFilepaths(currCode);
                childImage.SaveWebp(currFilepaths.WebpFilepath);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Function to load the image for this tile, and the set of child tiles
    // Usage: Image[,] childImages = KoreMapFileImageOperations.DivideIntoChildTileImages(tileCode, parentImage);

    public static Image[,] DivideIntoChildTileImages(KoreMapTileCode parentTileCode, Image parentImage)
    {
        // Determine the working tile image size (1024 X each dimension)
        int numChildTilesX = parentTileCode.ChildCodesWidth();
        int numChildTilesY = parentTileCode.ChildCodesHeight();
        int workingTileSize = 1024;
        int workingImageSizeX = workingTileSize * numChildTilesX;
        int workingImageSizeY = workingTileSize * numChildTilesY;

        // Resize the working image to a multiple of the output tile size, so the division is cleaner
        parentImage.Resize(workingImageSizeX, workingImageSizeY);

        // Get the child tiles, and iterate through them saving out the new tiles
        List<KoreMapTileCode> childCodesList = parentTileCode.ChildCodesList();
        Image[,] childImages = new Image[numChildTilesX, numChildTilesY];

        foreach (KoreMapTileCode currCode in childCodesList)
        {
            // Calculate the position to paste the child image
            Kore2DGridPos tilePos = currCode.ChildPositionInParent();
            int topLeftX = tilePos.PosX * workingTileSize;
            int topLeftY = tilePos.PosY * workingTileSize;
            Rect2I copyRect = new Rect2I(topLeftX, topLeftY, workingTileSize, workingTileSize);

            // Create the child image
            Image childImage = parentImage.GetRegion(copyRect);
            childImages[tilePos.PosX, tilePos.PosY] = childImage;
        }

        return childImages;
    }

    // --------------------------------------------------------------------------------------------


}
