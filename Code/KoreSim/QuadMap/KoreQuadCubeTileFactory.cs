
using System.Collections.Generic;

using KoreCommon;

namespace KoreSim;

// Define the tile for a quad cube map tile

public static class KoreQuadCubeTileFactory
{
    // Usage: KoreQuadCubeTile tile = KoreQuadCubeTileFactory.TileForCode(tileCode, radius: 10);
    public static KoreQuadCubeTile TileForCode(KoreQuadCubeTileCode code, double radius = 10)
    {
        KoreQuadCubeTile tile = new();
        tile.Code = code;

        // Go through the code and find the XYZ face on the cube
        KoreQuadFace face = KoreQuadFaceOps.FaceForCubeFace(code.Face);
        foreach (int quadrant in code.Quadrants)
            face = KoreQuadFaceOps.QuadrantForFace(face, quadrant);


        KoreQuadFace face2 = KoreQuadFaceOps.QuadrantOnFace(code);

        int numU = 10; // Setup the number of points (not triangles, points) across and down
        int numV = 10;
        KoreNumeric1DArray<double> uArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numU);
        KoreNumeric1DArray<double> vArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numV);

        // [y,x] list of points
        KoreXYZVector[,] points = new KoreXYZVector[numV, numU];

        // Loop across the points
        for (int currUId = 0; currUId < uArray.Length; currUId++)
        {
            double currU = uArray[currUId];

            for (int currVId = 0; currVId < vArray.Length; currVId++)
            {
                double currV = vArray[currVId];

                // Get the position on the face
                KoreXYZVector facepos = KoreQuadFaceOps.PositionOnFace2(face2, currU, currV);

                // Convert to lat/lon
                KoreLLPoint llpos = KoreLLPoint.FromXYZ(facepos);

                // Add the radius, to make an earth surface point
                KoreLLAPoint surfacePoint = new(llpos, radius);

                // Convert the LLA to an XYZ
                KoreXYZVector surfaceXYZ = surfacePoint.ToXYZ();

                // y,x to match [v,u]
                points[currVId, currUId] = surfaceXYZ;
            }
        }

        // Create the color mesh from the points
        tile.ColorMesh = new KoreColorMesh();
        KoreColorMeshOps.AddSurface(tile.ColorMesh, points, KoreColorPalette.DefaultColor);

        // setup the face center position
        {
            KoreXYZVector centerXYZpos = KoreQuadFaceOps.PositionOnFace(face, 0.5, 0.5);
            KoreLLAPoint centersurfacePoint = new(KoreLLPoint.FromXYZ(centerXYZpos), radius);
            tile.RwCenter = centersurfacePoint.ToXYZ();
        }

        return tile;
    }






    // Usage: KoreQuadCubeTile tile = KoreQuadCubeTileFactory.TileForCode(tileCode, radius: 10);
    public static KoreQuadCubeTile TileForCode2(KoreQuadCubeTileCode code, double radius = 10)
    {
        KoreQuadCubeTile tile = new();
        tile.Code = code;

        // Go through the code and find the XYZ face on the cube
        KoreQuadFace face = KoreQuadFaceOps.FaceForCubeFace(code.Face);
        foreach (int quadrant in code.Quadrants)
            face = KoreQuadFaceOps.QuadrantForFace(face, quadrant);


        KoreQuadFace face2 = KoreQuadFaceOps.QuadrantOnFace(code);

        int numU = 10; // Setup the number of points (not triangles, points) across and down
        int numV = 10;
        KoreNumeric1DArray<double> uArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numU);
        KoreNumeric1DArray<double> vArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numV);

        // [y,x] list of points
        KoreXYZVector[,] points = new KoreXYZVector[numV, numU];
        KoreColorRGB[,] colorlist = new KoreColorRGB[numV, numU];

        // Loop across the points
        for (int currUId = 0; currUId < uArray.Length; currUId++)
        {
            double currU = uArray[currUId];

            for (int currVId = 0; currVId < vArray.Length; currVId++)
            {
                double currV = vArray[currVId];

                // Get the position on the face
                KoreXYZVector facepos = KoreQuadFaceOps.PositionOnFace2(face2, currU, currV);

                // Convert to lat/lon
                KoreLLPoint llpos = KoreLLPoint.FromXYZ(facepos);

                // Grab the color for this lat long
                colorlist[currVId, currUId] = KoreSimFactory.Instance.ImageManager.ColorForPoint(llpos);

                // Add the radius, to make an earth surface point
                KoreLLAPoint surfacePoint = new(llpos, radius);

                // Convert the LLA to an XYZ
                KoreXYZVector surfaceXYZ = surfacePoint.ToXYZ();

                // y,x to match [v,u]
                points[currVId, currUId] = surfaceXYZ;
            }
        }

        // Create the color mesh from the points
        tile.ColorMesh = new KoreColorMesh();
        KoreColorMeshOps.AddSurface(tile.ColorMesh, points, colorlist);

        // setup the face center position
        {
            KoreXYZVector centerXYZpos = KoreQuadFaceOps.PositionOnFace(face, 0.5, 0.5);
            KoreLLAPoint centersurfacePoint = new(KoreLLPoint.FromXYZ(centerXYZpos), radius);
            tile.RwCenter = centersurfacePoint.ToXYZ();
        }

        return tile;
    }



}


