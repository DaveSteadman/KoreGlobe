using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoreSim.JSON;

#nullable enable

public class ScenPause : JSONMessage
{
    [JsonPropertyName("ScenTimeHMS")]
    public string ScenTimeHMS { get; set; } = string.Empty;

    public ScenPause()
    {
    }

    // -----------------------

    public float ScenTimeSecs
    {
        // get { TimeSpan ts = TimeFunctions.ParseTimeString(ScenTime); return (float)ts.TotalSeconds; }
        get { return 0; }
    }

    // -----------------------

    public static ScenPause? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("ScenPause", out JsonElement jsonContent))
                {
                    ScenPause? newMsg = JsonSerializer.Deserialize<ScenPause>(jsonContent.GetRawText());
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
