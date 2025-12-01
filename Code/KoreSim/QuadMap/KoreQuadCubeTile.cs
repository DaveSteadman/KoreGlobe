
using System.Collections.Generic;
using KoreCommon;

namespace KoreSim;

// Define the tile for a quad cube map tile

#nullable enable

public class KoreQuadCubeTile
{
    public KoreQuadCubeTileCode Code { get; set; } = new();
    public KoreXYZVector RwCenter { get; set; } = KoreXYZVector.Zero; // real world center point of the tile
    public KoreColorMesh ColorMesh { get; set; } = new(); // the color mesh for this tile
}


