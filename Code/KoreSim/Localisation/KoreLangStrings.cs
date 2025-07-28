using System;
using System.Collections.Generic;
using System.IO;

#nullable enable

namespace KoreCommon;

public class KoreLangStrings
{
    private static KoreLangStrings _instance = null!;
    private static readonly object _lock = new();

    private Dictionary<string, Dictionary<string, string>> _languageMap = new(StringComparer.OrdinalIgnoreCase);

    private string _activeLanguage = "English";
    public string CurrActiveLanguage => _activeLanguage;

    private static readonly string UndefinedEntry = "Undefined";

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    private KoreLangStrings(string filename)
    {
        LoadFile(filename);
    }

    public static KoreLangStrings Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    // Use the default filename if not specified
                    throw new InvalidOperationException("KoreLangStrings instance not initialized. Call KoreLangStrings.Initialize() first.");
                }
                return _instance;
            }
        }
    }

    public static void Initialize(string filename)
    {
        lock (_lock)
        {
            if (_instance != null)
                throw new InvalidOperationException("KoreLangStrings has already been initialized.");

            _instance = new KoreLangStrings(filename);
        }
    }

    public static bool IsInitialized
    {
        get
        {
            lock (_lock) { return _instance != null; }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load & Set
    // --------------------------------------------------------------------------------------------

    public void LoadFile(string filename)
    {
        lock (_lock)
        {
            _languageMap.Clear();
            foreach (var line in File.ReadAllLines(filename))
            {
                // Allow for empty lines and comments
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split(',', 3); // language, key, value
                if (parts.Length < 3) continue;

                string lang = parts[0].Trim();
                string key = parts[1].Trim();
                string val = parts[2].Trim();

                if (!_languageMap.ContainsKey(lang))
                    _languageMap[lang] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                _languageMap[lang][key] = val;
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public void SetActiveLanguage(string lang)
    {
        _activeLanguage = lang.Trim();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Get
    // --------------------------------------------------------------------------------------------

    public string Get(string key, string? fallback = null)
    {
        // Attempt to retrieve the value from the active language dictionary
        if (_languageMap.TryGetValue(_activeLanguage, out var dict) && dict.TryGetValue(key, out var value))
            return value;

        // If not found, check if a fallback language is provided
        if (!string.IsNullOrEmpty(fallback))
            return fallback;

        // If the fallback value is not provided, return an empty string
        return UndefinedEntry;
    }

    public string GetForLanguage(string key, string lang, string? fallback = null)
    {
        // Attempt to retrieve the value from the specified language dictionary
        if (_languageMap.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out var value))
            return value;

        // If not found, check if a fallback language is provided
        if (!string.IsNullOrEmpty(fallback))
            return fallback;

        // If the fallback value is not provided, return an empty string
        return UndefinedEntry;
    }

    public List<string> GetAvailableLanguages()
    {
        return new List<string>(_languageMap.Keys);
    }
}
