using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable

namespace KoreSim.JSON;

public class AppShutdown : JSONMessage
{
    public AppShutdown()
    {
    }

    // -----------------------

    public static AppShutdown? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("AppShutdown", out JsonElement jsonToken))
                {
                    AppShutdown newMsg = new AppShutdown();
                    return newMsg;
                }
                else
                {
                    return null;
                }
            }
        }
        catch (Exception)
        {
            return null;
        }
    }
} // end class

