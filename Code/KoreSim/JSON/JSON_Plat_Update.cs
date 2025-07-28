using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using KoreCommon;

namespace KoreSim.JSON;

#nullable enable

public class EntityUpdate : JSONMessage
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

    [JsonPropertyName("HeadingDegs")]
    public double HeadingDegs { get; set; } = 0.0;

    [JsonPropertyName("GrndSpeedMtrSec")]
    public double GrndSpeedMtrSec { get; set; } = 0.0;

    [JsonPropertyName("ClimbRateMtrSec")]
    public double ClimbRateMtrSec { get; set; } = 0.0;

    [JsonPropertyName("RollRateDegsSec")]
    public double RollRateDegsSec { get; set; } = 0.0;

    [JsonPropertyName("PitchRateDegsSec")]
    public double PitchRateDegsSec { get; set; } = 0.0;

    [JsonPropertyName("YawRateDegsSec")]
    public double YawRateDegsSec { get; set; } = 0.0;

    [JsonPropertyName("TurnRateDegsSec")]
    public double TurnRateDegsSec { get; set; } = 0.0;

    [JsonIgnore]
    public KoreLLAPoint Pos
    {
        get { return new KoreLLAPoint() { LatDegs = LatDegs, LonDegs = LongDegs, AltMslM = AltitudeMtrs }; }
        set { LatDegs = value.LatDegs; LongDegs = value.LonDegs; AltitudeMtrs = value.AltMslM; }
    }

    [JsonIgnore]
    public KoreAttitude Attitude
    {
        get { return new KoreAttitude() { RollClockwiseDegs = RollDegs, PitchUpDegs = PitchDegs, YawClockwiseDegs = YawDegs }; }
        set { RollDegs = value.RollClockwiseDegs; PitchDegs = value.PitchUpDegs; YawDegs = value.YawClockwiseDegs; }
    }

    [JsonIgnore]
    public KoreCourse Course
    {
        get { return new KoreCourse() { HeadingDegs = HeadingDegs, SpeedMps = GrndSpeedMtrSec, ClimbRateMps = ClimbRateMtrSec }; }
        set { HeadingDegs = value.HeadingDegs; GrndSpeedMtrSec = value.SpeedMps; }
    }

    [JsonIgnore]
    public KoreCourseDelta CourseDelta
    {
        get { return new KoreCourseDelta() { SpeedChangeMpMps = 0, HeadingChangeClockwiseDegsSec = -TurnRateDegsSec }; }
        set { TurnRateDegsSec = -value.HeadingChangeClockwiseDegsSec; }
    }

    public static EntityUpdate? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("EntityUpdate", out JsonElement jsonContent))
                {
                    EntityUpdate? newMsg = JsonSerializer.Deserialize<EntityUpdate>(jsonContent.GetRawText());
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




