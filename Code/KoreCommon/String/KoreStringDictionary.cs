// <fileheader>

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace KoreCommon;

#nullable enable

public class KoreStringDictionary
{
    private readonly Dictionary<string, object> _data = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> Keys => _data.Keys;
    public IEnumerable<KeyValuePair<string, object>> Entries => _data;

    // --------------------------------------------------------------------------------------------
    // MARK: Set/Get
    // --------------------------------------------------------------------------------------------

    public void Set(string key, object value)
    {
        if (value is KoreStringDictionary dict)
        {
            _data[key] = dict;
        }
        else
        {
            _data[key] = value?.ToString() ?? string.Empty;
        }
    }

    public string Get(string key, string fallback = "")
    {
        return _data.TryGetValue(key, out var val)
            ? val?.ToString() ?? fallback
            : fallback;
    }

    public bool Has(string key) => _data.ContainsKey(key);
    public bool Remove(string key) => _data.Remove(key);
    public void Clear() => _data.Clear();
    public int  Count => _data.Count;

    // --------------------------------------------------------------------------------------------

    // Slightly more complex set / get operations

    public string GetOrSetDefault(string key, string defaultValue)
    {
        if (!_data.ContainsKey(key))
            _data[key] = defaultValue;

        return Get(key);
    }

    public string GetOrThrow(string key)
    {
        return Get(key) ?? throw new KeyNotFoundException($"Key '{key}' not found.");
    }

    public IEnumerable<string> FindFirst(string keySubstring)
    {
        foreach (var key in _data.Keys)
        {
            if (key.Contains(keySubstring, StringComparison.OrdinalIgnoreCase))
                yield return key;
        }
    }

    public string this[string key]
    {
        get => Get(key) ?? throw new KeyNotFoundException($"Key '{key}' not found.");
        set => Set(key, value);
    }

    public List<string> KeysList()
    {
        return _data.Keys.ToList();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Consume
    // --------------------------------------------------------------------------------------------

    // Read a value and remove it from the dictionary, useful for dirty flags or other signals

    public bool Consume(string key, out string value)
    {
        if (_data.TryGetValue(key, out var val))
        {
            _data.Remove(key);
            string? v = val?.ToString();
            value = v ?? string.Empty;
            return true;
        }

        value = string.Empty;
        return false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    // Loop through all the objects in the dictionary and return a string report of their contents
    public string Report()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var kv in _data)
        {
            sb.AppendLine($"{kv.Key}: {kv.Value}");
        }
        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: JSON Export/Import
    // --------------------------------------------------------------------------------------------

    // Usage: string json = myDictionary.ExportJson();
    // Usage: myDictionary.ImportJson(json);

    public string ExportJson(bool indented = true)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            Converters = { new KoreStringDictionaryConverter() }
        };
        return JsonSerializer.Serialize(this, options);
    }

    public void ImportJson(string json)
    {
        _data.Clear();
        var options = new JsonSerializerOptions
        {
            Converters = { new KoreStringDictionaryConverter() }
        };

        var result = JsonSerializer.Deserialize<KoreStringDictionary>(json, options);
        if (result != null)
        {
            foreach (var kv in result._data)
                _data[kv.Key] = kv.Value;
        }
    }

    // --------------------------------------------------------------------------------------------
    // Custom converter for recursive support
    // --------------------------------------------------------------------------------------------

    private class KoreStringDictionaryConverter : JsonConverter<KoreStringDictionary>
    {
        public override KoreStringDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = new KoreStringDictionary();
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject");

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dict;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName");

                string? key = reader.GetString();
                if (key == null)
                    throw new JsonException("Property name was null");

                reader.Read();

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var nested = JsonSerializer.Deserialize<KoreStringDictionary>(ref reader, options);
                    if (key == null || nested == null)
                        throw new JsonException("Invalid nested dictionary format");

                    dict.Set(key, nested);
                }
                else
                {
                    string? value = reader.GetString();
                    if (value == null)
                        throw new JsonException("Property value was null");

                    dict.Set(key, value);
                }
            }

            return dict;
        }

        public override void Write(Utf8JsonWriter writer, KoreStringDictionary value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var kv in value._data)
            {
                writer.WritePropertyName(kv.Key);
                if (kv.Value is KoreStringDictionary nested)
                {
                    JsonSerializer.Serialize(writer, nested, options);
                }
                else
                {
                    writer.WriteStringValue(kv.Value?.ToString());
                }
            }
            writer.WriteEndObject();
        }
    }
}
