using System;
using KoreCommon;
using Godot;

#nullable enable

// ------------------------------------------------------------------------------------------------
// KoreZeroNodeSphere:
// - A debug sphere thats rooted to the KoreZeroNode and moves with the relocatable geometry.
// - Exists to provide a baseline relocabale geometry demonstration and debug reference for tile systems.
// - Real world XYZ/LLA is 0,0,0
// ------------------------------------------------------------------------------------------------

public partial class KoreZeroNodeSphere : Node3D
{
    private float SphereRadius = 10f;
    private KoreColorRGB SphereColor = KoreColorRGB.White;

    private KoreLLAPoint EarthZeroLLA = new() { LatDegs = 0, LonDegs = 0, AltMslM = 0 };

    // --------------------------------------------------------------------------------------------

    public KoreZeroNodeSphere(float radius, KoreColorRGB color)
    {
        SphereRadius = radius;
        SphereColor = color;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "ZeroNodeSphere-Test";

        CreateSphere();
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        // If we're in a pos-change cycle, update the tile location
        if (KoreRelocateOps.IsChangePeriod())
            UpdateTileLocation();
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    private void UpdateTileLocation()
    {
        // So RW XYZ 0,0,0 is the center of the test sphere.
        // Set the local position from the parent object
        Vector3 newRelocatedPos = KoreRelocateOps.RWtoGE(KoreXYZVector.Zero);

        // Set the local position from the parent object
        var transform = GlobalTransform;
        transform.Origin = newRelocatedPos;
        GlobalTransform = transform;
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    private void CreateSphere()
    {
        // Core Sphere
        {
            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("SmokedGlass");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(KoreXYZVector.Zero, SphereRadius, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = "CoreSphere - White" };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            AddChild(coloredMeshNode);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine() { Name = "CoreSphereLine - Wire" };
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }

        // X-Axis
        {
            KoreXYZVector posXYZ = new() { X = SphereRadius };

            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattDarkRed");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(posXYZ, SphereRadius / 8, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = "XAxisSphere - Red" };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            AddChild(coloredMeshNode);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine() { Name = "XAxisSphereLine - Wire" };
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }

        // Y-Axis
        {
            KoreXYZVector posXYZ = new() { Y = SphereRadius };

            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattDarkGreen");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(posXYZ, SphereRadius / 8, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = "YAxisSphere - Green" };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            AddChild(coloredMeshNode);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine() { Name = "YAxisSphereLine - Wire" };
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }

        // Z-Axis
        {
            KoreXYZVector posXYZ = new() { Z = SphereRadius };

            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattDarkBlue");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(posXYZ, SphereRadius / 8, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = "ZAxisSphere - Blue" };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            AddChild(coloredMeshNode);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine() { Name = "ZAxisSphereLine - Wire" };
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }
    }
}


