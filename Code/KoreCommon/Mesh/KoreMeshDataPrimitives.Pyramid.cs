using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshDataPrimitives
{
    // Create a basic pyramid mesh
    // - apexPoint: The apex point of the pyramid - which will be an offset from the node its ultimately attached to.
    // - apexBaseVector: The vector from the apex to the base center point.
    // - baseForwardVector: The vector, from the base centre point outwards, that defines the forward direction of the base
    // - width and height: relative to the forward or up direction of the base.
    public static KoreMeshData BasicPyramid(
        KoreXYZPoint apexPoint, KoreXYZVector apexBaseVector, KoreXYZVector baseForwardVector,
        float width, float height,
        KoreColorRGB color)
    {
        KoreMeshData koreMeshData = new KoreMeshData();

        // Calculate base center point
        KoreXYZPoint baseCenterPoint = apexPoint + apexBaseVector;
        
        // Ensure baseForwardVector is perpendicular to apexBaseVector
        // If they're not perpendicular, project baseForwardVector onto the plane perpendicular to apexBaseVector
        KoreXYZVector normalizedApexBase = apexBaseVector.Normalize();
        double dotProduct = KoreXYZVector.DotProduct(baseForwardVector, normalizedApexBase);
        KoreXYZVector projectedForward = baseForwardVector - (normalizedApexBase * dotProduct);
        KoreXYZVector normalizedForward = projectedForward.Normalize();
        
        // Create the right vector perpendicular to both apexBaseVector and baseForwardVector
        KoreXYZVector rightVector = KoreXYZVector.CrossProduct(normalizedForward, normalizedApexBase).Normalize();
        
        // Calculate the 4 base vertices for a square pyramid
        double halfWidth = width * 0.5;
        double halfHeight = height * 0.5;
        
        KoreXYZPoint baseVertex1 = baseCenterPoint + (normalizedForward * halfHeight) + (rightVector * halfWidth);   // front-right
        KoreXYZPoint baseVertex2 = baseCenterPoint + (normalizedForward * halfHeight) - (rightVector * halfWidth);   // front-left
        KoreXYZPoint baseVertex3 = baseCenterPoint - (normalizedForward * halfHeight) - (rightVector * halfWidth);   // back-left
        KoreXYZPoint baseVertex4 = baseCenterPoint - (normalizedForward * halfHeight) + (rightVector * halfWidth);   // back-right

        // Convert points to vectors for AddVertex (AddVertex expects KoreXYZVector)
        int idxApex = koreMeshData.AddVertex(new KoreXYZVector(apexPoint.X, apexPoint.Y, apexPoint.Z), null, color);
        int idxBase1 = koreMeshData.AddVertex(new KoreXYZVector(baseVertex1.X, baseVertex1.Y, baseVertex1.Z), null, color);
        int idxBase2 = koreMeshData.AddVertex(new KoreXYZVector(baseVertex2.X, baseVertex2.Y, baseVertex2.Z), null, color);
        int idxBase3 = koreMeshData.AddVertex(new KoreXYZVector(baseVertex3.X, baseVertex3.Y, baseVertex3.Z), null, color);
        int idxBase4 = koreMeshData.AddVertex(new KoreXYZVector(baseVertex4.X, baseVertex4.Y, baseVertex4.Z), null, color);
        
        // Add triangles for the pyramid faces
        koreMeshData.AddTriangle(idxApex, idxBase1, idxBase2, color); // front face
        koreMeshData.AddTriangle(idxApex, idxBase2, idxBase3, color); // left face
        koreMeshData.AddTriangle(idxApex, idxBase3, idxBase4, color); // back face
        koreMeshData.AddTriangle(idxApex, idxBase4, idxBase1, color); // right face
        
        // Add base (optional - you might want a solid base)
        koreMeshData.AddTriangle(idxBase1, idxBase4, idxBase3, color); // base triangle 1
        koreMeshData.AddTriangle(idxBase1, idxBase3, idxBase2, color); // base triangle 2
        
        // Add wireframe lines
        koreMeshData.AddLine(idxApex, idxBase1, color, color);
        koreMeshData.AddLine(idxApex, idxBase2, color, color);
        koreMeshData.AddLine(idxApex, idxBase3, color, color);
        koreMeshData.AddLine(idxApex, idxBase4, color, color);
        koreMeshData.AddLine(idxBase1, idxBase2, color, color);
        koreMeshData.AddLine(idxBase2, idxBase3, color, color);
        koreMeshData.AddLine(idxBase3, idxBase4, color, color);
        koreMeshData.AddLine(idxBase4, idxBase1, color, color);

        return koreMeshData;
    }

    // Usage: var pyramidMesh = KoreMeshDataPrimitives.BasicPyramidSharpEdges(apex, baseVector, forwardVector, 2.0f, 2.0f, new KoreColorRGB(255, 0, 0));
    public static KoreMeshData BasicPyramidSharpEdges(
        KoreXYZPoint apexPoint, KoreXYZVector apexBaseVector, KoreXYZVector baseForwardVector,
        float width, float height,
        KoreColorRGB color)
    {
        KoreMeshData koreMeshData = new KoreMeshData();

        // Calculate base center point
        KoreXYZPoint baseCenterPoint = apexPoint + apexBaseVector;
        
        // Ensure baseForwardVector is perpendicular to apexBaseVector
        KoreXYZVector normalizedApexBase = apexBaseVector.Normalize();
        double dotProduct = KoreXYZVector.DotProduct(baseForwardVector, normalizedApexBase);
        KoreXYZVector projectedForward = baseForwardVector - (normalizedApexBase * dotProduct);
        KoreXYZVector normalizedForward = projectedForward.Normalize();
        
        // Create the right vector perpendicular to both apexBaseVector and baseForwardVector
        KoreXYZVector rightVector = KoreXYZVector.CrossProduct(normalizedForward, normalizedApexBase).Normalize();
        
        // Calculate the 4 base vertices for a square pyramid
        double halfWidth = width * 0.5;
        double halfHeight = height * 0.5;
        
        KoreXYZVector apexVector = new KoreXYZVector(apexPoint.X, apexPoint.Y, apexPoint.Z);
        KoreXYZVector baseCenterVector = new KoreXYZVector(baseCenterPoint.X, baseCenterPoint.Y, baseCenterPoint.Z);
        
        KoreXYZVector baseVertex1 = baseCenterVector + (normalizedForward * halfHeight) + (rightVector * halfWidth);   // front-right
        KoreXYZVector baseVertex2 = baseCenterVector + (normalizedForward * halfHeight) - (rightVector * halfWidth);   // front-left
        KoreXYZVector baseVertex3 = baseCenterVector - (normalizedForward * halfHeight) - (rightVector * halfWidth);   // back-left
        KoreXYZVector baseVertex4 = baseCenterVector - (normalizedForward * halfHeight) + (rightVector * halfWidth);   // back-right

        // Create all triangles using AddIsolatedTriangle for sharp edges with proper face normals
        float noiseFactor = 0.001f; // Adjust noise factor as needed

        // Front face triangle (apex, base1, base2)
        koreMeshData.AddIsolatedTriangle(apexVector, baseVertex1, baseVertex2, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
        
        // Right face triangle (apex, base4, base1)
        koreMeshData.AddIsolatedTriangle(apexVector, baseVertex4, baseVertex1, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
        
        // Back face triangle (apex, base3, base4)
        koreMeshData.AddIsolatedTriangle(apexVector, baseVertex3, baseVertex4, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
        
        // Left face triangle (apex, base2, base3)
        koreMeshData.AddIsolatedTriangle(apexVector, baseVertex2, baseVertex3, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
        
        // Base triangles (2 triangles to form a square base)
        koreMeshData.AddIsolatedTriangle(baseVertex1, baseVertex4, baseVertex3, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
        koreMeshData.AddIsolatedTriangle(baseVertex1, baseVertex3, baseVertex2, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);

        // Add wireframe lines using shared vertices to avoid duplicating line geometry
        int idxApex = koreMeshData.AddVertex(apexVector, null, color);
        int idxBase1 = koreMeshData.AddVertex(baseVertex1, null, color);
        int idxBase2 = koreMeshData.AddVertex(baseVertex2, null, color);
        int idxBase3 = koreMeshData.AddVertex(baseVertex3, null, color);
        int idxBase4 = koreMeshData.AddVertex(baseVertex4, null, color);
        
        // Edges from apex to each base vertex
        koreMeshData.AddLine(idxApex, idxBase1, color, color);
        koreMeshData.AddLine(idxApex, idxBase2, color, color);
        koreMeshData.AddLine(idxApex, idxBase3, color, color);
        koreMeshData.AddLine(idxApex, idxBase4, color, color);
        
        // Base perimeter edges
        koreMeshData.AddLine(idxBase1, idxBase2, color, color);
        koreMeshData.AddLine(idxBase2, idxBase3, color, color);
        koreMeshData.AddLine(idxBase3, idxBase4, color, color);
        koreMeshData.AddLine(idxBase4, idxBase1, color, color);

        return koreMeshData;
    }
    
    
    // // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0));
    // public static KoreMeshData BasicCube(float size, KoreColorRGB color)
    // {
    //     var mesh = new KoreMeshData();

    //     // Define the vertices of the cube
    //     int v0 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size), null, color);
    //     int v1 = mesh.AddVertex(new KoreXYZVector(size, -size, -size), null, color);
    //     int v2 = mesh.AddVertex(new KoreXYZVector(size, size, -size), null, color);
    //     int v3 = mesh.AddVertex(new KoreXYZVector(-size, size, -size), null, color);
    //     int v4 = mesh.AddVertex(new KoreXYZVector(-size, -size, size), null, color);
    //     int v5 = mesh.AddVertex(new KoreXYZVector(size, -size, size), null, color);
    //     int v6 = mesh.AddVertex(new KoreXYZVector(size, size, size), null, color);
    //     int v7 = mesh.AddVertex(new KoreXYZVector(-size, size, size), null, color);

    //     // Lines
    //     mesh.AddLine(v0, v1, color, color);
    //     mesh.AddLine(v1, v5, color, color);
    //     mesh.AddLine(v5, v4, color, color);
    //     mesh.AddLine(v4, v0, color, color);
    //     mesh.AddLine(v2, v3, color, color);
    //     mesh.AddLine(v3, v7, color, color);
    //     mesh.AddLine(v7, v6, color, color);
    //     mesh.AddLine(v6, v2, color, color);
    //     mesh.AddLine(v0, v3, color, color);
    //     mesh.AddLine(v1, v2, color, color);
    //     mesh.AddLine(v4, v7, color, color);
    //     mesh.AddLine(v5, v6, color, color);

    //     // Triangles
    //     mesh.AddTriangle(v0, v1, v2); mesh.AddTriangle(v0, v2, v3);
    //     mesh.AddTriangle(v0, v3, v4); mesh.AddTriangle(v3, v7, v4);
    //     mesh.AddTriangle(v4, v7, v6); mesh.AddTriangle(v4, v6, v5);
    //     mesh.AddTriangle(v5, v6, v2); mesh.AddTriangle(v5, v2, v1);
    //     mesh.AddTriangle(v2, v7, v3); mesh.AddTriangle(v2, v6, v7); // top
    //     mesh.AddTriangle(v0, v5, v1); mesh.AddTriangle(v0, v4, v5); // bottom

    //     mesh.MakeValid();
    //     return mesh;
    // }

    // // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCubeSharpEdges(1.0f, new KoreColorRGB(255, 0, 0));
    // public static KoreMeshData BasicCubeSharpEdges(float size, KoreColorRGB color)
    // {
    //     var mesh = new KoreMeshData();

    //     // Create 24 vertices (4 per face) with proper face normals for sharp edges
    //     // Each face gets its own 4 vertices with the correct normal

    //     // Front face (normal: 0, 0, -1)
    //     var frontNormal = new KoreXYZVector(0, 0, -1);
    //     int f0 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size), frontNormal, color);
    //     int f1 = mesh.AddVertex(new KoreXYZVector(size, -size, -size), frontNormal, color);
    //     int f2 = mesh.AddVertex(new KoreXYZVector(size, size, -size), frontNormal, color);
    //     int f3 = mesh.AddVertex(new KoreXYZVector(-size, size, -size), frontNormal, color);

    //     // Back face (normal: 0, 0, 1)
    //     var backNormal = new KoreXYZVector(0, 0, 1);
    //     int b0 = mesh.AddVertex(new KoreXYZVector(size, -size, size), backNormal, color);
    //     int b1 = mesh.AddVertex(new KoreXYZVector(-size, -size, size), backNormal, color);
    //     int b2 = mesh.AddVertex(new KoreXYZVector(-size, size, size), backNormal, color);
    //     int b3 = mesh.AddVertex(new KoreXYZVector(size, size, size), backNormal, color);

    //     // Left face (normal: -1, 0, 0)
    //     var leftNormal = new KoreXYZVector(-1, 0, 0);
    //     int l0 = mesh.AddVertex(new KoreXYZVector(-size, -size, size), leftNormal, color);
    //     int l1 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size), leftNormal, color);
    //     int l2 = mesh.AddVertex(new KoreXYZVector(-size, size, -size), leftNormal, color);
    //     int l3 = mesh.AddVertex(new KoreXYZVector(-size, size, size), leftNormal, color);

    //     // Right face (normal: 1, 0, 0)
    //     var rightNormal = new KoreXYZVector(1, 0, 0);
    //     int r0 = mesh.AddVertex(new KoreXYZVector(size, -size, -size), rightNormal, color);
    //     int r1 = mesh.AddVertex(new KoreXYZVector(size, -size, size), rightNormal, color);
    //     int r2 = mesh.AddVertex(new KoreXYZVector(size, size, size), rightNormal, color);
    //     int r3 = mesh.AddVertex(new KoreXYZVector(size, size, -size), rightNormal, color);

    //     // Top face (normal: 0, 1, 0)
    //     var topNormal = new KoreXYZVector(0, 1, 0);
    //     int t0 = mesh.AddVertex(new KoreXYZVector(-size, size, -size), topNormal, color);
    //     int t1 = mesh.AddVertex(new KoreXYZVector(size, size, -size), topNormal, color);
    //     int t2 = mesh.AddVertex(new KoreXYZVector(size, size, size), topNormal, color);
    //     int t3 = mesh.AddVertex(new KoreXYZVector(-size, size, size), topNormal, color);

    //     // Bottom face (normal: 0, -1, 0)
    //     var bottomNormal = new KoreXYZVector(0, -1, 0);
    //     int bot0 = mesh.AddVertex(new KoreXYZVector(-size, -size, size), bottomNormal, color);
    //     int bot1 = mesh.AddVertex(new KoreXYZVector(size, -size, size), bottomNormal, color);
    //     int bot2 = mesh.AddVertex(new KoreXYZVector(size, -size, -size), bottomNormal, color);
    //     int bot3 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size), bottomNormal, color);

    //     // Add triangles for each face (2 triangles per face)
    //     // Front face
    //     mesh.AddTriangle(f0, f1, f2, color);
    //     mesh.AddTriangle(f0, f2, f3, color);

    //     // Back face
    //     mesh.AddTriangle(b0, b1, b2, color);
    //     mesh.AddTriangle(b0, b2, b3, color);

    //     // Left face
    //     mesh.AddTriangle(l0, l1, l2, color);
    //     mesh.AddTriangle(l0, l2, l3, color);

    //     // Right face
    //     mesh.AddTriangle(r0, r1, r2, color);
    //     mesh.AddTriangle(r0, r2, r3, color);

    //     // Top face
    //     mesh.AddTriangle(t0, t1, t2, color);
    //     mesh.AddTriangle(t0, t2, t3, color);

    //     // Bottom face
    //     mesh.AddTriangle(bot0, bot1, bot2, color);
    //     mesh.AddTriangle(bot0, bot2, bot3, color);

    //     // Add edge lines for wireframe (using separate vertices to avoid interfering with face normals)
    //     int v0 = mesh.AddVertex(new KoreXYZVector(-size, -size, -size), null, color);
    //     int v1 = mesh.AddVertex(new KoreXYZVector(size, -size, -size), null, color);
    //     int v2 = mesh.AddVertex(new KoreXYZVector(size, size, -size), null, color);
    //     int v3 = mesh.AddVertex(new KoreXYZVector(-size, size, -size), null, color);
    //     int v4 = mesh.AddVertex(new KoreXYZVector(-size, -size, size), null, color);
    //     int v5 = mesh.AddVertex(new KoreXYZVector(size, -size, size), null, color);
    //     int v6 = mesh.AddVertex(new KoreXYZVector(size, size, size), null, color);
    //     int v7 = mesh.AddVertex(new KoreXYZVector(-size, size, size), null, color);

    //     // Lines
    //     mesh.AddLine(v0, v1, color, color);
    //     mesh.AddLine(v1, v5, color, color);
    //     mesh.AddLine(v5, v4, color, color);
    //     mesh.AddLine(v4, v0, color, color);
    //     mesh.AddLine(v2, v3, color, color);
    //     mesh.AddLine(v3, v7, color, color);
    //     mesh.AddLine(v7, v6, color, color);
    //     mesh.AddLine(v6, v2, color, color);
    //     mesh.AddLine(v0, v3, color, color);
    //     mesh.AddLine(v1, v2, color, color);
    //     mesh.AddLine(v4, v7, color, color);
    //     mesh.AddLine(v5, v6, color, color);

    //     // Don't call MakeValid() as it would overwrite our carefully set normals
    //     return mesh;
    // }

    // // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCubeSharpEdges2(1.0f, new KoreColorRGB(255, 0, 0));
    // public static KoreMeshData BasicCubeSharpEdges2(float size, KoreColorRGB color)
    // {
    //     var mesh = new KoreMeshData();

    //     // Define the 8 corner vertices manually outside of the mesh
    //     var v0 = new KoreXYZVector(-size, -size, -size); // front bottom left
    //     var v1 = new KoreXYZVector(size, -size, -size);  // front bottom right
    //     var v2 = new KoreXYZVector(size, size, -size);   // front top right
    //     var v3 = new KoreXYZVector(-size, size, -size);  // front top left
    //     var v4 = new KoreXYZVector(-size, -size, size);  // back bottom left
    //     var v5 = new KoreXYZVector(size, -size, size);   // back bottom right
    //     var v6 = new KoreXYZVector(size, size, size);    // back top right
    //     var v7 = new KoreXYZVector(-size, size, size);   // back top left

    //     // Create all 12 triangles (2 per face) using AddIsolatedTriangle
    //     // Using the EXACT same triangle winding as the working BasicCube function
    //     // Each triangle automatically calculates and assigns the correct face normal

    //     float noiseFactor = 0.8f; // Adjust noise factor as needed

    //     // Copy the triangulation from BasicCube (which works perfectly)
    //     mesh.AddIsolatedTriangle(v0, v1, v2, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); mesh.AddIsolatedTriangle(v0, v2, v3, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
    //     mesh.AddIsolatedTriangle(v0, v3, v4, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); mesh.AddIsolatedTriangle(v3, v7, v4, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
    //     mesh.AddIsolatedTriangle(v4, v7, v6, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); mesh.AddIsolatedTriangle(v4, v6, v5, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
    //     mesh.AddIsolatedTriangle(v5, v6, v2, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); mesh.AddIsolatedTriangle(v5, v2, v1, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color);
    //     mesh.AddIsolatedTriangle(v2, v7, v3, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); mesh.AddIsolatedTriangle(v2, v6, v7, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); // top
    //     mesh.AddIsolatedTriangle(v0, v5, v1, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); mesh.AddIsolatedTriangle(v0, v4, v5, KoreColorOps.ColorWithRGBNoise(color, noiseFactor), color); // bottom

    //     // Add wireframe lines using the same 8 corner vertices
    //     int lv0 = mesh.AddVertex(v0, null, color);
    //     int lv1 = mesh.AddVertex(v1, null, color);
    //     int lv2 = mesh.AddVertex(v2, null, color);
    //     int lv3 = mesh.AddVertex(v3, null, color);
    //     int lv4 = mesh.AddVertex(v4, null, color);
    //     int lv5 = mesh.AddVertex(v5, null, color);
    //     int lv6 = mesh.AddVertex(v6, null, color);
    //     int lv7 = mesh.AddVertex(v7, null, color);

    //     // 12 edges of the cube
    //     mesh.AddLine(lv0, lv1, color, color); // front bottom
    //     mesh.AddLine(lv1, lv2, color, color); // front right
    //     mesh.AddLine(lv2, lv3, color, color); // front top
    //     mesh.AddLine(lv3, lv0, color, color); // front left
    //     mesh.AddLine(lv4, lv5, color, color); // back bottom
    //     mesh.AddLine(lv5, lv6, color, color); // back right
    //     mesh.AddLine(lv6, lv7, color, color); // back top
    //     mesh.AddLine(lv7, lv4, color, color); // back left
    //     mesh.AddLine(lv0, lv4, color, color); // left bottom
    //     mesh.AddLine(lv1, lv5, color, color); // right bottom
    //     mesh.AddLine(lv2, lv6, color, color); // right top
    //     mesh.AddLine(lv3, lv7, color, color); // left top

    //     return mesh;
    // }

    // // ---------------------------------------------------------------------------------------------

    // public static KoreMeshData SizedBox(
    //     double sizeUp, double sizeDown,
    //     double sizeLeft, double sizeRight,
    //     double sizeFront, double sizeBack,
    //     KoreColorRGB color)
    // {
    //     // Create a new KoreMeshData object
    //     var mesh = new KoreMeshData();

    //     // Define 8 unique vertices for the rectangular box
    //     // Front face vertices:
    //     int v0 = mesh.AddVertex(new KoreXYZVector(-sizeLeft, -sizeDown, -sizeFront), null, color); // Lower left front
    //     int v1 = mesh.AddVertex(new KoreXYZVector(sizeRight, -sizeDown, -sizeFront), null, color); // Lower right front
    //     int v2 = mesh.AddVertex(new KoreXYZVector(sizeRight, sizeUp, -sizeFront), null, color); // Upper right front
    //     int v3 = mesh.AddVertex(new KoreXYZVector(-sizeLeft, sizeUp, -sizeFront), null, color); // Upper left front

    //     // Back face vertices:
    //     int v4 = mesh.AddVertex(new KoreXYZVector(-sizeLeft, -sizeDown, sizeBack), null, color); // Lower left back
    //     int v5 = mesh.AddVertex(new KoreXYZVector(sizeRight, -sizeDown, sizeBack), null, color); // Lower right back
    //     int v6 = mesh.AddVertex(new KoreXYZVector(sizeRight, sizeUp, sizeBack), null, color); // Upper right back
    //     int v7 = mesh.AddVertex(new KoreXYZVector(-sizeLeft, sizeUp, sizeBack), null, color); // Upper left back

    //     // Define edges (lines)
    //     // Lines
    //     mesh.AddLine(v0, v1, color, color);
    //     mesh.AddLine(v1, v5, color, color);
    //     mesh.AddLine(v5, v4, color, color);
    //     mesh.AddLine(v4, v0, color, color);
    //     mesh.AddLine(v2, v3, color, color);
    //     mesh.AddLine(v3, v7, color, color);
    //     mesh.AddLine(v7, v6, color, color);
    //     mesh.AddLine(v6, v2, color, color);
    //     mesh.AddLine(v0, v3, color, color);
    //     mesh.AddLine(v1, v2, color, color);
    //     mesh.AddLine(v4, v7, color, color);
    //     mesh.AddLine(v5, v6, color, color);

    //     // Triangles
    //     mesh.AddTriangle(v0, v1, v2); mesh.AddTriangle(v0, v2, v3);
    //     mesh.AddTriangle(v4, v5, v6); mesh.AddTriangle(v4, v6, v7);
    //     mesh.AddTriangle(v0, v1, v5); mesh.AddTriangle(v0, v5, v4);
    //     mesh.AddTriangle(v1, v2, v6); mesh.AddTriangle(v1, v6, v5);
    //     mesh.AddTriangle(v2, v3, v7); mesh.AddTriangle(v2, v7, v6);
    //     mesh.AddTriangle(v3, v0, v4); mesh.AddTriangle(v3, v4, v7);

    //     return mesh;
    // }

    // // ---------------------------------------------------------------------------------------------

    // public static KoreMeshData SizedBox(
    //     KoreXYZBox box,
    //     KoreColorRGB? linecolor = null)
    // {
    //     return SizedBox(
    //         box.OffsetUp, box.OffsetDown,
    //         box.OffsetLeft, box.OffsetRight,
    //         box.OffsetForwards, box.OffsetBackwards,
    //         linecolor ?? KoreColorRGB.White);
    // }

    // // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCubeIsolatedTriangles(1.0f, new KoreColorRGB(255, 0, 0));
    // public static KoreMeshData BasicCubeIsolatedTriangles(float size, KoreColorRGB color)
    // {
    //     var mesh = new KoreMeshData();

    //     // Define the 8 corner vertices for reference
    //     var v0 = new KoreXYZVector(-size, -size, -size); // front bottom left
    //     var v1 = new KoreXYZVector(size, -size, -size);  // front bottom right
    //     var v2 = new KoreXYZVector(size, size, -size);   // front top right
    //     var v3 = new KoreXYZVector(-size, size, -size);  // front top left
    //     var v4 = new KoreXYZVector(-size, -size, size);  // back bottom left
    //     var v5 = new KoreXYZVector(size, -size, size);   // back bottom right
    //     var v6 = new KoreXYZVector(size, size, size);    // back top right
    //     var v7 = new KoreXYZVector(-size, size, size);   // back top left

    //     // Create all 12 triangles (2 per face) using AddIsolatedTriangle for sharp edges
    //     // Each triangle will automatically get the correct face normal

    //     // Front face (2 triangles)
    //     mesh.AddIsolatedTriangle(v0, v1, v2, color, color);
    //     mesh.AddIsolatedTriangle(v0, v2, v3, color, color);

    //     // Back face (2 triangles)
    //     mesh.AddIsolatedTriangle(v5, v4, v7, color, color);
    //     mesh.AddIsolatedTriangle(v5, v7, v6, color, color);

    //     // Left face (2 triangles)
    //     mesh.AddIsolatedTriangle(v4, v0, v3, color, color);
    //     mesh.AddIsolatedTriangle(v4, v3, v7, color, color);

    //     // Right face (2 triangles)
    //     mesh.AddIsolatedTriangle(v1, v5, v6, color, color);
    //     mesh.AddIsolatedTriangle(v1, v6, v2, color, color);

    //     // Top face (2 triangles)
    //     mesh.AddIsolatedTriangle(v3, v2, v6, color, color);
    //     mesh.AddIsolatedTriangle(v3, v6, v7, color, color);

    //     // Bottom face (2 triangles)
    //     mesh.AddIsolatedTriangle(v4, v5, v1, color, color);
    //     mesh.AddIsolatedTriangle(v4, v1, v0, color, color);

    //     // Add wireframe lines using shared vertices to avoid duplicating line geometry
    //     int lv0 = mesh.AddVertex(v0, null, color);
    //     int lv1 = mesh.AddVertex(v1, null, color);
    //     int lv2 = mesh.AddVertex(v2, null, color);
    //     int lv3 = mesh.AddVertex(v3, null, color);
    //     int lv4 = mesh.AddVertex(v4, null, color);
    //     int lv5 = mesh.AddVertex(v5, null, color);
    //     int lv6 = mesh.AddVertex(v6, null, color);
    //     int lv7 = mesh.AddVertex(v7, null, color);

    //     mesh.AddLine(lv0, lv1, color, color);
    //     mesh.AddLine(lv1, lv5, color, color);
    //     mesh.AddLine(lv5, lv4, color, color);
    //     mesh.AddLine(lv4, lv0, color, color);
    //     mesh.AddLine(lv2, lv3, color, color);
    //     mesh.AddLine(lv3, lv7, color, color);
    //     mesh.AddLine(lv7, lv6, color, color);
    //     mesh.AddLine(lv6, lv2, color, color);
    //     mesh.AddLine(lv0, lv3, color, color);
    //     mesh.AddLine(lv1, lv2, color, color);
    //     mesh.AddLine(lv4, lv7, color, color);
    //     mesh.AddLine(lv5, lv6, color, color);

    //     return mesh;
    // }

}
