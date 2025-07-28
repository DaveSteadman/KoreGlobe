using Godot;
using System;

public static class KoreWebpConverter
{
    // Usage: KoreWebpConverter.CompressPNGtoWEBP("texture.png", "texture.webp");
    public static void CompressPNGtoWEBP(string inputPngPath, string outputWebpPath)
    {
        // Load the .png file into an Image object
        Image image = new Image();
        var loadError = image.Load(inputPngPath);

        if (loadError != Error.Ok)
        {
            GD.PrintErr($"Failed to load image: {inputPngPath}");
            return;
        }

        // Save the compressed image as a .ctex file
        var saveError = image.SaveWebp(outputWebpPath, lossy: true, quality: 0.75f);

        if (saveError == Error.Ok)
        {
            GD.Print($"Image successfully compressed and saved as: {outputWebpPath}");
        }
        else
        {
            GD.PrintErr($"Failed to save compressed image: {outputWebpPath}");
        }
    }




}
