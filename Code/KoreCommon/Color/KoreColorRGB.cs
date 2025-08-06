// Custom colour class, protable between frameworks.

using System;

namespace KoreCommon;

public struct KoreColorRGB
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public float Rf => KoreColorIO.ByteToFloat(R);
    public float Gf => KoreColorIO.ByteToFloat(G);
    public float Bf => KoreColorIO.ByteToFloat(B);
    public float Af => KoreColorIO.ByteToFloat(A);

    public bool IsTransparent => (A < KoreColorIO.MaxByte);

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public KoreColorRGB(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public KoreColorRGB(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = KoreColorIO.MaxByte;
    }

    public KoreColorRGB(float r, float g, float b, float a)
    {
        R = KoreColorIO.FloatToByte(r);
        G = KoreColorIO.FloatToByte(g);
        B = KoreColorIO.FloatToByte(b);
        A = KoreColorIO.FloatToByte(a);
    }

    public KoreColorRGB(float r, float g, float b)
    {
        R = KoreColorIO.FloatToByte(r);
        G = KoreColorIO.FloatToByte(g);
        B = KoreColorIO.FloatToByte(b);
        A = KoreColorIO.MaxByte;
    }

    public static readonly KoreColorRGB Zero = new KoreColorRGB(KoreColorIO.MinByte, KoreColorIO.MinByte, KoreColorIO.MinByte, KoreColorIO.MinByte);
    public static readonly KoreColorRGB White = new KoreColorRGB(KoreColorIO.MaxByte, KoreColorIO.MaxByte, KoreColorIO.MaxByte, KoreColorIO.MaxByte);

}