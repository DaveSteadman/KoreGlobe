// <fileheader>



using Godot;

using KoreGIS;
using KoreCommon;

#nullable enable

public class KoreFlatMapTileMarkers
{
    public Node3D? TL = null;
    public Node3D? TR = null;
    public Node3D? BL = null;
    public Node3D? BR = null;
    public Node3D? Center = null;

    public void CreateMarkers()
    {
        TL = KoreMiniMeshGodotNodeFactory.CreateMarker("TL", KoreColorPalette.Find("Magenta"), 0.05f);
        TR = KoreMiniMeshGodotNodeFactory.CreateMarker("TR", KoreColorPalette.Find("Magenta"), 0.05f);
        BL = KoreMiniMeshGodotNodeFactory.CreateMarker("BL", KoreColorPalette.Find("Magenta"), 0.05f);
        BR = KoreMiniMeshGodotNodeFactory.CreateMarker("BR", KoreColorPalette.Find("Magenta"), 0.05f);
        Center = KoreMiniMeshGodotNodeFactory.CreateMarker("Center", KoreColorPalette.Find("Magenta"), 0.05f);
    }

    public void AddToNode(Node3D parentNode)
    {
        if (TL != null) parentNode.AddChild(TL);
        if (TR != null) parentNode.AddChild(TR);
        if (BL != null) parentNode.AddChild(BL);
        if (BR != null) parentNode.AddChild(BR);
        if (Center != null) parentNode.AddChild(Center);
    }

    public void PlaceMarkers(KoreLLBox llBox)
    {

        KoreLLPoint centerLL = llBox.CenterPoint;
        KoreLLAPoint centerLLA = new KoreLLAPoint()
        {
            LatDegs = centerLL.LatDegs,
            LonDegs = centerLL.LonDegs,
            RadiusM = KoreFlatMapManager.GeRadius
        };

        KoreXYZVector centerXYZ = centerLLA.ToXYZ();
        Vector3 centerGeXYZ = KoreMovingOrigin.RWtoGodotOffset(centerXYZ);


        // Top Left
        double topLatDegs    = llBox.MaxLatDegs;
        double bottomLatDegs = llBox.MinLatDegs;
        double leftLonDegs   = llBox.MinLonDegs;
        double rightLonDegs  = llBox.MaxLonDegs;

        KoreLLAPoint tlLLA = new KoreLLAPoint() { LatDegs = topLatDegs,    LonDegs = leftLonDegs,  RadiusM = KoreFlatMapManager.GeRadius };
        KoreLLAPoint trLLA = new KoreLLAPoint() { LatDegs = topLatDegs,    LonDegs = rightLonDegs, RadiusM = KoreFlatMapManager.GeRadius };
        KoreLLAPoint blLLA = new KoreLLAPoint() { LatDegs = bottomLatDegs, LonDegs = leftLonDegs,  RadiusM = KoreFlatMapManager.GeRadius };
        KoreLLAPoint brLLA = new KoreLLAPoint() { LatDegs = bottomLatDegs, LonDegs = rightLonDegs, RadiusM = KoreFlatMapManager.GeRadius };

        KoreXYZVector tlXYZ = tlLLA.ToXYZ();
        KoreXYZVector trXYZ = trLLA.ToXYZ();
        KoreXYZVector blXYZ = blLLA.ToXYZ();
        KoreXYZVector brXYZ = brLLA.ToXYZ();

        KoreXYZVector centerPosXYZ = KoreMovingOrigin.RWtoOffset(centerXYZ);
        KoreXYZVector tlOffsetXYZ  = KoreMovingOrigin.RWtoOffset(tlXYZ);
        KoreXYZVector trOffsetXYZ  = KoreMovingOrigin.RWtoOffset(trXYZ);
        KoreXYZVector blOffsetXYZ  = KoreMovingOrigin.RWtoOffset(blXYZ);
        KoreXYZVector brOffsetXYZ  = KoreMovingOrigin.RWtoOffset(brXYZ);

        KoreXYZVector tlCenterOffsetXYZ = centerPosXYZ.XYZTo(tlOffsetXYZ);
        KoreXYZVector trCenterOffsetXYZ = centerPosXYZ.XYZTo(trOffsetXYZ);
        KoreXYZVector blCenterOffsetXYZ = centerPosXYZ.XYZTo(blOffsetXYZ);
        KoreXYZVector brCenterOffsetXYZ = centerPosXYZ.XYZTo(brOffsetXYZ);

        if (TL != null) TL.Position = KoreConvPos.VecToV3(tlCenterOffsetXYZ);
        if (TR != null) TR.Position = KoreConvPos.VecToV3(trCenterOffsetXYZ);
        if (BL != null) BL.Position = KoreConvPos.VecToV3(blCenterOffsetXYZ);
        if (BR != null) BR.Position = KoreConvPos.VecToV3(brCenterOffsetXYZ);
        Center!.Position = Vector3.Zero;
    }

}
