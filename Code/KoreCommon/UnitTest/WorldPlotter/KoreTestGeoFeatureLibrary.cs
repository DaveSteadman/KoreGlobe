// <fileheader>

namespace KoreCommon.UnitTest;

/// <summary>
/// Unit tests for the KoreGeoFeatureLibrary - GeoJSON functionality
/// Tests are split into partial class files by geometry type:
/// - Point.cs: Point geometry tests
/// - Line.cs: LineString geometry tests
/// - Polygon.cs: Polygon geometry tests
/// </summary>
public static partial class KoreTestGeoFeatureLibrary
{
    public static void RunTests(KoreTestLog testLog)
    {
        TestSaveSinglePointToGeoJSON(testLog);
        TestSaveLineStringToGeoJSON(testLog);
        TestSavePolygonToGeoJSON(testLog);
        TestMultiSegmentRouteAcrossUK(testLog);
        TestFlexibleJoinRouteAcrossUK(testLog);
        TestRouteGeoJSONKoreIO(testLog);
    }
}

