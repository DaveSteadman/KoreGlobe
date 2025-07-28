using System.Collections.Generic;

// KoreCommandVersion
using KoreCommon;

namespace KoreSim;


public class KoreCommandEntityReportPos : KoreCommand
{
    public KoreCommandEntityReportPos()
    {
        Signature.Add("entity");
        Signature.Add("report");
        Signature.Add("pos");
    }

    public override string Execute(List<string> parameters)
    {
        int num = KoreEventDriver.NumEntities();
        string rep = KoreEventDriver.EntityPositionsReport();

        return $"Entity Positions Report:\n{rep}";
    }
}
