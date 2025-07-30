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
        KoreXYZPoint rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int pointCountLon = TileEleData.Width;
        int pointCountLat = TileEleData.Height;
        List<double> lonZeroListRads = KoreValueUtils.CreateRangeList(pointCountLon, RwTileLLBox.HalfDeltaLonRads, -RwTileLLBox.HalfDeltaLonRads); // Relative azimuth
        List<double> latListRads = KoreValueUtils.CreateRangeList(pointCountLat, RwTileLLBox.MaxLatRads, RwTileLLBox.MinLatRads);

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
                KoreLLAPoint rwLLAPointPos = new KoreLLAPoint() { LatRads = latRads, LonRads = lonRads, RadiusM = ele };
                KoreXYZPoint rwXYZPointPos = rwLLAPointPos.ToXYZ();
                KoreXYZVector rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                //rwXYZCenterOffset = rwXYZCenterOffset.Scale(KoreZeroOffset.RwToGeDistanceMultiplier);

                // Convert the Real-World position to the Game Engine position.
                v3Data[ix, jy] = new KoreXYZVector(rwXYZPointPos);

                // if (limitX || limitY) // Only do the edges, we don't use the middle.
                // {
                //     // Determine the tile position in the RW world, and then as an offset from the tile centre
                //     KoreLLAPoint rwLLABottomPos = new KoreLLAPoint() { LatRads = latRads, LonRads = lonRads, AltMslM = -1000 };
                //     KoreXYZPoint rwXYZBottomPos = rwLLABottomPos.ToXYZ();
                //     KoreXYZVector rwXYZBottomOffset = rwXYZZeroLonCenter.XYZTo(rwXYZBottomPos);
                // }
            }
        }

        // // Determine the LABEL POSITION while we're still in background processing functions, and then
        // // consume it when in the _process loop.
        // {
        //     // Determine the tile label elevation as 100m above the highest elevation in the tile.
        //     double tileMaxAlt = TileEleData.MaxVal();
        //     if (tileMaxAlt < 100) tileMaxAlt = 100;

        //     // Determine the label position in the RW world, and then as an offset from the tile centre
        //     KoreLLAPoint rwLLATileLabel = new KoreLLAPoint() { LatDegs = rwLLAZeroLonCenter.LatDegs, LonDegs = rwLLAZeroLonCenter.LonDegs, AltMslM = tileMaxAlt + 50 };

        //     //KoreLLAPoint  rwLLATileLabel       = new KoreLLAPoint() { LatDegs = RwTileCenterLLA.LatDegs, LonDegs = 0, RadiusM = KoreWorldConsts.EarthRadiusM - 1000 };
        //     KoreXYZPoint rwXYZTileLabel = rwLLATileLabel.ToXYZ();
        //     KoreXYZVector rwXYZTileLabelOffset = rwXYZZeroLonCenter.XYZTo(rwXYZTileLabel);
        //     //KoreXYZVector rwXYZTileLabelOffset = RwTileCenterXYZ.XYZTo(rwXYZTileLabel);

        //     // Convert the Real-World position to the Game Engine position.
        //     TileLabelOffset = new Vector3(
        //         (float)(rwXYZTileLabelOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
        //         (float)(rwXYZTileLabelOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
        //         (float)(rwXYZTileLabelOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));

        //     // - - - -

        //     KoreLLAPoint rwLLATileLabelN = rwLLATileLabel;
        //     rwLLATileLabelN.LatDegs = RwTileCenterLLA.LatDegs + 0.01;
        //     KoreXYZPoint rwXYZTileLabelN = rwLLATileLabelN.ToXYZ();
        //     KoreXYZVector rwXYZTileLabelNOffset = rwXYZZeroLonCenter.XYZTo(rwXYZTileLabelN);

        //     TileLabelOffsetN = new Vector3(
        //         (float)(rwXYZTileLabelNOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
        //         (float)(rwXYZTileLabelNOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
        //         (float)(rwXYZTileLabelNOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));

        //     KoreLLAPoint rwLLATileLabelBelow = rwLLATileLabel;
        //     rwLLATileLabelN.RadiusM = RwTileCenterLLA.RadiusM - 1000;
        //     KoreXYZPoint rwXYZTileLabelBelow = rwLLATileLabelN.ToXYZ();
        //     KoreXYZVector rwXYZTileLabelBelowOffset = rwXYZZeroLonCenter.XYZTo(rwXYZTileLabelBelow);

        //     TileLabelOffsetBelow = new Vector3(
        //         (float)(rwXYZTileLabelBelowOffset.X * KoreZeroOffset.RwToGeDistanceMultiplier),
        //         (float)(rwXYZTileLabelBelowOffset.Y * KoreZeroOffset.RwToGeDistanceMultiplier),
        //         (float)(rwXYZTileLabelBelowOffset.Z * KoreZeroOffset.RwToGeDistanceMultiplier));
        // }
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

        KoreUVBox uvBox = new KoreUVBox(0, 0, 1, 1);

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

        // var cubeMesh = KoreMeshDataPrimitives.BasicCubeIsolatedTriangles(1.0f, new KoreColorRGB(255, 0, 0));


        //var tileMesh = KoreMeshDataPrimitives.DropEdgeTile

        // KoreMeshDataPrimitives.AddTile(v3Data, UVx, UVy, false);
        // KoreMeshDataPrimitives.AddBoxEdges(v3Data, v3DataBottom, UVx, UVy, false);

        // KoreBinaryDataManager loadDB = new KoreBinaryDataManager("TileGeometry2.db");
        // string tilecodestr = TileCode.ToString();

        // if (!loadDB.DataExists(tilecodestr))
        // {
        //     byte[] data = KoreMeshDataIO.MeshDataToBytes2(meshBuilder.meshData);
        //     loadDB.Add(tilecodestr, data);
        // }
        // else
        // // {
        // //     byte[] data = loadDB.Get(tilecodestr);
        // //     meshBuilder.meshData = KoreMeshDataIO.BytesToMeshData2(data);
        // // }

        // // Build the mesh data and add it to the node
        // MeshInstance = new MeshInstance3D() { Name = "tileMesh" };
        // MeshInstance.Mesh = meshBuilder.BuildWithUV("Surface");
        // //MeshInstance.MaterialOverride = KoreMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        // MeshInstance.MaterialOverride = TileMaterial;


        GodotMeshPrimitives.AddChildDebugSphere(this, 0.1f, new KoreColorRGB(50, 150, 50));
        // {
        //     var cubeMesh1 = KoreMeshDataPrimitives.BasicSphere(0.1f, new KoreColorRGB(50, 50, 50), 12);

        //     KoreGodotLineMesh childMeshNode1 = new KoreGodotLineMesh();
        //     childMeshNode1.UpdateMesh(cubeMesh1);

        //     KoreGodotSurfaceMesh childSurfaceMeshNode1 = new KoreGodotSurfaceMesh();
        //     childSurfaceMeshNode1.UpdateMesh(cubeMesh1);

        //     AddChild(childMeshNode1);
        //     AddChild(childSurfaceMeshNode1);
        // }


        // AddChild(MeshInstance);

        // MeshInstance.Rotation = new Vector3(0, rotAz, 0);

        // // Set the visibility false, later functions will assign the correct visibility rules.
        // MeshInstance.Visible = false;


        // //AddChild(MeshInstance);

        // GEElements.Add(MeshInstance);

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