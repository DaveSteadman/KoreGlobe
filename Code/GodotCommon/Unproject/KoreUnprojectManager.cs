

// KoreUnprojectManager: Holds values to support unprojecting values

using System.Collections.Generic;
using Godot;

using KoreCommon;

#nullable enable

// KoreUnprojectManager: Static class holding application context and a point-of-contact for unproject operations.

public static class KoreUnprojectManager
{
    private static KoreLatestHolderClass<Camera3D> latestCamera = new KoreLatestHolderClass<Camera3D>();
    private static KoreLatestHolderClass<Viewport> latestViewport = new KoreLatestHolderClass<Viewport>();

    // --------------------------------------------------------------------------------------------

    // Called iteratively to keep the context of an unproject up to date

    public static void UpdateState(Camera3D updateCamera, Viewport updateViewport)
    {
        // Update the latest camera and viewport
        latestCamera.LatestValue = updateCamera;
        latestViewport.LatestValue = updateViewport;
    }

    // --------------------------------------------------------------------------------------------

    // Performs the called unproject operation using the latest camera and viewport
    // Usage: var (success, screenPos) = KoreUnprojectManager.UnprojectPoint(gepos);
    public static (bool, Vector2) UnprojectPoint(Vector3 gePoint)
    {
        if (latestCamera.HasValue && latestViewport.HasValue)
        {
            Camera3D camera = latestCamera.LatestValue!;
            Viewport viewport = latestViewport.LatestValue!;

            // Unproject the point from 3D to 2D
            var (screenPos, success) = KoreUnprojectOps.UnprojectPoint(gePoint, camera, viewport);
            return (success, screenPos);
        }

        // Return default false
        return (false, Vector2.Zero);
    }

    // Usage: var (success, screenRect) = KoreUnprojectManager.UnprojectPointList(gePosList);
    public static (bool, Rect2) UnprojectPointList(List<Vector3> gePointList)
    {
        if (latestCamera.HasValue && latestViewport.HasValue)
        {
            Camera3D camera = latestCamera.LatestValue!;
            Viewport viewport = latestViewport.LatestValue!;

            // Unproject the list of points
            var (boundingBox, success) = KoreUnprojectOps.UnprojectShapeBounds2(gePointList, camera, viewport);
            return (success, boundingBox);
        }

        // Return default false
        return (false, new Rect2());
    }


}