using System;

using KoreCommon;
using KoreSim.JSON;

namespace KoreSim;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class KoreMessageManager
{
    private void ProcessMessage_ScenLoad(ScenLoad scenLoadMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_ScenLoad: Name:{scenLoadMsg.ScenName} ScenPos:{scenLoadMsg.ScenPos}");
        //KoreSimFactory.Instance.UIState.ScenarioName = scenLoadMsg.ScenName;
        KoreEventDriver.DeleteAllEntities();
    }

    private void ProcessMessage_ScenStart(ScenStart scenStartMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_ScenStart");
        KoreEventDriver.SimClockReset();
        KoreEventDriver.SimClockStart();
    }

    private void ProcessMessage_ScenStop(ScenStop scenStopMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_ScenStop");
        KoreEventDriver.SimClockStop();
    }

    private void ProcessMessage_ScenPause(ScenPause scenPauseMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_ScenPause: ScenTime:{scenPauseMsg.ScenTimeHMS}");
        KoreEventDriver.SimClockStop();

        KoreEventDriver.SetSimTimeHMS(scenPauseMsg.ScenTimeHMS);
    }

    private void ProcessMessage_ScenCont(ScenCont scenContMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_ScenCont: ScenTime:{scenContMsg.ScenTimeHMS}");
        KoreEventDriver.SetSimTimeHMS(scenContMsg.ScenTimeHMS);
        KoreEventDriver.SimClockResume();
    }

    private void ProcessMessage_ClockSync(ClockSync clockSyncMsg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_ClockSync: ScenTimeHMS:{clockSyncMsg.ScenTimeHMS}");
        KoreEventDriver.SetSimTimeHMS(clockSyncMsg.ScenTimeHMS);
    }

}
