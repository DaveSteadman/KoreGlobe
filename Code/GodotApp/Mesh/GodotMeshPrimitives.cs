

// static class to perform mesh operations
using System;
using Godot;

using KoreCommon;

public static class GodotMeshPrimitives
{

    // Add a debug sphere to any Node3D.
    // Usage: GodotMeshPrimitives.AddChildDebugSphere(parentNode, 1.0f, new KoreColorRGB(255, 0, 0));
    public static void AddChildDebugSphere(Node3D parentNode, float radius, KoreColorRGB color)
    {
        // create the basic mesh data
        var cubeMesh1 = KoreMeshDataPrimitives.BasicSphere(radius, color, 12);

        // create the surface and line mesh nodes
        KoreGodotLineMesh lineMeshNode = new KoreGodotLineMesh() { Name = "SphereLines" };
        lineMeshNode.UpdateMesh(cubeMesh1);
        parentNode.AddChild(lineMeshNode);

        KoreGodotSurfaceMesh surfaceMeshNode = new KoreGodotSurfaceMesh() { Name = "SphereSurface" };
        surfaceMeshNode.UpdateMesh(cubeMesh1);
        parentNode.AddChild(surfaceMeshNode);
    }
}