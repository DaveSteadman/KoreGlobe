using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMiniMeshPrimitives
{
    // Usage: KoreMiniMesh cubeMesh = KoreMiniMeshPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0), new KoreColorRGB(255, 255, 255));
    public static KoreMiniMesh BasicCube(float size, KoreColorRGB col, KoreColorRGB lineCol)
    {
        var mesh = new KoreMiniMesh();

        // Add the colors
        int colorId     = mesh.AddColor(col);
        int lineColorId = mesh.AddColor(lineCol);

        // Define the vertices of the cube
        int v0 = mesh.AddVertex(new KoreXYZVector(size, size, -size)); // top left // Front Face
        int v1 = mesh.AddVertex(new KoreXYZVector(-size, size, -size)); // top right
        int v2 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size)); // bottom right
        int v3 = mesh.AddVertex(new KoreXYZVector(size, -size, -size)); // bottom left

        int v4 = mesh.AddVertex(new KoreXYZVector(size, size, size)); // top left // Back Face
        int v5 = mesh.AddVertex(new KoreXYZVector(-size, size, size)); // top right
        int v6 = mesh.AddVertex(new KoreXYZVector(-size, -size, size)); // bottom right
        int v7 = mesh.AddVertex(new KoreXYZVector(size, -size, size)); // bottom left

        // Lines
        mesh.AddLine(new KoreMiniMeshLine(v0, v1, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v1, v5, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v5, v4, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v4, v0, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v2, v3, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v3, v7, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v7, v6, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v6, v2, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v0, v3, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v1, v2, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v4, v7, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(v5, v6, lineColorId));

        List<int> allTris = new();

        // Top face
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v4, v5, v1)));
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v4, v1, v0)));

        // Front face
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v0, v1, v2)));
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v0, v2, v3)));

        // Left face
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v4, v0, v3)));
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v4, v3, v7)));

        // Right face
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v4, v7)));
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v1, v7, v2)));

        // Back face
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v4, v5, v6)));
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v4, v6, v7)));

        // Bottom face
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v0, v2, v6)));
        allTris.Add(mesh.AddTriangle(new KoreMiniMeshTri(v0, v6, v4)));

        // Groups
        mesh.AddGroup("All", new KoreMiniMeshGroup(colorId, allTris));

        return mesh;
    }



}
