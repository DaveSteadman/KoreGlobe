using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoreSim.JSON;

#nullable enable

public class ClockSync : JSONMessage
{
    [JsonPropertyName("ScenTimeHMS")]
    public string ScenTimeHMS { get; set; } = string.Empty;

    public ClockSync()
    {
    }

    // -----------------------

    public float ScenTimeSecs
    {
        // get { TimeSpan ts = TimeFunctions.ParseTimeString(ScenTimeHMS); return (float)ts.TotalSeconds; }
        get { return 0; }
    }

    // -----------------------

    public static ClockSync? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("ClockSync", out JsonElement jsonContent))
                {
                    ClockSync? newMsg = JsonSerializer.Deserialize<ClockSync>(jsonContent.GetRawText());
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


