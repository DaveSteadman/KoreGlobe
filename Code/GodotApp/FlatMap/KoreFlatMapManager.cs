
using System;
using System.Collections.Generic;

using KoreCommon;
using KoreGIS;

using Godot;

#nullable enable

// KoreFlatMapManager: Flat maps are drawn at the earth radius distance and allow us to render 2D content
// such as the satellite imagery, but also drawn content such as GeoJSON features, labels, icons.

public class KoreFlatMapManager
{
    Node3D? ParentNode = null;
    Node3D FlatTileCollectionNode = new Node3D() { Name = "FlatTileCollection" };

    public static double GeRadius = 2;

    public void CreateTestTile(Node3D parentNode)
    {
        ParentNode = parentNode;
        ParentNode.AddChild(FlatTileCollectionNode);

        // Create the tilecode and tile mesh data
        KoreMapTileCode tileCode = new KoreMapTileCode("BF");
        KoreFlatMapTile tile = new KoreFlatMapTile(tileCode);

        List<KoreMapTileCode> lvl0Codes = KoreMapTileCode.Lvl0Codes();

        foreach (KoreMapTileCode tc in lvl0Codes)
        {
            KoreFlatMapTile t = new KoreFlatMapTile(tc);
            FlatTileCollectionNode.AddChild(t);
        }


    }
}