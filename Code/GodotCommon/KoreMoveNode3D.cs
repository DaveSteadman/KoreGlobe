using Godot;

using KoreCommon;
using System;

// Class to provide linear movement functionality for a node, moving from start to end points
// with automatic rotation to face the movement direction
// Uses KoreCentralTime.RuntimeSecs for consistent timing
public partial class KoreMoveNode3D : Node3D
{
    // public editable values
    [Export]
    public Vector3 StartPoint = Vector3.Zero;
    [Export]
    public Vector3 EndPoint = new Vector3(0, 0, 10);

    [Export]
    public float MoveRateUnitsPerSec = 1.0f;

    [Export]
    public bool AutoRotateToFaceDirection = true;

    [Export]
    public bool LoopMovement = false; // If true, will loop back to start when reaching end

    // Private calculated values
    private Vector3 _moveDirection = Vector3.Zero;
    private float _totalDistance = 0.0f;
    private float _totalMoveTime = 0.0f;
    private Vector3 _startRotation = Vector3.Zero;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Calculate movement parameters
        CalculateMovementParameters();
        
        // Set the initial position
        Position = StartPoint;
        
        // Store the initial rotation before applying direction rotation
        _startRotation = Rotation;
        
        // Set the rotation to face the end point if enabled
        if (AutoRotateToFaceDirection && _moveDirection != Vector3.Zero)
        {
            ApplyDirectionRotation();
        }
    }

    public override void _Process(double delta)
    {
        // This method is called every frame, so we can update the position based on the elapsed time
        UpdatePosition();
    }

    private void CalculateMovementParameters()
    {
        // Calculate the movement direction and distance
        _moveDirection = (EndPoint - StartPoint).Normalized();
        _totalDistance = StartPoint.DistanceTo(EndPoint);
        
        // Calculate how long the movement should take
        if (MoveRateUnitsPerSec > 0)
        {
            _totalMoveTime = _totalDistance / MoveRateUnitsPerSec;
        }
        else
        {
            _totalMoveTime = 0; // Instant movement if rate is 0 or negative
        }
    }

    private void ApplyDirectionRotation()
    {
        // Create a transform that looks from start to end point
        Vector3 up = Vector3.Up;
        
        // Handle the case where direction is straight up or down
        if (Mathf.Abs(_moveDirection.Dot(Vector3.Up)) > 0.99f)
        {
            up = Vector3.Forward;
        }
        
        // Create a basis that faces the movement direction
        Basis lookBasis = Basis.LookingAt(_moveDirection, up);
        
        // Apply the rotation
        Rotation = lookBasis.GetEuler();
    }

    private void UpdatePosition()
    {
        if (_totalMoveTime <= 0)
        {
            // Instant movement
            Position = EndPoint;
            return;
        }

        // Get elapsed time since start
        double elapsedSecs = (double)KoreCentralTime.RuntimeSecs;
        
        // Calculate progress (0.0 to 1.0)
        float progress = (float)(elapsedSecs / _totalMoveTime);
        
        // Handle looping
        if (LoopMovement)
        {
            progress = progress % 1.0f;
        }
        else
        {
            progress = Mathf.Clamp(progress, 0.0f, 1.0f);
        }
        
        // Calculate current position using linear interpolation
        Vector3 currentPosition = StartPoint.Lerp(EndPoint, progress);
        
        // Apply the position
        Position = currentPosition;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------

    // Call this to recalculate movement if start/end points change at runtime
    public void RecalculateMovement()
    {
        CalculateMovementParameters();
        
        if (AutoRotateToFaceDirection && _moveDirection != Vector3.Zero)
        {
            ApplyDirectionRotation();
        }
    }

    // Get the current movement progress (0.0 to 1.0)
    public float GetMovementProgress()
    {
        if (_totalMoveTime <= 0)
            return 1.0f;
            
        double elapsedSecs = (double)KoreCentralTime.RuntimeSecs;
        float progress = (float)(elapsedSecs / _totalMoveTime);
        
        if (LoopMovement)
        {
            return progress % 1.0f;
        }
        else
        {
            return Mathf.Clamp(progress, 0.0f, 1.0f);
        }
    }

    // Check if the movement has completed (only relevant when LoopMovement is false)
    public bool IsMovementComplete()
    {
        if (LoopMovement)
            return false;
            
        return GetMovementProgress() >= 1.0f;
    }

    // Reset the movement to start from the beginning
    public void ResetMovement()
    {
        // Reset the timer by adjusting the central time (this is a bit of a hack)
        // In a real implementation, you might want to store a movement start time
        Position = StartPoint;
        
        if (AutoRotateToFaceDirection && _moveDirection != Vector3.Zero)
        {
            ApplyDirectionRotation();
        }
    }
}
