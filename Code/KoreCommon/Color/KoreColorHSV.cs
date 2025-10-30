// <fileheader>

// Custom colour class, protable between frameworks.


// HSV: Hue, Saturation, Value

namespace KoreCommon;

public struct KoreColorHSV
{
    public float H { get; set; } // Hue (0-360 degrees)
    public float S { get; set; } // Saturation (0-1)
    public float V { get; set; } // Value (0-1)
    public float A { get; set; } // Alpha (0-1)

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public KoreColorHSV(float h, float s, float v, float a)
    {
        H = h;
        S = s;
        V = v;
        A = a;
    }

    // --------------------------------------------------------------------------------------------

    public static KoreColorHSV Zero => new KoreColorHSV(0, 0, 0, 0);
    public static KoreColorHSV White => new KoreColorHSV(0, 0, 1, 1);

}
