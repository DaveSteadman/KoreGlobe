using SkiaSharp;

namespace KoreCommon.Plotter.NatoSymbolGen;

// --------------------------------------------------------------------------------------------
// MARK: Basic NATO Symbol Enums and Types
// --------------------------------------------------------------------------------------------

// Usage: var x = NatoSymbolAffiliation.Friendly;
public enum NatoSymbolAffiliation
{
    Unknown,
    Friendly,
    Neutral,
    Hostile
}

public enum NatoSymbolDomain
{
    Unknown,
    Space,
    Air,
    Ground,
    Sea_Surface,
    Subsurface
}

