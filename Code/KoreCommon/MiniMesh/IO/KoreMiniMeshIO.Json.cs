using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable

namespace KoreCommon;

// Functions to serialize and deserialize KoreMeshData to/from JSON format.
// Note that some elements are stored in custom string formats, prioritizing human-readability over strict JSON compliance.
// - If a user is after a higher performance serialization, they should use the binary format instead of text.

public static partial class KoreMiniMeshIO
{
    static string startColorName = "start";
    static string endColorName   = "end";
    //static string colorName      = "color";

    // --------------------------------------------------------------------------------------------
    // MARK: ToJson
    // --------------------------------------------------------------------------------------------

    // Save KoreMeshData to JSON (triangles as 3 points, lines as native structure)
    // Usage: string jsonStr = KoreMeshDataIO.ToJson(mesh, dense: true);
    public static string ToJson(KoreMiniMesh mesh, bool dense = false)
    {
        var obj = new
        {
            vertices  = mesh.Vertices,
            colors    = mesh.Colors,
            lines     = mesh.Lines,
            triangles = mesh.Triangles,
            groups    = mesh.Groups
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = !dense,
            AllowTrailingCommas = true,
            Converters = {
                new KoreMiniMeshVector3Converter(),
                new KoreMiniMeshColorConverter(),
                new KoreMiniMeshLineConverter(),
                new KoreMiniMeshTriConverter(),
                new KoreMiniMeshGroupConverter()
            }
        };
        return JsonSerializer.Serialize(obj, options);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: FromJson
    // --------------------------------------------------------------------------------------------

    // Load KoreMiniMesh from JSON (optimistic: ignore unknowns, default missing)
    public static KoreMiniMesh FromJson(string json)
    {
        var mesh = new KoreMiniMesh();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        // --- Vertices ---
        if (root.TryGetProperty("vertices", out var vertsProp) && vertsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var v in vertsProp.EnumerateObject())
                mesh.Vertices[int.Parse(v.Name)] = KoreMiniMeshVector3Converter.ReadElement(v.Value);
        }

        // --- Colors ---
        if (root.TryGetProperty("colors", out var colorsProp) && colorsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var c in colorsProp.EnumerateObject())
                mesh.Colors[int.Parse(c.Name)] = KoreMiniMeshColorConverter.ReadElement(c.Value);
        }

        // --- Lines ---
        if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var l in linesProp.EnumerateObject())
                mesh.Lines[int.Parse(l.Name)] = KoreMiniMeshLineConverter.ReadLine(l.Value);
        }

        // --- Triangles ---
        if (root.TryGetProperty("triangles", out var trisProp) && trisProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var tri in trisProp.EnumerateObject())
                mesh.Triangles[int.Parse(tri.Name)] = KoreMiniMeshTriConverter.ReadTriangle(tri.Value);
        }


        // --- Groups ---
        if (root.TryGetProperty("groups", out var groupsProp) && groupsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var g in groupsProp.EnumerateObject())
                mesh.Groups[g.Name] = KoreMiniMeshGroupConverter.ReadGroup(g.Value);
        }

        return mesh;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: CONVERTERS
    // --------------------------------------------------------------------------------------------

    private class KoreMiniMeshVector3Converter : JsonConverter<KoreXYZVector>
    {
        public override KoreXYZVector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadElement(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, KoreXYZVector value, JsonSerializerOptions options)
        {
            string str = KoreXYZVectorIO.ToString(value);
            writer.WriteStringValue(str);
        }

        public static KoreXYZVector ReadElement(JsonElement el)
        {
            // if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() == 3)
            //     return new KoreXYZVector(el[0].GetSingle(), el[1].GetSingle(), el[2].GetSingle());
            // return KoreXYZVector.Zero;

            string str = el.GetString() ?? "";
            if (!string.IsNullOrEmpty(str))
            {
                return KoreXYZVectorIO.FromString(str);
            }

            return KoreXYZVector.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------

    private class KoreMiniMeshColorConverter : JsonConverter<KoreColorRGB>
    {
        public override KoreColorRGB Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadElement(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, KoreColorRGB value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(KoreColorIO.RBGtoHexStringShort(value));
        }
        public static KoreColorRGB ReadElement(JsonElement el)
        {
            string? hex = el.GetString();
            if (hex != null)
                return KoreColorIO.HexStringToRGB(hex);
            return KoreColorRGB.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: LineConverter
    // --------------------------------------------------------------------------------------------

    private class KoreMiniMeshLineConverter : JsonConverter<KoreMiniMeshLine>
    {
        public override KoreMiniMeshLine Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLine(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMiniMeshLine value, JsonSerializerOptions options)
        {

            string str = $"{value.A}, {value.B}, {value.ColorId}";
            writer.WriteStringValue(str);
        }

        public static KoreMiniMeshLine ReadLine(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length < 3) throw new FormatException("Invalid KoreMiniMeshLine string format.");

                int pnt1Id = int.Parse(parts[0].Trim());
                int pnt2Id = int.Parse(parts[1].Trim());
                int colorId = int.Parse(parts[2].Trim());

                // If KoreMiniMeshLine has color fields, parse them here as needed.
                // For now, just use the two indices.
                return new KoreMiniMeshLine(pnt1Id, pnt2Id, colorId);
            }
            return new KoreMiniMeshLine(-1, -1, -1);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleConverter
    // --------------------------------------------------------------------------------------------

    private class KoreMiniMeshTriConverter : JsonConverter<KoreMiniMeshTri>
    {
        public override KoreMiniMeshTri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadTriangle(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMiniMeshTri value, JsonSerializerOptions options)
        {
            string str = $"{value.A}, {value.B}, {value.C}";
            writer.WriteStringValue(str);
        }

        public static KoreMiniMeshTri ReadTriangle(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length != 3) throw new FormatException("Invalid KoreMeshTriangle string format.");

                int a = int.Parse(parts[0]);
                int b = int.Parse(parts[1]);
                int c = int.Parse(parts[2]);

                return new KoreMiniMeshTri(a, b, c);
            }
            return new KoreMiniMeshTri(-1, -1, -1);
        }
    }


    // --------------------------------------------------------------------------------------------
    // MARK: GroupConverter
    // --------------------------------------------------------------------------------------------

    private class KoreMiniMeshGroupConverter : JsonConverter<KoreMiniMeshGroup>
    {
        public override KoreMiniMeshGroup Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadGroup(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMiniMeshGroup value, JsonSerializerOptions options)
        {
            // Format: "colorId: 1, TriIds: [1,2,3,4]"
            string triangleIdsList = string.Join(",", value.TriIdList);
            writer.WriteStringValue($"colorId: {value.ColorId}, TriIds: [{triangleIdsList}]");
        }

        public static KoreMiniMeshGroup ReadGroup(JsonElement el)
        {
            string? str = el.GetString() ?? "";

            if (!string.IsNullOrEmpty(str))
            {
                // Parse format: "colorId: 1, triangleIds: [1,2,3,4]"
                var parts = str.Split(',');
                if (parts.Length < 2)
                    throw new FormatException($"Invalid KoreMeshTriangleGroup string format. Expected at least 2 parts: {str}");

                // Parse colorId
                int colorId = int.Parse(parts[0].Split(':')[1].Trim());

                // Parse triangleIds - everything after "triangleIds: [" and before "]"
                string triangleIdsPart = str.Substring(str.IndexOf('[') + 1);
                triangleIdsPart = triangleIdsPart.Substring(0, triangleIdsPart.LastIndexOf(']'));

                var triangleIds = new List<int>();
                if (!string.IsNullOrWhiteSpace(triangleIdsPart))
                {
                    var idStrings = triangleIdsPart.Split(',');
                    foreach (var idStr in idStrings)
                    {
                        if (int.TryParse(idStr.Trim(), out int id))
                            triangleIds.Add(id);
                    }
                }

                return new KoreMiniMeshGroup(colorId, triangleIds);
            }

            return new KoreMiniMeshGroup(-1, new List<int>());
        }
    }
}

