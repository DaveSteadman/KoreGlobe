

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using KoreCommon;


//  {"EntityWayPoints":{"EntityName":"Red Leader","Count":4,
// "Legs":[
//     {"LatDegs":51.1599,"LongDegs":-0.6887,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":0.0,"WPType":"Origin"},
//     {"LatDegs":49.122049,"LongDegs":0.531608,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"linear"},
//     {"LatDegs":50.292102,"LongDegs":0.403041,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"arc"},
//     {"LatDegs":51.276886,"LongDegs":-1.384932,"AltitudeMtrs":3500.0,"GrndSpeedMtrsSec":277.8,"WPType":"linear"}]}}


namespace KoreSim.JSON;

#nullable enable


public class WayPoint
{
    [JsonPropertyName("LatDegs")]
    public double LatDegs { get; set; } = 0.0;

    [JsonPropertyName("LongDegs")]
    public double LongDegs { get; set; } = 0.0;

    [JsonPropertyName("AltitudeMtrs")]
    public double AltitudeMtrs { get; set; } = 0.0;

    [JsonPropertyName("GrndSpeedMtrsSec")]
    public double GrndSpeedMtrsSec { get; set; } = 0.0;

    [JsonPropertyName("WPType")]
    public string WPType { get; set; } = string.Empty;
}

public class EntityWayPoints : JSONMessage
{
    [JsonPropertyName("EntityName")]
    public string EntityName { get; set; } = string.Empty;

    [JsonPropertyName("Count")]
    public int Count { get; set; } = 0;

    [JsonPropertyName("Legs")]
    public List<WayPoint> Legs { get; set; } = new List<WayPoint>();

    // -----------------------

    // Additional accessor methods

    public KoreLLAPoint GetPoint(int pntIndex)
    {
        if (pntIndex < 0) return new KoreLLAPoint();
        if (pntIndex > Legs.Count - 1) return new KoreLLAPoint();

        WayPoint currleg = Legs[pntIndex];

        KoreLLAPoint currPos = new KoreLLAPoint
        {
            LatDegs = currleg.LatDegs,
            LonDegs = currleg.LongDegs,
            AltMslM = currleg.AltitudeMtrs
        };

        return currPos;
    }

    public List<KoreLLAPoint> Points()
    {
        List<KoreLLAPoint> points = new List<KoreLLAPoint>();

        foreach (WayPoint leg in Legs)
        {
            // Get the type of waypoint. Some are "unknown" which we need to filter out.
            bool isLegValid = false;
            if (leg.WPType.Equals("origin", StringComparison.OrdinalIgnoreCase)) isLegValid = true;
            else if (leg.WPType.Equals("linear", StringComparison.OrdinalIgnoreCase)) isLegValid = true;
            else if (leg.WPType.Equals("arc", StringComparison.OrdinalIgnoreCase)) isLegValid = true;

            if (isLegValid)
            {
                KoreLLAPoint currPos = new KoreLLAPoint
                {
                    LatDegs = leg.LatDegs,
                    LonDegs = leg.LongDegs,
                    AltMslM = leg.AltitudeMtrs
                };

                points.Add(currPos);
            }
        }

        return points;
    }

    // -----------------------

    public static EntityWayPoints? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("EntityWayPoints", out JsonElement jsonContent))
                {
                    EntityWayPoints? newMsg = JsonSerializer.Deserialize<EntityWayPoints>(jsonContent.GetRawText());

                    if (newMsg == null)
                    {
                        KoreCentralLog.AddEntry("EntityWayPoints -> JsonContent is null");
                        return null;
                    }

                    KoreCentralLog.AddEntry("EntityWayPoints -> JsonContent OK");
                    KoreCentralLog.AddEntry($"EntityWayPoints -> EntityName = {newMsg.EntityName}");
                    KoreCentralLog.AddEntry($"EntityWayPoints -> Count = {newMsg.Count}");

                    // quick validation that the message data lines up
                    if (newMsg != null && newMsg.Legs != null && newMsg.Count == newMsg.Legs.Count)
                        return newMsg;

                    return null;
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


