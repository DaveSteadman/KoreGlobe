using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace KoreCommon;

// KoreMeshData: A class to hold mesh data for 3D geometry.
// - points, lines, triangles, normals, UVs, vertex colors, line colors, and materials.

// COORDINATE SYSTEM SPECIFICATION:
// - X+: Right, Y+: Up, Z-: Forward (Godot native)
// - UVs use top-left origin (Godot/OpenGL style): U (X) incrementing right, V (Y) incrementing down to a 1,1 bottom right
// - Triangle winding: clockwise when viewed from outside (Godot native)


// Quick structure for a record of three index values for corners of a triangle
public readonly record struct KoreMeshIndex2(int A, int B);
public readonly record struct KoreMeshIndex3(int A, int B, int C);

public readonly record struct KoreMeshLineRef(
    KoreMeshIndex2 V,                 // position indices
    int            C                  // color index
);

public readonly record struct KoreMeshTriRef(
    KoreMeshIndex3 V,                 // position indices
    int            N,                 // normal indices
    KoreMeshIndex3 UV,                // uv indices
    int            MaterialIndex      // per-triangle material
);

public record struct KoreMesh2MaterialGroup(string MaterialName, List<int> TriangleIds);

public partial class KoreMeshData2
{
    // --- Core attribute buffers (deduplicated) ----------------------------------------------
    public Dictionary<int, KoreXYZVector>  Vertices        { get; } = [];   // unique XYZ
    public Dictionary<int, KoreXYZVector>  Normals         { get; } = [];   // unit vectors
    public Dictionary<int, KoreXYVector>   UVs             { get; } = [];   // primary UV set
    public Dictionary<int, KoreColorRGB>   Colors          { get; } = [];   // vertex/loop colors

    // --- Topology ---------------------------------------------------------------------------
    public Dictionary<int, KoreMeshLineRef> Lines          { get; } = [];   // optional edges (debug/wire)
    public Dictionary<int, KoreMeshTriRef>  Triangles      { get; } = [];   // per-face corner refs

    // --- Rendering ---------------------------------------------------------------------------
    public List<KoreMeshMaterial> Materials = [];
    public Dictionary<string, KoreMesh2MaterialGroup> MaterialGroups = []; // Tags for grouping triangles

    // Counters for unique IDs
    public int NextVertexId   = 0;
    public int NextNormalId   = 0;
    public int NextUVId       = 0;
    public int NextColorId    = 0;
    public int NextLineId     = 0;
    public int NextTriangleId = 0;
}
