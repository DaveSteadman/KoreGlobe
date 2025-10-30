// <fileheader>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KoreCommon;

#nullable enable

public static class KoreFileOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Standardise Path
    // --------------------------------------------------------------------------------------------

    // Function to standardize a path, changing any backslash characters to a "/".

    // Usage example: KoreFileOps.StandardizePath("C:\\Users\\User\\Documents\\file.txt");
    public static string StandardizePath(string inpath)
    {
        if (String.IsNullOrEmpty(inpath))
            return "";

        // First, replace all backslashes with forward slashes.
        string standardizedPath = inpath.Replace('\\', '/');

        // Then, ensure we do not turn protocol separators like "http://" into "http:/" by only replacing double slashes if they are not following ":".
        int protocolSeparatorIndex = standardizedPath.IndexOf("://");
        if (protocolSeparatorIndex != -1)
        {
            // Process the part before the protocol separator.
            string beforeProtocol = standardizedPath.Substring(0, protocolSeparatorIndex).Replace("//", "/");

            // Combine the processed part with the unmodified remainder of the path.
            standardizedPath = beforeProtocol + standardizedPath.Substring(protocolSeparatorIndex);
        }
        else
        {
            // If there's no protocol separator, just replace double slashes.
            standardizedPath = standardizedPath.Replace("//", "/");
        }

        return standardizedPath;
    }

    // List equivalent of StandardizePath
    public static List<string> StandardizePathList(List<string> paths)
    {
        List<string> standardizedPaths = new List<string>();
        foreach (string path in paths)
            standardizedPaths.Add(StandardizePath(path));

        return standardizedPaths;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Get File List
    // --------------------------------------------------------------------------------------------

    // List all the files under a given top level directory.
    public static List<string> Filenames(string startPath)
    {
        List<string> filenames = new List<string>();
        string normalizedStartPath = StandardizePath(startPath);

        try
        {
            string[] files = Directory.GetFiles(normalizedStartPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                filenames.Add(StandardizePath(file));
            }
        }
        catch //(Exception ex)
        {
            // Console.WriteLine($"Error retrieving files: {ex.Message}"); // Can't write to console, this has to be usable in any framework (Unity, Godot, Command line etc).
        }

        return filenames;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Joining Paths
    // --------------------------------------------------------------------------------------------

    // The standard Path.Combinbe can introduce a backslash "\" character.
    // This version with will:
    // - standardize the paths to use forward slashes.
    // - check for a backslah on the end of first or begoinning of the second path, removing it.
    // - Join the to paths toegether with a forward slash.
    //
    // Usage example: KoreFileOps.JoinPaths("C:/Users/User/Documents", "file.txt");
    //
    public static string JoinPaths(string path1, string path2)
    {
        string normalizedPath1 = StandardizePath(path1);
        string normalizedPath2 = StandardizePath(path2);

        if (normalizedPath1.EndsWith("/"))
            normalizedPath1 = normalizedPath1.Substring(0, normalizedPath1.Length - 1);

        if (normalizedPath2.StartsWith("/"))
            normalizedPath2 = normalizedPath2.Substring(1);

        return $"{normalizedPath1}/{normalizedPath2}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Directory
    // --------------------------------------------------------------------------------------------

    public static string DirectoryFromPath(string filepath)
    {
        // If the filepath is null or empty, return an empty string.
        if (string.IsNullOrEmpty(filepath))
            return string.Empty;

        string? directory = Path.GetDirectoryName(filepath);

        // If the directory is null or empty, return an empty string.
        if (string.IsNullOrEmpty(directory))
            return string.Empty;

        return directory;
    }

    public static bool DirectoryExists(string directory)
    {
        return Directory.Exists(directory);
    }

    // Usage: KoreFileOps.CreateDirectory("C:/Users/User/Documents/NewDirectory");
    // Creates a directory at the specified path. If the directory already exists, it does nothing.
    // This is a wrapper around the System.IO.Directory.CreateDirectory method.
    // It will create all directories in the path if they do not exist.
    public static void CreateDirectory(string directory)
    {
        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException($"Invalid file path: '{directory}'");

        // Directly create the directory - don't use DirectoryFromPath which treats it as a file path
        Directory.CreateDirectory(directory);
    }

        // Usage: KoreFileOps.CreateDirectoryForFile("C:/Users/User/Documents/file.txt");
    public static void CreateDirectoryForFile(string filepath)
    {
        string directory = DirectoryFromPath(filepath);

        if (!DirectoryExists(directory))
            CreateDirectory(directory);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: File Extension
    // --------------------------------------------------------------------------------------------

    // Check the filename string has the right extension, or add it if it doesn't.

    // Usage example: KoreFileOps.EnsureExtension("file.txt", ".json");
    public static string EnsureExtension(string filename, string extension)
    {
        if (filename.EndsWith(extension))
            return filename;
        else
            return $"{filename}{extension}";
    }

    // Usage example: KoreFileOps.GetExtension("file.txt");
    public static string GetExtension(string filepath)
    {
        // Determine the file extension
        string extension = System.IO.Path.GetExtension(filepath).ToLowerInvariant();

        return extension;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: File Rename
    // --------------------------------------------------------------------------------------------

    // Usage: KoreFileOps.RenameFile("oldpath.txt", "newpath.txt");
    // Renames (moves) a file from oldPath to newPath. Throws if the source does not exist.
    public static void RenameFile(string oldPath, string newPath)
    {
        if (!File.Exists(oldPath))
            throw new FileNotFoundException($"Source file not found: {oldPath}");

        // Ensure the target directory exists
        string? newDir = Path.GetDirectoryName(newPath);
        if (!string.IsNullOrEmpty(newDir) && !Directory.Exists(newDir))
            Directory.CreateDirectory(newDir);

        File.Move(oldPath, newPath);
    }

    // Usage: KoreFileOps.BulkRenameFiles(oldPaths, newPaths);
    // Renames (moves) files in bulk. Throws if the lists are not the same length or if any source file does not exist.
    public static void BulkRenameFiles(List<string> oldPaths, List<string> newPaths)
    {
        if (oldPaths.Count != newPaths.Count)
            throw new ArgumentException($"oldPaths and newPaths must have the same number of elements.");

        for (int i = 0; i < oldPaths.Count; i++)
        {
            RenameFile(oldPaths[i], newPaths[i]);
        }
    }
}

