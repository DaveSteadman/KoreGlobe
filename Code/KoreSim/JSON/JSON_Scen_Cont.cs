using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoreSim.JSON;

#nullable enable

public class ScenCont : JSONMessage
{
    [JsonPropertyName("ScenTimeHMS")]
    public string ScenTimeHMS { get; set; } = string.Empty;

    public ScenCont()
    {
    }

    // -----------------------

    public float ScenTimeSecs
    {
        // get { TimeSpan ts = TimeFunctions.ParseTimeString(ScenTime); return (float)ts.TotalSeconds; }
        get { return 0; }

    }

    // -----------------------

    public static ScenCont? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("ScenCont", out JsonElement jsonContent))
                {
                    ScenCont? newMsg = JsonSerializer.Deserialize<ScenCont>(jsonContent.GetRawText());
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



