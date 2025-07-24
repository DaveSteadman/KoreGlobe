using System;

// KoreXYVectorIO: Converts the KoreXYVector struct to and from various formats, such as JSON or binary.

namespace KoreCommon;

public static class KoreXYZVectorIO
{
    // Usage: string str = KoreXYVectorIO.ToString(new KoreXYZVector(1.0, 2.0, 3.0));
    public static string ToString(KoreXYZVector vector)
    {
        string formatStr = "0." + "0" + new string('#', 6);

        string xstr = vector.X.ToString(formatStr);
        string ystr = vector.Y.ToString(formatStr);
        string zstr = vector.Z.ToString(formatStr);

        return $"X:{xstr}, Y:{ystr}, Z:{zstr}";
    }

    // --------------------------------------------------------------------------------------------

    public static string ToStringWithDP(KoreXYZVector vector, int decimalPlaces = 7)
    {
        int limitedDP = KoreNumericUtils.LimitToRange<int>(decimalPlaces, 1, 7);
        string formatStr = "0." + "0" + new string('#', limitedDP - 1);

        string xstr = vector.X.ToString(formatStr);
        string ystr = vector.Y.ToString(formatStr);
        string zstr = vector.Z.ToString(formatStr);

        return $"X:{xstr}, Y:{ystr}, Z:{zstr}";
    }

    // --------------------------------------------------------------------------------------------


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