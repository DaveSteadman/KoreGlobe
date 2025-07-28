using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoreSim.JSON;


#nullable enable

public class JSONMessage
{
}

// --------------------------------------------------------------------------------------------

// Usage: IncomingMessageHandler.ProcessMessage("AppShutdown", message);
public class IncomingMessageHandler
{
    static public JSONMessage? ProcessMessage(string strtype, string msgText)
    {
        switch (strtype)
        {
            // System Control
            case "AppShutdown":    return AppShutdown.ParseJSON(msgText);
            case "NullMsg":        return NullMsg.ParseJSON(msgText);

            // Camera Control
            case "EntityFocus":      return EntityFocus.ParseJSON(msgText);

            // Entity
            case "EntityUpdate":     return EntityUpdate.ParseJSON(msgText);
            case "EntityPosition":   return EntityPosition.ParseJSON(msgText);
            case "EntityAdd":        return EntityAdd.ParseJSON(msgText);
            case "EntityDelete":     return EntityDelete.ParseJSON(msgText);
            case "EntityWayPoints":  return EntityWayPoints.ParseJSON(msgText);

            // Scenario / Time Control
            case "ScenLoad":       return ScenLoad.ParseJSON(msgText);
            case "ScenStart":      return ScenStart.ParseJSON(msgText);
            case "ScenStop":       return ScenStop.ParseJSON(msgText);
            case "ScenPause":      return ScenPause.ParseJSON(msgText);
            case "ScenCont":       return ScenCont.ParseJSON(msgText);
            case "ClockSync":      return ClockSync.ParseJSON(msgText);

            default:
                return null;
        }
    }

    static public string GetMessageTypeName(string message)
    {
        string substr = message.Substring(0, Math.Min(message.Length, 25));

        try
        {
            using (JsonDocument doc = JsonDocument.Parse(message))
            {
                JsonElement root = doc.RootElement;
                if (root.EnumerateObject().MoveNext())
                {
                    string firstPropertyName = root.EnumerateObject().First().Name;
                    return firstPropertyName;
                }
            }
        }
        catch (Exception)
        {
        }
        return string.Empty;
    }
} // end IncomingMessageHandler
