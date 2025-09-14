
using System.Collections.Generic;

using KoreCommon;
namespace KoreSim;

// Face, with points as seen from outside

public struct KoreQuadFace
{
    public enum CubeFace { Top, Bottom, Left, Right, Front, Back }

    static public Dictionary<CubeFace, string> CubeFaceNames = new()
    {
        { CubeFace.Top, "Top" },
        { CubeFace.Bottom, "Btm" },
        { CubeFace.Left, "Lft" },
        { CubeFace.Right, "Rgt" },
        { CubeFace.Front, "Frt" },
        { CubeFace.Back, "Bak" }
    };

    public KoreXYZVector topLeft { get; set; } = KoreXYZVector.Zero;
    public KoreXYZVector topRight { get; set; } = KoreXYZVector.Zero;
    public KoreXYZVector bottomLeft { get; set; } = KoreXYZVector.Zero;
    public KoreXYZVector bottomRight { get; set; } = KoreXYZVector.Zero;

    public KoreQuadFace() { }


    public static KoreQuadFace Zero => new KoreQuadFace();
}
