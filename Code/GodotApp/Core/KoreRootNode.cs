using Godot;
using System;
using System.IO;
using System.Collections.Generic;

using KoreCommon;
using KoreSim;

public partial class KoreRootNode : Node3D
{
    Node3D PlaformBaseNode;
    Node3D ModelResourceNode;

    // TestZeroOffset.WorldCamNode.CamNode

    // public static GloCameraMoverWorld WorldCamNode;

    private float MarkerSize = 0.2f;
    private bool WithWire = true;

    private float AnimAzDegs = 0f;
    private float AnimElDegs = 0f;
    private float AnimAzDelta = 1.55f;
    private float AnimElDelta = 1.05f;

    private float UIPollTimer = 0.0f;
    private float UIPollTimer2 = 0.0f;
    private float PollTimerTrailNode = 0.0f;

    //private GloLLAPoint    PlatformPos;
    //private GloCourse      PlatformCourse;
    //private GloCourseDelta PlatformCourseDelta;

    private float Timer5Sec = 0.0f;
    private bool OneShot = false;

    private bool OneShot_5Seconds = false;


    //private float TimerContrail = 0.0f;

    // private GloCyclicIdGenerator IdGen = new GloCyclicIdGenerator(250);
    // private string randomString = GloRandomStringGenerator.GenerateRandomString(5);

    // --------------------------------------------------------------------------------------------
    // MARK: Node _Ready and _Process
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        KoreCentralLog.AddEntry($"TestZeroOffset: _Ready // {KoreGlobals.VersionString}");
        GD.Print($"=== TestZeroOffset: _Ready // {KoreGlobals.VersionString} ===================================== ");

        // Setup the factory that holds a centralised reference to key objects
        KoreGodotFactory.Instance.CreateObjects(this);

        // Default the zero point.
        // KoreZeroNode.SetZeroNodePosition(45, 0);

        // Debug report the world consts
        KoreZeroOffset.ReportConsts();


        // Read the manually included sets of assets
        bool loadModels = true;
        if (loadModels)
        {
            try
            {
                KoreCentralLog.AddEntry("ModelImport // Stage 1: Load JSON files");
                string jsonMilitary = KoreGodotFilesystemOps.ReadStringFromFile("res://Resources/Assets/MilitaryVehicles/Inventory.json");
                string jsonCivilian = KoreGodotFilesystemOps.ReadStringFromFile("res://Resources/Assets/CivilianVehicles/Inventory.json");
                string jsonPlaceholders = KoreGodotFilesystemOps.ReadStringFromFile("res://Resources/Assets/PlaceholderModels/Inventory.json");
                // KoreGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(jsonMilitary);
                // KoreGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(jsonCivilian);
                // KoreGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(jsonPlaceholders);

                // KoreCentralLog.AddEntry("ModelImport // Stage 2: Load Models");
                // KoreGodotFactory.Instance.ModelLibrary.LoadModelToCache();
                // KoreCentralLog.AddEntry("ModelImport // Done");

            }
            catch (Exception ex)
            {
                KoreCentralLog.AddEntry("ModelImport // EXCEPTION: {ex.Message}");
            }
        }

        // bool DLCTesting = true;

        // if (DLCTesting)
        // {
        //     //GloDlcOperations.CreateDlc();

        //     List<string> dlcList = GloDlcOperations.ListLoadableDlcPaths();

        //     foreach (string dlc in dlcList)
        //         GloDlcOperations.LoadDlc(dlc);

        //     KoreCentralLog.AddEntry( GloDlcOperations.DlcReport() );

        //     // Find the JSON files in each DLC and report on them
        //     List<string> dlcTitlesList = GloDlcOperations.ListLoadedDlcTitles();
        //     foreach (string currDlcTitle in dlcTitlesList)
        //     {
        //         string invJson = GloDlcOperations.InventoryJsonForDLCTitle(currDlcTitle);
        //         KoreCentralLog.AddEntry($"DLC: {currDlcTitle} JSON: {invJson}");
        //     }

        //     string invJson2 = GloDlcOperations.InventoryJsonForDLCTitle("PlaceholderModels");
        //     KoreGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(invJson2);

        //     string invJson3 = GloDlcOperations.InventoryJsonForDLCTitle("MilitaryVehicles");
        //     KoreGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(invJson3);

        //     KoreCentralLog.AddEntry( KoreGodotFactory.Instance.ModelLibrary.ReportContent() );
        // }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!OneShot)
        {
            OneShot = true;
            GD.Print("OneShot: _Process");
            KoreCentralLog.AddEntry("OneShot: _Process");

        }

        if (!OneShot_5Seconds)
        {
            if (KoreCentralTime.RuntimeIntSecs > 5)
            {
                OneShot_5Seconds = true;
                KoreCentralLog.AddEntry("OneShot_5Seconds: _Process");
                GD.Print("OneShot_5Seconds: _Process");

                // KoreGodotFactory.Instance.ModelLibrary.CheckModelLoad();
                //var packedScene = GD.Load<PackedScene>("res://Scenes/TreeView.tscn");
                var packedScene = GD.Load<PackedScene>("res://Scenes/ui_top.tscn");

                // Instance it (turn PackedScene into a live node)
                Control newNode = packedScene.Instantiate<Control>();

                // Add it to the current node (or wherever you want)
                AddChild(newNode);

            }
        }

        // trigger every 5 seconds
        if (KoreCentralTime.RuntimeIntSecs > Timer5Sec)
        {
            Timer5Sec = KoreCentralTime.RuntimeIntSecs + 5;

            // Randomize the zero point in each run so we don't bake-in assumptions.
            // double randomLat = 50;  //GloValueUtils.RandomInRange(45, 60);
            // double randomLon = -2;  //GloValueUtils.RandomInRange(-8, -0);

            // GloZeroNode.SetZeroNodePosition(randomLat, randomLon);

            // KoreGodotFactory.Instance.ModelLibrary.CheckModelLoad();


        }
    }
}
