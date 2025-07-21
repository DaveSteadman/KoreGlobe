using System;
using System.Collections.Generic;


using Godot;

public static class KoreUnprojectOps
{
    public static bool IsPointInFrontOfCameraPlane(Vector3 gePosition, Camera3D camera)
    {
        // Calculate vector from camera position to the point
        Vector3 cameraToPoint = gePosition - camera.GlobalTransform.Origin;

        // Dot product with the negative Z basis to determine if the point is in front
        float dotProduct = cameraToPoint.Dot(-camera.GlobalTransform.Basis.Z);

        // If the dot product is positive, the point is in front of the camera
        return dotProduct > 0;
    }

    // --------------------------------------------------------------------------------------------

    public static (Vector2 position, bool success) UnprojectPoint(Vector3 worldPosition, Camera3D camera, Viewport viewport)
    {
        // Convert to camera-local space to determine visibility
        Vector3 cameraSpace = camera.GlobalTransform.AffineInverse() * worldPosition;
        if (cameraSpace.Z > 0)
            return (Vector2.Zero, false); // Behind camera

        Vector2 screenPosition = camera.UnprojectPosition(worldPosition);

        // Optional: Check screen bounds with margin
        Vector2 screenSize = viewport.GetVisibleRect().Size;
        float bufferFactor = 2f;
        bool success =
            screenPosition.X >= -screenSize.X * bufferFactor &&
            screenPosition.X <= screenSize.X * (1 + bufferFactor) &&
            screenPosition.Y >= -screenSize.Y * bufferFactor &&
            screenPosition.Y <= screenSize.Y * (1 + bufferFactor);

        return (screenPosition, success);
    }

    // --------------------------------------------------------------------------------------------

    public static (Rect2 boundingBox, bool success) UnprojectShapeBounds2(
        List<Vector3> gePointsList,
        Camera3D camera,
        Viewport viewport)
    {
        // Quick validation and early return if wrong inputs
        if (gePointsList == null || gePointsList.Count < 2)
            return (new Rect2(), false);
        
        List<Vector2> successfulUnprojectedPoints = new List<Vector2>();
        foreach (var point in gePointsList)
        {
            var (screenPos, success) = UnprojectPoint(point, camera, viewport);
            if (success)
                successfulUnprojectedPoints.Add(screenPos);
        }

        // If we have enough points to make a box
        if (successfulUnprojectedPoints.Count >= 2)
        {
            
            // print out the first point for debugging
            //GD.Print($"First unprojected point: {successfulUnprojectedPoints[0]}");
            
            float minX = successfulUnprojectedPoints[0].X, maxX = successfulUnprojectedPoints[0].X;
            float minY = successfulUnprojectedPoints[0].Y, maxY = successfulUnprojectedPoints[0].Y;

            // Skip index 0 since we used it to initialize min/max values
            for (int i = 1; i < successfulUnprojectedPoints.Count; i++)
            {
                var screenPos = successfulUnprojectedPoints[i];
                if (screenPos.X > maxX) maxX = screenPos.X;
                else if (screenPos.X < minX) minX = screenPos.X;
                if (screenPos.Y > maxY) maxY = screenPos.Y;
                else if (screenPos.Y < minY) minY = screenPos.Y;
            }

            return (new Rect2(minX, minY, maxX - minX, maxY - minY), true);
        }

        // Return default false
        return (new Rect2(), false);
    }


}

