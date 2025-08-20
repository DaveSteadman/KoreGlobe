using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

public partial class KoreSandbox3DScene : Node3D
{
    // Sandbox3DScene.UIMount
    public static Control? UIMount = null;

    // UI Timers
    private float UITimer = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms

    private float UISlowTimer = 0.0f;
    private float UISlowTimerInterval = 2.5f;

    public bool ToClose { get; set; } = false;

    private Button? CloseButton = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("KoreSandbox3DScene _Ready");
        CreateTestMeshData_Box();
        CreateTestMeshData_Surface();
        //CreateTestMeshData_Lathe();
        CreateTestMeshData_Cylinder();
        AddTestMeshData_BoxArc();
    }

    public override void _Process(double delta)
    {
        if (UITimer > KoreCentralTime.RuntimeSecs)
        {
            UITimer = KoreCentralTime.RuntimeSecs + UITimerInterval;
            // UpdateUI();
        }

        if (UISlowTimer > KoreCentralTime.RuntimeSecs)
        {
            UISlowTimer = KoreCentralTime.RuntimeSecs + UISlowTimerInterval;
            // UpdateUISlow();
        }
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
        Node3D opNode    = new Node3D() { Name = "OPNode" };
        AddChild(Cube1Node);
        AddChild(Cube2Node);
        AddChild(Cube3Node);
        AddChild(opNode);

        Cube1Node.Position = new Vector3(-1.5f, 0.3f, 0.2f);
        Cube2Node.Position = new Vector3(0, 0.5f, 0.2f);
        Cube3Node.Position = new Vector3(1.5f, 0.7f, 0.2f);
        opNode.Position    = new Vector3(0, 2f, 0);

        // 1
        {
            var cubeMesh1 = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.Find("MattRed"));

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(cubeMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(cubeMesh1, "All");

            KoreGodotNormalMesh childNormalMeshNode1 = new KoreGodotNormalMesh();
            childNormalMeshNode1.UpdateMesh(cubeMesh1, 0.15f); // Small normals for cube

            Cube1Node.AddChild(childMeshNode1);
            Cube1Node.AddChild(childSurfaceMeshNode1);
            Cube1Node.AddChild(childNormalMeshNode1);
        }

        // 2
        {
            var cubeMesh2 = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.Find("MattGreen"));

            KoreGodotLineMesh childMeshNode2 = new KoreGodotLineMesh();
            childMeshNode2.UpdateMesh(cubeMesh2);

            KoreGodotSurfaceMesh childSurfaceMeshNode2 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode2.UpdateMesh(cubeMesh2, "All");

            Cube2Node.AddChild(childMeshNode2);
            Cube2Node.AddChild(childSurfaceMeshNode2);
        }

        // 3
        {
            var cubeMesh3 = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.Find("MattBlue"));

            KoreGodotLineMesh childMeshNode3 = new KoreGodotLineMesh();
            childMeshNode3.UpdateMesh(cubeMesh3);

            KoreGodotSurfaceMesh childSurfaceMeshNode3 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode3.UpdateMesh(cubeMesh3, "All");

            Cube3Node.AddChild(childMeshNode3);
            Cube3Node.AddChild(childSurfaceMeshNode3);
        }

        // test orientation pyramid
        {
            var apexpos = new KoreXYZVector(0, 0, 0);
            var basepos = new KoreXYZVector(0, 0, 1);
            var basefwd = new KoreXYZVector(0, 1, 0);

            var opMesh = KoreMeshDataPrimitives.BasicPyramidSharpEdges(
                apexpos,
                basepos,
                basefwd,
                0.3f, 0.1f,
                KoreColorPalette.Find("Orange"),
                KoreMeshMaterialPalette.Find("MattOrange")
            );

            opMesh.SetNormalsFromTriangles();

            KoreGodotLineMesh childMeshOP = new KoreGodotLineMesh();
            childMeshOP.UpdateMesh(opMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNodeOP = new KoreGodotSurfaceMesh();
            childSurfaceMeshNodeOP.UpdateMesh(opMesh, "All");

            KoreGodotNormalMesh childNormalMeshNode1 = new KoreGodotNormalMesh();
            childNormalMeshNode1.UpdateMesh(opMesh, 0.15f); // Small normals for pyramid

            opNode.AddChild(childMeshOP);
            opNode.AddChild(childSurfaceMeshNodeOP);
            opNode.AddChild(childNormalMeshNode1);
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: SURFACE
    // ---------------------------------------------------------------------------------------------

    // KoreSandbox3DScene.CreateTestMeshData_Surface
    private void CreateTestMeshData_Surface()
    {
        Node3D Surface1Node = new Node3D() { Name = "Surface1Node" };
        AddChild(Surface1Node);
        Surface1Node.Position = new Vector3(0, -1.5f, 0);

        // 1
        {
            // Create a 2D point array
            int eleSize = 20;
            KoreNumeric2DArray<double> eleArray = new KoreNumeric2DArray<double>(eleSize, eleSize);
            KoreXYZVector[,] vertices = new KoreXYZVector[eleSize, eleSize];

            // Create a 3D sinewave pattern in the eleArray
            for (int i = 0; i < eleSize; i++)
            {
                for (int j = 0; j < eleSize; j++)
                {
                    eleArray[i, j] = Math.Sin((double)i / (eleSize - 1) * Math.PI * 2) * Math.Cos((double)j / (eleSize - 1) * Math.PI * 2);
                }
            }

            // Uptick [0,0] (top left)so its clearly shown, and downshift [size,0] (top right)
            eleArray[0, 0] += 1.0;
            eleArray[eleSize - 1, 0] -= 1.0;

            // Turn the eleArray into vertices with [0,0] as top-left
            double horizRange = 5.0; // Controls the horizontal extent of the surface
            for (int i = 0; i < eleSize; i++)
            {
                for (int j = 0; j < eleSize; j++)
                {
                    double currX = i / (eleSize - 1.0) * 2.0 * horizRange - horizRange; // X: left to right [-horizRange, +horizRange]
                    double currZ = horizRange - j / (eleSize - 1.0) * 2.0 * horizRange; // Z: top to bottom [+horizRange, -horizRange]
                    double currY = eleArray[i, j];
                    vertices[i, j] = new KoreXYZVector(currX, currY, currZ);
                }
            }

            // Create surface
            KoreMeshData surfaceMesh1 = KoreMeshDataPrimitives.Surface(vertices, KoreUVBox.Full);

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(surfaceMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(surfaceMesh1);

            KoreGodotNormalMesh childNormalMeshNode1 = new KoreGodotNormalMesh();
            childNormalMeshNode1.UpdateMesh(surfaceMesh1, 0.15f); // Small normals for pyramid

            // Set the surface material
            var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.2f, 0.8f, 0.6f));
            childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

            Surface1Node.AddChild(childMeshNode1);
            Surface1Node.AddChild(childSurfaceMeshNode1);
            Surface1Node.AddChild(childNormalMeshNode1);

            // Experiment in line visibility, fudging it a fraction higher.
            childMeshNode1.Position = new Vector3(0, 0.001f, 0);

            GD.Print("kk");
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: LATHE
    // ---------------------------------------------------------------------------------------------

    private void CreateTestMeshData_Lathe()
    {

        Node3D latheNode = new Node3D() { Name = "latheNode" };
        AddChild(latheNode);
        latheNode.Position = new Vector3(-2, 1.5f, 0);

        KoreXYZVector p1 = new KoreXYZVector(0, 0, 0);
        KoreXYZVector p2 = new KoreXYZVector(-0.2, 1, 1);

        List<LathePoint> lathePoints = new List<LathePoint>
        {
            new LathePoint(0, 0.5),
            new LathePoint(0.1, 0.5),
            new LathePoint(0.3, 0.2),
            new LathePoint(0.7, 0.2),
            new LathePoint(0.9, 0.5),
            new LathePoint(1, 0.5)
        };

        // Lathe the cplex shape
        KoreMeshData koreMeshData1 = KoreMeshDataPrimitives.Lathe(p1, p2, lathePoints, 12, true);


        KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
        childMeshNode1.UpdateMesh(koreMeshData1);

        KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
        childSurfaceMeshNode1.UpdateMesh(koreMeshData1);

        // define a material for the surface mesh
        var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.2f, 0.8f, 0.5f, 0.95f));
        childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

        latheNode.AddChild(childMeshNode1);
        latheNode.AddChild(childSurfaceMeshNode1);

        GD.Print("lathe done");
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: CYLINDER
    // ---------------------------------------------------------------------------------------------

    private void CreateTestMeshData_Cylinder()
    {
        Node3D cylinderNode = new Node3D() { Name = "cylinderNode" };
        AddChild(cylinderNode);
        cylinderNode.Position = new Vector3(3, 1.5f, 0); // Position to the right of lathe

        KoreXYZVector p1 = new KoreXYZVector(0, 0, 0);
        KoreXYZVector p2 = new KoreXYZVector(0, 2, 0);

        // Create a simple cylinder to test end cap winding
        KoreMeshData koreMeshData = KoreMeshDataPrimitives.Cylinder(p1, p2, 0.5, 0.5, 12, true);

        KoreGodotLineMesh childMeshNode = new KoreGodotLineMesh();
        childMeshNode.UpdateMesh(koreMeshData);

        KoreGodotSurfaceMesh childSurfaceMeshNode = new KoreGodotSurfaceMesh();
        childSurfaceMeshNode.UpdateMesh(koreMeshData);

        KoreGodotNormalMesh childNormalMeshNode = new KoreGodotNormalMesh();
        childNormalMeshNode.UpdateMesh(koreMeshData, 0.15f); // Small normals for cylinder

        // Use a different color to distinguish from lathe
        var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.8f, 0.3f, 0.1f));
        childSurfaceMeshNode.MaterialOverride = surfaceMaterial;

        cylinderNode.AddChild(childMeshNode);
        cylinderNode.AddChild(childSurfaceMeshNode);
        cylinderNode.AddChild(childNormalMeshNode);

        GD.Print("cylinder done");
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

            // Setup the material
            var material = KoreMeshMaterialPalette.Find("MattYellow");
            boxArcMesh.AddGroupWithMaterial("All", material);
            boxArcMesh.AddAllTrianglesToGroup("All");

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(boxArcMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(boxArcMesh, "All");

            // Use a different color to distinguish from lathe
            // var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.8f));
            // childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;


            BoxArcNode.AddChild(childMeshNode1);
            BoxArcNode.AddChild(childSurfaceMeshNode1);
        }
    }

}
