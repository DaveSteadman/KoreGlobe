using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshDataPrimitives
{
    public static KoreMeshData Cylinder(KoreXYZVector p1, KoreXYZVector p2, double p1radius, double p2radius, int sides, bool endsClosed)
    {
        var mesh = new KoreMeshData();

        // Direction and length
        KoreXYZVector axis = p2 - p1;
        double height = axis.Magnitude;
        if (height < 1e-6f || sides < 3) return mesh;

        axis = axis.Normalize();

        // Find a vector not parallel to axis for basis
        KoreXYZVector up      = Math.Abs(KoreXYZVector.DotProduct(axis, KoreXYZVector.Up)) < 0.99f ? KoreXYZVector.Up : KoreXYZVector.Right;
        KoreXYZVector side    = KoreXYZVector.CrossProduct(axis, up).Normalize();
        KoreXYZVector forward = KoreXYZVector.CrossProduct(axis, side).Normalize();

        // Generate circle points for both ends
        var p1Circle = new List<KoreXYZVector>();
        var p2Circle = new List<KoreXYZVector>();
        double angleStep = Math.Tau / sides;

        for (int i = 0; i < sides; i++)
        {
            double angle = i * angleStep;
            KoreXYZVector offset = (Math.Cos(angle) * side + Math.Sin(angle) * forward);
            p1Circle.Add(p1 + offset * p1radius);
            p2Circle.Add(p2 + offset * p2radius);
        }

        // Side faces (quads split into triangles)
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            // First triangle
            mesh.AddTriangle(p1Circle[i], p2Circle[i], p2Circle[next]);
            // Second triangle
            mesh.AddTriangle(p1Circle[i], p2Circle[next], p1Circle[next]);
        }

        // End caps
        if (endsClosed)
        {
            // p1 cap
            for (int i = 1; i < sides - 1; i++)
                mesh.AddTriangle(p1, p1Circle[i], p1Circle[i + 1]);

            // p2 cap
            for (int i = 1; i < sides - 1; i++)
                mesh.AddTriangle(p2, p2Circle[i + 1], p2Circle[i]);
        }

        // --- Add lines for wireframe ---
        KoreColorRGB lineColor = new KoreColorRGB(200, 200, 200); // Or choose as needed

        // Longitudinal lines
        for (int i = 0; i < sides; i++)
        {
            mesh.AddLine(p1Circle[i], p2Circle[i], lineColor, lineColor);
        }

        // Rings at each end
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            mesh.AddLine(p1Circle[i], p1Circle[next], lineColor, lineColor);
            mesh.AddLine(p2Circle[i], p2Circle[next], lineColor, lineColor);
        }

        return mesh;
    }
}
