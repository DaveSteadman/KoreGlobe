// <fileheader>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KoreCommon;

// A utility class of miscellaneous string operations

public static class KoreStringOps
{

    // --------------------------------------------------------------------------------------------
    // MARK: Letters to Numbers
    // --------------------------------------------------------------------------------------------

    // Usage: int index = KoreStringOps.NumberForLetter('a');

    public static int NumberForLetter(char letter)
    {
        if (letter >= 'a' && letter <= 'z')
            return letter - 'a';
        if (letter >= 'A' && letter <= 'Z')
            return letter - 'A';
        return -1;
    }

    public static char LetterForNumber(int number)
    {
        if (number >= 0 && number < 26)
            return (char)('a' + number);
        return ' ';
    }

    // --------------------------------------------------------------------------------------------

    // Reduce a string to a whitelist set of characters.

    // KoreStringOps.WhitelistString()
    public static string WhitelistString(string instr)
    {
        // Basic validation
        if (string.IsNullOrEmpty(instr))
            return string.Empty;

        // Remove any quotes, brackets or potentially problematic characters from the name, so it can serialise correctly
        // Define a whitelist of characters, and remove any that are not in the whitelist
        // Task implicitly removes whitespace
        char[] whitelist = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-".ToCharArray();
        instr = new string(instr.Where(c => whitelist.Contains(c)).ToArray());

        return instr;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String to Bool
    // --------------------------------------------------------------------------------------------

    // Any kind of string to a bool

    // Usage: bool isTrue = KoreStringOps.BoolForString("true");

    public static bool BoolForString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;

        if (str.Contains("true", StringComparison.OrdinalIgnoreCase)) return true;
        if (str.Contains("false", StringComparison.OrdinalIgnoreCase)) return false;
        if (str.Contains("yes", StringComparison.OrdinalIgnoreCase)) return true;
        if (str.Contains("no", StringComparison.OrdinalIgnoreCase)) return false;

        if (str.Contains("1")) return true;
        if (str.Contains("0")) return false;

        return false;
    }

    // Usage: string str = $"{KoreStringOps.StringForBoolTF(val)}";
    public static string StringForBoolTF(bool value) { return value ? "true" : "false"; }
    public static string StringForBoolPF(bool value) { return value ? "pass" : "fail"; }
    public static string StringForBoolYN(bool value) { return value ? "yes" : "no"; }
    public static string StringForBool10(bool value) { return value ? "1" : "0"; }

    // --------------------------------------------------------------------------------------------
    // MARK: String to Time
    // --------------------------------------------------------------------------------------------

    // Convert a string in the format "hh:mm:ss" to seconds
    // Usage: int seconds = KoreStringOps.TimeHmsToSeconds("01:23:45");

    public static int TimeHmsToSeconds(string timeHms)
    {
        if (string.IsNullOrEmpty(timeHms))
            return 0;

        string[] parts = timeHms.Split(':');
        if (parts.Length != 3) return 0;

        int hours = int.Parse(parts[0]);
        int mins = int.Parse(parts[1]);
        int secs = int.Parse(parts[2]);

        return (hours * 3600) + (mins * 60) + secs;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Random Strings
    // --------------------------------------------------------------------------------------------

    // Random strings, useful in object IDs, etc.

    private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    private static Random rng = new Random();

    public static string RandomString(int length = 5)
    {
        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[rng.Next(chars.Length)]);
        }
        return result.ToString();
    }

}

