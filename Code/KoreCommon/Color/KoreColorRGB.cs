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

    // --------------------------------------------------------------------------------------------
    // MARK: Changes
    // --------------------------------------------------------------------------------------------

    // Usage: KoreColorRGB.Lerp(col1, col2, t)
    // 0 = col1, 1 = col2, 0.5 = halfway between col1 and col2

    public static KoreColorRGB Lerp(KoreColorRGB col1, KoreColorRGB col2, float col2fraction)
    {
        if (col2fraction < 0.0f) col2fraction = 0.0f;
        if (col2fraction > 1.0f) col2fraction = 1.0f;

        float newRf = col1.Rf + (col2.Rf - col1.Rf) * col2fraction;
        float newGf = col1.Gf + (col2.Gf - col1.Gf) * col2fraction;
        float newBf = col1.Bf + (col2.Bf - col1.Bf) * col2fraction;
        float newAf = col1.Af + (col2.Af - col1.Af) * col2fraction;

        return new KoreColorRGB(newRf, newGf, newBf, newAf);
    }



}