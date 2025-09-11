

using System.Collections.Generic;
using KoreCommon;

namespace KoreSim;


public static class KoreQuadFaceOps
{
    // We need to remember here that we are "godot native" in our maths, so -ve Z is forward. The front face is +ve Z and the back face is -ve Z forward/away
    // All corners are set as observed from outside the cube.
    public static KoreQuadFace FaceForCubeFace(KoreQuadFace.CubeFace face)
    {
        KoreQuadFace qf = new();

        switch (face)
        {
            case KoreQuadFace.CubeFace.Top:
                // Looking down from above (Y=1), front is -Z, back is +Z
                qf.topLeft = new KoreXYZVector(-1, 1, -1);    // Front-left
                qf.topRight = new KoreXYZVector(1, 1, -1);    // Front-right
                qf.bottomLeft = new KoreXYZVector(-1, 1, 1);  // Back-left
                qf.bottomRight = new KoreXYZVector(1, 1, 1);  // Back-right
                break;

            case KoreQuadFace.CubeFace.Bottom:
                // Looking up from below (Y=-1), front is -Z, back is +Z
                qf.topLeft = new KoreXYZVector(-1, -1, 1);    // Back-left
                qf.topRight = new KoreXYZVector(1, -1, 1);    // Back-right
                qf.bottomLeft = new KoreXYZVector(-1, -1, -1); // Front-left
                qf.bottomRight = new KoreXYZVector(1, -1, -1); // Front-right
                break;

            case KoreQuadFace.CubeFace.Left:
                // Looking at left face (X=-1) from outside
                qf.topLeft = new KoreXYZVector(-1, 1, -1);
                qf.topRight = new KoreXYZVector(-1, 1, 1);
                qf.bottomLeft = new KoreXYZVector(-1, -1, -1);
                qf.bottomRight = new KoreXYZVector(-1, -1, 1);
                break;

            case KoreQuadFace.CubeFace.Right:
                // Looking at right face (X=1) from outside
                qf.topLeft = new KoreXYZVector(1, 1, 1);
                qf.topRight = new KoreXYZVector(1, 1, -1);
                qf.bottomLeft = new KoreXYZVector(1, -1, 1);
                qf.bottomRight = new KoreXYZVector(1, -1, -1);
                break;

            case KoreQuadFace.CubeFace.Front:
                // Looking at front face (Z=-1) from outside
                qf.topLeft = new KoreXYZVector(-1, 1, 1);
                qf.topRight = new KoreXYZVector(1, 1, 1);
                qf.bottomLeft = new KoreXYZVector(-1, -1, 1);
                qf.bottomRight = new KoreXYZVector(1, -1, 1);
                break;

            case KoreQuadFace.CubeFace.Back:
                // Looking at back face (Z=1) from outside
                qf.topLeft = new KoreXYZVector(1, 1, -1);
                qf.topRight = new KoreXYZVector(-1, 1, -1);
                qf.bottomLeft = new KoreXYZVector(1, -1, -1);
                qf.bottomRight = new KoreXYZVector(-1, -1, -1);
                break;
        }
        return qf;
    }

    // 0 | 1
    // -----
    // 2 | 3
    // Return the quadrant of a face, as seen from outside the cube

    public static KoreQuadFace QuadrantForFace(KoreQuadFace face, int quadrant)
    {
        KoreQuadFace qf = new();

        switch (quadrant)
        {
            case 0: // Top-left quadrant
                qf.topLeft = face.topLeft;
                qf.topRight = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    face.topLeft.Y,
                    (face.topLeft.Z + face.topRight.Z) / 2);
                qf.bottomLeft = new KoreXYZVector(
                    face.topLeft.X,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topLeft.Z + face.bottomLeft.Z) / 2);
                qf.bottomRight = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topLeft.Z + face.bottomLeft.Z) / 2);
                break;

            case 1: // Top-right quadrant
                qf.topLeft = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    face.topLeft.Y,  
                    (face.topLeft.Z + face.topRight.Z) / 2);
                qf.topRight = face.topRight;
                qf.bottomLeft = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topLeft.Z + face.bottomLeft.Z) / 2);
                qf.bottomRight = new KoreXYZVector(
                    face.topRight.X,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topRight.Z + face.bottomRight.Z) / 2);
                break;

            case 2: // Bottom-left quadrant
                qf.topLeft = new KoreXYZVector(
                    face.topLeft.X,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topLeft.Z + face.bottomLeft.Z) / 2);
                qf.topRight = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topLeft.Z + face.bottomLeft.Z) / 2);
                qf.bottomLeft = face.bottomLeft;
                qf.bottomRight = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    face.bottomLeft.Y,
                    (face.bottomLeft.Z + face.bottomRight.Z) / 2);
                break;

            case 3: // Bottom-right quadrant
                qf.topLeft = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topLeft.Z + face.bottomLeft.Z) / 2);
                qf.topRight = new KoreXYZVector(
                    face.topRight.X,
                    (face.topLeft.Y + face.bottomLeft.Y) / 2,
                    (face.topRight.Z + face.bottomRight.Z) / 2);
                qf.bottomLeft = new KoreXYZVector(
                    (face.topLeft.X + face.topRight.X) / 2,
                    face.bottomLeft.Y,
                    (face.bottomLeft.Z + face.bottomRight.Z) / 2);
                qf.bottomRight = face.bottomRight;
                break;
        }

        return qf;
    }

    // bilinear interpolation on the quad face
    // uFraction position from left (0.0) to right (1.0)
    // vFraction position from top (0.0) to bottom (1.0)
    public static KoreXYZVector PositionOnFace(KoreQuadFace face, double uFraction, double vFraction)
    {
        // Bilinear interpolation on the quad face
        // First interpolate along the top edge (v=0)
        KoreXYZVector topInterp = new KoreXYZVector(
            face.topLeft.X + uFraction * (face.topRight.X - face.topLeft.X),
            face.topLeft.Y + uFraction * (face.topRight.Y - face.topLeft.Y),
            face.topLeft.Z + uFraction * (face.topRight.Z - face.topLeft.Z)
        );

        // Then interpolate along the bottom edge (v=1)
        KoreXYZVector bottomInterp = new KoreXYZVector(
            face.bottomLeft.X + uFraction * (face.bottomRight.X - face.bottomLeft.X),
            face.bottomLeft.Y + uFraction * (face.bottomRight.Y - face.bottomLeft.Y),
            face.bottomLeft.Z + uFraction * (face.bottomRight.Z - face.bottomLeft.Z)
        );

        // Finally interpolate between top and bottom edges using vFraction
        return new KoreXYZVector(
            topInterp.X + vFraction * (bottomInterp.X - topInterp.X),
            topInterp.Y + vFraction * (bottomInterp.Y - topInterp.Y),
            topInterp.Z + vFraction * (bottomInterp.Z - topInterp.Z)
        );
    }
    
    
}




