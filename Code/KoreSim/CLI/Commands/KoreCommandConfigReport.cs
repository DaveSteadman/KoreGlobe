using System.Collections.Generic;
using System.Text;

using KoreCommon;
namespace KoreSim;

#nullable enable

public class KoreCommandConfigReport : KoreCommand
{
    public KoreCommandConfigReport()
    {
        Signature.Add("config");
        Signature.Add("report");
    }

    public override string HelpString => $"{SignatureString}";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();

        bool validOperation = true;

        if (KoreSimFactory.Instance.KoreConfig == null)
        {
            sb.AppendLine("KoreCommandConfigReport.Execute -> KoreSimFactory.Instance.KoreConfig is null");
            validOperation = false;
        }

        if (validOperation)
        {
            int numEntries = KoreSimFactory.Instance.KoreConfig?.Count ?? 0;
            sb.AppendLine($"Config Report: {numEntries} entries");

            sb.Append(KoreSimFactory.Instance.KoreConfig?.Report() ?? "No config available");
        }

        // -------------------------------------------------

        return sb.ToString();
    }
}