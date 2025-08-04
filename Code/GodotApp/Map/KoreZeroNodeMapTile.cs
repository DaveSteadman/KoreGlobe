using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot.NativeInterop;

using Godot;
using KoreCommon;
using KoreSim;

#nullable enable

// ------------------------------------------------------------------------------------------------

public class KoreZeroTileVisibilityStats
{
    public float distanceToHorizonM;
    public float distanceToTileCenterM;
    public float distanceFraction;

    public KoreZeroTileVisibilityStats()
    {
        // Default the stats to large values of tiles that would not be displayed
        distanceToHorizonM    = 1000000.0f;
        distanceToTileCenterM = 1000000.0f;
        distanceFraction      = 1.0f;
    }
}

// ------------------------------------------------------------------------------------------------

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreZeroNodeMapTile : Node3D
{
    // Tile Code and tile centre positions - Setup early in tile creation and then fixed.
    public  KoreMapTileCode  TileCode        = KoreMapTileCode.Zero;
    private KoreLLAPoint     RwTileCenterLLA = KoreLLAPoint.Zero; // Shortcut from the tilecode center
    private KoreXYZPoint     RwTileCenterXYZ = KoreXYZPoint.Zero; // Shortcut from the tilecode center
    private KoreLLBox        RwTileLLBox     = KoreLLBox.Zero; // Shortcut from the tilecode center

    // Parent/child tiles relationships
    public KoreZeroNodeMapTile?      ParentTile = null;
    public List<KoreZeroNodeMapTile> ChildTiles = new List<KoreZeroNodeMapTile>();

    // File paths used in creating the tile - setup early in creat process
    private KoreMapTileFilepaths Filepaths;

    // Real world elevation data, defining the tile's geometry
    public KoreNumeric2DArray<float> TileEleData  = new KoreNumeric2DArray<float>();
    private KoreXYZVector[,] v3Data;
    private KoreXYZVector[,] v3DataBottom;

    // UV Box - Child accessible values
    public KoreNumericRange<float> UVx   = KoreNumericRange<float>.ZeroToOne;
    public KoreNumericRange<float> UVy   = KoreNumericRange<float>.ZeroToOne;
    public KoreUVBoxDropEdgeTile   UVBox = KoreUVBoxDropEdgeTile.FullImage();

    // Materials and Meshes
    private ArrayMesh           TileMeshData;
    private Color               WireColor;
    public  StandardMaterial3D? TileMaterial = null;
    private Vector3             TileLabelOffset;
    private Vector3             TileLabelOffsetN;
    private Vector3             TileLabelOffsetBelow;
    public  bool                TileOwnsTexture = false;

    // Godot Game Engine objects
    // private MeshInstance3D MeshInstance  = new();
    // private KoreLineMesh3D  MeshInstanceW = new();
    
    private KoreMeshData TileMesh = new();
    
    private Label3D TileCodeLabel;
    private List<Node3D> GEElements = new List<Node3D>();

    // Internal timings - list to add some randomness to the update process
    public  KoreRandomLoopList RandomLoopList = new KoreRandomLoopList(10, 0.4f, 0.41f);
    private float              UIUpdateTimer  = 0.0f;

    // Construction Flags
    private bool ChildTileDataAvailable         = false;
    private bool BackgroundConstructionComplete = false;
    private bool ConstructionComplete           = false;
    private int  ConstructionStage              = 0;

    // Property to check if the material loading is done
    public bool IsDone => ConstructionComplete;

    // Running State Flags - Updated based on visibility rules and child tiles.
    public bool ActiveVisibility      = false;
    public bool ChildrenSetVisible    = false; // flag to more check intended child visibility state
    public bool VisibleState          = false;
    public bool ChildrenVisibleState  = false;
    public bool ChildrenActiveState   = false;

    // private bool ChildConstructionComplete    = false;

    private KoreLatestHolder<KoreZeroTileVisibilityStats> TileVisibilityStats = new KoreLatestHolder<KoreZeroTileVisibilityStats>( new KoreZeroTileVisibilityStats() );

    // --------------------------------------------------------------------------------------------

    // Internal constants - distances controlling tile visbility

    public static readonly int[]   TileSizePointsPerLvl    = [ 40,    40,     40,     40,      40,         40 ];
    public static readonly float[] LabelSizePerLvl         = [ 0.70f, 0.10f,  0.030f, 0.0040f, 0.00065f,   0.000060f ];
    public static readonly int[] dpPerLvl = { 0, 0, 0, 1, 2, 3};

    public static readonly float[] ChildTileDisplayForLvl  = [ 0.8f,  0.10f,  0.02f,  0.0045f, 0.00120f, 0.00000050f ];
    public static readonly float[] CreateChildTilesForLvl  = [ 1.0f,  0.15f,  0.04f,  0.0050f, 0.00140f, 0.00000055f ];
    public static readonly float[] DeleteChildTilesForLvl  = [ 1.2f,  0.25f,  0.80f,  0.0080f, 0.00180f, 0.00000070f ];

    public static readonly float[] ChildTileDisplayMForLvl = [ 500000f,  100000f,  20000f,  5000f, 1000f, 100f];
    public static readonly float[] CreateChildTilesMForLvl = [ 600000f,  200000f,  25000f,  7000f, 2000f, 200f];
    public static readonly float[] DeleteChildTilesMForLvl = [ 700000f,  300000f,  30000f,  9000f, 3000f, 300f];

    // --------------------------------------------------------------------------------------------

    //                                                               Lvl0, Lvl1, Lvl2, Lvl3, Lvl4, Lvl5
    public static readonly double[] childTileDisplayDistKmForLvl = { 2500, 800,  100,  20,   5,    1};

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreZeroNodeMapTile(KoreMapTileCode tileCode)
    {
        // Set the core Tilecode and node name.
        TileCode    = tileCode;
        Name        = tileCode.ToString();

        KoreCentralLog.AddEntry($"Creating KoreZeroNodeMapTile: {TileCode}");

        // Fire off the fully background task of creating/loading the tile elements asap.
        Task.Run(() => BackgroundTileCreation(tileCode));
        //Task.Run(() => DetermineChildTileAvailability(tileCode));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print($"Tile Ready: {TileCode}");
        
        TileVisibilityStats.LatestValue = new KoreZeroTileVisibilityStats();
        Task.Run(() => BackgroundProcessing());
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // GD.Print($"Tile Process: {TileCode} - Stage: {ConstructionStage} // {ConstructionComplete} {BackgroundConstructionComplete}");

        // If we are still building the tile, progress that
        if (!ConstructionComplete)
        {
            // If the tile is not fully constructed, but the background construction has concluded, process
            // the final construction steps in stages.
            if (BackgroundConstructionComplete)
            {
                GD.Print($"Tile: {TileCode} - Construction Stage: {ConstructionStage}");
                MainThreadFinalizeCreation();
                return;
            }
        }
        else
        {
            // If the tile is fully created, and we're in a cycle where the aero offset has moved, update the tile position.
            // Note that tile orientation is never changed, this is setup during the tile creation.
            if (KoreZeroNode.ZeroNodeUpdateCycle)
                LocateTile();

            // Slow the tile visibility processing down to a manageable pace
            if (UIUpdateTimer < KoreCentralTime.RuntimeSecs)
            {
                // Increment the timer by a slightly random amount to avoid all the tiles updating at the same time
                UIUpdateTimer = KoreCentralTime.RuntimeSecs + RandomLoopList.GetNext();
                UpdateVisbilityRules();
            }
        }
    }
    // --------------------------------------------------------------------------------------------

    public override void _ExitTree()
    {
        // Free the material and its texture if the tile owns it
        if (TileMaterial != null)
        {
            if (TileOwnsTexture && TileMaterial.AlbedoTexture != null)
            {
                TileMaterial.AlbedoTexture.Dispose();
                TileMaterial.AlbedoTexture = null;

                TileMaterial.Dispose();
                TileMaterial = null;
            }
        }

        // Free the mesh data
        if (TileOwnsTexture && TileMeshData != null)
        {
            TileMeshData.Dispose();
        }
        TileMeshData = null;

        // Free any other GPU resources
        // if (TileMesh != null)
        // {
        //     TileMesh.QueueFree();
        //     MeshInstance = null;
        // }

        // if (MeshInstanceW != null)
        // {
        //     MeshInstanceW.QueueFree();
        //     MeshInstanceW = null;
        // }

        if (TileCodeLabel != null)
        {
            TileCodeLabel.QueueFree();
            TileCodeLabel = null;
        }

        base._ExitTree();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Action Counter
    // --------------------------------------------------------------------------------------------

    public bool GrabbedActionCounter() => KoreGodotFactory.Instance.ZeroNodeMapManager.ActionCounter.TryConsume();

    // --------------------------------------------------------------------------------------------
    // MARK: Child Tiles
    // --------------------------------------------------------------------------------------------

    public void CreateChildTiles()
    {
        if (!TileCode.IsValid())
        {
            KoreCentralLog.AddEntry("Invalid tile code for child tile creation.");
            return;
        }

        List<KoreMapTileCode> childCodes = TileCode.ChildCodesList();

        foreach (KoreMapTileCode currcode in childCodes)
            ChildTiles.Add(new KoreZeroNodeMapTile(currcode));

        foreach (var currChildTile in ChildTiles)
        {
            AddChild(currChildTile);
            currChildTile.ParentTile = this;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Locate
    // --------------------------------------------------------------------------------------------

    // Locate the tile in the game engine. Note that the camera does all the rotations, the relocatable
    // gemotry is always aligned to the world axis, and the tile is always at the same orientation.
    // So this is just a translation.

    private void LocateTile()
    {
        // Set the local position from the parent object
        Vector3 newPos = KoreGeoConvOperations.RwToOffsetGe(RwTileCenterLLA);

        // Set the local position from the parent object
        var transform    = GlobalTransform;
        transform.Origin = newPos;
        GlobalTransform  = transform;
    }

}
