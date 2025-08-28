using System;
using System.Collections.Generic;

using Godot;

using KoreCommon;
using KoreSim;

#nullable enable

public static class KoreNodeUtils
{

    // Usage: AddChild(KoreNodeUtils.DebugSphere(0.3f))
    public static Node3D DebugSphere(float radius)
    {
        // Create a simple sphere mesh for debugging
        SphereMesh sphereMesh = new SphereMesh
        {
            Radius = radius,
            Height = radius * 2,
            RadialSegments = 16,
            Rings = 8
        };

        MeshInstance3D sphereInstance = new MeshInstance3D
        {
            Mesh = sphereMesh,
            Name = "DebugSphere"
        };

        return sphereInstance;
    }



}


