using System;

// KoreValueUtils: A static class for common (bool) value routines, useful as helper routines for higher-level functionality.

namespace KoreCommon;

public static partial class KoreValueUtils
{
    // Usage: KoreValueUtils.BoolToStr
    public static string BoolToStr(bool val)
    {
        if (val) return "True";
        return "False";
    }

    // Usage: KoreValueUtils.StrToBool
    public static bool StrToBool(string str, bool fallback = false)
    {
        if (string.IsNullOrWhiteSpace(str))
            return fallback;

        string trimmedStr = str.Trim().ToLowerInvariant();

        switch (trimmedStr)
        {
            case "true":
            case "t":
            case "yes":
            case "y":
                return true;
            case "false":
            case "f":
            case "no":
            case "n":
                return false;
            default:
                return fallback;
        }
    }
}
