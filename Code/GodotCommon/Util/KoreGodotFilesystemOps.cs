
using Godot;
using System.Collections.Generic;
//using System.IO;

using KoreCommon;

// Class to perform file operations on the Godot virtual file system under "res://".
// - List files in directories, supplying lists of paths that can then be directly accessed in other calls.
// - One intended use is finding files in DLCs after a PCK file has been mounted.

public static class KoreGodotFilesystemOps
{
    public enum EFileType { Files, Directories, Both };

    // Usage: string filePath = KoreGodotFilesystemOps.GetActualPath("res://assets/earth/earth.jpg");

    public static string GetActualPath(string resourcePath)
    {
        // Convert the resource path to an absolute path
        string absolutePath = ProjectSettings.GlobalizePath(resourcePath);

        return absolutePath;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: File & Dir Lists
    // --------------------------------------------------------------------------------------------

    // List all the files, in the godot virtual file system, under a given top level directory.

    // Usage: List<string> fileList = KoreGodotFilesystemOps.ListFiles("res://assets/earth");
    public static List<string> ListFiles(string topLevel, EFileType content = EFileType.Files, bool recursive = true)
    {
        List<string> fileList = new List<string>();

        GD.Print($"Listing contents of directory: {topLevel}");

        using var dir = DirAccess.Open(topLevel);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            while (!string.IsNullOrEmpty(fileName))
            {
                if (fileName == "." || fileName == "..")
                {
                    fileName = dir.GetNext();
                    continue;
                }

                if (dir.CurrentIsDir())
                {
                    if (content == EFileType.Directories || content == EFileType.Both)
                        fileList.Add(JoinPaths(topLevel, fileName));

                    if (recursive)
                    {
                        List<string> subList = ListFiles(JoinPaths(topLevel, fileName), content);
                        fileList.AddRange(subList);
                    }
                }
                else
                {
                    if (content == EFileType.Files || content == EFileType.Both)
                        fileList.Add(JoinPaths(topLevel, fileName));
                }
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }

        return fileList;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utilities
    // --------------------------------------------------------------------------------------------

    public static string JoinPaths(string path1, string path2)
    {
        if (path1.EndsWith("/")) path1 = path1.Substring(0, path1.Length - 1);
        if (path2.StartsWith("/")) path2 = path2.Substring(1);
        return $"{path1}/{path2}";
    }

    public static string JoinPaths(string path1, string path2, string path3)
    {
        return JoinPaths(JoinPaths(path1, path2), path3);
    }

    // Usage: string fileName = KoreGodotFilesystemOps.LastPathElement("res://assets/earth/earth.jpg");
    public static string LastPathElement(string path)
    {
        return path.Split("/")[^1]; // ^1 == last element in array in C# 8.0.
    }

    // Check a path exists in the Godot virtual file system.
    // Usage: bool exists = KoreGodotFilesystemOps.Exists("res://assets/earth/earth.jpg");
    public static bool Exists(string path)
    {
        bool exists = FileAccess.FileExists(path);
        return exists;
    }

    // Usage: string strFileContent = KoreGodotFilesystemOps.ReadStringFromFile(PathName);
    public static string ReadStringFromFile(string resPath)
    {
        using var file = FileAccess.Open(resPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr("Failed to open file: ", resPath);
            KoreCentralLog.AddEntry($"Failed to open file: {resPath}");
            return string.Empty;
        }
        return file.GetAsText();
    }

}
