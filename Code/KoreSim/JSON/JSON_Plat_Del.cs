using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KoreSim.JSON;

#nullable enable

public class EntityDelete : JSONMessage
{
    [JsonPropertyName("EntityName")]
    public string EntityName { get; set; } = string.Empty;

    public static EntityDelete? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("EntityDelete", out JsonElement jsonContent))
                {
                    EntityDelete? newMsg = JsonSerializer.Deserialize<EntityDelete>(jsonContent.GetRawText());
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



