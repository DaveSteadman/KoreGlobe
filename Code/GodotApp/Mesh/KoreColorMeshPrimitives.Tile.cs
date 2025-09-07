// <fileheader>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreColorMeshPrimitives
{
    // Create a sphere mesh for KoreColorMesh, with a given center, radius, and colormap
    // - We pick the number of lat/long segments from the color list dimensions
    // Usage: KoreColorMesh tileMesh = KoreColorMeshPrimitives.Tile(new KoreMapTileCode("FB"), eleData, colormap);
    public static KoreColorMesh Tile(
        KoreMapTileCode tileCode,
        KoreNumeric2DArray<float> tileEleData,
        KoreColorRGB[,] colormap)
    {
        var mesh = new KoreColorMesh();

        KoreLLPoint rwTileCenterLL = tileCode.LLBox.CenterPoint;

        // Define zero longitude center, so we can create the tile from relative (not absolute) angles and
        // more intuitively rotate the tile to the absolute longitude later.
        KoreLLAPoint rwLLAZeroLonCenter = new KoreLLAPoint()
        {
            LatDegs = rwTileCenterLL.LatDegs,
            LonDegs = 0,
            RadiusM = KoreWorldConsts.EarthRadiusM
        };
        KoreXYZVector rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int pointCountLon = tileEleData.Width;
        int pointCountLat = tileEleData.Height;
        List<double> lonZeroListRads = KoreValueUtils.CreateRangeList(pointCountLon, -rwLLAZeroLonCenter.LonDegs, rwLLAZeroLonCenter.LonDegs); // Relative azimuth - left to right (low to high longitude)
        List<double> latListRads = KoreValueUtils.CreateRangeList(pointCountLat, rwLLAZeroLonCenter.LatDegs, -rwLLAZeroLonCenter.LatDegs); // Max to min +90 -> -90. Start at top of tile

        int[,] pointIds = new int[pointCountLon, pointCountLat];

        for (int ix = 0; ix < pointCountLon; ix++)
        {
            // Create limit working variables, so we know when to populate the bottom/edge array.
            bool limitX = (ix == 0) || (ix == pointCountLon - 1);

            for (int jy = 0; jy < pointCountLat; jy++)
            {
                bool limitY = (jy == 0) || (jy == pointCountLat - 1);

                // Find the Real-World (RW) position for each point in the mesh.
                double lonRads = lonZeroListRads[ix];
                double latRads = latListRads[jy];
                double ele = tileEleData[ix, jy];

                // Determine the tile position in the RW world, and then as an offset from the tile centre
                KoreLLAPoint rwLLAPointPos = new KoreLLAPoint() { LatRads = latRads, LonRads = lonRads, RadiusM = 5 };
                KoreXYZVector rwXYZPointPos = rwLLAPointPos.ToXYZ();

                //KoreXYZVector rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // Convert from RW coordinates to Godot ones.
                // rwXYZCenterOffset = rwXYZCenterOffset.Scale(KoreZeroOffset.RwToGeDistanceMultiplier);
                //rwXYZCenterOffset = rwXYZCenterOffset.FlipZ();

                // Add the point and record the ID for the triangles
                int vertexId = mesh.AddVertex(rwXYZPointPos);
                pointIds[ix, jy] = vertexId;
            }
        }

        // Triangles
        // Create triangles between all adjacent latitude rows
        for (int lat = 0; lat < pointCountLat-1; lat++)
        {
            for (int lon = 0; lon < pointCountLon-1; lon++)
            {
                // Get the four vertices of the current quad
                int v1 = pointIds[lon, lat];         // current lat, current lon
                int v2 = pointIds[lon, lat + 1];     // next lat, current lon
                int v3 = pointIds[lon + 1, lat + 1]; // next lat, next lon
                int v4 = pointIds[lon + 1, lat];     // current lat, next lon

                // Use colormap coordinates
                KoreColorRGB col = colormap[lat % colormap.GetLength(0), lon % colormap.GetLength(1)];

                // Add the quad as two triangles using AddFace helper
                KoreColorMeshOps.AddFace(mesh, v1, v4, v3, v2, col);
            }
        }

        // Dump the mesh to JSON and a file for inspection
        string meshjson = KoreColorMeshIO.ToJson(mesh);
        File.WriteAllText($"UnitTestArtefacts/TileMesh-{tileCode}.json", meshjson);

        return mesh;
    }

}
