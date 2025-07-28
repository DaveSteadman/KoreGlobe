using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using KoreCommon;

namespace KoreSim.JSON;

#nullable enable

public class EntityPosition : JSONMessage
{
    [JsonPropertyName("EntityName")]
    public string EntityName { get; set; } = string.Empty;

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

    [JsonPropertyName("YawDegs")]
    public double YawDegs { get; set; } = 0.0;

    // ------------------------------------------------------------------------------------------------------------

    // Note: We currently receive a Yaw value that is actually the heading, so this is a fudge that
    // will ultimately need to be resolved.

    [JsonIgnore]
    public KoreLLAPoint Pos
    {
        get { return new KoreLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
        set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
    }

    [JsonIgnore]
    public KoreAttitude Attitude
    {
        get { return new KoreAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = 0 }; }
        set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; YawDegs = value.YawClockwiseDegs; }
    }

    [JsonIgnore]
    public KoreCourse Course
    {
        get { return new KoreCourse() { HeadingDegs = YawDegs, SpeedMps = 0 }; }
        set { YawDegs = value.HeadingDegs; }
    }

    // ------------------------------------------------------------------------------------------------------------

    public static EntityPosition? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("EntityPosition", out JsonElement jsonContent))
                {
                    EntityPosition? newMsg = JsonSerializer.Deserialize<EntityPosition>(jsonContent.GetRawText());
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



