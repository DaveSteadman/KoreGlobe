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
        AddTestMeshData_Sphere();
        AddTestMeshData_Ribbon();
        AddTestMeshData_BoxArc();
    }

    public override void _Process(double delta)
    {
        // This method is called every frame.
        // You can use 'delta' to make frame-rate independent calculations.
        // GD.Print("Processing frame with delta: " + delta);

    }


    // ---------------------------------------------------------------------------------------------
    // MARK: BOX
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
    // MARK: PYRAMID
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
    // ---------------------------------------------------------------------------------------------
    // MARK: SPHERE
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_Sphere()
    {
        // Function to add a test sphere mesh
        Node3D SphereNode = new Node3D() { Name = "SphereNode" };
        AddChild(SphereNode);
        SphereNode.Position = new Vector3(0, 1.6f, 0);

        // 1 - Basic Sphere
        {
            var sphereMesh1 = KoreMeshDataPrimitives.BasicSphere(0.5f, new KoreColorRGB(0, 0, 255), 16);

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(sphereMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(sphereMesh1);

            SphereNode.AddChild(childMeshNode1);
            SphereNode.AddChild(childSurfaceMeshNode1);
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: RIBBON
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_Ribbon()
    {
        // Function to add a test ribbon mesh
        Node3D RibbonNode = new Node3D() { Name = "RibbonNode" };
        AddChild(RibbonNode);
        RibbonNode.Position = new Vector3(0, 0, 1.6f);

        // Test Ribbon Mesh
        {
            var ribbonMesh = GodotMeshPrimitivesDropEdgeTile.TestRibbon();

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(ribbonMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(ribbonMesh);

            // debug print the mesh
            {
                GD.Print("Ribbon Mesh Vertices Count: " + ribbonMesh.Vertices.Count);
                GD.Print("Ribbon Mesh Triangles Count: " + ribbonMesh.Triangles.Count);
                GD.Print("Ribbon Mesh Lines Count: " + ribbonMesh.Lines.Count);
            }

            RibbonNode.AddChild(childMeshNode1);
            RibbonNode.AddChild(childSurfaceMeshNode1);
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: BOX ARC
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_BoxArc()
    {
        // Function to add a test box arc mesh
        Node3D BoxArcNode = new Node3D() { Name = "BoxArcNode" };
        AddChild(BoxArcNode);
        BoxArcNode.Position = new Vector3(0, 0, -3.6f);

        // Test Box Arc Mesh
        {
            var boxArcMesh = KoreMeshDataPrimitives.BoxArc(
                1.5f, 3.75f, // Inner and outer radius
                13.0f, // Vertical angle in degrees
                40.0f, 120.0f // Horizontal start and delta angles in degrees
            );

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(boxArcMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(boxArcMesh);

            BoxArcNode.AddChild(childMeshNode1);
            BoxArcNode.AddChild(childSurfaceMeshNode1);
        }
    }



}
