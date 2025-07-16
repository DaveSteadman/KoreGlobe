using System;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestColor
{
    // Usage: KoreTestColor.RunTests(testLog);
    public static void RunTests(KoreTestLog testLog)
    {
        try
        {
            TestColorString(testLog);
        }
        catch (Exception ex)
        {
            testLog.AddResult("KoreTestColor RunTests // Exception: ", false, ex.Message);
            return;
        }
    }


    // Test to create a basic color - convert it to JSON (text) and then back again, to ensure all the lists are correctly serialized and deserialized.
    public static void TestColorString(KoreTestLog testLog)
    {
        // Test the serialization of colors into hex strings of different lengths
        {
            var color1 = new KoreColorRGB(255, 0, 0);
            string strCol = KoreColorIO.RBGtoHexStringShort(color1);
            testLog.AddComment($"KoreColorIO RBGtoHexStringShort (3 char) 255,0,0: {strCol}");

            var color2 = new KoreColorRGB(255, 34, 17, 34);
            string strCol2 = KoreColorIO.RBGtoHexStringShort(color2);
            testLog.AddComment($"KoreColorIO RBGtoHexStringShort (4 char) 255,34,17,34: {strCol2}");

            var color3 = new KoreColorRGB(255, 32, 16, 255);
            string strCol3 = KoreColorIO.RBGtoHexStringShort(color3);
            testLog.AddComment($"KoreColorIO RBGtoHexStringShort (6 char) 255,32,16,255: {strCol3}");

            var color4 = new KoreColorRGB(250, 33, 17, 134);
            string strCol4 = KoreColorIO.RBGtoHexStringShort(color4);
            testLog.AddComment($"KoreColorIO RBGtoHexStringShort (8 char) 250,33,17,134: {strCol4}");
        }

        // Test the deserialization of colors from hex strings
        {
            var color1 = KoreColorIO.HexStringToRGB("F00");
            testLog.AddComment($"KoreColorIO HexStringToRGB (3 char) F00: {KoreColorIO.RBGtoHexStringShort(color1)}");

            var color2 = KoreColorIO.HexStringToRGB("#F234");
            testLog.AddComment($"KoreColorIO HexStringToRGB (4 char) #F234: {KoreColorIO.RBGtoHexStringShort(color2)}");

            var color3 = KoreColorIO.HexStringToRGB("FF2356");
            testLog.AddComment($"KoreColorIO HexStringToRGB (6 char) FF2356: {KoreColorIO.RBGtoHexStringShort(color3)}");

            var color4 = KoreColorIO.HexStringToRGB("FA211086");
            testLog.AddComment($"KoreColorIO HexStringToRGB (8 char) FA211086: {KoreColorIO.RBGtoHexStringShort(color4)}");
        }
    }
}


