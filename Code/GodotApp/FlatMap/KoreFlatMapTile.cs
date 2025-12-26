


using Godot;

using KoreGIS;
using KoreCommon;

#nullable enable


public partial class KoreFlatMapTile : Node3D
{
    public KoreMapTileCode TileCode;
    public KoreMeshData? TileMesh = null;

    public KoreFlatMapTileMarkers Markers = new KoreFlatMapTileMarkers();

    KoreLLAPoint CenterLLA = new KoreLLAPoint();
    KoreXYZVector CenterXYZ = new KoreXYZVector();

    public KoreFlatMapTile(KoreMapTileCode tileCode)
    {
        TileCode = tileCode;
        Name = $"FlatMapTile_{TileCode.ToString()}";

        CenterLLA = new KoreLLAPoint()
        {
            LatDegs = TileCode.LLBox.CenterPoint.LatDegs,
            LonDegs = TileCode.LLBox.CenterPoint.LonDegs,
            RadiusM = KoreFlatMapManager.GeRadius
        };
        CenterXYZ = CenterLLA.ToXYZ();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        KoreLLAPoint[,] tps = TilePoints(20);
        TileMesh = TileFromPoints(tps);


        var col = KoreColorPalette.Find("LightGray");
        col = KoreColorOps.ColorWithRGBNoise(col, 0.5f);
        var mat = new KoreMeshMaterial("TileMat", col, 0.0f, 0.7f);
        TileMesh.AddGroupWithMaterial("AllTris", mat);
        TileMesh.AddAllTrianglesToGroup("AllTris");

        KoreGodotLineMesh lineMeshNode = new KoreGodotLineMesh() { Name = $"Wireframe" };
        lineMeshNode.UpdateMesh(TileMesh);
        AddChild(lineMeshNode);

        KoreGodotSurfaceMesh surfaceMeshNode = new KoreGodotSurfaceMesh() { Name = $"Surface" };
        surfaceMeshNode.UpdateMesh(TileMesh, "AllTris");
        AddChild(surfaceMeshNode);

        // create a debug marker at the center of the tile
        Markers.CreateMarkers();
        Markers.AddToNode(this);
    }

    public override void _Process(double delta)
    {
        // Update the tile's position relative to the Zero Node
        // Note that the tile is a child of the manager, but that maintains a (0,0,0) offset from the zero node
        if (KoreMovingOrigin.IsChangePeriod())
            PlaceNode();
            Markers.PlaceMarkers(TileCode.LLBox);
    }

    public override void _ExitTree()
    {
    }

    // --------------------------------------------------------------------------------------------

    // Get a 2D array of LLA points for the tile
    // - pointsPerSide: Number of points along each side of the tile

    public KoreLLAPoint[,] TilePoints(int pointsPerSide)
    {
        // Get the LL box for the tile
        KoreLLBox boundsbox = TileCode.LLBox;

        // Create the output array of points
        // Y, X / top to bottom lat, left to right lon, so 0,0 is top-left
        KoreLLAPoint[,] points = new KoreLLAPoint[pointsPerSide, pointsPerSide];

        // Get the corner values and step sizes
        double latMinDegs = boundsbox.MinLatDegs; // Southern edge
        double latMaxDegs = boundsbox.MaxLatDegs; // Northern edge
        double lonMinDegs = boundsbox.MinLonDegs; // Western edge
        double lonMaxDegs = boundsbox.MaxLonDegs; // Eastern edge

        double latStepDegs = (latMaxDegs - latMinDegs) / (pointsPerSide - 1);
        double lonStepDegs = (lonMaxDegs - lonMinDegs) / (pointsPerSide - 1);

        // Create 1D arrays of latitudes and longitudes
        // Godot has 0,0 at top-left, so latitudes go from max to min, longitudes from min(west) to max(east)
        KoreNumeric1DArray<double> latVals = KoreNumeric1DArrayOps<double>.ListForRange(latMaxDegs, latMinDegs, pointsPerSide);
        KoreNumeric1DArray<double> lonVals = KoreNumeric1DArrayOps<double>.ListForRange(lonMinDegs, lonMaxDegs, pointsPerSide);

        // Iterate over the points, getting the LLA for each point
        for (int y = 0; y < pointsPerSide; y++)
        {
            for (int x = 0; x < pointsPerSide; x++)
            {
                points[y, x] = new KoreLLAPoint() { LatDegs = latVals[y], LonDegs = lonVals[x], RadiusM = KoreFlatMapManager.GeRadius };
            }
        }

        return points;
    }

    public KoreMeshData TileFromPoints(KoreLLAPoint[,] points)
    {
        // Convert LLA points to XYZ vertices
        int width = points.GetLength(1);
        int height = points.GetLength(0);
        KoreXYZVector[,] vertices = new KoreXYZVector[width, height];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Curr pos
                KoreLLAPoint currPosLLA = points[y, x];
                KoreXYZVector currPosXYZ = currPosLLA.ToXYZ();

                // Get the offset from center
                KoreXYZVector currOffsetXYZ = CenterXYZ.XYZTo(currPosXYZ);

                // Convert to GE Units and put into array
                //KoreXYZVector currOffsetGeXYZ = currOffsetXYZ.Scale(KoreMovingOrigin.RwToGeScaleMultiplier);
                vertices[x, y] = currOffsetXYZ;
            }
        }

        KoreMeshData surfaceMesh = KoreMeshDataPrimitives.Surface(vertices, KoreUVBox.Full);

        // Create and return the surface mesh
        return surfaceMesh;
    }

    // --------------------------------------------------------------------------------------------

    public void PlaceNode()
    {
        //if (TileNode == null) return;

        // var gePos  = KoreMovingOrigin.RWtoRWOffset(CenterXYZ);
        // var geGPos = KoreMovingOrigin.XYZtoGodot(gePos);



        KoreLLAPoint tlLLA = CenterLLA;
        KoreXYZVector tlXYZ = tlLLA.ToXYZ();
        Position = KoreMovingOrigin.RWtoGodotOffset(tlXYZ);



        // var nodePos = KoreMovingOrigin.RWtoGodotOffset(CenterXYZ);
        string nodePosStr = KoreConvPos.Vector3Str(Position);
        // Position = nodePos;

        //GD.Print($"KoreFlatMapTile.PlaceNode: Tile {TileCode} placed at {KoreConvPos.Vector3Str(TileNode.Position)}");
        GD.Print($"CenterXYZ: {KoreXYZVectorIO.ToStringWithDP(CenterXYZ, 2)}");
        GD.Print($"NodePos: {nodePosStr}");

        // Convert to GE offset
        // Vector3 geOffset = KoreMovingOriginOps.RwLLAToGeOffset(CenterLLA);

        // Place the node
        // TileNode.Position = geGPos;

        // // Place the marker node if it exists
        // if (MarkerCenterNode != null)
        // {
        //     MarkerCenterNode.Position = Vector3.Zero;
        // }
    }

}



