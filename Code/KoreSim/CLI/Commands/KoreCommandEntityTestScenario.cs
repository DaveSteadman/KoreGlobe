using System.Collections.Generic;

// KoreCommandVersion
using KoreCommon;

namespace KoreSim;


public class KoreCommandEntityTestScenario : KoreCommand
{
    public KoreCommandEntityTestScenario()
    {
        Signature.Add("entity");
        Signature.Add("test");
    }

    public override string Execute(List<string> parameters)
    {
        KoreCentralLog.AddEntry("KoreCommandEntityTestScenario");
        KoreEventDriver.SetupTestEntities();

        int num = KoreEventDriver.NumEntities();
        return $"KoreCommandEntityTestScenario:\n Number of Entities: {num}";
    }

}
