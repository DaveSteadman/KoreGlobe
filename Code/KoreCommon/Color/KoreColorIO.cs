using System;

// Static class to convert color structs

namespace KoreCommon;

public static class KoreColorIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Byte Util
    // --------------------------------------------------------------------------------------------

    // 0f = 0, 1f = 255, everything else is clamped to 0-255
    public static byte FloatToByte(float value)
    {
        if (value < 0f) return 0;
        if (value > 1f) return 255;
        return (byte)(value * 255);
    }

    public static float ByteToFloat(byte value)
    {
        return ((float)value) / 255f;
    }

    public static float LimitFloat(float value)
    {
        if (value < 0f) return 0f;
        if (value > 1f) return 1f;
        return value;
    }

    // Usage: byte maxByte = KoreColorIO.MaxByte;
    // Usage: byte minByte = KoreColorIO.MinByte;
    public static byte MaxByte => 255;
    public static byte MinByte => 0;

    // --------------------------------------------------------------------------------------------
    // MARK: To String
    // --------------------------------------------------------------------------------------------

    public static string RBGtoDecimalString(KoreColorRGB rgb)
    {
        return $"R:{rgb.R:F2}, G:{rgb.G:F2}, B:{rgb.B:F2}, A:{rgb.A:F2}";
    }

    public static string HSVtoDecimalString(KoreColorHSV hsv)
    {
        return $"H:{hsv.H:F2}, S:{hsv.S:F2}, V:{hsv.V:F2}, A:{hsv.A:F2}";
    }

    // ---------------------------------------------------------------------------------------------

    // Usage: string hexString = KoreColorIO.RBGtoHexString(new KoreColorRGB(0.5f, 0.2f, 0.8f, 1.0f));
    public static string RBGtoHexString(KoreColorRGB rgb)
    {
        return $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}{rgb.A:X2}";
    }

    // Usage: string hexString = KoreColorIO.RBGtoHexStringShort(new KoreColorRGB(0.5f, 0.2f, 0.8f, 1.0f));
    // Note: This function returns an 8-character hex string without the alpha channel, but if the characters
    // are all the same, it will return a 4-character hex string. And if the apha is 255 (opaque), it will return a 3 character string.
    public static string RBGtoHexStringShort(KoreColorRGB rgb)
    {
        //Convert each channel to a string
        string rStr = rgb.R.ToString("X2");
        string gStr = rgb.G.ToString("X2");
        string bStr = rgb.B.ToString("X2");
        string aStr = rgb.A.ToString("X2");

        // If the two characters in a channel match, we can shorten the string
        // - rgb.RByte >> 4 isolates the upper nibble
        // - rgb.RByte & 0xF isolates the lower nibble
        // - If they're equal, then that byte is a valid shorthand hex like 0xAA (i.e. 'A' duplicated)
        bool canShorten =
            ((rgb.R >> 4) == (rgb.R & 0xF)) &&
            ((rgb.G >> 4) == (rgb.G & 0xF)) &&
            ((rgb.B >> 4) == (rgb.B & 0xF)) &&
            ((rgb.A >> 4) == (rgb.A & 0xF));

        if (canShorten)
        {
            string rStrShort = rStr[0].ToString();
            string gStrShort = gStr[0].ToString();
            string bStrShort = bStr[0].ToString();
            string aStrShort = aStr[0].ToString();

            if (aStr == "FF" || aStrShort == "F")
            {
                // If the alpha channel is fully opaque, we can shorten the string further
                return $"#{rStrShort}{gStrShort}{bStrShort}";
            }
            return $"#{rStrShort}{gStrShort}{bStrShort}{aStrShort}";
        }

        if (aStr == "FF")
            return $"#{rStr}{gStr}{bStr}";

        return $"#{rStr}{gStr}{bStr}{aStr}";
    }

    public static string RBGtoHexStringNoAlpha(KoreColorRGB rgb)
    {
        return $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: From String
    // --------------------------------------------------------------------------------------------

    // Usage: bool validStr = KoreColorIO.IsValidRGBString("#112233");
    public static bool IsValidRGBString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        // Trim any whitespace or hash characters
        str = str.Trim().ToUpperInvariant();
        if (str.StartsWith("#"))
            str = str.Substring(1);

        if (str.Length != 6 && str.Length != 8)
            return false;

        // If any characters are not hex, return false
        foreach (char c in str)
        {
            if (!Uri.IsHexDigit(c))
                return false;
        }

        // No criteria to fail, return true
        return true;
    }

    // --------------------------------------------------------------------------------------------

    // An input like "#112233" or "#112233FF". Returns the zero color on any uncertainty.
    public static KoreColorRGB HexStringToRGB(string hexString)
    {
        // Trim any whitespace characters, or haash characters off the front
        hexString = hexString.Trim().ToUpperInvariant();
        if (hexString.StartsWith("#"))
            hexString = hexString.Substring(1);

        // Extract based on string length
        int strLength = hexString.Length;

        switch (strLength)
        {
            case 3:
                {
                    // Convert 3-character hex to 6-character hex #RGB
                    byte r = byte.Parse(hexString.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
                    byte g = byte.Parse(hexString.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
                    byte b = byte.Parse(hexString.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);

                    // convert the 1-character values to 2-character values
                    r = (byte)((r << 4) | r); // e.g. 0x1 -> 0x11
                    g = (byte)((g << 4) | g); // e.g. 0x2 -> 0x22
                    b = (byte)((b << 4) | b); // e.g. 0x3 -> 0x33

                    return new KoreColorRGB(r, g, b, 255);
                }
            case 4:
                {
                    // Convert 4-character hex to 8-character hex #RGBA
                    byte r = byte.Parse(hexString.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
                    byte g = byte.Parse(hexString.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
                    byte b = byte.Parse(hexString.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
                    byte a = byte.Parse(hexString.Substring(3, 1), System.Globalization.NumberStyles.HexNumber);

                    // convert the 1-character values to 2-character values
                    r = (byte)((r << 4) | r); // e.g. 0x1 -> 0x11
                    g = (byte)((g << 4) | g); // e.g. 0x2 -> 0x22
                    b = (byte)((b << 4) | b); // e.g. 0x3 -> 0x33
                    a = (byte)((a << 4) | a); // e.g. 0x4 -> 0x44

                    return new KoreColorRGB(r, g, b, a);
                }
            case 6:
                {
                    // Convert 6-character hex to 8-character hex #RRGGBB
                    byte r = byte.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    byte g = byte.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    byte b = byte.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                    return new KoreColorRGB(r, g, b, 255);
                }
            case 8:
                {
                    // Convert 8-character hex to 8-character hex #RRGGBBAA
                    byte r = byte.Parse(hexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    byte g = byte.Parse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    byte b = byte.Parse(hexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    byte a = byte.Parse(hexString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

                    return new KoreColorRGB(r, g, b, a);
                }
            default:
                {
                    // Invalid length, return zero color
                    return new KoreColorRGB(0, 0, 0, 255);
                }
        }
    }
}

