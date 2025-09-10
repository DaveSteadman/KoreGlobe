using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;
using SkiaSharp;

using KoreCommon;
using KoreCommon.SkiaSharp;
using KoreSim;


#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Background
    // --------------------------------------------------------------------------------------------

    // Create rules:
    // - Need to have the elevation file to gate the creation of a child tile, or it blocks the
    //   creation of the while set. No more exprapolation of elevation data.
    // - Will use the tile's own image, the parent tile's image if it exists, or a default image if it doesn't.

    private async Task BackgroundTileCreation(KoreMapTileCode tileCode)
    {
        // GD.Print($"Starting Create: {tileCode}");
        try
        {
            // Starting: Set the flags that will be used later to determine activity around the tile while we construct it.
            ConstructionComplete = false;
            ActiveVisibility = false;

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // ----------------------------
            // File IO and Tile center

            // Setup some basic elements of the tile ahead of the main elevation and image loading.
            Filepaths = new KoreMapTileFilepaths(TileCode); // Figure out the file paths for the tile

            // Setup the tile center shortcuts
            RwTileLLBox = KoreMapTileCode.LLBoxForCode(TileCode);
            RwTileCenterLLA = new KoreLLAPoint(RwTileLLBox.CenterPoint);
            RwTileCenterXYZ = RwTileCenterLLA.ToXYZ();

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();


            BackgroundColorMesh();

            // GD.Print($"Ending Create: {tileCode}");
            BackgroundConstructionComplete = true;

            // ----------------------------

            // Loop until the main construction is complete, and then tidy up.

            while (!ConstructionComplete)
            {
                await Task.Delay(100);
            }

            if (ColorMeshNode != null)
                ColorMeshNode.PostCreateTidyUp();

            TileColorMesh = null;  // free up memory                
            Filepaths.Init();
            
            // Clear any temporary objects
            GC.Collect(0, GCCollectionMode.Optimized);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            KoreCentralLog.AddEntry($"An error occurred: {ex.Message}");
        }
    }

    private void BackgroundColorMesh()
    {
        // Create or load the color mesh for the tile
        if (true)//!Filepaths.MeshFileExists)
        {
            KoreNumeric2DArray<float> eleData = LoadTileEleArr();

            // create a color map
            int dummyAzCount = 60;
            int dummyElCount = 60;

            if (eleData.MaxVal() < 0.01)
            {
                dummyAzCount = 10;
                dummyElCount = 10;
            }
            
            // Source the key ele and color data
            KoreColorRGB[,] colorMap = TileImage(dummyAzCount, dummyElCount);

            // create the color mesh
            TileColorMesh = KoreColorMeshPrimitives.CenteredSphereSection(
                    llBox: RwTileLLBox,
                    radius: KoreZeroOffset.GeEarthRadius,//(float)KoreWorldConsts.EarthRadiusM,
                    colormap: colorMap,
                    tileEleData: eleData);

            // serialise the mesh to a binary file
            byte[] meshdata = KoreColorMeshIO.ToBytes(TileColorMesh, KoreColorMeshIO.DataSize.AsFloat);
            System.IO.File.WriteAllBytes(Filepaths.MeshFilepath, meshdata);
        }
        else
        {
            // read the mesh from a binary file
            byte[] meshdata = System.IO.File.ReadAllBytes(Filepaths.MeshFilepath);
            TileColorMesh = KoreColorMeshIO.FromBytes(meshdata, KoreColorMeshIO.DataSize.AsFloat);
        }

        // create the godot renderer for the color mesh
        ColorMeshNode = new KoreColorMeshGodot() { Visible = false, Name = "ColorMeshNode" };
        ColorMeshNode.UpdateMeshBackground(TileColorMesh);
    }

    private void MainThreadColorMesh()
    {
        if (ColorMeshNode != null)
        {
            AddChild(ColorMeshNode);
            ColorMeshNode.Visible = false;
            ColorMeshNode.UpdateMeshMainThread();
            ColorMeshNode.Name = "TileExperiment";

            double lon = TileCode.LLBox.CenterPoint.LonRads;
            ColorMeshNode.Rotation = new Vector3(0, (float)lon, 0);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Main Thread
    // --------------------------------------------------------------------------------------------

    // A main thread function that is called a few times to complete the tile creation in stages.
    // - Grabs a limited ActionCounter/Token to throttle the workload.

    private void MainThreadFinalizeCreation()
    {
        //GD.Print($"MainThreadFinalizeCreation: {TileCode}");

        switch (ConstructionStage)
        {
            case 0:
                // if (GrabbedActionCounter())
                // {
                    // CreateMeshTileSurfacePoints();
                    ConstructionStage = 1;
                    MainThreadColorMesh();
                // }

                // start a timer
                // float startTime = KoreCentralTime.RuntimeSecs;
                // ConstructionComplete = true;
                // BackgroundConstructionComplete = true;

                // float endTime = KoreCentralTime.RuntimeSecs;

                // KoreCentralLog.AddEntry($"KoreZeroNodeMapTile: {TileCode} created in {endTime - startTime} seconds.");

                break;

            default:
                ConstructionComplete = true;
                BackgroundConstructionComplete = true;

                KoreCentralLog.AddEntry($"DONE KoreZeroNodeMapTile: {TileCode}");

                LocateTile();

                if (TileCode.MapLvl == 0)
                {
                    SetVisibility(true);
                    ActiveVisibility = true;

                    // Add a delay so we don't look to assess visibility of all tiles immediately.
                    UIUpdateTimer = KoreCentralTime.RuntimeSecs + (RandomLoopList.GetNext() / 2f);
                }

                break;
        }
    }

    // --------------------------------------------------------------------------------------------

    private void AddDebugOriginSphere()
    {
        float sphereRad = 1f / (float)((TileCode.MapLvl * 4) + 1);
        var sphereInstance = new MeshInstance3D { Name = "TestSphere", Mesh = new SphereMesh { Radius = sphereRad, Height = sphereRad*2f } };
        AddChild(sphereInstance);

        GEElements.Add(sphereInstance);
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Label
    // --------------------------------------------------------------------------------------------

    // Add a label to the middle of the tile, oriented to be flat to the surface

    private void LabelTile(KoreMapTileCode tileCode)
    {
        // // Get the tile center forthe label position
        // KoreLLPoint posLL = new KoreLLPoint(RwTileCenterLLA);

        // // Decimal points on the lat/long per tile level. Avoid superfluous 0's on the end
        // string format = "F" + dpPerLvl[TileCode.MapLvl];

        // // Create the tile code string
        // string tileCodeBoxStr =
        //     $"...[{RwTileLLBox.MaxLatDegs.ToString(format)}, {RwTileLLBox.MaxLonDegs.ToString(format)}]\n" +
        //     $"[{RwTileLLBox.MinLatDegs.ToString(format)}, {RwTileLLBox.MinLonDegs.ToString(format)}]...";

        // // Determine the size and create the label
        // float KPixelSize = LabelSizePerLvl[tileCode.MapLvl];
        // TileCodeLabel = KoreLabel3DFactory.CreateLabel($"{tileCode.ToString()}\n{tileCodeBoxStr}", KPixelSize);

        // AddChild(TileCodeLabel);
        // TileCodeLabel.Visible = false;

        // // Determine the GameEngine positions and orientation
        // Godot.Vector3 v3VectN     = (TileLabelOffsetN     - TileLabelOffset).Normalized();
        // Godot.Vector3 v3VectBelow = (TileLabelOffsetBelow - TileLabelOffset).Normalized();

        // TileCodeLabel.Position = TileLabelOffset;
        // TileCodeLabel.LookAt(v3VectN, v3VectBelow);

        // // rotate to consider the lat long of the tile
        // float rotAz = (float)(RwTileCenterLLA.LonRads) + (float)KoreValueUtils.DegsToRads(90);
        // float rotEl = -1f * (float)(RwTileCenterLLA.LatRads); // We created the tile with relative elevation, so apply the absolute value to orient it to its latitude.
        // TileCodeLabel.Rotation  = new Vector3(rotEl, rotAz, 0);
    }


}