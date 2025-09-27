
using System.Collections.Generic;

using KoreCommon;
namespace KoreSim;

// Face, with points as seen from outside

public struct KoreQuadFace
{
    // KoreQuadFace.CubeFace
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

    // Cube face corner positions
    public KoreXYZVector TopLeft { get; set; } = KoreXYZVector.Zero;
    public KoreXYZVector TopRight { get; set; } = KoreXYZVector.Zero;
    public KoreXYZVector BottomLeft { get; set; } = KoreXYZVector.Zero;
    public KoreXYZVector BottomRight { get; set; } = KoreXYZVector.Zero;

    public KoreXYZVector Center => (TopLeft + TopRight + BottomLeft + BottomRight) / 4.0;

    public KoreQuadFace() { }


    public static KoreQuadFace Zero => new KoreQuadFace();
}
