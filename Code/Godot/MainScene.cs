using Godot;
using System;

using KoreCommon;

public partial class MainScene : Node3D
{

    // ---------------------------------------------------------------------------------------------
    // MARK: Node3D
    // ---------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // This method is called when the node is added to the scene.
        GD.Print("MainScene is ready!");
        
        CreateTestMeshData();
    }

    public override void _Process(double delta)
    {
        // This method is called every frame.
        // You can use 'delta' to make frame-rate independent calculations.
        // GD.Print("Processing frame with delta: " + delta);

    }


    // ---------------------------------------------------------------------------------------------
    // MARK: Test Common Mesh
    // ---------------------------------------------------------------------------------------------

    // Functions to test KoreMeshData

    private void CreateTestMeshData()
    {
        // Create a simple mesh data for testing
        var cubeMesh = KoreMeshDataPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0));        
        var cubeMesh2 = KoreMeshDataPrimitives.BasicCubeSharpEdges(1.0f, new KoreColorRGB(255, 0, 0));
        var cubeMesh3 = KoreMeshDataPrimitives.BasicCubeSharpEdges2(1.0f, new KoreColorRGB(255, 0, 0, 128));

        if (cubeMesh == null)
        {
            GD.PrintErr("Failed to create test mesh data.");
            return;
        }


        KoreGodotLineMesh childMeshNode = new KoreGodotLineMesh();
        childMeshNode.UpdateMesh(cubeMesh3);
        
        KoreGodotSurfaceMesh childSurfaceMeshNode = new KoreGodotSurfaceMesh();
        childSurfaceMeshNode.UpdateMesh(cubeMesh3);
        
        AddChild(childMeshNode);
        AddChild(childSurfaceMeshNode);
    }


}
