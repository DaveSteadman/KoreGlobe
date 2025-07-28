
using System;

using KoreCommon;
using KoreSim;

using Godot;

#nullable enable

// Class representing the zero anchor position in the game engine world.

public partial class KoreZeroNode : Node3D
{
    // The root node for all entities, made static so it can be accessed from anywhere.
    public static Node3D EntityRootNode = new Node3D() { Name = "EntityRootNode" };

    public static Node3D? ZeroNode = null;

    private static KoreLLAPoint ZeroPosToApply = KoreLLAPoint.Zero;

    private float Timer1Hz = 0.0f;

    // Usage: KoreZeroNode.ZeroNodeUpdateCycle
    static public bool ZeroNodeUpdateCycle { get; private set; } = false;

    static private bool UpdateTrigger = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "ZeroNode";

        // Setup the Zero Node.
        ZeroNode = this;

        // Add the EntityRootNode to the ZeroNode,
        //AddChild(EntityRootNode);

        // CreateDebugMarker();

        //GloLineMesh3D lineCube = new GloLineMesh3D();
        //AddChild(lineCube);

        //AddChild(GodotEntityManager);
        //CallDeferred("SetZeroNodePositionDeferred");
    }

    public override void _Process(double delta)
    {
        if ((Timer1Hz < KoreCentralTime.RuntimeSecs) || (UpdateTrigger))
        {
            UpdateTrigger = false;
            Timer1Hz = KoreCentralTime.RuntimeSecs + 2.0f;
            // CallDeferred("SetZeroNodePositionDeferred");
        }
        else
        {
            // CallDeferred("SetZeroNodePositionDeferred2");
            //ZeroNodeUpdateCycle = false;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: ZeroNode Position
    // --------------------------------------------------------------------------------------------

    // Usage: GloZeroNode.TriggerZeroPosUpdate();
    public static void TriggerZeroPosUpdate()
    {
        UpdateTrigger = true;
    }

    // Usage: KoreZeroNode.SetZeroNodePosition(pos);
    public static void SetZeroNodePosition(KoreLLAPoint pos)
    {
        ZeroPosToApply = pos;
    }

    // Usage: KoreZeroNode.SetZeroNodePosition(53.0, -6.0);
    public static void SetZeroNodePosition(double latDegs, double lonDegs)
    {
        // Set the position of the ZeroNode.
        KoreLLAPoint pos = new KoreLLAPoint()
        {
            LatDegs = latDegs,
            LonDegs = lonDegs,
            RadiusM = KoreWorldConsts.EarthRadiusM
        };

        ZeroPosToApply = pos;
    }

    // Internal function to actually apply the new zero offset position, of the node's own timeline.

    private void SetZeroNodePositionDeferred()
    {
        // Set the position of the ZeroNode.
        KoreZeroOffset.SetLLA(ZeroPosToApply);

        // Update the cnsequent positions of the ZeroNode and EarthCoreNode.
        KoreGodotFactory.Instance.ZeroNode.Position = KoreZeroOffset.GeZeroPoint();
        //KoreGodotFactory.Instance.EarthCoreNode.Position = KoreZeroOffset.GeCorePoint();

        ZeroNodeUpdateCycle = true;
    }

    private void SetZeroNodePositionDeferred2()
    {

        ZeroNodeUpdateCycle = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Helpers
    // --------------------------------------------------------------------------------------------

    private void CreateDebugMarker()
    {
        // float markerSize  = 2f;

        // Node3D zeroNodeMarker = KorePrimitiveFactory.CreateSphereNode("Marker", new Vector3(0f, 0f, 0f), markerSize, KoreColorUtil.Colors["OffRed"], true);
        // AddChild(zeroNodeMarker);

        // zeroNodeMarker.AddChild( KorePrimitiveFactory.AxisMarkerNode(markerSize, markerSize/4) );
    }
}
