// <fileheader>

using System;
using System.Collections.Generic;

namespace KoreCommon;

// Static class to create KoreMiniMesh primitives
// This class is used to create various 3D shapes and meshes

public static partial class KoreMiniMeshPrimitives
{
    /// <summary>
    /// Create a pyramid mesh for KoreMiniMesh with explicit base orientation control
    /// </summary>
    /// <param name="pApex">Apex point of the pyramid</param>
    /// <param name="pBaseCenter">Center point of the base</param>
    /// <param name="baseReferenceDirection">Reference direction to orient the base (will be projected onto base plane)</param>
    /// <param name="width">Width of the base (perpendicular to reference direction)</param>
    /// <param name="height">Height of the base (along reference direction)</param>
    /// <param name="baseClosed">Whether to create the base face</param>
    /// <param name="material">Material for the pyramid surface</param>
    /// <param name="lineCol">Color for wireframe lines</param>
    /// <returns>KoreMiniMesh pyramid</returns>
    public static KoreMiniMesh CreatePyramid(
        KoreXYZVector pApex,
        KoreXYZVector pBaseCenter,
        KoreXYZVector baseReferenceDirection,
        double width,
        double height,
        bool baseClosed,
        KoreMiniMeshMaterial material,
        KoreColorRGB lineCol)
    {
        var mesh = new KoreMiniMesh();

        // Add material and line color
        mesh.AddMaterial(material);
        int lineColorId = mesh.AddColor(lineCol);
        string matName = material.Name;

        // Calculate pyramid axis (from base to apex)
        KoreXYZVector axis = (pApex - pBaseCenter).Normalize();
        double pyramidHeight = (pApex - pBaseCenter).Magnitude;

        if (pyramidHeight < 1e-6) throw new ArgumentException("Pyramid height must be greater than zero");

        // Use the same plane creation logic as the cylinder for consistent behavior
        // This ensures the base is perpendicular to the axis
        KoreXYZPlane basePlane = KoreXYZPlane.MakePlane(pBaseCenter, axis, baseReferenceDirection);

        // Calculate the 4 base vertices using the plane's coordinate system
        double halfWidth = width * 0.5;
        double halfHeight = height * 0.5;

        // Create 2D points in the plane's coordinate system
        var baseVertex1_2D = new KoreXYVector(halfWidth, halfHeight);   // front-right
        var baseVertex2_2D = new KoreXYVector(-halfWidth, halfHeight);  // front-left
        var baseVertex3_2D = new KoreXYVector(-halfWidth, -halfHeight); // back-left
        var baseVertex4_2D = new KoreXYVector(halfWidth, -halfHeight);  // back-right

        // Project to 3D using the plane
        KoreXYZVector baseVertex1Pos = basePlane.Project2DTo3D(baseVertex1_2D);
        KoreXYZVector baseVertex2Pos = basePlane.Project2DTo3D(baseVertex2_2D);
        KoreXYZVector baseVertex3Pos = basePlane.Project2DTo3D(baseVertex3_2D);
        KoreXYZVector baseVertex4Pos = basePlane.Project2DTo3D(baseVertex4_2D);

        // Add vertices to mesh
        int apexVertex = mesh.AddVertex(pApex);
        int baseVertex1 = mesh.AddVertex(baseVertex1Pos);
        int baseVertex2 = mesh.AddVertex(baseVertex2Pos);
        int baseVertex3 = mesh.AddVertex(baseVertex3Pos);
        int baseVertex4 = mesh.AddVertex(baseVertex4Pos);

        List<int> allTriangles = new List<int>();

        // Create the side faces (triangles from apex to base edges)
        allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(apexVertex, baseVertex2, baseVertex1))); // front face
        allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(apexVertex, baseVertex3, baseVertex2))); // left face
        allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(apexVertex, baseVertex4, baseVertex3))); // back face
        allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(apexVertex, baseVertex1, baseVertex4))); // right face

        // Add base face if requested
        if (baseClosed)
        {
            // Base triangles (wind inward - normal pointing away from apex)
            allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(baseVertex1, baseVertex2, baseVertex3))); // base triangle 1
            allTriangles.Add(mesh.AddTriangle(new KoreMiniMeshTri(baseVertex1, baseVertex3, baseVertex4))); // base triangle 2
        }

        // Create wireframe lines
        // Base rectangle lines
        mesh.AddLine(new KoreMiniMeshLine(baseVertex1, baseVertex2, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(baseVertex2, baseVertex3, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(baseVertex3, baseVertex4, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(baseVertex4, baseVertex1, lineColorId));

        // Lines from apex to each base vertex
        mesh.AddLine(new KoreMiniMeshLine(apexVertex, baseVertex1, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(apexVertex, baseVertex2, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(apexVertex, baseVertex3, lineColorId));
        mesh.AddLine(new KoreMiniMeshLine(apexVertex, baseVertex4, lineColorId));

        // Add diagonal lines on base if it's closed
        if (baseClosed)
        {
            mesh.AddLine(new KoreMiniMeshLine(baseVertex1, baseVertex3, lineColorId)); // diagonal 1
            mesh.AddLine(new KoreMiniMeshLine(baseVertex2, baseVertex4, lineColorId)); // diagonal 2
        }

        // Create groups
        mesh.AddGroup("All", new KoreMiniMeshGroup(matName, allTriangles));

        return mesh;
    }

    /// <summary>
    /// Create a pyramid with automatic reference direction selection
    /// </summary>
    /// <param name="pApex">Apex point of the pyramid</param>
    /// <param name="pBaseCenter">Center point of the base</param>
    /// <param name="width">Width of the base</param>
    /// <param name="height">Height of the base</param>
    /// <param name="baseClosed">Whether to create the base face</param>
    /// <param name="material">Material for the pyramid surface</param>
    /// <param name="lineCol">Color for wireframe lines</param>
    /// <returns>KoreMiniMesh pyramid</returns>
    public static KoreMiniMesh CreatePyramidAuto(
        KoreXYZVector pApex,
        KoreXYZVector pBaseCenter,
        double width,
        double height,
        bool baseClosed,
        KoreMiniMeshMaterial material,
        KoreColorRGB lineCol)
    {
        // Calculate pyramid axis
        KoreXYZVector axis = (pApex - pBaseCenter).Normalize();
        
        // Use ArbitraryPerpendicular to get a consistent reference direction
        KoreXYZVector baseReference = axis.ArbitraryPerpendicular();
        
        return CreatePyramid(pApex, pBaseCenter, baseReference, width, height, baseClosed, material, lineCol);
    }

    /// <summary>
    /// Create a circular pyramid mesh for KoreMiniMesh
    /// </summary>
    /// <param name="pApex">Apex point of the pyramid</param>
    /// <param name="pBaseCenter">Center point of the base</param>
    /// <param name="baseRadius">Radius of the base circle</param>
    /// <param name="sides">Number of sides for the base (3=triangle, 4=square, etc.)</param>
    /// <param name="baseClosed">Whether to create the base face</param>
    /// <param name="material">Material for the pyramid surface</param>
    /// <param name="lineCol">Color for wireframe lines</param>
    /// <returns>KoreMiniMesh pyramid</returns>
    public static KoreMiniMesh CreateCircularPyramid(
        KoreXYZVector pApex,
        KoreXYZVector pBaseCenter,
        double baseRadius,
        int sides,
        bool baseClosed,
        KoreMiniMeshMaterial material,
        KoreColorRGB lineCol)
    {
        if (sides < 3) throw new ArgumentException("Pyramid must have at least 3 sides");
        
        var mesh = new KoreMiniMesh();

        // Add material and line color
        mesh.AddMaterial(material);
        int lineColorId = mesh.AddColor(lineCol);
        string matName = material.Name;

        // Calculate pyramid axis (from base to apex)
        KoreXYZVector axis = (pApex - pBaseCenter).Normalize();
        double height = (pApex - pBaseCenter).Magnitude;

        if (height < 1e-6) throw new ArgumentException("Pyramid height must be greater than zero");

        List<int> allTriangles = new List<int>();

        // Generate base circle points using AddCirclePoints
        // The base should be perpendicular to the apex-base axis
        List<int> baseCircle = KoreMiniMeshOps.AddCirclePoints(mesh, pBaseCenter, axis, baseRadius, sides);

        // Add the apex vertex
        int apexVertex = mesh.AddVertex(pApex);

        // Create the side faces using fan pattern from apex to base circle
        allTriangles.AddRange(KoreMiniMeshOps.AddFan(mesh, apexVertex, baseCircle, true));

        // Add base face if requested
        if (baseClosed)
        {
            // Bottom face - wind inward (normal pointing down the axis away from apex)
            int baseCenterVertex = mesh.AddVertex(pBaseCenter);
            allTriangles.AddRange(KoreMiniMeshOps.AddFan(mesh, baseCenterVertex, baseCircle, false));
        }

        // Create wireframe lines
        // Base circle lines
        KoreMiniMeshOps.AddCircleLines(mesh, baseCircle, lineColorId);

        // Lines from apex to each base vertex
        for (int i = 0; i < sides; i++)
        {
            mesh.AddLine(new KoreMiniMeshLine(apexVertex, baseCircle[i], lineColorId));
        }

        // Add radial lines from base center if base is closed
        if (baseClosed)
        {
            int baseCenterVertex = mesh.Vertices.Count - 1; // Last vertex added

            // Add a few radial lines (not all, to avoid clutter)
            int radialLines = Math.Min(4, sides);
            for (int i = 0; i < radialLines; i++)
            {
                int idx = i * sides / radialLines;
                mesh.AddLine(new KoreMiniMeshLine(baseCenterVertex, baseCircle[idx], lineColorId));
            }
        }

        // Create groups
        mesh.AddGroup("All", new KoreMiniMeshGroup(matName, allTriangles));

        return mesh;
    }

    /// <summary>
    /// Create a simple pyramid with default parameters
    /// </summary>
    /// <param name="center">Center point of the base</param>
    /// <param name="axis">Axis direction from base to apex (will be normalized)</param>
    /// <param name="height">Height of the pyramid</param>
    /// <param name="width">Width of the base</param>
    /// <param name="depth">Depth of the base</param>
    /// <param name="material">Material for the pyramid surface</param>
    /// <param name="lineCol">Color for wireframe lines</param>
    /// <returns>KoreMiniMesh pyramid</returns>
    public static KoreMiniMesh Pyramid(
        KoreXYZVector center,
        KoreXYZVector axis,
        double height,
        double width,
        double depth,
        KoreMiniMeshMaterial material,
        KoreColorRGB lineCol)
    {
        axis = axis.Normalize();
        KoreXYZVector pBaseCenter = center;
        KoreXYZVector pApex = center + axis * height;
        
        return CreatePyramidAuto(pApex, pBaseCenter, width, depth, true, material, lineCol);
    }
}
