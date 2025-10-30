// <fileheader>


using System.Text;
using System.IO;
using System.Collections.Generic;

namespace KoreCommon;

#nullable enable

public class KoreCommandFileRename : KoreCommand
{
    public KoreCommandFileRename()
    {
        Signature.Add("file");
        Signature.Add("rename");
    }

    public override string HelpString => $"{SignatureString} <old_path> <new_path> [test]";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 2 || parameters.Count > 3)
            return $"KoreCommandFileRename: Invalid parameter count. Usage: {HelpString}";

        string oldPath = parameters[0];
        string newPath = parameters[1];
        bool testMode = parameters.Count == 3 && parameters[2].ToLower() == "test";

        if (string.IsNullOrEmpty(oldPath)) return "KoreCommandFileRename: Old path string empty.";
        if (string.IsNullOrEmpty(newPath)) return "KoreCommandFileRename: New path string empty.";

        string fixedOldPath = KoreFileOps.StandardizePath(oldPath);
        string fixedNewPath = KoreFileOps.StandardizePath(newPath);

        bool validOp = true; // Set a flag true, and then false if any check fails. Allows us to accumulate all the errors.

        // Print paths for test mode or regular operation
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"KoreCommandFileRename: Old path: {fixedOldPath}");
        sb.AppendLine($"KoreCommandFileRename: New path: {fixedNewPath}");

        // Test file existence conditions
        if (!File.Exists(fixedOldPath))
        {
            sb.AppendLine($"KoreCommandFileRename: ERROR - Old path does not exist: {fixedOldPath}");
            validOp = false;
        }
        else
        {
            if (testMode)
                sb.AppendLine($"KoreCommandFileRename: ✓ Old file exists");
        }
        if (File.Exists(fixedNewPath))
        {
            sb.AppendLine($"KoreCommandFileRename: ERROR - New path already exists: {fixedNewPath}");
            validOp = false;
        }
        else
        {
            if (testMode)
                sb.AppendLine($"KoreCommandFileRename: ✓ New path is available");
        }

        // Check if we can write to the destination directory
        string? newDir = Path.GetDirectoryName(fixedNewPath);
        if (!string.IsNullOrEmpty(newDir) && !Directory.Exists(newDir))
        {
            sb.AppendLine($"KoreCommandFileRename: ERROR - Destination directory does not exist: {newDir}");
            validOp = false;
        }
        else
        {
            if (testMode)
                sb.AppendLine($"KoreCommandFileRename: ✓ Destination directory exists");
        }

        if (testMode)
        {
            if (validOp)
                sb.AppendLine("KoreCommandFileRename: TEST MODE - All checks PASSED. Rename operation would succeed.");
            else
                sb.AppendLine("KoreCommandFileRename: TEST MODE - Some checks FAILED. Rename operation would not succeed.");
            return sb.ToString();
        }

        // Actually perform the rename
        try
        {
            KoreFileOps.RenameFile(fixedOldPath, fixedNewPath);
            sb.AppendLine($"KoreCommandFileRename: ✓ Successfully renamed file from {fixedOldPath} to {fixedNewPath}");
        }
        catch (System.Exception ex)
        {
            sb.AppendLine($"KoreCommandFileRename: ERROR - Rename failed: {ex.Message}");
        }

        return sb.ToString();
    }
}