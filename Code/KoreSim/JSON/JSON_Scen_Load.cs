using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using KoreCommon;

namespace KoreSim.JSON;

#nullable enable

public class ScenLoad : JSONMessage
{
    [JsonPropertyName("ScenName")]
    public string ScenName { get; set; } = string.Empty;

    [JsonPropertyName("Lat")]
    public double Lat { get; set; } = 0.0;

    [JsonPropertyName("Long")]
    public double Long { get; set; } = 0.0;

    [JsonPropertyName("EarthModel")]
    public string EarthModel { get; set; } = string.Empty;

    [JsonPropertyName("RxName")]
    public string RxName { get; set; } = string.Empty;

    [JsonPropertyName("DFModel")]
    public string DFModel { get; set; } = string.Empty;

    // --------------------------------------------------------------------------------------------

    public KoreLLAPoint ScenPos { get { return new KoreLLAPoint() { LatDegs = Lat, LonDegs = Long }; } }

    // --------------------------------------------------------------------------------------------

    public static ScenLoad? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("ScenLoad", out JsonElement jsonContent))
                {
                    ScenLoad? newMsg = JsonSerializer.Deserialize<ScenLoad>(jsonContent.GetRawText());
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
