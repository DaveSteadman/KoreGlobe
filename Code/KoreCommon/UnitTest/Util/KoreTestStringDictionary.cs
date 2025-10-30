// <fileheader>

using System;
using System.Diagnostics;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestStringDictionary
{
    public static void RunTests(KoreTestLog testLog)
    {
        TestRoundTrip(testLog);
    }

    private static void TestRoundTrip(KoreTestLog testLog)
    {
        // Create dictionary, write values of differing types
        var original = new KoreStringDictionary();
        original.Set("name", "Cube01");
        original.Set("note", "unit cube");
        KoreStringDictionaryOps.WriteDouble(original, "size", 1.5);
        KoreStringDictionaryOps.WriteBool(original, "visible", true);

        // Export to JSON
        string serializedJson = original.ExportJson(indented: false);
        KoreCentralLog.AddEntry($"Serialized JSON: {serializedJson}");


        // Import from JSON
        var restored = new KoreStringDictionary();
        restored.ImportJson(serializedJson);

        // Test values are correctly imported, as basic strings
        testLog.AddResult("restored.Get(name) == Cube01", (restored.Get("name") == "Cube01"), restored.ExportJson(indented: false));
        testLog.AddResult("restored.Get(note) == unit cube", (restored.Get("note") == "unit cube"));
        testLog.AddResult("restored.Get(visible) == True", (restored.Get("visible") == "True"));

        // Test missing values
        testLog.AddResult("restored.Has(notexist)", (restored.Has("notexist") == false));

        // Test values, read back in their original types
        testLog.AddResult(
            "ReadDouble(size) == 1.5",
            KoreValueUtils.EqualsWithinTolerance(KoreStringDictionaryOps.ReadDouble(restored, "size"), 1.5));

        testLog.AddResult(
            "ReadDouble(visible) returns fallback -1 (invalid type)",
            KoreValueUtils.EqualsWithinTolerance(KoreStringDictionaryOps.ReadDouble(restored, "visible"), -1));

        testLog.AddResult(
            "ReadBool(visible) == true", KoreStringDictionaryOps.ReadBool(restored, "visible") == true);

    }
}