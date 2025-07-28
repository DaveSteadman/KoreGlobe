using System;
using System.Text.Json;

namespace KoreSim.JSON;

#nullable enable

public class ScenStop : JSONMessage
{
    public ScenStop()
    {
    }

    // -----------------------

    public static ScenStop? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("ScenStop", out JsonElement jsonToken))
                {
                    ScenStop? newMsg = JsonSerializer.Deserialize<ScenStop>(jsonToken.GetRawText());
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
