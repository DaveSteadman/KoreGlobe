using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using KoreCommon;

namespace KoreSim.JSON;

#nullable enable

public class EntityAdd : JSONMessage
{
    [JsonPropertyName("EntityName")]
    public string EntityName { get; set; } = string.Empty;

    [JsonPropertyName("EntityCategory")]
    public string EntityCategory { get; set; } = string.Empty;

    [JsonPropertyName("ThreatType")]
    public string ThreatType { get; set; } = string.Empty;

    [JsonPropertyName("Display")]
    public bool Display { get; set; } = false;

    [JsonPropertyName("EntityClass")]
    public string EntityClass { get; set; } = string.Empty;

    [JsonPropertyName("EntityDispSymb")]
    public string EntityDispSymb { get; set; } = string.Empty;

    [JsonPropertyName("LatDegs")]
    public double LatDegs { get; set; } = 0.0;

    [JsonPropertyName("LongDegs")]
    public double LongDegs { get; set; } = 0.0;

    [JsonPropertyName("AltitudeMtrs")]
    public double AltitudeMtrs { get; set; } = 0.0;

    [JsonPropertyName("RollDegs")]
    public double RollDegs { get; set; } = 0.0;

    [JsonPropertyName("PitchDegs")]
    public double PitchDegs { get; set; } = 0.0;

    [JsonPropertyName("HeadingDegs")]
    public double HeadingDegs { get; set; } = 0.0;

    [JsonIgnore]
    public KoreLLAPoint Pos
    {
        get { return new KoreLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
        set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
    }

    [JsonIgnore]
    public KoreAttitude Attitude
    {
        get { return new KoreAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = 0.0 }; }
        set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; }
    }

    [JsonIgnore]
    public KoreCourse Course
    {
        get { return new KoreCourse() { HeadingDegs = HeadingDegs }; }
        set { HeadingDegs = value.HeadingDegs; }
    }

    public static EntityAdd? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("EntityAdd", out JsonElement jsonContent))
                {
                    EntityAdd? newMsg = JsonSerializer.Deserialize<EntityAdd>(jsonContent.GetRawText());
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
