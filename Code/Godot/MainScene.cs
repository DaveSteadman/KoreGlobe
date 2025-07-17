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

        CreateTestMeshData_Box();
        AddTestMeshData_Pyramid();
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

    private void CreateTestMeshData_Box()
    {
        Node3D Cube1Node = new Node3D() { Name = "Cube1Node" };
        Node3D Cube2Node = new Node3D() { Name = "Cube2Node" };
        Node3D Cube3Node = new Node3D() { Name = "Cube3Node" };
        AddChild(Cube1Node);
        AddChild(Cube2Node);
        AddChild(Cube3Node);

        Cube1Node.Position = new Vector3(-1.1f, 0, 0);
        Cube2Node.Position = new Vector3(0, 0, 0);
        Cube3Node.Position = new Vector3(1.1f, 0, 0);

        // 1
        {
            var cubeMesh1 = KoreMeshDataPrimitives.BasicCube(0.5f, new KoreColorRGB(255, 0, 0));

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(cubeMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(cubeMesh1);

            Cube1Node.AddChild(childMeshNode1);
            Cube1Node.AddChild(childSurfaceMeshNode1);
        }

        // 2
        {
            var cubeMesh2 = KoreMeshDataPrimitives.BasicCubeSharpEdges(0.5f, new KoreColorRGB(255, 0, 0));

            KoreGodotLineMesh childMeshNode2 = new KoreGodotLineMesh();
            childMeshNode2.UpdateMesh(cubeMesh2);

            KoreGodotSurfaceMesh childSurfaceMeshNode2 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode2.UpdateMesh(cubeMesh2);

            Cube2Node.AddChild(childMeshNode2);
            Cube2Node.AddChild(childSurfaceMeshNode2);
        }

        // 3
        {
            var cubeMesh3 = KoreMeshDataPrimitives.BasicCubeSharpEdges2(0.5f, new KoreColorRGB(255, 0, 0, 128));

            KoreGodotLineMesh childMeshNode3 = new KoreGodotLineMesh();
            childMeshNode3.UpdateMesh(cubeMesh3);

            KoreGodotSurfaceMesh childSurfaceMeshNode3 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode3.UpdateMesh(cubeMesh3);

            Cube3Node.AddChild(childMeshNode3);
            Cube3Node.AddChild(childSurfaceMeshNode3);
        }
    }

    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_Pyramid()
    {
        Node3D Pyramid1Node = new Node3D() { Name = "Pyramid1Node" };
        Node3D Pyramid2Node = new Node3D() { Name = "Pyramid2Node" };
        AddChild(Pyramid1Node);
        AddChild(Pyramid2Node);

        Pyramid1Node.Position = new Vector3(-1.1f, 1.6f, 0);
        Pyramid2Node.Position = new Vector3(1.1f, 1.6f, 0);

        // 1 - Basic Pyramid
        {
            var pyramidMesh1 = KoreMeshDataPrimitives.BasicPyramid(
                new KoreXYZPoint(0, 0, 0), // Apex point
                new KoreXYZVector(0, -1, 0), // Apex to base center
                new KoreXYZVector(1, 0, 0), // Base forward vector
                1.0f, 1.0f, // Width and height
                new KoreColorRGB(20, 100, 20) // Color
            );

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(pyramidMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(pyramidMesh1);

            Pyramid1Node.AddChild(childMeshNode1);
            Pyramid1Node.AddChild(childSurfaceMeshNode1);
        }

        // 2 - Sharp Edges Pyramid
        {
            var pyramidMesh2 = KoreMeshDataPrimitives.BasicPyramidSharpEdges(
                new KoreXYZPoint(0, 0, 0), // Apex point
                new KoreXYZVector(0, -1, 0), // Apex to base center
                new KoreXYZVector(1, 0, 0), // Base forward vector
                1.0f, 1.0f, // Width and height
                new KoreColorRGB(20, 100, 20) // Color
            );

            KoreGodotLineMesh childMeshNode2 = new KoreGodotLineMesh();
            childMeshNode2.UpdateMesh(pyramidMesh2);

            KoreGodotSurfaceMesh childSurfaceMeshNode2 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode2.UpdateMesh(pyramidMesh2);

            Pyramid2Node.AddChild(childMeshNode2);
            Pyramid2Node.AddChild(childSurfaceMeshNode2);
        }
    }
}
