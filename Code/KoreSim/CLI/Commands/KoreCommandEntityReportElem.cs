using System.Collections.Generic;

// KoreCommandVersion
using KoreCommon;

namespace KoreSim;


public class KoreCommandEntityReportElem : KoreCommand
{
    public KoreCommandEntityReportElem()
    {
        Signature.Add("entity");
        Signature.Add("report");
        Signature.Add("elem");
    }

    public override string Execute(List<string> parameters)
    {
        int num = KoreEventDriver.NumEntities();
        string rep = KoreEventDriver.EntityElementsReport();

        return $"Entity Elements Report:\n{rep}";
    }
}
