using System;
using System.Collections.Generic;
using KoreCommon;
using KoreGIS;

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
                qf.TopLeft = new KoreXYZVector(-1, 1, -1);    // Front-left
                qf.TopRight = new KoreXYZVector(1, 1, -1);    // Front-right
                qf.BottomLeft = new KoreXYZVector(-1, 1, 1);  // Back-left
                qf.BottomRight = new KoreXYZVector(1, 1, 1);  // Back-right
                break;

            case KoreQuadFace.CubeFace.Bottom:
                // Looking up from below (Y=-1), front is -Z, back is +Z
                qf.TopLeft = new KoreXYZVector(-1, -1, 1);    // Back-left
                qf.TopRight = new KoreXYZVector(1, -1, 1);    // Back-right
                qf.BottomLeft = new KoreXYZVector(-1, -1, -1); // Front-left
                qf.BottomRight = new KoreXYZVector(1, -1, -1); // Front-right
                break;

            case KoreQuadFace.CubeFace.Left:
                // Looking at left face (X=-1) from outside
                qf.TopLeft = new KoreXYZVector(-1, 1, -1);
                qf.TopRight = new KoreXYZVector(-1, 1, 1);
                qf.BottomLeft = new KoreXYZVector(-1, -1, -1);
                qf.BottomRight = new KoreXYZVector(-1, -1, 1);
                break;

            case KoreQuadFace.CubeFace.Right:
                // Looking at right face (X=1) from outside
                qf.TopLeft = new KoreXYZVector(1, 1, 1);
                qf.TopRight = new KoreXYZVector(1, 1, -1);
                qf.BottomLeft = new KoreXYZVector(1, -1, 1);
                qf.BottomRight = new KoreXYZVector(1, -1, -1);
                break;

            case KoreQuadFace.CubeFace.Front:
                // Looking at front face (Z=1) from outside. Godot has -ve Z forward/away
                qf.TopLeft = new KoreXYZVector(-1, 1, 1);
                qf.TopRight = new KoreXYZVector(1, 1, 1);
                qf.BottomLeft = new KoreXYZVector(-1, -1, 1);
                qf.BottomRight = new KoreXYZVector(1, -1, 1);
                break;

            case KoreQuadFace.CubeFace.Back:
                // Looking at back face (Z=-1) from outside
                qf.TopLeft = new KoreXYZVector(1, 1, -1);
                qf.TopRight = new KoreXYZVector(-1, 1, -1);
                qf.BottomLeft = new KoreXYZVector(1, -1, -1);
                qf.BottomRight = new KoreXYZVector(-1, -1, -1);
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
                qf.TopLeft = face.TopLeft;
                qf.TopRight = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    face.TopLeft.Y,
                    (face.TopLeft.Z + face.TopRight.Z) / 2);
                qf.BottomLeft = new KoreXYZVector(
                    face.TopLeft.X,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopLeft.Z + face.BottomLeft.Z) / 2);
                qf.BottomRight = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopLeft.Z + face.BottomLeft.Z) / 2);
                break;

            case 1: // Top-right quadrant
                qf.TopLeft = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    face.TopLeft.Y,
                    (face.TopLeft.Z + face.TopRight.Z) / 2);
                qf.TopRight = face.TopRight;
                qf.BottomLeft = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopLeft.Z + face.BottomLeft.Z) / 2);
                qf.BottomRight = new KoreXYZVector(
                    face.TopRight.X,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopRight.Z + face.BottomRight.Z) / 2);
                break;

            case 2: // Bottom-left quadrant
                qf.TopLeft = new KoreXYZVector(
                    face.TopLeft.X,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopLeft.Z + face.BottomLeft.Z) / 2);
                qf.TopRight = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopLeft.Z + face.BottomLeft.Z) / 2);
                qf.BottomLeft = face.BottomLeft;
                qf.BottomRight = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    face.BottomLeft.Y,
                    (face.BottomLeft.Z + face.BottomRight.Z) / 2);
                break;

            case 3: // Bottom-right quadrant
                qf.TopLeft = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopLeft.Z + face.BottomLeft.Z) / 2);
                qf.TopRight = new KoreXYZVector(
                    face.TopRight.X,
                    (face.TopLeft.Y + face.BottomLeft.Y) / 2,
                    (face.TopRight.Z + face.BottomRight.Z) / 2);
                qf.BottomLeft = new KoreXYZVector(
                    (face.TopLeft.X + face.TopRight.X) / 2,
                    face.BottomLeft.Y,
                    (face.BottomLeft.Z + face.BottomRight.Z) / 2);
                qf.BottomRight = face.BottomRight;
                break;
        }

        return qf;
    }

    // --------------------------------------------------------------------------------------------

    public static KoreXYZVector AverageXYZ(KoreQuadFace face)
    {
        return new KoreXYZVector(
            (face.TopLeft.X + face.TopRight.X + face.BottomLeft.X + face.BottomRight.X) / 4,
            (face.TopLeft.Y + face.TopRight.Y + face.BottomLeft.Y + face.BottomRight.Y) / 4,
            (face.TopLeft.Z + face.TopRight.Z + face.BottomLeft.Z + face.BottomRight.Z) / 4
        );
    }

    // Usage: KoreLLPoint centerLL = KoreQuadFaceOps.AverageLL(face);
    public static KoreLLPoint AverageLL(KoreQuadFace face)
    {
        return KoreLLPoint.FromXYZ(AverageXYZ(face));
    }

    // --------------------------------------------------------------------------------------------

    // Given a tile code, return the KoreQuadFace that represents the area on the cube face
    // represented by the tile code.
    // The tile divisions are done by angle, to minimize distortion.

    // Usage: KoreQuadFace tileFace = KoreQuadFaceOps.QuadrantOnFace(tileCode);

    public static KoreQuadFace QuadrantOnFace(KoreQuadCubeTileCode tileCode)
    {
        // Protect ourselves from the maths going wrong with too many levels
        int numTileLevels = tileCode.Quadrants.Count;
        if (numTileLevels > 10) return KoreQuadFace.Zero;

        // Angular Division
        // We know that at the top level (level 0), each face spans 90 degrees,
        // and each subsequent level halves the angle span of each tile.
        int numDivisions = (int)Math.Pow(2, numTileLevels); // 2^n divisions for n levels: 0=1, 1=2, 2=4, 3=8, 4=16
        double angleSpanDegs = 90.0 / numDivisions; // Each tile spans this many degrees

        // Determine the angular position on the face based on the specific quadrants chosen at each level.
        // We calculate the angular offset for both U and V axes separately.

        List<int> axisOffsets = new(); // Angular offsets for each level
        for (int i = 0; i < numTileLevels; i++)
        {
            // For each level, the offset is half of the previous level's offset
            int numOffsetsThisLevel = numDivisions / (int)Math.Pow(2, i + 1);
            axisOffsets.Add(numOffsetsThisLevel);
        }

        // Loop through each quadrant in the tile code to determine angular position
        int uAngularPosition = 0;
        int vAngularPosition = 0;
        for (int i = 0; i < tileCode.Quadrants.Count; i++)
        {
            // Adjust angular positions based on the current quadrant and axis offsets
            // Quadrant layout reminder:
            // 0 | 1
            // -----
            // 2 | 3
            int currQuadrant = tileCode.Quadrants[i];

            // X (U) axis - angular position
            if (currQuadrant == 1 || currQuadrant == 3)
            {
                uAngularPosition += axisOffsets[i];
            }

            // Y (V) axis - angular position
            if (currQuadrant == 2 || currQuadrant == 3)
            {
                vAngularPosition += axisOffsets[i];
            }
        }

        // Convert angular positions to angles (in radians, spanning -45° to +45° for each axis)
        double uAngle0 = (uAngularPosition * angleSpanDegs - 45.0) * Math.PI / 180.0;
        double vAngle0 = (vAngularPosition * angleSpanDegs - 45.0) * Math.PI / 180.0;
        double uAngle1 = ((uAngularPosition + 1) * angleSpanDegs - 45.0) * Math.PI / 180.0;
        double vAngle1 = ((vAngularPosition + 1) * angleSpanDegs - 45.0) * Math.PI / 180.0;

        // Convert angles to cube face coordinates using trigonometry (-1 to +1 range)
        double u0Fraction = (Math.Tan(uAngle0) + 1.0) / 2.0; // Convert from -1..+1 to 0..1
        double v0Fraction = (Math.Tan(vAngle0) + 1.0) / 2.0;
        double u1Fraction = (Math.Tan(uAngle1) + 1.0) / 2.0;
        double v1Fraction = (Math.Tan(vAngle1) + 1.0) / 2.0;


        // Create the face, so the overall XYZ orientations are known
        KoreQuadFace startingFace = FaceForCubeFace(tileCode.Face);

        // Lerp the corners of the face to get the position of the top-left corner of the tile
        KoreXYZVector newTopLeft = PositionOnFace(startingFace, u0Fraction, v0Fraction);
        KoreXYZVector newTopRight = PositionOnFace(startingFace, u1Fraction, v0Fraction);
        KoreXYZVector newBottomLeft = PositionOnFace(startingFace, u0Fraction, v1Fraction);
        KoreXYZVector newBottomRight = PositionOnFace(startingFace, u1Fraction, v1Fraction);

        KoreQuadFace newFace = new()
        {
            TopLeft = newTopLeft,
            TopRight = newTopRight,
            BottomLeft = newBottomLeft,
            BottomRight = newBottomRight
        };
        return newFace;
    }




    // bilinear interpolation on the quad face
    // uFraction position from left (0.0) to right (1.0)
    // vFraction position from top (0.0) to bottom (1.0)
    public static KoreXYZVector PositionOnFace(KoreQuadFace face, double uFraction, double vFraction)
    {
        // Bilinear interpolation on the quad face
        // First interpolate along the top edge (v=0)
        KoreXYZVector topInterp = new KoreXYZVector(
            face.TopLeft.X + uFraction * (face.TopRight.X - face.TopLeft.X),
            face.TopLeft.Y + uFraction * (face.TopRight.Y - face.TopLeft.Y),
            face.TopLeft.Z + uFraction * (face.TopRight.Z - face.TopLeft.Z)
        );

        // Then interpolate along the bottom edge (v=1)
        KoreXYZVector bottomInterp = new KoreXYZVector(
            face.BottomLeft.X + uFraction * (face.BottomRight.X - face.BottomLeft.X),
            face.BottomLeft.Y + uFraction * (face.BottomRight.Y - face.BottomLeft.Y),
            face.BottomLeft.Z + uFraction * (face.BottomRight.Z - face.BottomLeft.Z)
        );

        // Finally interpolate between top and bottom edges using vFraction
        return new KoreXYZVector(
            topInterp.X + vFraction * (bottomInterp.X - topInterp.X),
            topInterp.Y + vFraction * (bottomInterp.Y - topInterp.Y),
            topInterp.Z + vFraction * (bottomInterp.Z - topInterp.Z)
        );
    }

    // Angular interpolation on the quad face to match QuadrantOnFace's angular subdivision
    // uFraction position from left (0.0) to right (1.0)
    // vFraction position from top (0.0) to bottom (1.0)
    public static KoreXYZVector PositionOnFace2(KoreQuadFace face, double uFraction, double vFraction)
    {
        // Convert fractions to angles (-45° to +45° range for each axis)
        double uAngle = Math.Atan2(2.0 * uFraction - 1.0, 1.0); // Convert 0..1 fraction to angle
        double vAngle = Math.Atan2(2.0 * vFraction - 1.0, 1.0);

        // Convert angles back to cube coordinates (-1 to +1)
        double uCoord = Math.Tan(uAngle);
        double vCoord = Math.Tan(vAngle);

        // Normalize back to 0..1 range for bilinear interpolation
        double uNorm = (uCoord + 1.0) / 2.0;
        double vNorm = (vCoord + 1.0) / 2.0;

        // Clamp to valid range
        uNorm = Math.Max(0.0, Math.Min(1.0, uNorm));
        vNorm = Math.Max(0.0, Math.Min(1.0, vNorm));

        // Now do standard bilinear interpolation with the corrected coordinates
        // First interpolate along the top edge (v=0)
        KoreXYZVector topInterp = new KoreXYZVector(
            face.TopLeft.X + uNorm * (face.TopRight.X - face.TopLeft.X),
            face.TopLeft.Y + uNorm * (face.TopRight.Y - face.TopLeft.Y),
            face.TopLeft.Z + uNorm * (face.TopRight.Z - face.TopLeft.Z)
        );

        // Then interpolate along the bottom edge (v=1)
        KoreXYZVector bottomInterp = new KoreXYZVector(
            face.BottomLeft.X + uNorm * (face.BottomRight.X - face.BottomLeft.X),
            face.BottomLeft.Y + uNorm * (face.BottomRight.Y - face.BottomLeft.Y),
            face.BottomLeft.Z + uNorm * (face.BottomRight.Z - face.BottomLeft.Z)
        );

        // Finally interpolate between top and bottom edges using vNorm
        return new KoreXYZVector(
            topInterp.X + vNorm * (bottomInterp.X - topInterp.X),
            topInterp.Y + vNorm * (bottomInterp.Y - topInterp.Y),
            topInterp.Z + vNorm * (bottomInterp.Z - topInterp.Z)
        );
    }

}





