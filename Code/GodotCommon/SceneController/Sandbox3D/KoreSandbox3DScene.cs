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
        CreateTestMeshData_Lathe();
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
        AddChild(Cube1Node);
        AddChild(Cube2Node);
        AddChild(Cube3Node);

        Cube1Node.Position = new Vector3(-1.1f, 0, 0);
        Cube2Node.Position = new Vector3(0, 0, 0);
        Cube3Node.Position = new Vector3(1.1f, 0, 0);

        // 1
        {
            var cubeMesh1 = KoreMeshDataPrimitives.BasicCube(0.5f, KoreMeshMaterialPalette.Find("MattRed"));

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(cubeMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(cubeMesh1);

            Cube1Node.AddChild(childMeshNode1);
            Cube1Node.AddChild(childSurfaceMeshNode1);

            GD.Print("kk");
        }

        // 2
        {
            var cubeMesh2 = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.Find("MattGreen"));

            KoreGodotLineMesh childMeshNode2 = new KoreGodotLineMesh();
            childMeshNode2.UpdateMesh(cubeMesh2);

            KoreGodotSurfaceMesh childSurfaceMeshNode2 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode2.UpdateMesh(cubeMesh2);

            Cube2Node.AddChild(childMeshNode2);
            Cube2Node.AddChild(childSurfaceMeshNode2);
        }

        // 3
        {
            var cubeMesh3 = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.Find("MattBlue"));

            KoreGodotLineMesh childMeshNode3 = new KoreGodotLineMesh();
            childMeshNode3.UpdateMesh(cubeMesh3);

            KoreGodotSurfaceMesh childSurfaceMeshNode3 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode3.UpdateMesh(cubeMesh3);

            Cube3Node.AddChild(childMeshNode3);
            Cube3Node.AddChild(childSurfaceMeshNode3);
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

            // Turn the eleArray into vertices
            double horizRange = 5.0; // Controls the horizontal extent of the surface
            for (int i = 0; i < eleSize; i++)
            {
                for (int j = 0; j < eleSize; j++)
                {
                    double currX = i / (eleSize - 1.0) * 2.0 * horizRange - horizRange; // Normalize to [-horizRange, horizRange]
                    double currZ = j / (eleSize - 1.0) * 2.0 * horizRange - horizRange; // Normalize to [-horizRange, horizRange]
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

            // Set the surface material
            var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.2f, 0.8f, 0.6f));
            childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

            Surface1Node.AddChild(childMeshNode1);
            Surface1Node.AddChild(childSurfaceMeshNode1);

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
        latheNode.Position = new Vector3(0, 1.5f, 0);

        KoreXYZVector p1 = new KoreXYZVector(0, 0, 0);
        KoreXYZVector p2 = new KoreXYZVector(0, 1, 1);

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

        // Use a different color to distinguish from lathe
        var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.8f, 0.3f, 0.1f));
        childSurfaceMeshNode.MaterialOverride = surfaceMaterial;

        cylinderNode.AddChild(childMeshNode);
        cylinderNode.AddChild(childSurfaceMeshNode);

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

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(boxArcMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(boxArcMesh);

            // Use a different color to distinguish from lathe
            var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.8f));
            childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;


            BoxArcNode.AddChild(childMeshNode1);
            BoxArcNode.AddChild(childSurfaceMeshNode1);
        }
    }

}
