using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot.NativeInterop;

using Godot;
using KoreCommon;
using KoreSim;
using KoreGIS;

#nullable enable

// ------------------------------------------------------------------------------------------------

// public class KoreQuadZNTileVisibilityStats
// {
//     public float distanceToHorizonM;
//     public float distanceToTileCenterM;
//     public float distanceFraction;

//     public KoreQuadZNTileVisibilityStats()
//     {
//         // Default the stats to large values of tiles that would not be displayed
//         distanceToHorizonM    = 1000000.0f;
//         distanceToTileCenterM = 1000000.0f;
//         distanceFraction      = 1.0f;
//     }
// }

// ------------------------------------------------------------------------------------------------

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreQuadZNMapTile : Node3D
{
    public KoreQuadCubeTileCode TileCode { get; set; } = KoreQuadCubeTileCode.Zero;
    public string TileCodeStr { get; set; } = "Zero";
    public KoreQuadFace TileQuadFace { get; set; } = KoreQuadFace.Zero;

    // Tile Code and tile centre positions - Setup early in tile creation and then fixed.
    private KoreLLAPoint RwTileCenterLLA = KoreLLAPoint.Zero; // Shortcut from the tilecode center
    public KoreXYZVector RwTileCenterXYZ = KoreXYZVector.Zero; // Shortcut from the tilecode center
    // private KoreLLBox        RwTileLLBox     = KoreLLBox.Zero; // Shortcut from the tilecode center

    // Parent/child tiles relationships
    public KoreQuadZNMapTile? ParentTile = null;
    public List<KoreQuadZNMapTile> ChildTiles = new List<KoreQuadZNMapTile>();

    public double DrawRadius = 0.0;

    private bool tileValid = true; // internal validity flag - set false when the tile should be deleted and background processing stopped
    private bool ConstructionComplete = false; // Flag set when the tile is fully constructed

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreQuadZNMapTile(KoreQuadCubeTileCode tileCode, double drawRadius)
    {
        // Set the core Tilecode and node name.
        TileCode = tileCode;
        TileCodeStr = TileCode.CodeToString();
        Name = TileCodeStr;

        DrawRadius = drawRadius;

        // Initial flag state
        ConstructionComplete = false;
        tileValid = true;

        // Get the face from the tilecode
        TileQuadFace = KoreQuadFaceOps.QuadrantOnFace(TileCode);
        RwTileCenterXYZ = TileQuadFace.Center;
        RwTileCenterXYZ = RwTileCenterXYZ.Scale(DrawRadius);
        //RwTileCenterXYZ = new KoreXYZVector(0,0,13.0f); // Place the tile at a fixed radius for now

        // Debug
        DebugSphere();

        string vecstr = KoreXYZVectorIO.ToStringWithDP(RwTileCenterXYZ, 4);
        GD.Print($"Creating KoreQuadZNMapTile: {TileCodeStr} at {vecstr}");

        // Fire off the fully background task of creating/loading the tile elements asap.
        Task.Run(() => BackgroundTileCreation());
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // GD.Print($"Tile Ready: {TileCode}");
        KoreCentralLog.AddEntry($"Creating KoreQuadZNMapTile: {TileCode.CodeToString()}");

        // TileVisibilityStats.LatestValue = new KoreZeroTileVisibilityStats();

        // Kick-off the background tile processing (for issues like visibility).
        // Task.Run(() => BackgroundProcessing());

        // Define the real world tile center positions that we use to anchor the tile in the GE world.
        // RwTileCenterLLA = new KoreLLAPoint(KoreQuadFaceOps.AverageLL(TileQuadFace), DrawRadius);
        // RwTileCenterXYZ = RwTileCenterLLA.ToXYZ();

        //RwTileCenterXYZ = TileQuadFace.Center;
        //RwTileCenterXYZ.Magnitude = DrawRadius;

        CreateTileFromData();

        //     // KoreColorMesh colorMesh = KoreColorMeshPrimitives.Tile(TileCode, tileEleData, colormap);
        //     // //KoreColorMesh colorMesh = KoreColorMeshPrimitives.BasicSphere(KoreXYZVector.Zero, 1f, colormap);
        //     // KoreColorMeshGodot colorMeshNode = new KoreColorMeshGodot();
        //     // colorMeshNode.UpdateMesh(colorMesh);
        //     // AddChild(colorMeshNode);

        //     //GD.Print($"{TileCode} COLORMESH // Vertices: {tileMesh.Vertices.Count} // Triangles: {tileMesh.Triangles.Count}");
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // GD.Print($"Tile Process: {TileCode} - Stage: {ConstructionStage} // {ConstructionComplete} {BackgroundConstructionComplete}");

        // if (!ConstructionComplete)
        // {

        // }
        // else
        {
            // // IF we're in a pos-change cycle, update the tile location
            // if (KoreRelocateOps.IsChangePeriod())
            //     UpdateTileLocation();
        }

        //     // If we are still building the tile, progress that
        //     if (!ConstructionComplete)
        //     {
        //         // If the tile is not fully constructed, but the background construction has concluded, process
        //         // the final construction steps in stages.
        //         if (BackgroundConstructionComplete)
        //         {
        //             // GD.Print($"Tile: {TileCode} - Construction Stage: {ConstructionStage}");
        //             if (GrabbedActionCounter())
        //                 CallDeferred(nameof(MainThreadFinalizeCreation));
        //             return;
        //         }
        //     }
        //     else
        //     {
        //         // If the tile is fully created, and we're in a cycle where the aero offset has moved, update the tile position.
        //         // Note that tile orientation is never changed, this is setup during the tile creation.

        //         // Slow the tile visibility processing down to a manageable pace
        //         if (UIUpdateTimer < KoreCentralTime.RuntimeSecs)
        //         {
        //             // Increment the timer by a slightly random amount to avoid all the tiles updating at the same time
        //             UIUpdateTimer = KoreCentralTime.RuntimeSecs + RandomLoopList.GetNext();
        //             CallDeferred(nameof(UpdateVisbilityRules));
        //         }
        //     }
    }

    // --------------------------------------------------------------------------------------------

    // public override void _ExitTree()
    // {
    //     // Clear large data arrays immediately
    //     // TileColormap = null;
    //     // TileEleData = new KoreNumeric2DArray<float>(); // Reset to empty
    //     TileColorMesh = null;

    //     // Aggressively clear child collections
    //     foreach (var child in ChildTiles)
    //     {
    //         child?.QueueFree();
    //     }
    //     ChildTiles.Clear();

    //     foreach (var element in GEElements)
    //     {
    //         element?.QueueFree();
    //     }
    //     GEElements.Clear();

    //     // Free Godot nodes
    //     ColorMeshNode?.QueueFree();
    //     ColorMeshNode = null;

    //     TileCodeLabel?.QueueFree();
    //     TileCodeLabel = null;

    //     // Force immediate cleanup
    //     GC.Collect(0, GCCollectionMode.Optimized);

    //     base._ExitTree();
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: Action Counter
    // // --------------------------------------------------------------------------------------------


    // --------------------------------------------------------------------------------------------
    // MARK: Background
    // --------------------------------------------------------------------------------------------

    // Create rules:
    // - Need to have the elevation file to gate the creation of a child tile, or it blocks the
    //   creation of the while set. No more exprapolation of elevation data.
    // - Will use the tile's own image, the parent tile's image if it exists, or a default image if it doesn't.

    private async Task BackgroundTileCreation()
    {
        try
        {
            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // // Define he tile's quadrant face
            // KoreQuadFace TileQuadFace = KoreQuadFaceOps.QuadrantOnFace(TileCode);


            // bool tileinDB = false; // KoreQuadZNMapTileDBManager.HasBytesForName(TileCodeStr);

            // if (tileinDB)
            //     LoadTileFromDB();
            // else
            //     CreateTileFromData();

        }
        catch (Exception ex)
        {
            KoreCentralLog.AddEntry($"Error in KoreQuadZNMapTile BackgroundTileCreation: ({TileCode.CodeToString()}) // {ex.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------

    private async Task BackgroundProcessing()
    {
        try
        {
            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // Loop forever, doing periodic processing
            while (tileValid)
            {
                if (!ConstructionComplete)
                    continue;

                // Wait for the next cycle
                await Task.Delay(TimeSpan.FromSeconds(1.0));

                // Do periodic background processing here
            }
        }
        catch (Exception ex)
        {
            KoreCentralLog.AddEntry($"Error in KoreQuadZNMapTile BackgroundProcessing: ({TileCode.CodeToString()}) // {ex.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Child Tiles
    // --------------------------------------------------------------------------------------------

    private void LoadTileFromDB()
    {

    }

    private void CreateTileFromData()
    {
        KoreQuadCubeTile tile = KoreQuadCubeTileFactory.TileForCode3(TileCode, radius: DrawRadius);

        KoreColorMeshGodot coloredMeshNode = new() { Name = $"QuadMesh_{TileCodeStr}" };
        AddChild(coloredMeshNode);

        coloredMeshNode.UpdateMesh(tile.ColorMesh);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Locate
    // --------------------------------------------------------------------------------------------

    // Locate the tile in the game engine. Note that the camera does all the rotations, the relocatable
    // gemotry is always aligned to the world axis, and the tile is always at the same orientation.
    // So this is just a translation.

    private void UpdateTileLocation()
    {
        // Set the local position from the parent object
        //Vector3 newPos = KoreGeoConvOps.RwToOffsetGe(RwTileCenterLLA);
        var gePos = KoreMovingOrigin.RWtoRWOffsetG(RwTileCenterXYZ);

        // Set the local position from the parent object
        var transform = GlobalTransform;
        transform.Origin = gePos;
        GlobalTransform = transform;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Debug
    // --------------------------------------------------------------------------------------------

    private void DebugSphere()
    {
        float sphereRadius = 1f;

        // Create a debug sphere at the tile center
        var sphereMesh = new SphereMesh();
        sphereMesh.Radius = sphereRadius;
        sphereMesh.Height = sphereRadius * 2;
        sphereMesh.RadialSegments = 8;
        sphereMesh.Rings = 4;

        var sphereInstance = new MeshInstance3D() { Name = $"DebugSphere_{TileCodeStr}" };
        sphereInstance.Mesh = sphereMesh;

        sphereInstance.MaterialOverlay = new StandardMaterial3D()
        {
            AlbedoColor = new Color(1, 0, 0, 0.5f),
            Metallic = 0.0f,
            Roughness = 0.5f
        };
        // sphereInstance.Translation = new Vector3(0, 0, 0);
        AddChild(sphereInstance);
    }

}
