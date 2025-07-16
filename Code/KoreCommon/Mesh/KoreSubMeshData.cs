using System.Collections.Generic;

#nullable enable

namespace KoreCommon;

// KoreSubMeshData: Handles the organization of sub-mesh data within a larger mesh structure.
// - contains properties for the sub-mesh name, mesh data, scale, offset, and rotation.

public class KoreSubMeshData
{
    public string       Name     { get; set; } = string.Empty;
    public KoreMeshData  Mesh     { get; set; } = new KoreMeshData();
    public double       Scale    { get; set; } = 1.0; // Scale applies to mesh after any offset translation and rotation
    public KoreXYZVector Offset   { get; set; } = new KoreXYZVector(0.0, 0.0, 0.0);
    public KoreAttitude  Rotation { get; set; } = new KoreAttitude(0.0, 0.0, 0.0);

    public KoreSubMeshData()
    {
        Clear();
    }

    public void Clear()
    {
        Name     = "SubMesh";
        Mesh     = new KoreMeshData();
        Scale    = 1.0;
        Offset   = new KoreXYZVector(0.0, 0.0, 0.0);
        Rotation = new KoreAttitude(0.0, 0.0, 0.0);
    }
}