// <fileheader>

#nullable enable

using System.Collections.Generic;

namespace KoreCommon;

/// <summary>
/// A collection of geographic features
/// </summary>
public class KoreGeoFeatureCollection
{
    public List<KoreGeoFeature> Features { get; set; } = new List<KoreGeoFeature>();
    public KoreLLBox? BoundingBox { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}