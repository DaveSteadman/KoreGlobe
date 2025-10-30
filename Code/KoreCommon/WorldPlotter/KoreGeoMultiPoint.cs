// <fileheader>

#nullable enable

using System.Collections.Generic;

namespace KoreCommon;

/// <summary>
/// A collection of geographic points
/// </summary>
public class KoreGeoMultiPoint : KoreGeoFeature
{
    public List<KoreLLPoint> Points { get; set; } = new List<KoreLLPoint>();
    public double Size { get; set; } = 5.0;
    public KoreColorRGB Color { get; set; } = KoreColorRGB.Black;
    public KoreLLBox? BoundingBox { get; private set; }

    public void CalcBoundingBox()
    {
        BoundingBox = Points.Count > 0 ? KoreLLBox.FromList(Points) : null;
    }
}