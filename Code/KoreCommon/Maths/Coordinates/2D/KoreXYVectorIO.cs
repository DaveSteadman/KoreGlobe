using System;

// KoreXYVectorIO: Converts the KoreXYVector struct to and from various formats, such as JSON or binary.

namespace KoreCommon;

public static class KoreXYVectorIO
{
    // Usage: string str = KoreXYVectorIO.ToString(new KoreXYVector(1.0, 2.0));
    public static string ToString(KoreXYVector vector)
    {
        return $"X:{vector.X:F3}, Y:{vector.Y:F3}";
    }

    // Usage: KoreXYVector vector = KoreXYVectorIO.FromString("X:1.0, Y:2.0");
    public static KoreXYVector FromString(string str)
    {
        var parts = str.Split(',');
        if (parts.Length != 2) throw new FormatException("Invalid KoreXYVector string format.");

        double x = double.Parse(parts[0].Split(':')[1]);
        double y = double.Parse(parts[1].Split(':')[1]);

        return new KoreXYVector(x, y);
    }
}