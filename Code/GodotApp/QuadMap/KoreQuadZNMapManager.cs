using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

// Class to take ownership of the complexity in creating ZeroNode map tiles.
public partial class KoreQuadZNMapManager : Node3D
{
    // Map load ref position
    public static KoreLLAPoint LoadRefLLA = new() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    public static KoreXYZVector LoadRefXYZ => LoadRefLLA.ToXYZ();
    public static float DistanceToHorizonM = 0;

    // lvl0 tile list
    private List<KoreQuadZNMapTile> Lvl0Tiles = new List<KoreQuadZNMapTile>();

    private float currTimer = 0;
    private float currTimerIncrement = 1.0f; // 1sec



    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreQuadZNMapManager(Node zeroNode)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialise the Manager node itself
        Name = "QuadZNMapManager";
        CreateLvl0Tiles();

    //     GodotMeshPrimitives.AddChildDebugSphere(this, 0.1f, KoreColorPalette.Colors["DarkBlue"]);

    // }

    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(double delta)
    // {
    //     ActionCounter.Refresh(5); // Tile create actions per update cycle (frame)

    //     // If we have a timer, increment it
    //     if (KoreCentralTime.CheckTimer(ref currTimer, currTimerIncrement))
    //     {
    //         // Get the latest camera and viewport
    //         Viewport viewport = GetViewport();
    //         Camera3D? camera = GetViewport().GetCamera3D();
    //         if (camera != null)
    //             KoreUnprojectManager.UpdateState(camera, viewport);

    //         // Loop across all the lvl0 tiles, accumulate and output the tile count.
    //         int totalTileCount = 0;
    //         foreach (var tile in Lvl0Tiles)
    //         {
    //             //tile.CountChildTiles();
    //             totalTileCount += tile.TileCount;
    //         }
    //         //GD.Print($"Total Tile Count: {totalTileCount} // {Lvl0Tiles.Count}");
    //     }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lvl0 Tiles
    // --------------------------------------------------------------------------------------------

    // Initialising call to create the top level tiles, which then recursively service the creation and display of all the lower level tiles.

    public void CreateLvl0Tiles()
    {
        KoreCentralLog.AddEntry("KoreQuadZNMapManager: Creating the 8 Lvl0 Face Tiles");


        // ----------------------------

        // Debug inject a command line to load a global image for the tile generation
        string cmd = "sat loadtile UnitTestArtefacts/bluemarble_2000x1000.webp     -90 -180     90    180";
        (bool success, string response) = KoreSimFactory.Instance.ConsoleInterface.RunSingleCommand(cmd);

        GD.Print($"Calling: ConsoleInterface.RunSingleCommand() = {success} : {response}");

        // sleep for a second to let the command process
        System.Threading.Thread.Sleep(1000);

        // ----------------------------

        // Create the 8 face tiles
        double drawRadius = 13.0; // Place the tiles at a fixed radius for now
        foreach (KoreQuadFace.CubeFace currFace in Enum.GetValues(typeof(KoreQuadFace.CubeFace)))
        {
            //KoreQuadFace.CubeFace currFace = KoreQuadFace.CubeFace.Front;

            // Create the face tile code
            KoreQuadCubeTileCode currFaceTileCode = new() { Face = currFace, Quadrants = new List<int> { } };

            // Create the tile object, that will kickstart its own tile loading and display process.
            KoreQuadZNMapTile currZNMapTile = new(currFaceTileCode, 13);

            string vecstr1 = KoreXYZVectorIO.ToStringWithDP(currZNMapTile.RwTileCenterXYZ, 4);
            GD.Print($"KoreQuadZNMapManager: Tile: {currZNMapTile.TileCodeStr} Created {vecstr1}");

            // Add the tile to the scene and our internal list
            Lvl0Tiles.Add(currZNMapTile);
            AddChild(currZNMapTile);

            drawRadius += 0.1;
        }

        // Create one set of child tile faces for testing
        KoreQuadCubeTileCode rootTileCode = new() { Face = KoreQuadFace.CubeFace.Front, Quadrants = new List<int> { } };

        // Generate child tile codes
        List<KoreQuadCubeTileCode> childTileCodes = rootTileCode.GenerateChildTileCodes();

        foreach (var childCode in childTileCodes)
        {
            GD.Print($"Child Tile Code: {childCode.CodeToString()}");

            // Create the tile object, that will kickstart its own tile loading and display process.
            KoreQuadZNMapTile currZNMapTile = new(childCode, 13.4);

            string vecstr1 = KoreXYZVectorIO.ToStringWithDP(currZNMapTile.RwTileCenterXYZ, 4);
            GD.Print($"KoreQuadZNMapManager: Tile: {currZNMapTile.TileCodeStr} Created {vecstr1}");

            // Add the tile to the scene and our internal list
            //Lvl0Tiles.Add(currZNMapTile);
            AddChild(currZNMapTile);

            //drawRadius += 0.1;

        }

        // {
        //     KoreQuadFace.CubeFace currFace = KoreQuadFace.CubeFace.Left;

        //     // Create the face tile code
        //     KoreQuadCubeTileCode currFaceTileCode = new() { Face = currFace, Quadrants = new List<int> { } };

        //     // Create the tile object, that will kickstart its own tile loading and display process.
        //     KoreQuadZNMapTile currZNMapTile = new(currFaceTileCode, 13.1);

        //     string vecstr = KoreXYZVectorIO.ToStringWithDP(currZNMapTile.RwTileCenterXYZ, 4);
        //     GD.Print($"KoreQuadZNMapManager: Tile: {currZNMapTile.TileCodeStr} Created {vecstr}");

        //     // Add the tile to the scene and our internal list
        //     Lvl0Tiles.Add(currZNMapTile);
        //     AddChild(currZNMapTile);
        // }

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation Query
    // --------------------------------------------------------------------------------------------

    // private KoreZeroNodeMapTile? GetLvl0Tile(KoreMapTileCode tileCode)
    // {
    //     // Get the name of the first lvl0
    //     string tileName = tileCode.ToString();

    //     KoreMapTileCode? lvl0Code = KoreMapTileCode.CodeToLvl(tileCode, 0);
    //     if (lvl0Code == null) return null;

    //     // Get the tile node for this tile code
    //     return (KoreZeroNodeMapTile)FindChild(lvl0Code.ToString());
    // }

    // // Get the elevation at a given LL point, as loaded by the map tiles.

    // public float GetElevation(KoreLLPoint llPoint)
    // {
    //     KoreMapTileCode? tileCode = new KoreMapTileCode(llPoint.LatDegs, llPoint.LonDegs, 0);

    //     KoreZeroNodeMapTile? lvl0Tile = GetLvl0Tile(tileCode);

    //     if (lvl0Tile == null) return KoreElevationUtils.InvalidEle;

    //     float ele = KoreElevationUtils.InvalidEle;

    //     foreach (KoreZeroNodeMapTile currTile in Lvl0Tiles)
    //     {
    //         if (currTile.IsPointInTile(llPoint))
    //         {
    //             ele = currTile.GetElevation(llPoint);
    //             break;
    //         }
    //     }

    //     return lvl0Tile.GetElevation(llPoint);
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: Visibility
    // // --------------------------------------------------------------------------------------------


}
