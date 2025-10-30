// <fileheader>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;

#nullable enable

namespace KoreCommon;

// KoreCentralLog:
// - A logfile capability available regardless of the dotnet environment the code is ultimately used in (Unity, Godot, Console, etc...)
// - Is substantially used by the rest of the framework to log messages, errors, and other information.
// - A static class allows us to log messages from within internal functions. Has some basic file and thread protection.

public static class KoreCentralLog
{
    // A list of all the entries that have not yet been written to the log file.
    private static List<string> UnsavedEntries = new List<string>();

    // Latest entries, a buffer of the latest 200 entries
    private static List<string> LatestEntries = new List<string>();

    // The lock object to ensure thread safety when accessing the log entries.
    private static readonly object LogLock = new object();

    private static int LogEntryCount = 0;

    // Filename management
    private static bool FilenameSet = false;
    private static string Filename = "undefined.log";

    // output file and control
    private static System.Threading.Timer WriteTimer;

    // --------------------------------------------------------------------------------------------
    // MARK: Construction
    // --------------------------------------------------------------------------------------------

    static KoreCentralLog()
    {
        // Create a timer that triggers write operations every second
        WriteTimer = new System.Threading.Timer(WriteLogEntries, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    // Usage: KoreCentralLog.SetFilename("mylogfile.log");
    public static void SetFilename(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            throw new ArgumentException("Filename cannot be null or empty.");

        lock (LogLock)
        {
            Filename = filename;
            FilenameSet = true;
        }
    }

    // --------------------------------------------------------------------------------------------

    public static void AddEntry(string entry)
    {
        if (string.IsNullOrEmpty(entry))
            throw new ArgumentException("Entry cannot be null or empty.");

        string timestamp = KoreCentralTime.TimestampLocal;
        string formattedEntry = $"{timestamp} : {entry}";

        lock (LogLock)
        {
            // Add the entry to the display list, and cull any old entries - note that the latest entries are always at the end of the list.
            LatestEntries.Add(formattedEntry);
            while (LatestEntries.Count > 200) // Maintain latest 200 entries
                LatestEntries.RemoveAt(0);

            // Update the count until it gets out of control
            if (LogEntryCount < 9999)
                LogEntryCount++;

            // Add the entry to the log entries list to be saved later
            UnsavedEntries.Add(formattedEntry);

            // Cull the unsaved entries list to maintain a reasonable size
            while (UnsavedEntries.Count > 5000) // Maintain latest 1000 entries
                UnsavedEntries.RemoveAt(0);
        }
    }

    // --------------------------------------------------------------------------------------------

    private static void WriteLogEntries(object? state)
    {
        // We can only write to the log if the filename is set.
        // If we have nothing to write, return early.
        lock (LogLock)
        {
            if (!FilenameSet) return;
            if (UnsavedEntries.Count == 0) return;

            try
            {
                File.AppendAllLines(Filename, UnsavedEntries);
                UnsavedEntries.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public static void Shutdown()
    {
        WriteTimer.Dispose();
    }

    // --------------------------------------------------------------------------------------------

    public static List<string> GetLatestEntries()
    {
        lock (LogLock)
        {
            return new List<string>(LatestEntries);
        }
    }

    public static int GetLogEntryCount()
    {
        return LogEntryCount;
    }
}