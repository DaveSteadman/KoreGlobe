// <fileheader>

using System;
using System.Collections.Generic;
using System.IO;
using KoreCommon;
using KoreCommon.SkiaSharp;

namespace KoreCommon.UnitTest;

public static partial class KoreTestMeshUvOps
{
    // KoreTestMeshUvOps.CreateBoxWithUV
    public static KoreMeshData CreateBoxWithUV(double width = 1.0, double height = 3.0, double depth = 1.0)
    {
        var mesh = new KoreMeshData();

        float thirds = 1f / 3f;
        float twothirds = 2f / 3f;

        double halfWidth = width / 2.0;
        double halfDepth = depth / 2.0;

        // 8 Cube Point Locations
        // Use native x is right, y+ height, -Z for depth away.
        // We'll put the 0,0,0 point of the box middle of the base
        KoreXYZVector topBackLeft = new(-halfWidth, height, -halfDepth);
        KoreXYZVector topBackRight = new(halfWidth, height, -halfDepth);
        KoreXYZVector topFrontLeft = new(-halfWidth, height, halfDepth);
        KoreXYZVector topFrontRight = new(halfWidth, height, halfDepth);
        KoreXYZVector bottomBackLeft = new(-halfWidth, 0, -halfDepth);
        KoreXYZVector bottomBackRight = new(halfWidth, 0, -halfDepth);
        KoreXYZVector bottomFrontLeft = new(-halfWidth, 0, halfDepth);
        KoreXYZVector bottomFrontRight = new(halfWidth, 0, halfDepth);

        // Top Face
        {
            KoreXYVector debugUvTL = new KoreXYVector(thirds, thirds);
            KoreXYVector debugUvTR = new KoreXYVector(twothirds, thirds);
            KoreXYVector debugUvBR = new KoreXYVector(twothirds, twothirds);
            KoreXYVector debugUvBL = new KoreXYVector(thirds, twothirds);

            mesh.AddFace(topBackLeft, topBackRight, topFrontRight, topFrontLeft,
                    debugUvTL, debugUvTR, debugUvBR, debugUvBL,
                    "TopFace"); // Top face - may need UV rotation
        }

        // Front Face
        {
            KoreXYVector debugUvTL = new KoreXYVector(thirds, twothirds);
            KoreXYVector debugUvTR = new KoreXYVector(twothirds, twothirds);
            KoreXYVector debugUvBR = new KoreXYVector(twothirds, 1);
            KoreXYVector debugUvBL = new KoreXYVector(thirds, 1);

            mesh.AddFace(topFrontLeft, topFrontRight, bottomFrontRight, bottomFrontLeft,
                    debugUvTL, debugUvTR, debugUvBR, debugUvBL,
                    "FrontFace"); // Front face - may need UV rotation
        }

        // Left Face
        {
            KoreXYVector debugUvTL = new KoreXYVector(thirds, thirds);
            KoreXYVector debugUvTR = new KoreXYVector(thirds, twothirds);
            KoreXYVector debugUvBR = new KoreXYVector(0, twothirds);
            KoreXYVector debugUvBL = new KoreXYVector(0, thirds);

            mesh.AddFace(topBackLeft, topFrontLeft, bottomFrontLeft, bottomBackLeft,
                    debugUvTL, debugUvTR, debugUvBR, debugUvBL,
                    "LeftFace"); // Left face - may need UV rotation
        }

        // Right Face
        {
            KoreXYVector debugUvTL = new KoreXYVector(twothirds, twothirds);
            KoreXYVector debugUvTR = new KoreXYVector(twothirds, thirds);
            KoreXYVector debugUvBR = new KoreXYVector(1, thirds);
            KoreXYVector debugUvBL = new KoreXYVector(1, twothirds);

            mesh.AddFace(topFrontRight, topBackRight, bottomBackRight, bottomFrontRight,
                    debugUvTL, debugUvTR, debugUvBR, debugUvBL,
                    "RightFace"); // Right face - may need UV rotation
        }

        // Back Face
        {
            KoreXYVector debugUvTL = new KoreXYVector(twothirds, thirds);
            KoreXYVector debugUvTR = new KoreXYVector(thirds, thirds);
            KoreXYVector debugUvBR = new KoreXYVector(thirds, 0);
            KoreXYVector debugUvBL = new KoreXYVector(twothirds, 0);

            mesh.AddFace(topBackRight, topBackLeft, bottomBackLeft, bottomBackRight,
                    debugUvTL, debugUvTR, debugUvBR, debugUvBL,
                    "BackFace"); // Back face - may need UV rotation
        }

        // Bottom Face (top right of 3x3 texture)
        {
            KoreXYVector debugUvTL = new KoreXYVector(0, 0);
            KoreXYVector debugUvTR = new KoreXYVector(thirds, 0);
            KoreXYVector debugUvBR = new KoreXYVector(thirds, thirds);
            KoreXYVector debugUvBL = new KoreXYVector(0, thirds);

            mesh.AddFace(bottomFrontLeft, bottomFrontRight, bottomBackRight, bottomBackLeft,
                    debugUvTL, debugUvTR, debugUvBR, debugUvBL,
                    "BottomFace"); // Bottom face - may need UV rotation
        }

        KoreColorRGB lineColor = new KoreColorRGB(1, 1, 1);

        // Top Lines
        mesh.AddLine(topFrontLeft, topFrontRight, lineColor);
        mesh.AddLine(topFrontRight, topBackRight, lineColor);
        mesh.AddLine(topBackRight, topBackLeft, lineColor);
        mesh.AddLine(topBackLeft, topFrontLeft, lineColor);

        // Bottom Lines
        mesh.AddLine(bottomFrontLeft, bottomFrontRight, lineColor);
        mesh.AddLine(bottomFrontRight, bottomBackRight, lineColor);
        mesh.AddLine(bottomBackRight, bottomBackLeft, lineColor);
        mesh.AddLine(bottomBackLeft, bottomFrontLeft, lineColor);

        // Vertical Lines
        mesh.AddLine(topFrontLeft, bottomFrontLeft, lineColor);
        mesh.AddLine(topFrontRight, bottomFrontRight, lineColor);
        mesh.AddLine(topBackRight, bottomBackRight, lineColor);
        mesh.AddLine(topBackLeft, bottomBackLeft, lineColor);

        // Add all triangles to an "All" group with the default material
        mesh.AddDefaultMaterial();
        mesh.AddAllTrianglesToGroup("All");
        mesh.SetGroupMaterialName("All", KoreMeshMaterialPalette.DefaultMaterialName);


        KoreMeshDataEditOps.SetNormalsFromTriangles(mesh);

        // dump the mesh JSON to the log for analysis
        string json = KoreMeshDataIO.ToJson(mesh, dense: false);
        KoreCentralLog.AddEntry($"TextBox: {json}");

        return mesh;
    }
}
