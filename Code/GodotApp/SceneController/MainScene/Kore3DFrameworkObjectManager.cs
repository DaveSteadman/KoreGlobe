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
    public Node3D? ZeroOffsetMarkerNode = null;

    public Node3D? XAxisNode = null;
    public Node3D? YAxisNode = null;
    public Node3D? ZAxisNode = null;

    // ---------------------------------------------------------------------------------------------

    public void CreateNodes(Node3D parentNode)
    {
        MarkerZeroNodesParent = new Node3D() { Name = "FrameworkMarkers" };
        parentNode.AddChild(MarkerZeroNodesParent);

        CreateMarkerNode(ref ZeroMarkerNode, "ZeroMarker", KoreColorPalette.Find("Cyan"), .5f);
        if (ZeroMarkerNode != null) MarkerZeroNodesParent.AddChild(ZeroMarkerNode);

        CreateMarkerNode(ref ZeroOffsetMarkerNode, "ZeroOffsetMarker", KoreColorPalette.Find("Red"), .5f);
        if (ZeroOffsetMarkerNode != null) MarkerZeroNodesParent.AddChild(ZeroOffsetMarkerNode);

        if (ZeroMarkerNode != null)
        {
            // Create axis spheres - offset by 1 GE unit along each axis (XYZ => RBG)
            CreateMarkerNode(ref XAxisNode, "XAxisMarker", KoreColorPalette.Find("Red"), .2f);
            if (XAxisNode != null) MarkerZeroNodesParent.AddChild(XAxisNode);
            CreateMarkerNode(ref YAxisNode, "YAxisMarker", KoreColorPalette.Find("Green"), .2f);
            if (YAxisNode != null) MarkerZeroNodesParent.AddChild(YAxisNode);
            CreateMarkerNode(ref ZAxisNode, "ZAxisMarker", KoreColorPalette.Find("Blue"), .2f);
            if (ZAxisNode != null) MarkerZeroNodesParent.AddChild(ZAxisNode);
        }
        PlaceNodes();
    }

    // ---------------------------------------------------------------------------------------------

    public void UpdateNodes()
    {
        // Are we in an update cycle?
        if (!KoreMovingOrigin.IsNewOffsetPending()) return;

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
            if (XAxisNode != null) XAxisNode.Position = new Godot.Vector3(1.0f, 0.0f, 0.0f);
            if (YAxisNode != null) YAxisNode.Position = new Godot.Vector3(0.0f, 1.0f, 0.0f);
            if (ZAxisNode != null) ZAxisNode.Position = new Godot.Vector3(0.0f, 0.0f, 1.0f);

        }
    }

    // ---------------------------------------------------------------------------------------------

    private void CreateMarkerNode(ref Node3D? nodeRef, string nodeName, KoreColorRGB koreColor, float size)
    {
        if (nodeRef != null) return;

        // Create a marker node using KoreMiniMesh (similar to CreateDebugMarker)
        nodeRef = new Node3D() { Name = nodeName };

        // Convert Godot Color to KoreColorRGB
        // KoreColorRGB koreColor = new KoreColorRGB(color.R, color.G, color.B);

        // Create material from color
        koreColor.A = KoreColorIO.FloatToByte(0.75f);
        KoreMiniMeshMaterial mat = new KoreMiniMeshMaterial("TempMaterial", koreColor, 0.4f, 0.4f);
        KoreColorRGB lineCol = KoreColorRGB.White;

        // Create sphere mesh using KoreMiniMesh primitives
        KoreMiniMesh sphereMesh = KoreMiniMeshPrimitives.BasicSphere(KoreXYZVector.Zero, size, 16, mat, lineCol);

        // Add colored surface
        KoreMiniMeshGodotColoredSurface coloredMeshNode = new KoreMiniMeshGodotColoredSurface() { Name = nodeName + "_Surface" };
        coloredMeshNode.UpdateMesh(sphereMesh, "All");
        nodeRef.AddChild(coloredMeshNode);

        // Add wireframe lines
        KoreMiniMeshGodotLine lineMeshNode = new KoreMiniMeshGodotLine() { Name = nodeName + "_Wire" };
        lineMeshNode.UpdateMesh(sphereMesh, "All");
        nodeRef.AddChild(lineMeshNode);
    }

}


