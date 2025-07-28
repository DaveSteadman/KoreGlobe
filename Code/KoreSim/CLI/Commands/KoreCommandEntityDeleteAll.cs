using System.Collections.Generic;

// KoreCommandEntityDelete
using KoreCommon;

namespace KoreSim;

public class KoreCommandEntityDeleteAll : KoreCommand
{
    public KoreCommandEntityDeleteAll()
    {
        Signature.Add("entity");
        Signature.Add("delall");
    }

    public override string HelpString => $"{SignatureString} <Entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 0)
        {
            return "KoreCommandEntityDeleteAll.Execute -> incorrect number of parameters";
        }

        KoreEventDriver.DeleteAllEntities();

        KoreCentralLog.AddEntry($"KoreCommandEntityDeleteAll.Execute");

        string retString = "Done.";
        return retString;
    }
}
