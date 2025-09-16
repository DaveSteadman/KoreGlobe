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
        Name = "ZeroNode";

        CreateSphere();
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        // If we're in a pos-change cycle, update the tile location
        if (KoreZeroOffset.IsPosChangeCycle)
            UpdateTileLocation();
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    private void UpdateTileLocation()
    {
        // Set the local position from the parent object
        Vector3 newPos = KoreGeoConvOps.RwToOffsetGe(EarthZeroLLA);

        // Set the local position from the parent object
        var transform = GlobalTransform;
        transform.Origin = newPos;
        GlobalTransform = transform;
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    private void CreateSphere()
    {
        // Core Sphere
        {
            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattWhite");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(KoreXYZVector.Zero, SphereRadius, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface();
            AddChild(coloredMeshNode);
            coloredMeshNode.UpdateMesh(sphereMesh);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }

        // X-Axis
        {
            KoreXYZVector posXYZ = new() { X = SphereRadius };

            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattDarkRed");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(posXYZ, SphereRadius / 8, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface();
            AddChild(coloredMeshNode);
            coloredMeshNode.UpdateMesh(sphereMesh);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }

        // Y-Axis
        {
            KoreXYZVector posXYZ = new() { Y = SphereRadius };

            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattDarkGreen");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(posXYZ, SphereRadius / 8, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface();
            AddChild(coloredMeshNode);
            coloredMeshNode.UpdateMesh(sphereMesh);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }

        // Z-Axis
        {
            KoreXYZVector posXYZ = new() { Z = SphereRadius };
            
            KoreMiniMeshMaterial mat = KoreMiniMeshMaterialPalette.Find("MattDarkBlue");
            KoreColorRGB lineCol = KoreColorRGB.White;
            KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(posXYZ, SphereRadius/8, 16, mat, lineCol);

            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface();
            AddChild(coloredMeshNode);
            coloredMeshNode.UpdateMesh(sphereMesh);

            KoreMiniMeshGodotLine lineMeshNode1 = new KoreMiniMeshGodotLine();
            lineMeshNode1.UpdateMesh(sphereMesh, "All");
            AddChild(lineMeshNode1);
        }
    }
}