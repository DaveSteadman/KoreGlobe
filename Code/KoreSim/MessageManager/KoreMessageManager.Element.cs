using System;
using System.Collections.Generic;

using KoreCommon;
using KoreSim.JSON;

namespace KoreSim;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class KoreMessageManager
{
    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Waypoints
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityWayPoints(EntityWayPoints entityWayPtsMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_EntityWayPoints: Name:{entityWayPtsMsg.EntityName}");

        string entityName = entityWayPtsMsg.EntityName;
        List<KoreLLAPoint> points = entityWayPtsMsg.Points();



        // KoreEventDriver.EntitySetRoute(entityName, points);
    }


}
