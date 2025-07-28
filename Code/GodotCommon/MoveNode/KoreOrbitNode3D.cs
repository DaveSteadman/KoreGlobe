using Godot;

using KoreCommon;
using System;

// Class to provide orbital movement functionality for a node, orbiting around a center point
// with configurable orbit radius, speed, and starting angle
// Uses KoreCentralTime.RuntimeSecs for consistent timing
public partial class KoreOrbitNode3D : Node3D
{
    // public editable values
    [Export]
    public Vector3 OrbitCenter = Vector3.Zero;
    
    [Export]
    public float OrbitDistance = 5.0f;
    
    [Export]
    public float OrbitSpeedDegsPerSec = 45.0f; // Degrees per second
    
    [Export]
    public float StartAngleDegs = 0.0f; // Starting angle in degrees
    
    [Export]
    public Vector3 OrbitAxis = Vector3.Up; // Axis of rotation (default: Y-axis for horizontal orbit)
    
    [Export]
    public bool AutoRotateToFaceCenter = false; // If true, node will face the orbit center
    
    [Export]
    public bool AutoRotateToFaceTangent = false; // If true, node will face the movement direction

    // Private calculated values
    private Vector3 _orbitPlaneU = Vector3.Zero; // First vector in orbit plane
    private Vector3 _orbitPlaneV = Vector3.Zero; // Second vector in orbit plane
    private Vector3 _initialRotation = Vector3.Zero;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Zero the transform of child nodes to ensure they start from the correct position
        foreach (Node child in GetChildren())
        {
            if (child is Node3D childNode3D)
                childNode3D.Transform = new Transform3D(Basis.Identity, Vector3.Zero);
        }        
        
        // Calculate the orbit plane vectors
        CalculateOrbitPlane();
        
        // Store the initial rotation before applying orbital rotation
        _initialRotation = Rotation;
        
        // Set the initial position
        UpdateOrbitPosition();
    }

    public override void _Process(double delta)
    {
        // This method is called every frame, so we can update the position based on the elapsed time
        UpdateOrbitPosition();
    }

    private void CalculateOrbitPlane()
    {
        // Normalize the orbit axis
        Vector3 normalizedAxis = OrbitAxis.Normalized();
        
        // Create two perpendicular vectors in the plane perpendicular to the orbit axis
        // Choose an arbitrary vector that's not parallel to the orbit axis
        Vector3 arbitrary = Vector3.Right;
        if (Mathf.Abs(normalizedAxis.Dot(Vector3.Right)) > 0.9f)
        {
            arbitrary = Vector3.Forward;
        }
        
        // Calculate the two basis vectors for the orbit plane
        _orbitPlaneU = normalizedAxis.Cross(arbitrary).Normalized();
        _orbitPlaneV = normalizedAxis.Cross(_orbitPlaneU).Normalized();
    }

    private void UpdateOrbitPosition()
    {
        // Get elapsed time since start
        double elapsedSecs = (double)KoreCentralTime.RuntimeSecs;
        
        // Calculate current angle
        double currentAngleDegs = StartAngleDegs + OrbitSpeedDegsPerSec * elapsedSecs;
        
        // Wrap angle to 0-360 degrees
        currentAngleDegs = currentAngleDegs % 360.0;
        
        // Convert to radians
        double currentAngleRads = KoreAngle.DegsToRads(currentAngleDegs);
        
        // Calculate position in orbit plane using parametric circle equation
        Vector3 orbitOffset = (float)(OrbitDistance * Math.Cos(currentAngleRads)) * _orbitPlaneU +
                             (float)(OrbitDistance * Math.Sin(currentAngleRads)) * _orbitPlaneV;
        
        // Set the new position
        Position = OrbitCenter + orbitOffset;
        
        // Handle rotation options
        if (AutoRotateToFaceCenter)
        {
            ApplyCenterFacingRotation();
        }
        else if (AutoRotateToFaceTangent)
        {
            ApplyTangentFacingRotation(currentAngleRads);
        }
    }

    private void ApplyCenterFacingRotation()
    {
        // Calculate direction from current position to orbit center
        Vector3 toCenter = (OrbitCenter - Position).Normalized();
        
        if (toCenter != Vector3.Zero)
        {
            // Create a basis that looks toward the center
            Vector3 up = OrbitAxis.Normalized();
            
            // Handle the case where toCenter is parallel to up vector
            if (Mathf.Abs(toCenter.Dot(up)) > 0.99f)
            {
                up = _orbitPlaneU;
            }
            
            Basis lookBasis = Basis.LookingAt(toCenter, up);
            
            // Apply the rotation - this should affect child nodes
            Transform3D currentTransform = Transform;
            currentTransform.Basis = lookBasis;
            Transform = currentTransform;
        }
    }

    private void ApplyTangentFacingRotation(double currentAngleRads)
    {
        // Calculate tangent direction (perpendicular to radius in orbit plane)
        Vector3 tangentDirection = (float)(-Math.Sin(currentAngleRads)) * _orbitPlaneU +
                                  (float)(Math.Cos(currentAngleRads)) * _orbitPlaneV;
        
        if (tangentDirection != Vector3.Zero)
        {
            // Create a basis that looks in the tangent direction
            Vector3 up = OrbitAxis.Normalized();
            
            Basis lookBasis = Basis.LookingAt(tangentDirection, up);
            
            // Apply the rotation - this should affect child nodes
            Transform3D currentTransform = Transform;
            currentTransform.Basis = lookBasis;
            Transform = currentTransform;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------

    // Call this to recalculate orbit if parameters change at runtime
    public void RecalculateOrbit()
    {
        CalculateOrbitPlane();
        UpdateOrbitPosition();
    }

    // Get the current orbit angle in degrees (0-360)
    public float GetCurrentOrbitAngle()
    {
        double elapsedSecs = (double)KoreCentralTime.RuntimeSecs;
        double currentAngleDegs = StartAngleDegs + OrbitSpeedDegsPerSec * elapsedSecs;
        return (float)(currentAngleDegs % 360.0);
    }

    // Get the current position on the orbit as a normalized progress (0.0 to 1.0)
    public float GetOrbitProgress()
    {
        return GetCurrentOrbitAngle() / 360.0f;
    }

    // Set the orbit to a specific angle (useful for positioning)
    public void SetOrbitAngle(float angleDegs)
    {
        // Calculate what the start angle should be to achieve the desired current angle
        double elapsedSecs = (double)KoreCentralTime.RuntimeSecs;
        StartAngleDegs = angleDegs - (float)(OrbitSpeedDegsPerSec * elapsedSecs);
        
        // Update position immediately
        UpdateOrbitPosition();
    }

    // Reverse the orbit direction
    public void ReverseOrbitDirection()
    {
        OrbitSpeedDegsPerSec = -OrbitSpeedDegsPerSec;
    }

    // Get the current distance from orbit center (should match OrbitDistance unless modified)
    public float GetCurrentDistanceFromCenter()
    {
        return Position.DistanceTo(OrbitCenter);
    }
    
    // Force update of child node transforms (call this if children aren't updating properly)
    public void ForceChildTransformUpdate()
    {
        // Force a transform update on all child nodes
        foreach (Node child in GetChildren())
        {
            if (child is Node3D childNode3D)
            {
                // Force the child to update its transform
                childNode3D.Transform = childNode3D.Transform;
            }
        }
    }
}
