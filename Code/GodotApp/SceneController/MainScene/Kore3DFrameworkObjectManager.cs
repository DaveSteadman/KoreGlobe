using Godot;
using System;
using System.Runtime;

using KoreCommon;
using KoreSim;
using System.Collections.Generic;

#nullable enable

// Kore3DFrameworkObjectManager: Create a number of test and layout nodes in the relocatable scene
// that are useful for analysis and debugging.

public class Kore3DFrameworkObjectManager
{
    public Node3D? MarkerZeroNodesParent = null;

    public Node3D? ZeroMarkerNode = null;
    public Node3D? ZeroMarkerShellNode = null;
    public Node3D? ZeroOffsetMarkerNode = null;

    public Node3D? XAxisNode = null;
    public Node3D? YAxisNode = null;
    public Node3D? ZAxisNode = null;

    // ---------------------------------------------------------------------------------------------

    public void CreateNodes(Node3D parentNode)
    {
        MarkerZeroNodesParent = new Node3D() { Name = "FrameworkMarkers" };
        parentNode.AddChild(MarkerZeroNodesParent);

        // Zero - Earth Core
        ZeroMarkerNode = KoreMiniMeshGodotNodeFactory.CreateMarker("ZeroMarker", KoreColorPalette.Find("Cyan"), 0.5f);
        if (ZeroMarkerNode != null) MarkerZeroNodesParent.AddChild(ZeroMarkerNode);

        // Zero - Earth Surface (Wireframe only)
        ZeroMarkerShellNode = KoreMiniMeshGodotNodeFactory.CreateSphere(
            "ZeroMarkerShell",
            position: KoreXYZVector.Zero, radius: 1f, segments: 32,
            KoreMiniMeshMaterialPalette.DefaultMaterial, KoreColorRGB.White,
            includeSurface: false, includeWireframe: true);
        if (ZeroMarkerShellNode != null) ZeroMarkerNode!.AddChild(ZeroMarkerShellNode);

        // Zero Offset - Relocatable Origin
        ZeroOffsetMarkerNode = KoreMiniMeshGodotNodeFactory.CreateMarker("ZeroOffsetMarker", KoreColorPalette.Find("Red"), 0.1f);
        if (ZeroOffsetMarkerNode != null) MarkerZeroNodesParent.AddChild(ZeroOffsetMarkerNode);

        if (ZeroMarkerNode != null)
        {
            // Create axis spheres - offset by 1 GE unit along each axis (XYZ => RBG)
            XAxisNode = KoreMiniMeshGodotNodeFactory.CreateMarker("XAxisMarker", KoreColorPalette.Find("Red"), 0.2f, alpha: 0.75f);
            if (XAxisNode != null) MarkerZeroNodesParent.AddChild(XAxisNode);

            YAxisNode = KoreMiniMeshGodotNodeFactory.CreateMarker("YAxisMarker", KoreColorPalette.Find("Green"), 0.2f, alpha: 0.75f);
            if (YAxisNode != null) MarkerZeroNodesParent.AddChild(YAxisNode);

            ZAxisNode = KoreMiniMeshGodotNodeFactory.CreateMarker("ZAxisMarker", KoreColorPalette.Find("Blue"), 0.2f, alpha: 0.75f);
            if (ZAxisNode != null) MarkerZeroNodesParent.AddChild(ZAxisNode);
        }
        PlaceNodes();
    }

    // ---------------------------------------------------------------------------------------------

    public void UpdateNodes()
    {
        // Are we in an update cycle?
        if (!KoreMovingOrigin.IsChangePeriod()) return;

        // calc and place the nodes
        PlaceNodes();
    }

    public void PlaceNodes()
    {
        // Zero node goes at the RW 0,0,0 position
        if (ZeroMarkerNode != null)
        {
            KoreXYZVector zeroPos = KoreXYZVector.Zero;
            Godot.Vector3 geZeroPos = KoreMovingOrigin.RWtoGodotOffset(zeroPos);
            ZeroMarkerNode.Position = geZeroPos;
        }

        // Offset position goes at the RW Offset, which should calculate to the GE position of zero
        if (ZeroOffsetMarkerNode != null)
        {
            KoreXYZVector offsetPos = KoreMovingOrigin.RwOrigin;
            Godot.Vector3 geOffsetPos = KoreMovingOrigin.RWtoGodotOffset(offsetPos);
            ZeroOffsetMarkerNode.Position = geOffsetPos;
        }

        if (ZeroMarkerNode != null)
        {
            if (XAxisNode != null) XAxisNode.Position = KoreMovingOrigin.RWtoGodotOffset(new KoreXYZVector(1.0f, 0.0f, 0.0f));
            if (YAxisNode != null) YAxisNode.Position = KoreMovingOrigin.RWtoGodotOffset(new KoreXYZVector(0.0f, 1.0f, 0.0f));
            if (ZAxisNode != null) ZAxisNode.Position = KoreMovingOrigin.RWtoGodotOffset(new KoreXYZVector(0.0f, 0.0f, 1.0f));
        }

        string p1Str = KoreConvPos.Vector3Str(ZeroOffsetMarkerNode!.Position);
        string p2Str = KoreConvPos.Vector3Str(ZeroMarkerNode!.Position);
        GD.Print($"ZeroOffsetMarkerNode.Position {p1Str} // ZeroMarkerNode.Position {p2Str}");
    }

    // ---------------------------------------------------------------------------------------------

    private void CreateMarkerNode(ref Node3D? nodeRef, string nodeName, KoreColorRGB koreColor, float size)
    {
        if (nodeRef != null) return;

        nodeRef = KoreMiniMeshGodotNodeFactory.CreateMarker(nodeName, koreColor, size, alpha: 0.75f);
    }

}


