// <fileheader>

using System;
using System.Text;

namespace KoreCommon;


// Small util class to generate random strings of a given length, for use in IDs.

// usage: string randomString = KoreRandomStringGenerator.GenerateRandomString(5);
public static class KoreRandomStringGenerator
{
    private static readonly char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    private static Random rng = new Random();

    public static string GenerateRandomString(int length = 5)
    {
        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[rng.Next(chars.Length)]);
        }
        return result.ToString();
    }
}