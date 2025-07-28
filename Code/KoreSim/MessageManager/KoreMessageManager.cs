using System;
using System.Threading;

using KoreCommon;

using KoreSim.JSON;

#nullable enable

namespace KoreSim;

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class KoreMessageManager
{
    private bool ThreadRunning = false;

    // --------------------------------------------------------------------------------------------

    public void Start()
    {
        ThreadRunning = true;

        // Start the thread
        Thread t = new Thread(IncomingMessageMonitor);
        t.Start();
    }

    // --------------------------------------------------------------------------------------------

    // Thread function, looking for new messages
    public void IncomingMessageMonitor()
    {
        // Loop forever
        while (ThreadRunning)
        {
            // Check for new messages
            while (KoreEventDriver.HasIncomingMessage())
            {
                // Get the message - still in text format
                KoreMessageText? nullableMessage = KoreEventDriver.GetIncomingMessage();

                if (nullableMessage != null)
                {
                    // Explicitly cast nullable type to non-nullable type
                    KoreMessageText message = (KoreMessageText)nullableMessage;
                    // Process the message
                    ProcessMessage(message);
                }
            }

            // Sleep for a bit
            System.Threading.Thread.Sleep(100); // milliseconds
        }
    }

    // --------------------------------------------------------------------------------------------

    public void EndThread()
    {
        ThreadRunning = false;
    }

    // --------------------------------------------------------------------------------------------

    void ProcessMessage(KoreMessageText message)
    {
        if (!message.HasValidContent())
        {
            KoreCentralLog.AddEntry("KoreMessageManager.ProcessMessage: Invalid message content received.");
            return;
        }

        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage: {message}");

        // Get the message type
        string msgType = IncomingMessageHandler.GetMessageTypeName(message.msgData);

        // Convert the message to a JSON object
        JSONMessage? msg = IncomingMessageHandler.ProcessMessage(msgType, message.msgData);

        if (msg == null)
        {
            KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage: Failed to parse message type: {msgType}");
            return;
        }

        if (msg is EntityAdd entityAddMsg) { ProcessMessage_EntityAdd(entityAddMsg); }
        else if (msg is EntityDelete entityDelMsg) { ProcessMessage_EntityDelete(entityDelMsg); }
        else if (msg is EntityPosition entityPosMsg) { ProcessMessage_EntityPosition(entityPosMsg); }
        else if (msg is EntityUpdate entityUpdMsg) { ProcessMessage_EntityUpdate(entityUpdMsg); }
        else if (msg is EntityWayPoints entityWayPtsMsg) { ProcessMessage_EntityWayPoints(entityWayPtsMsg); }
        else if (msg is EntityFocus entityFocusMsg) { ProcessMessage_EntityFocus(entityFocusMsg); }

        else if (msg is ScenLoad scenLoadMsg) { ProcessMessage_ScenLoad(scenLoadMsg); }
        else if (msg is ScenStart scenStartMsg) { ProcessMessage_ScenStart(scenStartMsg); }
        else if (msg is ScenStop scenStopMsg) { ProcessMessage_ScenStop(scenStopMsg); }
        else if (msg is ScenPause scenPauseMsg) { ProcessMessage_ScenPause(scenPauseMsg); }
        else if (msg is ScenCont scenContMsg) { ProcessMessage_ScenCont(scenContMsg); }
        else if (msg is ClockSync clockSyncMsg) { ProcessMessage_ClockSync(clockSyncMsg); }

        // else if (msg is EntityElement_AddCircularScan entityElemAddCircScanMsg) { ProcessMessage_EntityElement_AddCircularScan(entityElemAddCircScanMsg); }
        else
        {
            KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage: Unknown message type: {msgType} // {message.msgData}");
        }

    }

}

