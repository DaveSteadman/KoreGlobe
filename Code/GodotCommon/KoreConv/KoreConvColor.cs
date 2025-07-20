using KoreCommon;
using Godot;

public static class KoreConvColor
{
    // Usage: Color col = KoreConvColor.ToGodotColor(koreColor);
    public static Color ToGodotColor(KoreColorRGB color)
    {
        return new Color(color.Rf, color.Gf, color.Bf, color.Af);
    }

    public static KoreColorRGB FromGodotColor(Color color)
    {
        return new KoreColorRGB(color.R, color.G, color.B, color.A);
    }
}