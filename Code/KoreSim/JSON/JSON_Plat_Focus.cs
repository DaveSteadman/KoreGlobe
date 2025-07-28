using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoreSim.JSON;

#nullable enable

public class EntityFocus : JSONMessage
{
    [JsonPropertyName("EntityName")]
    public string EntityName { get; set; } = string.Empty;

    public static EntityFocus? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("EntityFocus", out JsonElement jsonContent))
                {
                    EntityFocus? newMsg = JsonSerializer.Deserialize<EntityFocus>(jsonContent.GetRawText());
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
