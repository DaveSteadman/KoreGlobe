
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace KoreCommon;

#nullable enable

public class KoreCliCmdFileRename : KoreCommand
{
    public KoreCliCmdFileRename()
    {
        Signature.Add("file");
        Signature.Add("rename");
    }

    public override string HelpString => $"{SignatureString} <old_path> <new_path> [test]";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 2 || parameters.Count > 3)
            return $"KoreCliCmdFileRename: Invalid parameter count. Usage: {HelpString}";

        string oldPath = parameters[0];
        string newPath = parameters[1];
        bool testMode = parameters.Count == 3 && parameters[2].ToLower() == "test";

        if (string.IsNullOrEmpty(oldPath)) return "KoreCliCmdFileRename: Old path string empty.";
        if (string.IsNullOrEmpty(newPath)) return "KoreCliCmdFileRename: New path string empty.";

        string fixedOldPath = KoreFileOps.StandardizePath(oldPath);
        string fixedNewPath = KoreFileOps.StandardizePath(newPath);

        bool validOp = true; // Set a flag true, and then false if any check fails. Allows us to accumulate all the errors.

        // Print paths for test mode or regular operation
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"KoreCliCmdFileRename: Old path: {fixedOldPath}");
        sb.AppendLine($"KoreCliCmdFileRename: New path: {fixedNewPath}");

        // Test file existence conditions
        if (!File.Exists(fixedOldPath))
        {
            sb.AppendLine($"KoreCliCmdFileRename: ERROR - Old path does not exist: {fixedOldPath}");
            validOp = false;
        }
        else
        {
            if (testMode)
                sb.AppendLine($"KoreCliCmdFileRename: ✓ Old file exists");
        }
        if (File.Exists(fixedNewPath))
        {
            sb.AppendLine($"KoreCliCmdFileRename: ERROR - New path already exists: {fixedNewPath}");
            validOp = false;
        }
        else
        {
            if (testMode)
                sb.AppendLine($"KoreCliCmdFileRename: ✓ New path is available");
        }

        // Check if we can write to the destination directory
        string? newDir = Path.GetDirectoryName(fixedNewPath);
        if (!string.IsNullOrEmpty(newDir) && !Directory.Exists(newDir))
        {
            sb.AppendLine($"KoreCliCmdFileRename: ERROR - Destination directory does not exist: {newDir}");
            validOp = false;
        }
        else
        {
            if (testMode)
                sb.AppendLine($"KoreCliCmdFileRename: ✓ Destination directory exists");
        }

        if (testMode)
        {
            if (validOp)
                sb.AppendLine("KoreCliCmdFileRename: TEST MODE - All checks PASSED. Rename operation would succeed.");
            else
                sb.AppendLine("KoreCliCmdFileRename: TEST MODE - Some checks FAILED. Rename operation would not succeed.");
            return sb.ToString();
        }

        // Actually perform the rename
        try
        {
            KoreFileOps.RenameFile(fixedOldPath, fixedNewPath);
            sb.AppendLine($"KoreCliCmdFileRename: ✓ Successfully renamed file from {fixedOldPath} to {fixedNewPath}");
        }
        catch (System.Exception ex)
        {
            sb.AppendLine($"KoreCliCmdFileRename: ERROR - Rename failed: {ex.Message}");
        }

        return sb.ToString();
    }
}