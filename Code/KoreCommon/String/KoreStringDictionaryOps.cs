using System;
using System.Globalization;

namespace KoreCommon;


public static class KoreStringDictionaryOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Boolean
    // --------------------------------------------------------------------------------------------

    public static void WriteBool(KoreStringDictionary dict, string key, bool value)
    {
        dict.Set(key, KoreValueUtils.BoolToStr(value));
    }

    public static bool ReadBool(KoreStringDictionary dict, string key, bool fallback = false)
    {
        return KoreValueUtils.StrToBool(dict.Get(key), fallback);
    }

    public static bool ConsumeBool(KoreStringDictionary dict, string key, out bool value)
    {
        if (!dict.Has(key))
        {
            value = false;
            return false;
        }

        value = KoreValueUtils.StrToBool(dict.Get(key));
        return true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Int
    // --------------------------------------------------------------------------------------------

    // Usage: KoreStringDictionaryOps.WriteInt(dict, "myKey", 42);
    public static void WriteInt(KoreStringDictionary dict, string key, int value)
    {
        dict.Set(key, value.ToString(CultureInfo.InvariantCulture));
    }

    // Usage: int myValue = KoreStringDictionaryOps.ReadInt(dict, "myKey");
    public static int ReadInt(KoreStringDictionary dict, string key, int fallback = -1)
    {
        var raw = dict.Get(key);
        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : fallback;
    }

    public static bool ConsumeInt(KoreStringDictionary dict, string key, out int value)
    {
        var raw = dict.Get(key);
        if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
        {
            dict.Remove(key);
            return true;
        }

        value = -1;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Float
    // --------------------------------------------------------------------------------------------

    public static void WriteFloat(KoreStringDictionary dict, string key, float value, int sigFig = 4)
    {
        string format = $"F{sigFig}";
        dict.Set(key, value.ToString(format, CultureInfo.InvariantCulture));
    }

    public static float ReadFloat(KoreStringDictionary dict, string key, float fallback = -1f)
    {
        var raw = dict.Get(key);
        return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : fallback;
    }

    public static bool ConsumeFloat(KoreStringDictionary dict, string key, out float value)
    {
        var raw = dict.Get(key);
        if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            dict.Remove(key);
            return true;
        }

        value = -1f;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Double
    // --------------------------------------------------------------------------------------------

    public static void WriteDouble(KoreStringDictionary dict, string key, double value, int sigFig = 7)
    {
        string format = $"F{sigFig}";
        dict.Set(key, value.ToString(format, CultureInfo.InvariantCulture));
    }

    public static double ReadDouble(KoreStringDictionary dict, string key, double fallback = -1)
    {
        var raw = dict.Get(key);
        return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : fallback;
    }

    public static bool ConsumeDouble(KoreStringDictionary dict, string key, out double value)
    {
        var raw = dict.Get(key);
        if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            dict.Remove(key);
            return true;
        }

        value = -1;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Colour
    // --------------------------------------------------------------------------------------------

    public static void WriteColour(KoreStringDictionary dict, string key, KoreColorRGB value)
    {
        dict.Set(key, KoreColorIO.RBGtoHexString(value));
    }

    public static KoreColorRGB ReadColour(KoreStringDictionary dict, string key, KoreColorRGB fallback)
    {
        var raw = dict.Get(key);

        if (!KoreColorIO.IsValidRGBString(raw)) return fallback;

        return KoreColorIO.HexStringToRGB(raw);
    }

    public static bool ConsumeColour(KoreStringDictionary dict, string key, out KoreColorRGB value)
    {
        var raw = dict.Get(key);
        if (KoreColorIO.IsValidRGBString(raw))
        {
            value = KoreColorIO.HexStringToRGB(raw);
            dict.Remove(key);
            return true;
        }

        value = KoreColorRGB.Zero;
        return false;
    }

}
