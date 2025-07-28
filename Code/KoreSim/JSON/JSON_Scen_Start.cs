using System;
using System.Text.Json;

namespace KoreSim.JSON;

#nullable enable

public class ScenStart : JSONMessage
{
    public ScenStart()
    {
    }

    // -----------------------

    public static ScenStart? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("ScenStart", out JsonElement jsonToken))
                {
                    ScenStart? newMsg = JsonSerializer.Deserialize<ScenStart>(jsonToken.GetRawText());
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
