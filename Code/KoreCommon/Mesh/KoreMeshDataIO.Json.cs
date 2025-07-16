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

public static partial class KoreMeshDataIO
{
    static string startColorName = "start";
    static string endColorName   = "end";
    static string colorName      = "color";

    // --------------------------------------------------------------------------------------------
    // MARK: ToJson
    // --------------------------------------------------------------------------------------------

    // Save KoreMeshData to JSON (triangles as 3 points, lines as native structure)
    public static string ToJson(KoreMeshData mesh, bool dense = false)
    {
        var obj = new
        {
            vertices        = mesh.Vertices,
            lines           = mesh.Lines,
            triangles       = mesh.Triangles,
            normals         = mesh.Normals,
            uvs             = mesh.UVs,
            vertexColors    = mesh.VertexColors,
            lineColors      = mesh.LineColors,
            triangleColors  = mesh.TriangleColors,
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = !dense,
            AllowTrailingCommas = true,
            Converters = {
                new Vector3Converter(),
                new Vector2Converter(),
                new ColorConverter(),
                new TriangleConverter(),
                new LineConverter(),
                new KoreMeshTriangleColourConverter(),
                new KoreMeshLineColourConverter()
            }
        };
        return JsonSerializer.Serialize(obj, options);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: FromJson
    // --------------------------------------------------------------------------------------------

    // Load KoreMeshData from JSON (optimistic: ignore unknowns, default missing)
    public static KoreMeshData FromJson(string json)
    {
        var mesh = new KoreMeshData();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        // --- Vertices ---
        if (root.TryGetProperty("vertices", out var vertsProp) && vertsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var v in vertsProp.EnumerateObject())
                mesh.Vertices[int.Parse(v.Name)] = Vector3Converter.ReadVector3(v.Value);
        }

        // --- Lines ---
        if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var l in linesProp.EnumerateObject())
                mesh.Lines[int.Parse(l.Name)] = LineConverter.ReadLine(l.Value);
        }

        // --- Triangles ---
        if (root.TryGetProperty("triangles", out var trisProp) && trisProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var tri in trisProp.EnumerateObject())
                mesh.Triangles[int.Parse(tri.Name)] = TriangleConverter.ReadTriangle(tri.Value);
        }

        // --- Normals ---
        if (root.TryGetProperty("normals", out var normalsProp) && normalsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var n in normalsProp.EnumerateObject())
                mesh.Normals[int.Parse(n.Name)] = Vector3Converter.ReadVector3(n.Value);
        }

        // --- UVs ---
        if (root.TryGetProperty("uvs", out var uvsProp) && uvsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var uv in uvsProp.EnumerateObject())
                mesh.UVs[int.Parse(uv.Name)] = Vector2Converter.ReadVector2(uv.Value);
        }

        // --- VertexColors ---
        if (root.TryGetProperty("vertexColors", out var colorsProp) && colorsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var c in colorsProp.EnumerateObject())
                mesh.VertexColors[int.Parse(c.Name)] = ColorConverter.ReadColor(c.Value);
        }

        // --- LineColors ---
        if (root.TryGetProperty("lineColors", out var lineColorsProp) && lineColorsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var c in lineColorsProp.EnumerateObject())
                mesh.LineColors[int.Parse(c.Name)] = KoreMeshLineColourConverter.ReadLineColour(c.Value);
        }

        // --- TriangleColors ---
        if (root.TryGetProperty("triangleColors", out var triangleColorsProp) && triangleColorsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var c in triangleColorsProp.EnumerateObject())
                mesh.TriangleColors[int.Parse(c.Name)] = KoreMeshTriangleColourConverter.ReadTriangleColour(c.Value);
        }

        return mesh;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Converters
    // --------------------------------------------------------------------------------------------

    private class Vector3Converter : JsonConverter<KoreXYZVector>
    {
        public override KoreXYZVector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadVector3(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, KoreXYZVector value, JsonSerializerOptions options)
        {
            string str = KoreXYZVectorIO.ToString(value);
            writer.WriteStringValue(str);
        }

        public static KoreXYZVector ReadVector3(JsonElement el)
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

    private class Vector2Converter : JsonConverter<KoreXYVector>
    {
        public override KoreXYVector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadVector2(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, KoreXYVector value, JsonSerializerOptions options)
        {
            string str = KoreXYVectorIO.ToString(value);
            writer.WriteStringValue(str);
        }
        public static KoreXYVector ReadVector2(JsonElement el)
        {
            string str = el.GetString() ?? "";
            if (!string.IsNullOrEmpty(str))
            {
                return KoreXYVectorIO.FromString(str);
            }

            return KoreXYVector.Zero;
        }
    }

    // --------------------------------------------------------------------------------------------

    private class ColorConverter : JsonConverter<KoreColorRGB>
    {
        public override KoreColorRGB Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadColor(doc.RootElement);
        }
        public override void Write(Utf8JsonWriter writer, KoreColorRGB value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(KoreColorIO.RBGtoHexStringShort(value));
        }
        public static KoreColorRGB ReadColor(JsonElement el)
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

    private class LineConverter : JsonConverter<KoreMeshLine>
    {
        public override KoreMeshLine Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLine(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMeshLine value, JsonSerializerOptions options)
        {

            string str = $"{value.A}, {value.B}";
            writer.WriteStringValue(str);
        }

        public static KoreMeshLine ReadLine(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length < 2) throw new FormatException("Invalid KoreMeshLine string format.");

                int pnt1Id = int.Parse(parts[0].Trim());
                int pnt2Id = int.Parse(parts[1].Trim());

                // If KoreMeshLine has color fields, parse them here as needed.
                // For now, just use the two indices.
                return new KoreMeshLine(pnt1Id, pnt2Id);
            }
            return new KoreMeshLine(0, 0);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleConverter
    // --------------------------------------------------------------------------------------------

    private class TriangleConverter : JsonConverter<KoreMeshTriangle>
    {
        public override KoreMeshTriangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadTriangle(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMeshTriangle value, JsonSerializerOptions options)
        {
            string str = $"{value.A}, {value.B}, {value.C}";
            writer.WriteStringValue(str);
        }

        public static KoreMeshTriangle ReadTriangle(JsonElement el)
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

                return new KoreMeshTriangle(a, b, c);
            }
            return new KoreMeshTriangle(0, 0, 0);
        }
    }


    // --------------------------------------------------------------------------------------------
    // MARK: LineColourConverter
    // --------------------------------------------------------------------------------------------

    private class KoreMeshLineColourConverter : JsonConverter<KoreMeshLineColour>
    {
        public override KoreMeshLineColour Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadLineColour(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMeshLineColour value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{startColorName}: {KoreColorIO.RBGtoHexStringShort(value.StartColor)}, {endColorName}: {KoreColorIO.RBGtoHexStringShort(value.EndColor)}");
        }

        public static KoreMeshLineColour ReadLineColour(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(',');
                if (parts.Length != 2) throw new FormatException($"Invalid KoreMeshLineColour string format. {str}.");
    
                string startColorStr = parts[0].Split(':')[1].Trim();
                string endColorStr   = parts[1].Split(':')[1].Trim();

                //Console.WriteLine($"KoreMeshLineColourConverter: ReadLineColour: lineIndex: {lineIndex}, startColorStr: {startColorStr}, endColorStr: {endColorStr}");

                KoreColorRGB startColor = KoreColorIO.HexStringToRGB(startColorStr);
                KoreColorRGB endColor   = KoreColorIO.HexStringToRGB(endColorStr);

                return new KoreMeshLineColour(startColor, endColor);
            }
            return new KoreMeshLineColour(KoreColorRGB.White, KoreColorRGB.White);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: TriangleColourConverter
    // --------------------------------------------------------------------------------------------

    private class KoreMeshTriangleColourConverter : JsonConverter<KoreMeshTriangleColour>
    {
        public override KoreMeshTriangleColour Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return ReadTriangleColour(doc.RootElement);
        }

        public override void Write(Utf8JsonWriter writer, KoreMeshTriangleColour value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{colorName}: {KoreColorIO.RBGtoHexStringShort(value.Color)}");
        }

        public static KoreMeshTriangleColour ReadTriangleColour(JsonElement el)
        {
            // read the string representation
            string? str = el.GetString() ?? "";

            // split by comma
            if (!string.IsNullOrEmpty(str))
            {
                var parts = str.Split(':');
                // if (parts.Length != 2) throw new FormatException("Invalid KoreMeshTriangle string format.");

                if (parts[0] != colorName)
                    throw new FormatException($"Invalid KoreMeshTriangleColour string format. Expected '{colorName}' but got '{parts[0]}'.");
                    
                string triColorStr = parts[1].Trim();

                KoreColorRGB triColor = KoreColorIO.HexStringToRGB(triColorStr);

                return new KoreMeshTriangleColour(triColor);
            }
            return new KoreMeshTriangleColour(KoreColorRGB.White);
        }
    }
}

