// <fileheader>

using System;

using KoreCommon;
namespace KoreCommon.UnitTest;

public static class KoreTestList2D
{
    // KoreTestList2D.RunTests(testLog)
    public static void RunTests(KoreTestLog testLog)
    {
        Test2DList(testLog);

    }

    public static void Test2DList(KoreTestLog testLog)
    {
        {
            KoreNumeric2DArray<double> list = new KoreNumeric2DArray<double>(3, 3);

            var size = list.Size;


            testLog.AddResult("KoreNumeric2DArray Test2DList Size", (size.Width == 3 && size.Height == 3));

        }
    }
}