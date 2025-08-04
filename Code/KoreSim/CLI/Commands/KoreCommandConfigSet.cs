using System.Collections.Generic;
using System.Text;

using KoreCommon;
namespace KoreSim;

public class KoreCommandConfigSet : KoreCommand
{
    public KoreCommandConfigSet()
    {
        Signature.Add("config");
        Signature.Add("set");
    }

    public override string HelpString => $"{SignatureString} <name> <value>";

    public override string Execute(List<string> parameters)
    {
        StringBuilder sb = new StringBuilder();
        bool validOperation = true;

        if (parameters.Count != 2)
        {
            return "KoreCommandConfigSet: Wrong number of parameters. Expected 2. Got " + parameters.Count;
        }

        string name = parameters[0];
        string value = parameters[1];

        if (KoreSimFactory.Instance.KoreConfig == null)
        {
            sb.AppendLine("KoreCommandConfigSet.Execute -> KoreSimFactory.Instance.KoreConfig is null");
            validOperation = false;
        }

        if (validOperation)
        {
            KoreSimFactory.Instance.KoreConfig.Set(name, value);
            sb.AppendLine($"KoreCommandConfigSet.Execute -> Set '{name}' to '{value}'");
            
            KoreSimFactory.Instance.SaveConfig(KoreSimFactory.ConfigPath);
        }

        // -------------------------------------------------

        return sb.ToString();
    }
}