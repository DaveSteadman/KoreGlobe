using Godot;
using System;
using System.Collections.Generic;

using KoreCommon;

public partial class KoreGodot2DDrawTest : Node2D
{
    public List<Vector3> _testPoints = new List<Vector3>();
    public Rect2 BoundingBox = new Rect2(10, 10, 100, 100);
    public Node2D? SpriteNode;
    public bool BoxValid = false;

    public override void _Ready()
    {
        // This method is called when the node is added to the scene.
        GD.Print("KoreGodot2DDrawTest is ready!");


        var cubeMesh1 = KoreMeshDataPrimitives.BasicCube(0.5f, new KoreColorRGB(255, 0, 0));
        KoreXYZBox cubeBBox = cubeMesh1.GetBoundingBox();
        _testPoints = KoreConvPos.KoreXYZBoxToV3List(cubeBBox);
        
        SpriteNode = GetNodeOrNull<Node2D>("SpriteNode");
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
}
