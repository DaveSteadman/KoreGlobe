using Godot;
using System;
using System.Collections.Generic;

using KoreCommon;
using KoreCommon.SkiaSharp;
using SkiaSharp;

#nullable enable

public partial class KoreGodot2DDrawTest : Node2D
{
    public List<Vector3> _testPoints = new List<Vector3>();
    public Rect2 BoundingBox = new Rect2(10, 10, 100, 100);
    public Sprite2D? SpriteNode;
    public bool BoxValid = false;




    public override void _Ready()
    {
        // This method is called when the node is added to the scene.
        GD.Print("KoreGodot2DDrawTest is ready!");


        var cubeMesh1 = KoreMeshDataPrimitives.BasicCube(0.5f, new KoreColorRGB(255, 0, 0));
        KoreXYZBox cubeBBox = cubeMesh1.GetBoundingBox();
        _testPoints = KoreConvPos.KoreXYZBoxToV3List(cubeBBox);

        SpriteNode = GetNodeOrNull<Sprite2D>("SpriteNode");
    }

    public override void _Process(double delta)
    {
        // Get the camera and viewport
        Camera3D camera = GetViewport().GetCamera3D();
        Viewport viewport = GetViewport();

        (Rect2 boundingBox, bool success) = KoreUnprojectOps.UnprojectShapeBounds2(_testPoints, camera, viewport);
        if (success)
        {
            BoundingBox = boundingBox;
            //GD.Print($"Bounding Box: {BoundingBox}");

            // Tell Godot to redraw this node
            QueueRedraw();
        }
        BoxValid = success;

        CreateNewImage();

    }


    public override void _Draw()
    {
        if (BoxValid)
        {
            DrawRect(BoundingBox, new Color(1, 1, 1, 1f), filled: false, width: 2);

            if (SpriteNode != null)
            {
                // Draw the sprite node at the center of the bounding box
                Vector2 center = BoundingBox.Position + BoundingBox.Size / 2;
                SpriteNode.Position = center;
            }

        }
    }

    // --------------------------------------------------------------------------------------------

    public void CreateNewImage()
    {
        // Create a new image with the bounding box size
        // Image image = new Image();
        // image.Create((int)BoundingBox.Size.x, (int)BoundingBox.Size.y, false, Image.Format.Rgba8);

        // // Fill the image with a color (e.g., red)
        // image.Fill(new Color(1, 0, 0, 1));

        // // Create a texture from the image
        // ImageTexture texture = new ImageTexture();
        // texture.CreateFromImage(image);

        // // Set the texture to the sprite node if it exists
        if (SpriteNode == null)
            return;
            
        // {
        //     SpriteNode.Texture = texture;
        // }

        // Create a SkiaSharp canvas at our 50x50 size
        KoreSkiaSharpPlotter plotter = new KoreSkiaSharpPlotter(100, 100);

        // Draw an inset rect in the bounds
        SKRect bounds = new SKRect(0, 0, 100, 100);
        SKRect insetRect = new SKRect(5, 5, 95, 95);

        plotter.DrawRect(bounds, new SKPaint { Color = SKColors.White, Style = SKPaintStyle.Fill });
        plotter.DrawRect(insetRect, new SKPaint { Color = SKColors.Blue, Style = SKPaintStyle.Fill });

        string str = $"{KoreCentralTime.RuntimeIntSecs}";
        plotter.DrawTextCentered(str, new SKPoint(50, 50), 30);

        // Convert the SkiaSharp canvas to a byte array and onto Godot
        byte[] imgArray = plotter.GetPngBytes();
        Image image = new Image();
        image.LoadPngFromBuffer(imgArray);

        // dispose of the image and plotter
        plotter.Dispose();
        //image.Dispose();

        // Convert the image to a texture
        ImageTexture texture = ImageTexture.CreateFromImage(image);

        // Assign the texture to the Sprite2D's texture
        SpriteNode.Texture = texture;
    }
}
