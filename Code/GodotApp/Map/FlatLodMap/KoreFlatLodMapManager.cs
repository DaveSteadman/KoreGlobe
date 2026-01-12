// <fileheader>

using System;
using System.Collections.Generic;

using KoreCommon;
using KoreGIS;

using Godot;

#nullable enable

// KoreFlatMapManager: Flat maps are drawn at the earth radius distance and allow us to render 2D content
// such as the satellite imagery, but also drawn content such as GeoJSON features, labels, icons.

public class KoreFlatLodMapManager
{
    Node3D? ParentNode = null;
    Node3D FlatTileCollectionNode = new Node3D() { Name = "FlatTileCollection" };

    public KoreMeshMaterial? GlobalTileMaterial = null;

    public static double GeRadius = 2;

    // --------------------------------------------------------------------------------------------

    public void CreateTestTile(Node3D parentNode)
    {
        ParentNode = parentNode;
        ParentNode.AddChild(FlatTileCollectionNode);

        CreateDefaultMaterial();

        bool testOne = false;

        if (testOne)
        {
            // Create the tilecode and tile mesh data
            KoreMapTileCode tileCode1 = new KoreMapTileCode("BF");
            KoreFlatMapTile tile1 = new KoreFlatMapTile(tileCode1, GlobalTileMaterial);
            FlatTileCollectionNode.AddChild(tile1);

            KoreMapTileCode tileCode2 = new KoreMapTileCode("CG");
            KoreFlatMapTile tile2 = new KoreFlatMapTile(tileCode2, GlobalTileMaterial);
            FlatTileCollectionNode.AddChild(tile2);
        }
        else
        {
            List<KoreMapTileCode> lvl0Codes = KoreMapTileCode.Lvl0Codes();
            foreach (KoreMapTileCode tc in lvl0Codes)
            {
                KoreFlatMapTile t = new KoreFlatMapTile(tc, GlobalTileMaterial);
                FlatTileCollectionNode.AddChild(t);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public void CreateDefaultMaterial()
    {
        var col = KoreColorPalette.Find("LightGray");

        var filename = "UnitTestArtefacts/world_map_world.png";
        GlobalTileMaterial = new KoreMeshMaterial("TileMatTex", col, 0.0f, 0.7f, filename);
    }
}


