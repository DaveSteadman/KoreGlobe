using System.Collections.Generic;

// KoreCommandEntityAdd
using KoreCommon;

namespace KoreSim;

public class KoreCommandEntityAdd : KoreCommand
{
    public KoreCommandEntityAdd()
    {
        Signature.Add("entity");
        Signature.Add("add");
    }

    public override string HelpString => $"{SignatureString} <Entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "KoreCommandEntityAdd.Execute -> insufficient parameters";
        }

        string entityName = parameters[0];
        string retString = "";

        // Commands exist to perform their task, delete any pre-existing Entity by the name
        if (KoreEventDriver.DoesEntityExist(entityName))
        {
            KoreEventDriver.DeleteEntity(entityName);
            retString += $"Entity {entityName} deleted. ";
        }

        KoreEventDriver.AddEntity(entityName);
        retString += $"Entity {entityName} added.";

        // Set the default Entity details - adding it with no location will create rendering div0's etc.
        KoreEventDriver.DefaultEntityDetails(entityName);

        KoreCentralLog.AddEntry($"KoreCommandEntityAdd.Execute -> {retString}");
        return retString;
    }
}
