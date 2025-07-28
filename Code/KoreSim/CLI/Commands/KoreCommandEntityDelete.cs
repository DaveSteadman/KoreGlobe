using System.Collections.Generic;

// KoreCommandEntityDelete
using KoreCommon;

namespace KoreSim;

public class KoreCommandEntityDelete : KoreCommand
{

    public KoreCommandEntityDelete()
    {
        Signature.Add("entity");
        Signature.Add("del");
    }

    public override string HelpString => $"{SignatureString} <Entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "KoreCommandEntityDelete.Execute -> insufficient parameters";
        }

        string entityName = parameters[0];
        string retString = "";

        // Delete the Entity
        if (KoreEventDriver.DoesEntityExist(entityName))
        {
            KoreEventDriver.DeleteEntity(entityName);
            retString = $"Entity {entityName} deleted.";
        }
        else
        {
            retString = $"Entity {entityName} not found.";
        }

        KoreCentralLog.AddEntry($"KoreCommandEntityDelete.Execute -> {retString}");
        return retString;
    }
}
