
// using System;

// using GloNetworking;

// #nullable enable

// public class KoreEventDriver
// {
//     // Top level map object to act on
//     GloMap MapData = new GloMap();

//     GloNetworkHub? NetHub;
//     public GloConsole? ConsoleInterface;

//     // Unit Testing
//     public GloTestLog testLog = GloTestCenter.RunCoreTests();

//     // ---------------------------------------------------------------------------------------------
//     // Constructor
//     // ---------------------------------------------------------------------------------------------

//     public KoreEventDriver()
//     {
//     }

//     // ---------------------------------------------------------------------------------------------

//     public void ExitApplication()
//     {
//         // Stop the networking
//         StopNetworking();

//         // Stop the console interface
//         KoreCentralLog.AddEntry("Stopping console interface...");
//         ConsoleInterface?.Stop();
//     }

//     // ---------------------------------------------------------------------------------------------

//     public void SetRootDir(string rootDir)
//     {
//         // Check the root dir is valid
//         if (string.IsNullOrEmpty(rootDir)) return;
//         if (!System.IO.Directory.Exists(rootDir)) return;

//         // Assign the dir to the map, and ensure the basic directories are setup.
//         MapData?.SetRootDir(rootDir);

//         if (MapData != null)
//         {
//             KoreCentralLog.AddEntry($"SetRootDir: {MapData.RootDir}");
//         }
//     }

//     public string ReportRootDir()
//     {
//         if (MapData == null) return "No Map Data";
//         if (string.IsNullOrEmpty(MapData.RootDir)) return "No Root Dir";

//         return MapData.RootDir;
//     }

//     // ---------------------------------------------------------------------------------------------

//     public void CreateBaseDirectories()
//     {
//         // Validity Checks - Map, RootDir, RootDir Exists
//         if (MapData == null) return;
//         if (string.IsNullOrEmpty(MapData.RootDir)) return;
//         if (!System.IO.Directory.Exists(MapData.RootDir)) return;

//         GloMapOperations.CreateBaseDirectories(MapData.RootDir);
//     }

//     // ---------------------------------------------------------------------------------------------
//     // Basic networking control
//     // ---------------------------------------------------------------------------------------------

//     public void StartNetworking()
//     {
//         if (NetHub == null)
//         {
//             NetHub = new GloNetworkHub();
//         }
//     }

//     public void StopNetworking()
//     {
//         if (NetHub != null)
//         {
//             NetHub.endAllConnections();
//             NetHub = null;
//         }
//     }

//     public string ReportNetworkingStatus()
//     {
//         if (NetHub == null)
//             return "No Network Comms Hub";
//         return NetHub.debugDump();
//     }

//     public string ReportLocalIP()
//     {
//         if (NetHub == null)
//             return "No Network Comms Hub";
//         return NetHub.localIPAddrStr();
//     }

//     // ---------------------------------------------------------------------------------------------
//     // Basic tile management
//     // ---------------------------------------------------------------------------------------------

//     public void CreateTileEle(string tilecodeStr, int horizRes)
//     {
//         if (String.IsNullOrEmpty(tilecodeStr))
//         {
//             KoreCentralLog.AddEntry("CreateTileEle: Empty tilecodeStr");
//             return;
//         }
//         if ((tilecodeStr.Length < 2) || (tilecodeStr.Length > 20))
//         {
//             KoreCentralLog.AddEntry($"CreateTileEle: Invalid tilecodeStr length: >{tilecodeStr}<");
//             return;
//         }
//         //GloMapTileCode newTileCode = new GloMapTileCode(tilecodeStr);

//         //string tileEleFilename = System.IO.Path.Combine(MapData.RootDir, "Ele", "Level0_45x40Degs", tilecodeStr + ".bin");
//     }

// }
