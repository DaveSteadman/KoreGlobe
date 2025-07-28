using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshDataPrimitives
{
    // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0));
    public static KoreMeshData Surface(Vector3[,] vertices)
    {
        var mesh = new KoreMeshData();

        return mesh;
    }

}
