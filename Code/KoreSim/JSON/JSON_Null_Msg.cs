using System;
using System.Text.Json;

namespace KoreSim.JSON;

#nullable enable

public class NullMsg : JSONMessage
{
    public static NullMsg? ParseJSON(string json)
    {
        try
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                if (doc.RootElement.TryGetProperty("NullMsg", out JsonElement jsonContent))
                {
                    return new NullMsg();
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
