using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Mesh Points
    // --------------------------------------------------------------------------------------------

    // Inputs being the RwAzElBox and TileEleData, we create a mesh that represents the tile.
    // Context being the RwtoGe scaling factor.

    public void CreateMeshPoints()
    {
        // Define zero longitude center, so we can create the tile from relative (not absolute) angles and
        // more intuitively rotate the tile to the absolute longitude later.
        KoreLLAPoint rwLLAZeroLonCenter = new KoreLLAPoint()
        {
            LatDegs = RwTileCenterLLA.LatDegs,
            LonDegs = 0,
            RadiusM = KoreWorldConsts.EarthRadiusM
        };
        KoreXYZVector rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int pointCountLon = TileEleData.Width;
        int pointCountLat = TileEleData.Height;
        List<double> lonZeroListRads = KoreValueUtils.CreateRangeList(pointCountLon, -RwTileLLBox.HalfDeltaLonRads, RwTileLLBox.HalfDeltaLonRads); // Relative azimuth - left to right (low to high longitude)
        List<double> latListRads     = KoreValueUtils.CreateRangeList(pointCountLat, RwTileLLBox.MaxLatRads, RwTileLLBox.MinLatRads);

        // NOTE: The ranges mean the [0,0] is TOP LEFT

        // Simplicity: Create 2 x 2D arrays for the top and bottom of the tile. We'll only use the edges of the bottom.
        v3Data       = new KoreXYZVector[pointCountLon, pointCountLat];
        v3DataBottom = new KoreXYZVector[pointCountLon, pointCountLat];

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
                double ele = TileEleData[ix, jy];

                // Determine the tile position in the RW world, and then as an offset from the tile centre
                KoreLLAPoint rwLLAPointPos = new KoreLLAPoint() { LatRads = latRads, LonRads = lonRads, AltMslM = ele };
                KoreXYZVector rwXYZPointPos = rwLLAPointPos.ToXYZ();
                KoreXYZVector rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // GD Print the tilecode and LLA of the TL point
                if (ix == 0 && jy == 0)
                {
                    GD.Print($"KoreZeroNodeMapTile: {TileCode} // ele: {ele:F2} / TL LLA: {rwLLAPointPos}");
                }



                rwXYZCenterOffset = rwXYZCenterOffset.Scale(KoreZeroOffset.RwToGeDistanceMultiplier);

                // Convert the Real-World position to the Game Engine position.
                v3Data[ix, jy] = rwXYZCenterOffset;  // new KoreXYZVector(rwXYZPointPos);

                if (limitX || limitY) // Only do the edges, we don't use the middle.
                {
                    // Determine the tile position in the RW world, and then as an offset from the tile centre
                    KoreLLAPoint rwLLABottomPos = new KoreLLAPoint() { LatRads = latRads, LonRads = lonRads, AltMslM = -1000 };
                    KoreXYZVector rwXYZBottomPos = rwLLABottomPos.ToXYZ();
                    KoreXYZVector rwXYZBottomOffset = rwXYZZeroLonCenter.XYZTo(rwXYZBottomPos);

                    v3DataBottom[ix, jy] = rwXYZBottomOffset;
                }
                else
                    v3DataBottom[ix, jy] = KoreXYZVector.Zero; // No bottom point for the middle of the tile.
            }

        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Process
    // --------------------------------------------------------------------------------------------

    public void CreateMeshTileSurface()
    {
        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwTileCenterLLA.LonRads); // We created the tile with relative azimuth, so apply the absolute value to orient it to its longitude.

        // Create the game-engine mesh from the V3s
        KoreMeshData meshData = new();

        KoreUVBox uvBox = new KoreUVBox(new KoreXYVector(0, 0), new KoreXYVector(1, 1));

        meshData = KoreMeshDataPrimitives.Surface(v3Data, uvBox);

        KoreGodotLineMesh surfaceLineMesh = new KoreGodotLineMesh();
        surfaceLineMesh.UpdateMesh(meshData);
        AddChild(surfaceLineMesh);

        surfaceLineMesh.Name = "SurfaceLineMesh";


        // debug print the JSON of the mesh
        // string meshJson = KoreMeshDataIO.ToJson(meshData, dense: false);
        // GD.Print("Mesh JSON: " + meshJson);


        KoreGodotSurfaceMesh surfaceMesh = new KoreGodotSurfaceMesh();
        surfaceMesh.UpdateMesh(meshData);
        AddChild(surfaceMesh);
        surfaceMesh.Name = "SurfaceMesh";

        // Apply the mesh material
        surfaceMesh.MaterialOverride = TileMaterial;


        GodotMeshPrimitives.AddChildDebugSphere(this, 0.1f, new KoreColorRGB(50, 150, 50));

    }

    // --------------------------------------------------------------------------------------------

    // Create ribbon meshes for the "drop tile edges", the skirt around the tile that prevents visible
    // gaps between tiles.

    // Assumes v3Data and v3DataBottom are suitably populated.

    public void CreateTileDropEdge()
    {
        double minUvY = UVBox.MinY;
        double maxUvY = UVBox.MaxY;
        double minUvX = UVBox.MinX;
        double maxUvX = UVBox.MaxX;

        // LEFT EDGE - Increasing Y-index top to bottom of top
        // get the top and bottom lists of points
        List<KoreXYZVector> leftUpperPoints = new List<KoreXYZVector>();
        List<KoreXYVector> leftLowerUVs = new List<KoreXYVector>();
        for (int y = 0; y < v3Data.GetLength(1); y++)
        {
            leftUpperPoints.Add(v3Data[0, y]);
            leftLowerUVs.Add(new KoreXYVector(minUvX, maxUvY - (y / (double)v3Data.GetLength(1)) * (maxUvY - minUvY)));
        }

        List<KoreXYZVector> leftLowerPoints = new List<KoreXYZVector>();
        List<KoreXYVector> leftUpperUVs = new List<KoreXYVector>();
        for (int y = 0; y < v3DataBottom.GetLength(1); y++)
        {
            leftLowerPoints.Add(v3DataBottom[0, y]);
            leftUpperUVs.Add(new KoreXYVector(minUvX, minUvY + (y / (double)v3DataBottom.GetLength(1)) * (maxUvY - minUvY)));
        }

        //

        // To visualise travelling down the ribbon, with visble tiles upwards, upper points are on the left
        KoreMeshData leftRibbonMesh = KoreMeshDataPrimitives.Ribbon(
            leftUpperPoints, leftUpperUVs,
            leftLowerPoints, leftLowerUVs,
            isClosed: false);

    }

    // --------------------------------------------------------------------------------------------

    // Turn the elevation data into a 2D array of Vector3s, which we can then use to create a mesh.

    // Input = TileEleData
    // Output = v3Data

    public void CreateMeshTileSurfacePoints()
    {

        GD.Print("CreateMeshTileSurfacePoints: Creating mesh points for tile: " + TileCode);

        // Create the mesh points from the elevation data
        CreateMeshPoints();

        // Create the mesh surface from the points
        CreateMeshTileSurface();
    }
}
