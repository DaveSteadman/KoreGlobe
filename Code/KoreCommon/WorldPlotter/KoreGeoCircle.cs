// <fileheader>

#nullable enable

namespace KoreCommon;

/// <summary>
/// A geographic circle defined by center point and radius
/// </summary>
public class KoreGeoCircle : KoreGeoFeature
{
    public KoreLLPoint Center { get; set; }
    public double RadiusMeters { get; set; }
    public KoreColorRGB? FillColor { get; set; }
    public KoreColorRGB? StrokeColor { get; set; }
    public double StrokeWidth { get; set; } = 1.0;
}