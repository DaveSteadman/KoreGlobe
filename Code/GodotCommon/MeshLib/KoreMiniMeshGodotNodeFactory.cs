// <fileheader>

// Factory for creating visual debug nodes using KoreMiniMesh primitives with colored surfaces and wireframes

using System;
using Godot;
using KoreCommon;

#nullable enable

public static class KoreMiniMeshGodotNodeFactory
{
    // Creates a node with sphere mesh, colored surface and optional wireframe

    // Example Usage:
    //      Node3D sphereNode = KoreMiniMeshGodotNodeFactory.CreateSphere(
    //                              "MySphere",
    //                              position: pos, radius: 1f, segments: 24,
    //                              KoreMiniMeshMaterialPalette.Find("SmokedGlass"), KoreColorRGB.White)
    //                              includeSurface: true, includeWireframe: true);

    public static Node3D CreateSphere(
        string nodeName,
        KoreXYZVector position, float radius, int segments,
        KoreMiniMeshMaterial surfaceMat, KoreColorRGB wireColor,
        bool includeSurface = true, bool includeWireframe = true)
    {
        Node3D node = new Node3D() { Name = nodeName };

        // Create material and mesh
        KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(position, radius, segments, surfaceMat, wireColor);

        // Add colored surface
        if (includeSurface)
        {
            KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface()
            {
                Name = nodeName + "_Surface"
            };
            coloredMeshNode.UpdateMesh(sphereMesh, "All");
            node.AddChild(coloredMeshNode);
        }

        // Add wireframe lines if requested
        if (includeWireframe)
        {
            KoreMiniMeshGodotLine lineMeshNode = new KoreMiniMeshGodotLine()
            {
                Name = nodeName + "_Wire"
            };
            lineMeshNode.UpdateMesh(sphereMesh, "All");
            node.AddChild(lineMeshNode);
        }

        return node;
    }

    // Creates a sphere at origin with default settings - useful for markers
    public static Node3D CreateMarker(string nodeName, KoreColorRGB color, float size, float alpha = 0.75f)
    {
        color.A = KoreColorIO.FloatToByte(alpha);
        var tempMat = new KoreMiniMeshMaterial(color, metallic: 0.5f, roughness: 0.5f);

        return CreateSphere(
            nodeName: nodeName,
            position: KoreXYZVector.Zero, radius: size, segments: 16,
            surfaceMat: tempMat, wireColor: KoreColorRGB.White,
            includeSurface: true, includeWireframe: true);
    }

}
