
using System;

using KoreCommon;

namespace KoreSim;

#nullable enable

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class KoreEventDriver
{
    public static void SetRootDir(string rootDir)
    {
        // Check the root dir is valid
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        //KoreSimFactory.Instance.MapIOManager.SetRootDir(rootDir);

        KoreCentralLog.AddEntry($"SetRootDir: {rootDir}");
    }

    // ---------------------------------------------------------------------------------------------

    public static string ReportRootDir()
    {
        return "undefined"; // KoreSimFactory.Instance.MapIOManager.ReportRootDir();
    }

    // ---------------------------------------------------------------------------------------------

    public static void CreateBaseDirectories()
    {
        // string rootDir = "undefined"; // KoreSimFactory.Instance.MapIOManager.ReportRootDir();

        // // Validity Checks - Map, RootDir, RootDir Exists
        // if (string.IsNullOrEmpty(rootDir)) return;
        // if (!System.IO.Directory.Exists(rootDir)) return;

        // GloMapOperations.CreateBaseDirectories(rootDir);
    }

}