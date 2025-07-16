using System;

namespace KoreCommon;


public static class KoreEnumStringOps
{
    // Parse a string into an enum value. Returns the fallback if the input is invalid.
    // Usage: var x = KoreEnumStringUtils.ParseOrDefault("MyEnumValue", MyEnum.DefaultValue);
    // Note: Enum.Parse is case-sensitive by default, so we use ignoreCase: true.
    public static T ParseOrDefault<T>(string input, T fallback) where T : struct, Enum
    {
        return Enum.TryParse<T>(input, ignoreCase: true, out var result) ? result : fallback;
    }

    // Try to parse a string into an enum value. Returns true if successful.
    // Usage: if (KoreEnumStringUtils.TryParse("MyEnumValue", out MyEnum result)) { ... }
    public static bool TryParse<T>(string input, out T result) where T : struct, Enum
    {
        return Enum.TryParse<T>(input, ignoreCase: true, out result);
    }

    // Convert an enum value to its string name.
    // Usage: var name = KoreEnumStringUtils.ToString(MyEnum.Value);
    public static string ToString<T>(T value) where T : struct, Enum
    {
        return value.ToString();
    }

    // Check if a string represents a valid enum name.
    // Usage: if (KoreEnumStringUtils.IsDefinedName<MyEnum>("MyEnumValue")) { ... }
    public static bool IsDefinedName<T>(string name) where T : struct, Enum
    {
        return Enum.TryParse<T>(name, ignoreCase: true, out var parsed) && Enum.IsDefined(typeof(T), parsed);
    }
}