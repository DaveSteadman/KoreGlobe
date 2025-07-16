using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using KoreCommon;
namespace KoreCommon.UnitTest;


public static class KoreTestList1D
{
    // KoreTestList1D.RunTests(testLog)
    public static void RunTests(KoreTestLog testLog)
    {
        Test1DList(testLog);
        Test1DIntList(testLog);
    }

    private static void Test1DList(KoreTestLog testLog)
    {
        {
            KoreNumeric1DArray<double> list = new KoreNumeric1DArray<double>(10);

            // sums to 45
            for (int i = 0; i < 10; i++)
                list[i] = i;
            testLog.AddResult("KoreNumeric1DArray List<double> Sum", KoreValueUtils.EqualsWithinTolerance(list.Sum(), 45.0));

            testLog.AddResult("KoreNumeric1DArray Length", KoreValueUtils.EqualsWithinTolerance(list.Length, 10));
            list.Add(10.0);
            testLog.AddResult("KoreNumeric1DArray Length After Add", KoreValueUtils.EqualsWithinTolerance(list.Length, 11));
            list.RemoveAtIndex(3);
            testLog.AddResult("KoreNumeric1DArray Length After RemoveAtIndex", KoreValueUtils.EqualsWithinTolerance(list.Length, 10));
        }
    }

    private static void Test1DIntList(KoreTestLog testLog)
    {
        {
            var list = KoreNumeric1DArrayOps<int>.CreateArrayByStep(0, 1, 10);

            // print the array
            StringBuilder sb = new StringBuilder();
            sb.Append("KoreNumeric1DArray List<int>: KoreNumeric1DArrayOps<int>.CreateArrayByStep(0, 1, 10); => ");
            sb.AppendJoin(", ", list);
            testLog.AddComment(sb.ToString());
        }
        {
            StringBuilder sb = new StringBuilder();

            var list = KoreNumeric1DArrayOps<int>.CreateArrayByCount(0, 8, 4);
            sb.Append("KoreNumeric1DArray List<int>: KoreNumeric1DArrayOps<int>.CreateArrayByCount(0, 8, 4); => ");
            sb.AppendJoin(", ", list);
            testLog.AddComment(sb.ToString());
        }
        {
            StringBuilder sb = new StringBuilder();

            var list = KoreNumeric1DArrayOps<float>.CreateArrayByCount(0, 10, 20);
            sb.Append("KoreNumeric1DArray List<float>: KoreNumeric1DArrayOps<float>.CreateArrayByCount(0, 10, 21); => ");
            sb.AppendJoin(", ", list.Select(x => x.ToString("F2")));
            testLog.AddComment(sb.ToString());
        }
    }
}


