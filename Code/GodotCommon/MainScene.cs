using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

public partial class MainScene : Node3D
{
    // MainScene.UIMount
    public static Control? UIMount = null;

    // ---------------------------------------------------------------------------------------------
    // MARK: Node3D
    // ---------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // // Find the place to mount other UI elements - to be at the top of the tree
        // UIMount = GetNodeOrNull<Control>("UIMount");
        // if (UIMount == null) GD.PrintErr("MainScene: UIMount node not found.");

        // if (UIMount != null)
        // {
        //     // Load the UI Top scene first (so it appears underneath)
        //     PackedScene uiTopScene = GD.Load<PackedScene>("res://Scenes/UITop.tscn");
        //     Node uiTop = uiTopScene.Instantiate();
        //     AddChild(uiTop);

        //     // Load and run the splash screen (so it appears on top)
        //     PackedScene splashScene = GD.Load<PackedScene>("res://Scenes/SplashScreen.tscn");
        //     Node splash = splashScene.Instantiate();
        //     AddChild(splash);
        // }

        // This method is called when the node is added to the scene.
        GD.Print("MainScene is ready!");

        CreateTestMeshData_Box();
        AddTestMeshData_Pyramid();
        AddTestMeshData_Sphere();
        AddTestMeshData_Ribbon();
        AddTestMeshData_BoxArc();
        AddTestMeshData_Cylinder();
        AddTestMeshData_Bezier();
        AddTestMeshData_Hemisphere();
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

    // ---------------------------------------------------------------------------------------------
    // MARK: CYLINDER
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_Cylinder()
    {
        // Function to add a test cylinder mesh
        Node3D CylinderNode = new Node3D() { Name = "CylinderNode" };
        AddChild(CylinderNode);
        CylinderNode.Position = new Vector3(0, -1.6f, 0);

        // Test Cylinder Mesh
        {
            // Cylinder(KoreXYZVector p1, KoreXYZVector p2, double p1radius, double p2radius, int sides, bool endsClosed)

            KoreXYZVector p1 = new KoreXYZVector(-1, 3, 3);
            KoreXYZVector p2 = new KoreXYZVector(2, 3, 3);

            var cylinderMesh = KoreMeshDataPrimitives.Cylinder(p1, p2, 0.5, 1.5, 24, true);

            // Debug print the mesh to JSON
            // string jsonStr = KoreMeshDataIO.ToJson(cylinderMesh, dense: false);
            // //GD.Print("KoreGodotSurfaceMesh: UpdateMesh - Mesh Data: ", jsonStr);

            // // dump the string out to a text file
            // System.IO.File.WriteAllText("DEBUG_cylinderMesh.txt", jsonStr);

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(cylinderMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(cylinderMesh);

            CylinderNode.AddChild(childMeshNode1);
            CylinderNode.AddChild(childSurfaceMeshNode1);
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: BEZIER
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_Bezier()
    {
        // Function to add a test Bezier mesh
        Node3D BezierNode = new Node3D() { Name = "BezierNode" };
        AddChild(BezierNode);
        BezierNode.Position = new Vector3(7f, 1f, 7f);

        // Test Bezier Mesh
        {
            var bezierMesh = KoreMeshDataPrimitives.Bezier3Line(
                new KoreXYZVector(0, 0, 0),
                new KoreXYZVector(1, 1, 0),
                new KoreXYZVector(2, 0.5, 0),
                new KoreColorRGB(255, 255, 255),
                50);

            // Debug print the mesh to JSON to file
            // string jsonStr = KoreMeshDataIO.ToJson(bezierMesh, dense: false);
            // System.IO.File.WriteAllText("DEBUG_bezierMesh.txt", jsonStr);

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh() { Name = "Bezier3LineMesh" };
            childMeshNode1.UpdateMesh(bezierMesh);

            // KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            // childSurfaceMeshNode1.UpdateMesh(bezierMesh);

            BezierNode.AddChild(childMeshNode1);
            // BezierNode.AddChild(childSurfaceMeshNode1);
        }


        // 4 point bezier
        {
            var bezierMesh = KoreMeshDataPrimitives.Bezier4Line(
                new KoreXYZVector(0, 0, 0.1),
                new KoreXYZVector(1, 1, 0.1),
                new KoreXYZVector(2, 0.5, 0.1),
                new KoreXYZVector(3, 1, 0.1),
                new KoreColorRGB(255, 255, 255),
                50);

            KoreGodotLineMesh childMeshNode4 = new KoreGodotLineMesh() { Name = "Bezier4LineMesh" };
            childMeshNode4.UpdateMesh(bezierMesh);

            BezierNode.AddChild(childMeshNode4);
        }

        // Many-point Bezier
        {
            // Use sin/cos operations on XZ axis, with an ever-increaing Y, to create a rising spiral of points
            double radius = 1.0;
            double radiusInc = 0.005;
            double startingY = 1;
            double yInc = 0.01;
            double numPoints = 500;
            int divisions = 50; // Number of segments to divide the Bezier curve into
            double angleIncrement = KoreAngle.DegsToRads(5); // Angle increment for each point

            List<KoreXYZPoint> controlPoints = new List<KoreXYZPoint>();

            for (int i = 0; i < numPoints; i++)
            {
                double angle = i * angleIncrement;
                double x = radius * Math.Cos(angle);
                double z = radius * Math.Sin(angle);
                double y = startingY + (i * yInc);

                controlPoints.Add(new KoreXYZPoint(x, y, z));
                radius += radiusInc; // Increase the radius for the next point
            }

            // Create the Bezier mesh from the control points
            List<KoreXYZPoint> points = KoreMeshDataPrimitives.PointsListFromBezier(controlPoints, divisions);

            List<KoreXYZVector> vpoints = new List<KoreXYZVector>();
            foreach (KoreXYZPoint point in points)
            {
                vpoints.Add(new KoreXYZVector(point.X, point.Y, point.Z));
            }

            KoreMeshData bezierMesh = new KoreMeshData();
            bezierMesh.AddPolyLine(vpoints, KoreColorPalette.Colors["Orange"]);

            KoreGodotLineMesh childMeshNode4 = new KoreGodotLineMesh() { Name = "BezierPolyLineMesh" };
            childMeshNode4.UpdateMesh(bezierMesh);

            BezierNode.AddChild(childMeshNode4);

        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: HEMISPHERE
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_Hemisphere()
    {
        // Function to add a test hemisphere mesh
        Node3D HemisphereNode = new Node3D() { Name = "HemisphereNode" };
        AddChild(HemisphereNode);
        HemisphereNode.Position = new Vector3(0, -1.0f, 2.5f);

        // Test Hemisphere Mesh
        {
            var hemisphereMesh = KoreMeshDataPrimitives.BasicHemisphere(2.5f, new KoreColorRGB(50, 100, 50, 150), 16);

            hemisphereMesh.OffsetAllVertices(new KoreXYZVector(5, 0, 5));

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(hemisphereMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(hemisphereMesh);

            HemisphereNode.AddChild(childMeshNode1);
            HemisphereNode.AddChild(childSurfaceMeshNode1);
        }
    }



}
