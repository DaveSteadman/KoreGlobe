// <fileheader>

using System;
using System.IO;
using System.Collections.Generic;

using Godot;
using SkiaSharp;

using KoreCommon;
using KoreCommon.UnitTest;
using KoreCommon.SkiaSharp;
using KoreSim;

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
        AddTestMeshData_TexBox();

        AddTestMeshData_MiniMeshBox();
        AddTestMeshData_MiniMeshSphere();
        AddTestMeshData_MiniMeshCylinder();
        AddTestMeshData_MiniMeshPyramid();
        //AddTestMeshData_ColorMeshSphere();
        AddTestMeshData_QuadMesh();
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
        Node3D opNode = new Node3D() { Name = "OPNode" };
        AddChild(Cube1Node);
        AddChild(Cube2Node);
        AddChild(Cube3Node);
        AddChild(opNode);

        Cube1Node.Position = new Vector3(-1.5f, 0.3f, 0.2f);
        Cube2Node.Position = new Vector3(0, 0.5f, 0.2f);
        Cube3Node.Position = new Vector3(1.5f, 0.7f, 0.2f);
        opNode.Position = new Vector3(0, 2f, 0);

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

            KoreMeshDataEditOps.SetNormalsFromTriangles(opMesh);

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

                    vertices[i, j] = vertices[i, j].FlipZ();
                }
            }






            // Create surface
            KoreMeshData surfaceMesh1 = KoreMeshDataPrimitives.Surface(vertices, KoreUVBox.Full);


            // Add material with texture
            var MiscTriMaterial = new KoreMeshMaterial("MiscTriMaterial", new KoreColorRGB(200, 10, 10));
            MiscTriMaterial.Filename = "MiscTriTex.png"; // Use existing test image
            surfaceMesh1.AddMaterial(MiscTriMaterial);
            surfaceMesh1.AddAllTrianglesToGroup("MiscTriGroup");
            surfaceMesh1.SetGroupMaterialName("MiscTriGroup", "MiscTriMaterial");


            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(surfaceMesh1);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(surfaceMesh1, "MiscTriGroup", "UnitTestArtefacts");

            KoreGodotNormalMesh childNormalMeshNode1 = new KoreGodotNormalMesh();
            childNormalMeshNode1.UpdateMesh(surfaceMesh1, 0.15f); // Small normals for pyramid

            // Set the surface material
            // var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.2f, 0.8f, 0.6f));
            // childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

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

        KoreGodotNormalMesh childNormalMeshNode = new KoreGodotNormalMesh();
        childNormalMeshNode.UpdateMesh(koreMeshData1, 0.15f); // Small normals for cylinder

        // define a material for the surface mesh
        var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.2f, 0.8f, 0.5f, 0.95f));
        childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

        latheNode.AddChild(childMeshNode1);
        latheNode.AddChild(childSurfaceMeshNode1);
        latheNode.AddChild(childNormalMeshNode);

        GD.Print("lathe done");
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: CYLINDER
    // ---------------------------------------------------------------------------------------------

    private void CreateTestMeshData_Cylinder()
    {
        Node3D cylinderNode = new Node3D() { Name = "cylinderNode" };
        Node3D oilBarrelNode = new Node3D() { Name = "oilBarrelNode" };

        AddChild(cylinderNode);
        cylinderNode.Position = new Vector3(3, 1.5f, 0); // Position to the right of lathe

        AddChild(oilBarrelNode);
        oilBarrelNode.Position = new Vector3(-3, 1.5f, 0); // Position to the right of cylinder

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


        {
            KoreMeshData oilBarrelMesh = KoreTestMeshUvOps.CreateOilBarrelWithUV(16, 0.4, 1.2);

            // Dump the mesh to JSON for debugging
            string json = KoreMeshDataIO.ToJson(oilBarrelMesh, dense: false);
            // GD.Print("Oil Barrel Mesh JSON:", json);

            KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            childMeshNode1.UpdateMesh(oilBarrelMesh);

            KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
            childSurfaceMeshNode1.UpdateMesh(oilBarrelMesh, "OilBarrel", "UnitTestArtefacts");

            KoreGodotNormalMesh childNormalMeshNode1 = new KoreGodotNormalMesh();
            childNormalMeshNode1.UpdateMesh(oilBarrelMesh, 0.15f); // Small normals for cube

            oilBarrelNode.AddChild(childMeshNode1);
            oilBarrelNode.AddChild(childSurfaceMeshNode1);
            oilBarrelNode.AddChild(childNormalMeshNode1);
        }


        GD.Print("cylinder done");

    }

    // ---------------------------------------------------------------------------------------------
    // MARK: TEX BOX
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_TexBox()
    {
        // Add the holding node for the meshes
        Node3D texBoxNode = new Node3D() { Name = "texBoxNode" };
        AddChild(texBoxNode);
        texBoxNode.Position = new Vector3(1.5f, 1.5f, 2f);

        // Add a little debug sphere on the origin
        texBoxNode.AddChild(KoreNodeUtils.DebugSphere(0.01f));

        // Setup the basic mesh
        KoreMeshData koreMeshData = KoreTestMeshUvOps.CreateBoxWithUV(0.5, 0.5, 0.5);

        // Add the texture material
        var texBoxMaterial = new KoreMeshMaterial("texBoxMaterial", new KoreColorRGB(200, 10, 10));
        texBoxMaterial.Filename = "texbox2.png"; // Use existing test image
        koreMeshData.AddMaterial(texBoxMaterial);
        koreMeshData.SetGroupMaterialName("All", "texBoxMaterial");

        // Setup the mesh render options
        KoreGodotLineMesh childMeshNode = new KoreGodotLineMesh();
        childMeshNode.UpdateMesh(koreMeshData);

        KoreGodotSurfaceMesh childSurfaceMeshNode = new KoreGodotSurfaceMesh();
        childSurfaceMeshNode.UpdateMesh(koreMeshData, "All", "UnitTestArtefacts");

        KoreGodotNormalMesh childNormalMeshNode = new KoreGodotNormalMesh();
        childNormalMeshNode.UpdateMesh(koreMeshData, 0.1f); // Small normals for cylinder

        texBoxNode.AddChild(childMeshNode);
        texBoxNode.AddChild(childSurfaceMeshNode);
        texBoxNode.AddChild(childNormalMeshNode);

        // Save clean version without vertex numbers
        string cleanPath = "UnitTestArtefacts/tex_cube_clean.png";
        KoreMeshDataUvOps.SaveUVLayout(koreMeshData, cleanPath, 1024, true, true);

        // Export the texbox to an Obj/MTL file
        {
            string objPath = "UnitTestArtefacts/texcube.obj";
            string mtlPath = "UnitTestArtefacts/texcube.mtl";

            // Export to OBJ/MTL
            var (objContent, mtlContent) = KoreMeshDataIO.ToObjMtl(koreMeshData, "texcube", "texcube");
            File.WriteAllLines(objPath, new[] { objContent });
            File.WriteAllLines(mtlPath, new[] { mtlContent });
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

    // ---------------------------------------------------------------------------------------------
    // MARK: Mini Mesh Box
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_MiniMeshBox()
    {
        // Function to add a test mini mesh box
        Node3D MiniMeshBoxNode = new Node3D() { Name = "MiniMeshBoxNode" };
        AddChild(MiniMeshBoxNode);
        MiniMeshBoxNode.Position = new Vector3(3, 3, -3.6f);

        // Test Mini Mesh Box
        {
            // Import the mesh
            string objContent2 = File.ReadAllText("UnitTestArtefacts/MiniMesh_Cube.obj");
            string mtlContent2 = File.ReadAllText("UnitTestArtefacts/MiniMesh_Cube.mtl");
            KoreMiniMesh miniMesh = KoreMiniMeshIO.FromObjMtl(objContent2, mtlContent2);

            // Import from JSON
            // string jsonContent = File.ReadAllText("UnitTestArtefacts/MiniMesh_Cube.json");
            // KoreMiniMesh miniMesh = KoreMiniMeshIO.FromJson(jsonContent);

            // var miniMesh = KoreMiniMeshPrimitives.BasicCube(0.5f, KoreMiniMeshMaterialPalette.Find("MattCyan"), KoreColorRGB.White);

            // KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            // childMeshNode1.UpdateMesh(miniMesh);

            // loop through each group in the mesh, and add a surface renderer for each
            foreach (var group in miniMesh.Groups)
            {
                KoreMiniMeshGodotSurface childSurfaceMeshNode = new KoreMiniMeshGodotSurface();
                MiniMeshBoxNode.AddChild(childSurfaceMeshNode);
                childSurfaceMeshNode.UpdateMesh(miniMesh, group.Key);
            }

            // KoreMiniMeshGodotSurface childSurfaceMeshNode1 = new KoreMiniMeshGodotSurface();
            // childSurfaceMeshNode1.UpdateMesh(miniMesh, "MattCyan");

            // KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            // lineMeshNode1.UpdateMesh(miniMesh, "All");


            // KoreMiniMeshGodotNormal normalMeshNode1 = new KoreMiniMeshGodotNormal();
            // normalMeshNode1.UpdateMesh(miniMesh, "All", 0.1f);

            // Use a different color to distinguish from lathe
            // var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.8f));
            // childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

            // Add the child nodes to the MiniMeshBoxNode
            // MiniMeshBoxNode.AddChild(childSurfaceMeshNode1);
            // MiniMeshBoxNode.AddChild(lineMeshNode1);
            // MiniMeshBoxNode.AddChild(normalMeshNode1);

            // Export the mesh to Obj/MTL
            var (objContent, mtlContent) = KoreMiniMeshIO.ToObjMtl(miniMesh, "MiniMesh_Cube", "MiniMesh_Cube");
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Cube.obj", objContent);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Cube.mtl", mtlContent);

            // dump to JSON
            string json = KoreMiniMeshIO.ToJson(miniMesh);
            //GD.Print($"========> Cube JSON: \n{json}\n");
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Cube.json", json);

        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Mini Mesh Sphere
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_MiniMeshSphere()
    {
        // Function to add a test mini mesh sphere
        Node3D MiniMeshSphereNode = new Node3D() { Name = "MiniMeshSphereNode" };
        AddChild(MiniMeshSphereNode);
        MiniMeshSphereNode.Position = new Vector3(-3, 3, -3.6f);

        // Test Mini Mesh Sphere
        {
            // Import the mesh
            // string objContent2 = File.ReadAllText("UnitTestArtefacts/MiniMesh_Sphere_edit.obj");
            // string mtlContent2 = File.ReadAllText("UnitTestArtefacts/MiniMesh_Sphere_edit.mtl");
            // KoreMiniMesh miniMesh = KoreMiniMeshIO.FromObjMtl(objContent2, mtlContent2);

            // Import from JSON
            // string jsonContent = File.ReadAllText("UnitTestArtefacts/MiniMesh_Sphere_v.json");
            // KoreMiniMesh miniMesh = KoreMiniMeshIO.FromJson(jsonContent);

            KoreXYZVector center = new KoreXYZVector(0, 0, 0);

            var miniMesh = KoreMiniMeshPrimitives.BasicSphere(center, 0.5f, 64, KoreMiniMeshMaterialPalette.Find("StainedGlassBlue"), KoreColorRGB.White);
            KoreMiniMeshOps.VariateGroup(miniMesh, "All", 0.5f, 32);

            // KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
            // childMeshNode1.UpdateMesh(miniMesh);

            // loop through each group in the mesh, and add a surface renderer for each
            // foreach (var group in miniMesh.Groups)
            // {
            //     KoreMiniMeshGodotSurface childSurfaceMeshNode = new KoreMiniMeshGodotSurface();
            //     MiniMeshSphereNode.AddChild(childSurfaceMeshNode);
            //     childSurfaceMeshNode.UpdateMesh(miniMesh, group.Key);
            // }

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface();
            MiniMeshSphereNode.AddChild(coloredMeshNode);
            coloredMeshNode.UpdateMesh(miniMesh, "All");


            // KoreMiniMeshGodotSurface childSurfaceMeshNode1 = new KoreMiniMeshGodotSurface();
            // childSurfaceMeshNode1.UpdateMesh(miniMesh, "MattCyan");

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(miniMesh, "All");
            MiniMeshSphereNode.AddChild(lineMeshNode1);

            // KoreMiniMeshGodotNormal normalMeshNode1 = new KoreMiniMeshGodotNormal();
            // normalMeshNode1.UpdateMesh(miniMesh, "All", 0.1f);

            // Use a different color to distinguish from lathe
            // var surfaceMaterial = KoreGodotMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.8f));
            // childSurfaceMeshNode1.MaterialOverride = surfaceMaterial;

            // Add the child nodes to the MiniMeshSphereNode
            // MiniMeshSphereNode.AddChild(childSurfaceMeshNode1);
            // MiniMeshSphereNode.AddChild(normalMeshNode1);

            // Export the mesh to Obj/MTL
            var (objContent, mtlContent) = KoreMiniMeshIO.ToObjMtl(miniMesh, "MiniMesh_Sphere", "MiniMesh_Sphere");
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Sphere.obj", objContent);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Sphere.mtl", mtlContent);

            // dump to JSON
            string json = KoreMiniMeshIO.ToJson(miniMesh);
            //GD.Print($"========> Sphere JSON: \n{json}\n");
            // File.WriteAllText("UnitTestArtefacts/MiniMesh_Sphere.json", json);

            // Make a variated version and dump to JSON
            // KoreMiniMeshOps.VariateGroup(miniMesh, "All", 0.2f, 5);
            string vjson = KoreMiniMeshIO.ToJson(miniMesh);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Sphere_v.json", vjson);

        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Mini Mesh Cylinder
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_MiniMeshCylinder()
    {
        // Function to add a test mini mesh cylinder
        Node3D MiniMeshCylinderNode = new Node3D() { Name = "MiniMeshCylinderNode" };
        AddChild(MiniMeshCylinderNode);
        MiniMeshCylinderNode.Position = new Vector3(0, 3, -3.6f); // Between the box and sphere

        // Test Mini Mesh Cylinder
        {
            KoreXYZVector p1 = new KoreXYZVector(0, -0.5f, 0); // Bottom center
            KoreXYZVector p2 = new KoreXYZVector(2, 0.5f, 1);  // Top center
            double p1Radius = 0.1;
            double p2Radius = 0.4;
            int sides = 12;

            var miniMesh = KoreMiniMeshPrimitives.CreateCylinder(
                p1, p2, p1Radius, p2Radius,
                sides, true,
                KoreMiniMeshMaterialPalette.Find("StainedGlassBlue"), KoreColorRGB.White);

            // loop through each group in the mesh, and add a surface renderer for each
            foreach (var group in miniMesh.Groups)
            {
                KoreMiniMeshGodotSurface childSurfaceMeshNode = new KoreMiniMeshGodotSurface();
                MiniMeshCylinderNode.AddChild(childSurfaceMeshNode);
                childSurfaceMeshNode.UpdateMesh(miniMesh, group.Key);
            }

            // Add wireframe lines
            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(miniMesh, "All");
            MiniMeshCylinderNode.AddChild(lineMeshNode1);

            // Export the mesh to Obj/MTL
            var (objContent, mtlContent) = KoreMiniMeshIO.ToObjMtl(miniMesh, "MiniMesh_Cylinder", "MiniMesh_Cylinder");
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Cylinder.obj", objContent);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Cylinder.mtl", mtlContent);

            // dump to JSON
            string json = KoreMiniMeshIO.ToJson(miniMesh);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Cylinder.json", json);

            // Print vertex count for debugging
            GD.Print($"Cylinder vertices: {miniMesh.Vertices.Count}, triangles: {miniMesh.Triangles.Count}");
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Color Mesh Sphere
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_ColorMeshSphere()
    {
        // Function to add a test mini mesh sphere
        Node3D ColorMeshSphereNode = new Node3D() { Name = "ColorMeshSphereNode" };
        AddChild(ColorMeshSphereNode);
        ColorMeshSphereNode.Position = new Vector3(-3, 3, 0); // Position to the left of the cylinder

        // Test Color Mesh Sphere
        {
            KoreXYZVector center = new KoreXYZVector(0, 0, 0);

            // Load an image, create a colormap from it
            string imagePath = "UnitTestArtefacts/TestImage_Input.png";
            SKBitmap image = KoreSkiaSharpBitmapOps.LoadBitmap(imagePath);
            KoreColorRGB[,] colormap = KoreSkiaSharpBitmapOps.SampleBitmapColors(image, 360, 180);

            // load the sphere from binary file
            // byte[] readbytes = File.ReadAllBytes("UnitTestArtefacts/ColorMesh_Sphere.bin");
            // var colorMesh = KoreColorMeshIO.FromBytes(readbytes);

            // Create a color mesh sphere
            var colorMesh = KoreColorMeshPrimitives.BasicSphere(center, 0.8f, colormap);

            // Dump to JSON for review
            // string json = KoreColorMeshIO.ToJson(colorMesh);
            // File.WriteAllText("UnitTestArtefacts/ColorMesh_Sphere.json", json);

            /// Dump to a byte/binary file
            // byte[] bytes = KoreColorMeshIO.ToBytes(colorMesh);
            // File.WriteAllBytes("UnitTestArtefacts/ColorMesh_Sphere.bin", bytes);

            // Create the Godot Rendered
            KoreColorMeshGodot coloredMeshNode = new();
            ColorMeshSphereNode.AddChild(coloredMeshNode);
            coloredMeshNode.UpdateMesh(colorMesh);

            // // dump to JSON
            // string json = KoreColorMeshIO.ToJson(colorMesh);
            // File.WriteAllText("UnitTestArtefacts/ColorMesh_Sphere.json", json);

            // // Make a variated version and dump to JSON
            // // KoreColorMeshOps.VariateGroup(colorMesh, "All", 0.2f, 5);
            // string vjson = KoreColorMeshIO.ToJson(colorMesh);
            // File.WriteAllText("UnitTestArtefacts/ColorMesh_Sphere_v.json", vjson);

        }

        {

            KoreMapTileCode tileCode = new KoreMapTileCode("BH");

            KoreElevationTile? eleTile = KoreElevationTileIO.ReadFromTextFile("C:/Util/Data/GitRepos/KoreCommonTest/UnitTestArtefacts/Ele_BF_BF.arr");


            if (eleTile == null)
            {
                // Handle the case where the elevation tile is not found
                GD.PrintErr("Elevation tile not found");
                return;
            }

            // KoreNumeric2DArray<float> tileEleData = eleTile!.ElevationData;
            // tileEleData.Scale(0.01f, 0.1f); // Scale to a reasonable range for testing

            // int countAz = tileEleData.Width;
            // int countEl = tileEleData.Height;


            int countAz = 50;
            int countEl = 50;
            KoreNumeric2DArray<float> tileEleData = new KoreNumeric2DArray<float>(countAz, countEl);
            tileEleData.SetAllNoise(0.01f, 0.02f);
            // KoreColorRGB[,] colormap = new KoreColorRGB[countAz, countEl];
            // for (int i = 0; i < countAz; i++)
            //     for (int j = 0; j < countEl; j++)
            //         colormap[i, j] = KoreColorOps.ColorWithRGBNoise(KoreColorPalette.Colors["MutedCyan"], 0.5f);

            // Load an image, create a colormap from it
            string imagePath = "UnitTestArtefacts/Sat_BF.webp";
            SKBitmap image = KoreSkiaSharpBitmapOps.LoadBitmap(imagePath);
            KoreColorRGB[,] colormap = KoreSkiaSharpBitmapOps.SampleBitmapColors(image, countAz, countEl);

            //KoreAzElBox azEl = new KoreAzElBox() { MinAzDegs = 30, MaxAzDegs = 60, MinElDegs = 30, MaxElDegs = 60 };

            KoreLLBox llBox = tileCode.LLBox;


            KoreColorMesh sphereSectionMesh = KoreColorMeshPrimitives.SphereSection(KoreXYZVector.Zero, llBox, 0.83, colormap, tileEleData);


            //KoreColorMesh colorMesh = KoreColorMeshPrimitives.Tile(tileCode, tileEleData, colormap);
            //KoreColorMesh colorMesh = KoreColorMeshPrimitives.BasicSphere(KoreXYZVector.Zero, 1f, colormap);
            KoreColorMeshGodot colorMeshNode = new KoreColorMeshGodot();
            colorMeshNode.Name = "TileExperiment";
            ColorMeshSphereNode.AddChild(colorMeshNode);
            colorMeshNode.UpdateMesh(sphereSectionMesh);
        }
    }


    // ---------------------------------------------------------------------------------------------
    // MARK: Mini Mesh Pyramid
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_MiniMeshPyramid()
    {
        // Function to add a test mini mesh pyramid
        Node3D MiniMeshPyramidNode = new Node3D() { Name = "MiniMeshPyramidNode" };
        AddChild(MiniMeshPyramidNode);
        MiniMeshPyramidNode.Position = new Vector3(3, 3, 0); // Position to the right of the cylinder

        // Test Mini Mesh Pyramid
        {
            KoreXYZVector pApex = new KoreXYZVector(0, 0, 0);      // Apex point
            KoreXYZVector pBaseCenter = new KoreXYZVector(1f, -0.5f, 1f); // Base center

            // Test with explicit control over base orientation
            // This reference direction will be projected onto the base plane (perpendicular to apex-base axis)
            KoreXYZVector baseReference = new KoreXYZVector(1, 0, 0.2); // Reference direction for base orientation

            double width = 0.6;
            double height = 0.8;

            var miniMesh = KoreMiniMeshPrimitives.CreatePyramid(
                pApex, pBaseCenter, baseReference, width, height, true,
                KoreMiniMeshMaterialPalette.Find("StainedGlassRed"), KoreColorRGB.White);

            // loop through each group in the mesh, and add a surface renderer for each
            foreach (var group in miniMesh.Groups)
            {
                KoreMiniMeshGodotSurface childSurfaceMeshNode = new KoreMiniMeshGodotSurface();
                MiniMeshPyramidNode.AddChild(childSurfaceMeshNode);
                childSurfaceMeshNode.UpdateMesh(miniMesh, group.Key);
            }

            // Add wireframe lines
            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(miniMesh, "All");
            MiniMeshPyramidNode.AddChild(lineMeshNode1);

            // Export the mesh to Obj/MTL
            var (objContent, mtlContent) = KoreMiniMeshIO.ToObjMtl(miniMesh, "MiniMesh_Pyramid", "MiniMesh_Pyramid");
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Pyramid.obj", objContent);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Pyramid.mtl", mtlContent);

            // dump to JSON
            string json = KoreMiniMeshIO.ToJson(miniMesh);
            File.WriteAllText("UnitTestArtefacts/MiniMesh_Pyramid.json", json);

            // Print vertex count for debugging
            GD.Print($"Pyramid vertices: {miniMesh.Vertices.Count}, triangles: {miniMesh.Triangles.Count}");
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Quad Sphere
    // ---------------------------------------------------------------------------------------------

    public void AddTestMeshData_QuadMesh()
    {
        // Function to add a test Quad mesh
        Node3D QuadMeshNode = new Node3D() { Name = "QuadMeshNode" };
        AddChild(QuadMeshNode);
        QuadMeshNode.Position = new Vector3(5.5f, 3, 0); // Position to the right of the cylinder

        //KoreQuadCubeTileCode tileCode = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Front, Quadrants = new List<int> { 0, 1, 2, 3 } };

        KoreQuadCubeTileCode tileCodeFront = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Front };
        KoreQuadCubeTileCode tileCodeBack = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Back };
        KoreQuadCubeTileCode tileCodeLeft = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Left };
        KoreQuadCubeTileCode tileCodeRight = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right };
        KoreQuadCubeTileCode tileCodeUp = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Top };
        KoreQuadCubeTileCode tileCodeDown = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Bottom };

        KoreQuadCubeTile tileFront = KoreQuadCubeTileFactory.TileForCode(tileCodeFront, radius: 1.0);
        KoreQuadCubeTile tileBack = KoreQuadCubeTileFactory.TileForCode(tileCodeBack, radius: 1.01);
        KoreQuadCubeTile tileLeft = KoreQuadCubeTileFactory.TileForCode(tileCodeLeft, radius: 1.02);
        KoreQuadCubeTile tileRight = KoreQuadCubeTileFactory.TileForCode(tileCodeRight, radius: 1.03);
        KoreQuadCubeTile tileUp = KoreQuadCubeTileFactory.TileForCode(tileCodeUp, radius: 1.04);
        KoreQuadCubeTile tileDown = KoreQuadCubeTileFactory.TileForCode(tileCodeDown, radius: 1.05);

        KoreColorMeshOps.SetAllColors(tileFront.ColorMesh, KoreColorPalette.RandomColor());
        KoreColorMeshOps.SetAllColors(tileBack.ColorMesh, KoreColorPalette.RandomColor());
        KoreColorMeshOps.SetAllColors(tileLeft.ColorMesh, KoreColorPalette.RandomColor());
        KoreColorMeshOps.SetAllColors(tileRight.ColorMesh, KoreColorPalette.RandomColor());
        KoreColorMeshOps.SetAllColors(tileUp.ColorMesh, KoreColorPalette.RandomColor());
        KoreColorMeshOps.SetAllColors(tileDown.ColorMesh, KoreColorPalette.RandomColor());

        KoreColorMeshGodot coloredMeshNodeFront = new() { Name = "QuadMesh_Front" }; QuadMeshNode.AddChild(coloredMeshNodeFront);
        KoreColorMeshGodot coloredMeshNodeBack = new() { Name = "QuadMesh_Back" }; QuadMeshNode.AddChild(coloredMeshNodeBack);
        KoreColorMeshGodot coloredMeshNodeLeft = new() { Name = "QuadMesh_Left" }; QuadMeshNode.AddChild(coloredMeshNodeLeft);
        KoreColorMeshGodot coloredMeshNodeRight = new() { Name = "QuadMesh_Right" }; QuadMeshNode.AddChild(coloredMeshNodeRight);
        KoreColorMeshGodot coloredMeshNodeUp = new() { Name = "QuadMesh_Up" }; QuadMeshNode.AddChild(coloredMeshNodeUp);
        KoreColorMeshGodot coloredMeshNodeDown = new() { Name = "QuadMesh_Down" }; QuadMeshNode.AddChild(coloredMeshNodeDown);

        coloredMeshNodeFront.UpdateMesh(tileFront.ColorMesh);
        coloredMeshNodeBack.UpdateMesh(tileBack.ColorMesh);
        coloredMeshNodeLeft.UpdateMesh(tileLeft.ColorMesh);
        coloredMeshNodeRight.UpdateMesh(tileRight.ColorMesh);
        coloredMeshNodeUp.UpdateMesh(tileUp.ColorMesh);
        coloredMeshNodeDown.UpdateMesh(tileDown.ColorMesh);

        // - - - -

        KoreQuadCubeTileCode tileCodeFront0 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Front, Quadrants = new List<int> { 0 } };
        KoreQuadCubeTileCode tileCodeFront1 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Front, Quadrants = new List<int> { 1 } };
        KoreQuadCubeTileCode tileCodeFront2 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Front, Quadrants = new List<int> { 2 } };
        KoreQuadCubeTileCode tileCodeFront3 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Front, Quadrants = new List<int> { 3 } };

        KoreQuadCubeTile tileFront0 = KoreQuadCubeTileFactory.TileForCode2(tileCodeFront0, radius: 1.1);
        KoreQuadCubeTile tileFront1 = KoreQuadCubeTileFactory.TileForCode2(tileCodeFront1, radius: 1.11);
        KoreQuadCubeTile tileFront2 = KoreQuadCubeTileFactory.TileForCode2(tileCodeFront2, radius: 1.12);
        KoreQuadCubeTile tileFront3 = KoreQuadCubeTileFactory.TileForCode2(tileCodeFront3, radius: 1.13);

        // KoreColorMeshOps.SetAllColors(tileFront0.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileFront1.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileFront2.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileFront3.ColorMesh, KoreColorPalette.RandomColor());

        KoreColorMeshGodot coloredMeshNodeFront0 = new() { Name = "QuadMesh_Front_0" }; QuadMeshNode.AddChild(coloredMeshNodeFront0);
        KoreColorMeshGodot coloredMeshNodeFront1 = new() { Name = "QuadMesh_Front_1" }; QuadMeshNode.AddChild(coloredMeshNodeFront1);
        KoreColorMeshGodot coloredMeshNodeFront2 = new() { Name = "QuadMesh_Front_2" }; QuadMeshNode.AddChild(coloredMeshNodeFront2);
        KoreColorMeshGodot coloredMeshNodeFront3 = new() { Name = "QuadMesh_Front_3" }; QuadMeshNode.AddChild(coloredMeshNodeFront3);

        coloredMeshNodeFront0.UpdateMesh(tileFront0.ColorMesh);
        coloredMeshNodeFront1.UpdateMesh(tileFront1.ColorMesh);
        coloredMeshNodeFront2.UpdateMesh(tileFront2.ColorMesh);
        coloredMeshNodeFront3.UpdateMesh(tileFront3.ColorMesh);

        // - - - -

        KoreQuadCubeTileCode tileCodeTop0 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Top, Quadrants = new List<int> { 0 } };
        KoreQuadCubeTileCode tileCodeTop1 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Top, Quadrants = new List<int> { 1 } };
        KoreQuadCubeTileCode tileCodeTop2 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Top, Quadrants = new List<int> { 2 } };
        KoreQuadCubeTileCode tileCodeTop3 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Top, Quadrants = new List<int> { 3 } };

        KoreQuadCubeTile tileTop0 = KoreQuadCubeTileFactory.TileForCode2(tileCodeTop0, radius: 1.1);
        KoreQuadCubeTile tileTop1 = KoreQuadCubeTileFactory.TileForCode2(tileCodeTop1, radius: 1.11);
        KoreQuadCubeTile tileTop2 = KoreQuadCubeTileFactory.TileForCode2(tileCodeTop2, radius: 1.12);
        KoreQuadCubeTile tileTop3 = KoreQuadCubeTileFactory.TileForCode2(tileCodeTop3, radius: 1.13);

        // KoreColorMeshOps.SetAllColors(tileFront0.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileFront1.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileFront2.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileFront3.ColorMesh, KoreColorPalette.RandomColor());

        KoreColorMeshGodot coloredMeshNodeTop0 = new() { Name = "QuadMesh_Top_0" }; QuadMeshNode.AddChild(coloredMeshNodeTop0);
        KoreColorMeshGodot coloredMeshNodeTop1 = new() { Name = "QuadMesh_Top_1" }; QuadMeshNode.AddChild(coloredMeshNodeTop1);
        KoreColorMeshGodot coloredMeshNodeTop2 = new() { Name = "QuadMesh_Top_2" }; QuadMeshNode.AddChild(coloredMeshNodeTop2);
        KoreColorMeshGodot coloredMeshNodeTop3 = new() { Name = "QuadMesh_Top_3" }; QuadMeshNode.AddChild(coloredMeshNodeTop3);

        coloredMeshNodeTop0.UpdateMesh(tileTop0.ColorMesh);
        coloredMeshNodeTop1.UpdateMesh(tileTop1.ColorMesh);
        coloredMeshNodeTop2.UpdateMesh(tileTop2.ColorMesh);
        coloredMeshNodeTop3.UpdateMesh(tileTop3.ColorMesh);

        // - - - -

        KoreQuadCubeTileCode tileCodeRight00 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 0, 0 } };
        KoreQuadCubeTileCode tileCodeRight01 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 0, 1 } };
        KoreQuadCubeTileCode tileCodeRight02 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 0, 2 } };
        KoreQuadCubeTileCode tileCodeRight03 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 0, 3 } };

        KoreQuadCubeTile tileRight00 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight00, radius: 1.15);
        KoreQuadCubeTile tileRight01 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight01, radius: 1.16);
        KoreQuadCubeTile tileRight02 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight02, radius: 1.17);
        KoreQuadCubeTile tileRight03 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight03, radius: 1.18);

        KoreColorMeshGodot coloredMeshNodeRight00 = new() { Name = "QuadMesh_Right_00" }; QuadMeshNode.AddChild(coloredMeshNodeRight00);
        KoreColorMeshGodot coloredMeshNodeRight01 = new() { Name = "QuadMesh_Right_01" }; QuadMeshNode.AddChild(coloredMeshNodeRight01);
        KoreColorMeshGodot coloredMeshNodeRight02 = new() { Name = "QuadMesh_Right_02" }; QuadMeshNode.AddChild(coloredMeshNodeRight02);
        KoreColorMeshGodot coloredMeshNodeRight03 = new() { Name = "QuadMesh_Right_03" }; QuadMeshNode.AddChild(coloredMeshNodeRight03);

        coloredMeshNodeRight00.UpdateMesh(tileRight00.ColorMesh);
        coloredMeshNodeRight01.UpdateMesh(tileRight01.ColorMesh);
        coloredMeshNodeRight02.UpdateMesh(tileRight02.ColorMesh);
        coloredMeshNodeRight03.UpdateMesh(tileRight03.ColorMesh);

        KoreQuadCubeTileCode tileCodeRight0 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 0 } };
        KoreQuadCubeTileCode tileCodeRight1 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 1 } };
        KoreQuadCubeTileCode tileCodeRight2 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 2 } };
        KoreQuadCubeTileCode tileCodeRight3 = new KoreQuadCubeTileCode() { Face = KoreQuadFace.CubeFace.Right, Quadrants = new List<int> { 3 } };

        KoreQuadCubeTile tileRight0 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight0, radius: 1.1);
        KoreQuadCubeTile tileRight1 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight1, radius: 1.11);
        KoreQuadCubeTile tileRight2 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight2, radius: 1.12);
        KoreQuadCubeTile tileRight3 = KoreQuadCubeTileFactory.TileForCode2(tileCodeRight3, radius: 1.13);

        // KoreColorMeshOps.SetAllColors(tileRight0.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileRight1.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileRight2.ColorMesh, KoreColorPalette.RandomColor());
        // KoreColorMeshOps.SetAllColors(tileRight3.ColorMesh, KoreColorPalette.RandomColor());

        KoreColorMeshGodot coloredMeshNodeRight0 = new() { Name = "QuadMesh_Right_0" }; QuadMeshNode.AddChild(coloredMeshNodeRight0);
        KoreColorMeshGodot coloredMeshNodeRight1 = new() { Name = "QuadMesh_Right_1" }; QuadMeshNode.AddChild(coloredMeshNodeRight1);
        KoreColorMeshGodot coloredMeshNodeRight2 = new() { Name = "QuadMesh_Right_2" }; QuadMeshNode.AddChild(coloredMeshNodeRight2);
        KoreColorMeshGodot coloredMeshNodeRight3 = new() { Name = "QuadMesh_Right_3" }; QuadMeshNode.AddChild(coloredMeshNodeRight3);

        coloredMeshNodeRight0.UpdateMesh(tileRight0.ColorMesh);
        coloredMeshNodeRight1.UpdateMesh(tileRight1.ColorMesh);
        coloredMeshNodeRight2.UpdateMesh(tileRight2.ColorMesh);
        coloredMeshNodeRight3.UpdateMesh(tileRight3.ColorMesh);

        // - - - -

        KoreQuadZNMapTileDBManager.CreateOrOpenDB();
        byte[] x = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        KoreQuadZNMapTileDBManager.SetBytesForName("TestSeq", x);
    }

}
