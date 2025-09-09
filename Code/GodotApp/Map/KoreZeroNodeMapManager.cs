using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

// Class to take ownership of the complexity in creating ZeroNode map tiles.
public partial class KoreZeroNodeMapManager : Node3D
{
    // Root game engine on which to parent map tiles
    //private Node ZeroNode;
    public static int CurrMaxMapLvl = 5; // default value - updated from config in constructor

    // Common map path point (save the config being queried excessively) // KoreZeroNodeMapManager.MapRootPath
    public static string MapRootPath = "";

    // Map load ref position
    public static KoreLLAPoint LoadRefLLA = new() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    public static KoreXYZVector LoadRefXYZ => LoadRefLLA.ToXYZ();
    public static float DistanceToHorizonM = 0;

    // Tile Action counter - Sets up a number of tile creation actions to be performed per _Process call.
    public static KoreActionCounter ActionCounter = new(10);

    // lvl0 tile list
    private List<KoreZeroNodeMapTile> Lvl0Tiles = new List<KoreZeroNodeMapTile>();

    private float currTimer = 0;
    private float currTimerIncrement = 1.0f; // 1sec

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreZeroNodeMapManager(Node zeroNode)
    {
        ///ZeroNode = zeroNode;

        // Read the debug flag from config
        if (KoreGodotFactory.Instance.Config == null)
        {
            GD.PrintErr("KoreZeroNodeMapManager: Config is null, cannot read settings.");
            return;
        }

        GD.Print($"KoreZeroNodeMapManager: Constructor");

        // Create and debug draw a lvl0 tile
        // Lvl0Tiles.Add(new KoreZeroNodeMapTile(new KoreMapTileCode("BF")));
        // Lvl0Tiles.Add(new KoreZeroNodeMapTile(new KoreMapTileCode("AG")));
        // Lvl0Tiles.Add(new KoreZeroNodeMapTile(new KoreMapTileCode("BG")));
        // Lvl0Tiles.Add(new KoreZeroNodeMapTile(new KoreMapTileCode("CG")));
        // Lvl0Tiles.Add(new KoreZeroNodeMapTile(new KoreMapTileCode("BH")));


        List<KoreMapTileCode> lvl0CodesList = KoreMapTileCode.Lvl0Codes();
        foreach (KoreMapTileCode lvl0Code in lvl0CodesList)
        {
            KoreZeroNodeMapTile tile = new KoreZeroNodeMapTile(lvl0Code);
            Lvl0Tiles.Add(tile);
        }


        // Loop through the tiles and rotate them onto their center longitude
       // int i = 0;
        foreach (KoreZeroNodeMapTile tile in Lvl0Tiles)
        {
            //i++;
            AddChild(tile);
            double lon = tile.TileCode.LLBox.CenterPoint.LonRads;
            //tile.Rotation = new Vector3(0, (float)lon, 0);

            //if (i > 20) break;
        }





        //CreateLvl0Tiles();

        CurrMaxMapLvl = 0;
        if (KoreGodotFactory.Instance.Config.Has("MaxMapLvl"))
            CurrMaxMapLvl = KoreStringDictionaryOps.ReadInt(KoreGodotFactory.Instance.Config, "MaxMapLvl");
        else
            KoreStringDictionaryOps.WriteInt(KoreGodotFactory.Instance.Config, "MaxMapLvl", 0);

        MapRootPath = "";
        if (KoreGodotFactory.Instance.Config.Has("MapRootPath"))
            MapRootPath = KoreGodotFactory.Instance.Config.Get("MapRootPath");
        else
            KoreGodotFactory.Instance.Config.Set("MapRootPath", MapRootPath);

        KoreSimFactory.Instance.SaveConfig(KoreSimFactory.ConfigPath);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialise the Manager node itself
        Name = "ZeroNodeMapManager";
        //CreateLvl0Tiles();

        GodotMeshPrimitives.AddChildDebugSphere(this, 0.1f, KoreColorPalette.Colors["DarkBlue"]);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        ActionCounter.Refresh(5); // Tile create actions per update cycle (frame)

        // If we have a timer, increment it
        if (currTimer < KoreCentralTime.RuntimeSecs)
        {
            currTimer = KoreCentralTime.RuntimeSecs + currTimerIncrement;

            // Get the latest camera and viewport
            Viewport viewport = GetViewport();
            Camera3D? camera = GetViewport().GetCamera3D();
            if (camera != null)
                KoreUnprojectManager.UpdateState(camera, viewport);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lvl0 Tiles
    // --------------------------------------------------------------------------------------------

    // Initialising call to create the top level tiles, which then recursively service the creation and display of all the lower level tiles.

    public void CreateLvl0Tiles()
    {
        // if (ZeroNode == null)
        // {
        //     GD.PrintErr("ERROR: ZeroNodeMapManager ZeroNode Null");
        //     return;
        // }

        KoreCentralLog.AddEntry("Creating Lvl0 Tiles");

        for (int latId = 0; latId < 6; latId++)
        {
            for (int lonId = 0; lonId < 12; lonId++)
            {
                KoreMapTileCode currTileCode = new KoreMapTileCode(lonId, latId);

                KoreZeroNodeMapTile tile = new KoreZeroNodeMapTile(currTileCode);
                AddChild(tile);
                Lvl0Tiles.Add(tile);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation Query
    // --------------------------------------------------------------------------------------------

    private KoreZeroNodeMapTile? GetLvl0Tile(KoreMapTileCode tileCode)
    {
        // Get the name of the first lvl0
        string tileName = tileCode.ToString();

        KoreMapTileCode? lvl0Code = KoreMapTileCode.CodeToLvl(tileCode, 0);
        if (lvl0Code == null) return null;

        // Get the tile node for this tile code
        return (KoreZeroNodeMapTile)FindChild(lvl0Code.ToString());
    }

    // Get the elevation at a given LL point, as loaded by the map tiles.

    public float GetElevation(KoreLLPoint llPoint)
    {
        KoreMapTileCode? tileCode = new KoreMapTileCode(llPoint.LatDegs, llPoint.LonDegs, 0);

        KoreZeroNodeMapTile? lvl0Tile = GetLvl0Tile(tileCode);

        if (lvl0Tile == null) return KoreElevationUtils.InvalidEle;

        float ele = KoreElevationUtils.InvalidEle;

        foreach (KoreZeroNodeMapTile currTile in Lvl0Tiles)
        {
            if (currTile.IsPointInTile(llPoint))
            {
                ele = currTile.GetElevation(llPoint);
                break;
            }
        }

        return lvl0Tile.GetElevation(llPoint);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public static void SetLoadRefLLA(KoreLLAPoint newLoadRefLLA)
    {
        LoadRefLLA = newLoadRefLLA;

        DistanceToHorizonM = (float)(KoreWorldOps.DistanceToHorizonM(LoadRefLLA.AltMslM));

        if (DistanceToHorizonM < 10000) KoreZeroNodeMapManager.DistanceToHorizonM = 10000; // minimise value at 10km
    }




    public static void SetMaxMapLvl(int maxMapLvl)
    {
        CurrMaxMapLvl = KoreValueUtils.Clamp(maxMapLvl, 0, KoreMapTileCode.MaxMapLvl);

        // Save the max map level to config

        var config = KoreGodotFactory.Instance.Config;
        config.Set("MaxMapLvl", CurrMaxMapLvl);
    }

    public void UpdateInfoVisibility(bool infoVisible)
    {
        GD.Print($"KoreZeroNodeMapManager: UpdateInfoVisibility {infoVisible}");

        foreach (KoreZeroNodeMapTile tile in Lvl0Tiles)
        {
            tile.UpdateInfoVisibility(infoVisible);
            tile.UpdateChildInfoVisibility(infoVisible);
        }
    }


}
