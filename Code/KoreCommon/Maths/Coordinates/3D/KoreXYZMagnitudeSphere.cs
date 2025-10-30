// <fileheader>

using System;

namespace KoreCommon;

public class KoreXYZMagnitudeSphere
{
    public KoreXYZVector Center { get; private set; }

    // A 2D array for each integer azel angle, with the value being the height at that point
    public KoreFloat2DArray Heightmap { get; private set; } = new(360, 180);

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYZMagnitudeSphere(KoreXYZVector center, KoreFloat2DArray hm)
    {
        Center = center;
        Heightmap = hm;
    }

}