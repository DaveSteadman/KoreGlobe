using System;

// KoreXYVectorIO: Converts the KoreXYVector struct to and from various formats, such as JSON or binary.

namespace KoreCommon;

public static class KoreXYZVectorIO
{
    // Usage: string str = KoreXYVectorIO.ToString(new KoreXYZVector(1.0, 2.0, 3.0));
    public static string ToString(KoreXYZVector vector)
    {
        string xstr = vector.X.ToString("F7");
        if (xstr.Contains("."))
        {
            xstr = xstr.TrimEnd('0');
            if (xstr.EndsWith(".")) xstr = xstr.TrimEnd('.');
        }

        string ystr = vector.Y.ToString("F7");
        if (ystr.Contains("."))
        {
            ystr = ystr.TrimEnd('0');
            if (ystr.EndsWith(".")) ystr = ystr.TrimEnd('.');
        }

        string zstr = vector.Z.ToString("F7");
        if (zstr.Contains("."))
        {
            zstr = zstr.TrimEnd('0');
            if (zstr.EndsWith(".")) zstr = zstr.TrimEnd('.');
        }

        return $"X:{xstr}, Y:{ystr}, Z:{zstr}";
    }

    // Usage: KoreXYZVector vector = KoreXYZVectorIO.FromString("X:1.0, Y:2.0, Z:3.0");
    public static KoreXYZVector FromString(string str)
    {
        var parts = str.Split(',');
        if (parts.Length != 3) throw new FormatException("Invalid KoreXYZVector string format.");

        double x = double.Parse(parts[0].Split(':')[1]);
        double y = double.Parse(parts[1].Split(':')[1]);
        double z = double.Parse(parts[2].Split(':')[1]);

        return new KoreXYZVector(x, y, z);
    }
}