using Godot;

using System.Collections.Generic;

using KoreCommon;
using KoreGIS;

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



    // --------------------------------------------------------------------------------------------



    // Usage: KoreQuadCubeTile tile = KoreQuadCubeTileFactory.TileForCode(tileCode, radius: 10);
    public static KoreQuadCubeTile TileForCode2(KoreQuadCubeTileCode code, double radius = 10)
    {
        KoreQuadCubeTile tile = new();
        tile.Code = code;

        // Go through the code and find the XYZ face on the cube
        KoreQuadFace face = KoreQuadFaceOps.QuadrantOnFace(code);

        int numU = 50; // Setup the number of points (not triangles, points) across and down from a top-left 0,0
        int numV = 55;
        KoreNumeric1DArray<double> uArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numU);
        KoreNumeric1DArray<double> vArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numV);

        // [y,x] list of points
        KoreXYZVector[,] points = new KoreXYZVector[numV, numU];
        KoreColorRGB[,] colorlist = new KoreColorRGB[numV, numU];


        string tilecodestr = code.CodeToString();
        KoreLLPoint topleftll = KoreLLPoint.FromXYZ(face.TopLeft);
        KoreLLPoint bottomrightll = KoreLLPoint.FromXYZ(face.BottomRight);
        string tileboxstr = $"TL({topleftll.LatDegs:0.000},{topleftll.LonDegs:0.000}) BR({bottomrightll.LatDegs:0.000},{bottomrightll.LonDegs:0.000})";

        GD.Print($"Gen Quad Tile {tilecodestr} // {tileboxstr}");

        // Loop across the points
        for (int currUId = 0; currUId < uArray.Length; currUId++)
        {
            double currU = uArray[currUId];

            for (int currVId = 0; currVId < vArray.Length; currVId++)
            {
                double currV = vArray[currVId];

                // Get the position on the face - flip U coordinate for proper image mapping
                double flippedU = 1.0 - currU;  // Reverse U coordinate for longitude progression
                KoreXYZVector facepos = KoreQuadFaceOps.PositionOnFace2(face, currU, currV);

                // Convert to lat/lon
                KoreLLPoint llpos = KoreLLPoint.FromXYZ(facepos);

                // KoreXYZVector facepos2 = facepos;
                // facepos2.Z *= -1; // Invert Z for the conversion
                // KoreLLPoint surfaceLL = KoreLLPoint.FromXYZ(facepos2);
                // surfaceLL.LonDegs -= 90;
                // surfaceLL.LonDegs = KoreNumericRange<double>.ZeroTo360Degrees.Apply(surfaceLL.LonDegs);

                // Grab the color for this lat long
                colorlist[currVId, currUId] = KoreGISHub.ImageManager.ColorForPoint(llpos);

                // Add the radius, to make an earth surface point
                KoreLLAPoint surfacePoint = new(llpos, radius);

                if (currUId == 0 && currVId == 0)
                    surfacePoint.RadiusM += 0.01;

                // Convert the LLA to an XYZ
                KoreXYZVector surfaceXYZ = surfacePoint.ToXYZ();

                // report a few positions from the top corner
                if (currUId + currVId < 3)
                {
                    GD.Print($"UV:({currUId}, {currVId}) // LL {llpos.LatDegs:0.000},{llpos.LonDegs:0.000} // XYZ {surfaceXYZ.X:0.000},{surfaceXYZ.Y:0.000},{surfaceXYZ.Z:0.000}");
                }


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

    // --------------------------------------------------------------------------------------------

    // Usage: KoreQuadCubeTile tile = KoreQuadCubeTileFactory.TileForCode(tileCode, radius: 10);

    // Create the tile mesh, with the origin point at the LL center of the tile, so we add an additional
    // offset to the mesh points. This is part of the relocatable geometry.

    public static KoreQuadCubeTile TileForCode3(KoreQuadCubeTileCode code, double radius = 10)
    {
        KoreQuadCubeTile tile = new();
        tile.Code = code;

        // Go through the code and find the XYZ face on the cube
        KoreQuadFace face = KoreQuadFaceOps.QuadrantOnFace(code);

        KoreLLPoint centerLL = KoreQuadFaceOps.AverageLL(face);
        KoreLLAPoint centerLLA = new(centerLL, radius);
        KoreXYZVector centerXYZ = centerLLA.ToXYZ();

        int numU = 130; // Setup the number of points (not triangles, points) across and down from a top-left 0,0
        int numV = 132;
        KoreNumeric1DArray<double> uArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numU);
        KoreNumeric1DArray<double> vArray = KoreNumeric1DArrayOps<double>.CreateArrayByCount(0, 1, numV);

        // [y,x] list of points
        KoreXYZVector[,] points = new KoreXYZVector[numV, numU];
        KoreColorRGB[,] colorlist = new KoreColorRGB[numV, numU];




        string tilecodestr = code.CodeToString();
        KoreLLPoint topleftll = KoreLLPoint.FromXYZ(face.TopLeft);
        KoreLLPoint bottomrightll = KoreLLPoint.FromXYZ(face.BottomRight);
        string tileboxstr = $"TL({topleftll.LatDegs:0.000},{topleftll.LonDegs:0.000}) BR({bottomrightll.LatDegs:0.000},{bottomrightll.LonDegs:0.000})";

        GD.Print($"Gen Quad Tile {tilecodestr} // {tileboxstr}");

        // Loop across the points
        for (int currUId = 0; currUId < uArray.Length; currUId++)
        {
            double currU = uArray[currUId];

            for (int currVId = 0; currVId < vArray.Length; currVId++)
            {
                double currV = vArray[currVId];

                // Get the position on the face - flip U coordinate for proper image mapping
                double flippedU = 1.0 - currU;  // Reverse U coordinate for longitude progression
                KoreXYZVector facepos = KoreQuadFaceOps.PositionOnFace2(face, currU, currV);

                // Convert to lat/lon
                KoreLLPoint llpos = KoreLLPoint.FromXYZ(facepos);

                // Grab the color for this lat long
                colorlist[currVId, currUId] = KoreGISHub.ImageManager.ColorForPoint(llpos);

                // Add the radius, to make an earth surface point
                KoreLLAPoint surfacePoint = new(llpos, radius);

                if (currUId == 0 && currVId == 0)
                    surfacePoint.RadiusM += 0.01;

                // Convert the LLA to an XYZ
                KoreXYZVector surfaceXYZ = surfacePoint.ToXYZ();

                // Adjust the point to be relative to the tile center
                KoreXYZVector xyzToCenterPoint = centerXYZ.XYZTo(surfaceXYZ);

                // report a few positions from the top corner
                if (currUId + currVId < 3)
                {
                    GD.Print($"UV:({currUId}, {currVId}) // LL {llpos.LatDegs:0.000},{llpos.LonDegs:0.000} // XYZ {xyzToCenterPoint.X:0.000},{xyzToCenterPoint.Y:0.000},{xyzToCenterPoint.Z:0.000}");
                }

                // y,x to match [v,u]
                points[currVId, currUId] = xyzToCenterPoint;
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


