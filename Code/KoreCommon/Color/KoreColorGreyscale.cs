// <fileheader>



// Custom colour class, protable between frameworks.

using System;

namespace KoreCommon;

public struct KoreColorGreyscale
{
    public byte V { get; set; }

    public float Vf => V / 255f;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public KoreColorGreyscale(byte v)
    {
        V = v;
    }

    public KoreColorGreyscale(float v)
    {
        V = KoreColorIO.FloatToByte(v);
    }

    // --------------------------------------------------------------------------------------------

    public static readonly KoreColorGreyscale Zero = new KoreColorGreyscale(KoreColorIO.MinByte);
    public static readonly KoreColorGreyscale White = new KoreColorGreyscale(KoreColorIO.MaxByte);
    public static readonly KoreColorGreyscale Black = new KoreColorGreyscale(KoreColorIO.MinByte);
}


