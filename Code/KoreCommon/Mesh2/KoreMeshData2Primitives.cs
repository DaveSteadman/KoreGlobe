using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshData2Primitives
{
    // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0));
    public static KoreMeshData BasicCube(float size, KoreMeshMaterial mat)
    {
        var mesh = new KoreMeshData2();

        mesh.AddMaterial(mat);
        KoreColorRGB color = mat.BaseColor;
        KoreColorRGB lineColor = KoreColorRGB.White;
        int lineColorId = mesh.AddColor(lineColor);

        // defines a 3x3 grid of squares for the UV elements of a texture. Centre square is front, sides are sides, top is up, bottom is down.
        // top left square is the back face


        // Define the vertices of the cube
        int v0 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size)); // bottom right
        int v1 = mesh.AddVertex(new KoreXYZVector(size, -size, -size)); // bottom left
        int v2 = mesh.AddVertex(new KoreXYZVector(size, size, -size)); // top left
        int v3 = mesh.AddVertex(new KoreXYZVector(-size, size, -size)); // top right

        int v4 = mesh.AddVertex(new KoreXYZVector(-size, -size, size)); // bottom right
        int v5 = mesh.AddVertex(new KoreXYZVector(size, -size, size)); // bottom left
        int v6 = mesh.AddVertex(new KoreXYZVector(size, size, size)); // top left
        int v7 = mesh.AddVertex(new KoreXYZVector(-size, size, size)); // top right

        KoreXYZVector normalUp = new KoreXYZVector(0, 1, 0);
        KoreXYZVector normalDown = new KoreXYZVector(0, -1, 0);
        KoreXYZVector normalLeft = new KoreXYZVector(-1, 0, 0);
        KoreXYZVector normalRight = new KoreXYZVector(1, 0, 0);
        KoreXYZVector normalFront = new KoreXYZVector(0, 0, 1); // front facing observer
        KoreXYZVector normalBack = new KoreXYZVector(0, 0, -1);

        int normalUpId = mesh.AddNormal(normalUp);
        int normalDownId = mesh.AddNormal(normalDown);
        int normalLeftId = mesh.AddNormal(normalLeft);
        int normalRightId = mesh.AddNormal(normalRight);
        int normalFrontId = mesh.AddNormal(normalFront);
        int normalBackId = mesh.AddNormal(normalBack);

        // Lines
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v0, v1), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v1, v5), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v5, v4), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v4, v0), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v2, v3), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v3, v7), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v7, v6), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v6, v2), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v0, v3), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v1, v2), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v4, v7), lineColorId));
        mesh.AddLine(new KoreMeshLineRef(new KoreMeshIndex2(v5, v6), lineColorId));

        // UVs
        // Have the square split into a 3x3 grid, with each square corresponding to a face of the cube
        // - The Center [1,1] face is the top, with other sides draped around it.
        // - [0,0] is the bottom face
        // - UVs added for each row, 0,1,2,3, 4,5,6,7, 8,9,10,11, 12,13,14,15
        double third = 1.0 / 3.0;
        double twoThirds = 2.0 / 3.0;
        int uv0 = mesh.AddUV(new KoreXYVector(0, 0));               // 0
        int uv1 = mesh.AddUV(new KoreXYVector(third, 0));          // 1
        int uv2 = mesh.AddUV(new KoreXYVector(twoThirds, 0));     // 2
        int uv3 = mesh.AddUV(new KoreXYVector(1, 0));          // 3

        int uv4 = mesh.AddUV(new KoreXYVector(0, third));          // 4
        int uv5 = mesh.AddUV(new KoreXYVector(third, third));     // 5
        int uv6 = mesh.AddUV(new KoreXYVector(twoThirds, third)); // 6
        int uv7 = mesh.AddUV(new KoreXYVector(1, third));     // 7

        int uv8 = mesh.AddUV(new KoreXYVector(0, twoThirds)); // 8
        int uv9 = mesh.AddUV(new KoreXYVector(third, twoThirds)); // 9
        int uv10 = mesh.AddUV(new KoreXYVector(twoThirds, twoThirds)); // 10
        int uv11 = mesh.AddUV(new KoreXYVector(1, twoThirds)); // 11

        int uv12 = mesh.AddUV(new KoreXYVector(0, 1)); // 12
        int uv13 = mesh.AddUV(new KoreXYVector(third, 1)); // 13
        int uv14 = mesh.AddUV(new KoreXYVector(twoThirds, 1)); // 14
        int uv15 = mesh.AddUV(new KoreXYVector(1, 1)); // 15

        // Triangles - using CW winding when viewed from outside
        // Front face (Z = -size) - looking at it from positive Z
        // For CW: v0 (bottom-left) → v1 (bottom-right) → v2 (top-right)
        //          v0 (bottom-left) → v2 (top-right) → v3 (top-left)
        mesh.AddTriangle(new KoreMeshTriRef(V:new KoreMeshIndex3(v0, v2, v1), N:normalBackId, UV:new KoreMeshIndex3(0, 1, 2), 0));
        mesh.AddTriangle(v0, v2, v3);

        // Left face (X = -size) - looking at it from positive X
        mesh.AddTriangle(v0, v7, v4); mesh.AddTriangle(v0, v3, v7);

        // Back face (Z = +size) - looking at it from negative Z
        mesh.AddTriangle(v5, v7, v6); mesh.AddTriangle(v5, v4, v7);

        // Right face (X = +size) - looking at it from negative X
        mesh.AddTriangle(v1, v6, v2); mesh.AddTriangle(v1, v5, v6);

        // Top face (Y = +size) - looking at it from negative Y
        mesh.AddTriangle(v3, v6, v7); mesh.AddTriangle(v3, v2, v6);

        // Bottom face (Y = -size) - looking at it from positive Y
        mesh.AddTriangle(v0, v5, v1); mesh.AddTriangle(v0, v4, v5);

        mesh.AddAllTrianglesToGroup("All");
        mesh.SetGroupMaterialName("All", mat.Name);

        //mesh.SetNormalsFromTriangles();

        //mesh.MakeValid();
        return mesh;
    }
}