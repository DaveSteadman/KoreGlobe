using System;



using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using KoreCommon;
using KoreSim.JSON;

namespace KoreSim;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class KoreMessageManager
{
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityFocus(EntityFocus msg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_EntityFocus: {msg.EntityName}");
        //KoreGodotFactory.Instance.UIMsgQueue.EnqueueMessage(msg);
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityAdd(EntityAdd msg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_EntityAdd: {msg.EntityName}");

        // Check if the Entity already exists
        if (!KoreEventDriver.DoesEntityExist(msg.EntityName))
        {
            KoreEventDriver.AddEntity(msg.EntityName, msg.EntityClass);
        }

        if (KoreEventDriver.DoesEntityExist(msg.EntityName))
        {
            // Name (like tail number), Class (like F-16), Category (like aircraft)
            // KoreEventDriver.SetEntityType(
            //     msg.EntityName, msg.EntityClass, msg.EntityCategory);

            KoreEventDriver.SetEntityStartDetails(
                msg.EntityName, msg.Pos, msg.Attitude, msg.Course);

            KoreEventDriver.SetEntityCurrDetails(
                msg.EntityName, msg.Pos, msg.Attitude, msg.Course, KoreCourseDelta.Zero);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityDelete(EntityDelete msg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_EntityDelete: {msg.EntityName}");

        // Check if the Entity exists
        if (KoreEventDriver.DoesEntityExist(msg.EntityName))
        {
            KoreEventDriver.DeleteEntity(msg.EntityName);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityUpdate(EntityUpdate msg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_EntityUpdate: {msg.EntityName}");

        // Check if the Entity exists
        if (KoreEventDriver.DoesEntityExist(msg.EntityName))
        {
            KoreEventDriver.SetEntityCurrDetails(
                msg.EntityName, msg.Pos, msg.Attitude, msg.Course, msg.CourseDelta);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityPosition(EntityPosition msg)
    {
        KoreCentralLog.AddEntry($"KoreMessageManager.ProcessMessage_EntityPosition: {msg.EntityName}");

        // Check if the Entity exists
        if (KoreEventDriver.DoesEntityExist(msg.EntityName))
        {
            KoreEventDriver.SetEntityPosition(msg.EntityName, msg.Pos);
            KoreEventDriver.SetEntityCourse(msg.EntityName, msg.Course);
            KoreEventDriver.SetEntityAttitude(msg.EntityName, msg.Attitude);
        }
    }
}

