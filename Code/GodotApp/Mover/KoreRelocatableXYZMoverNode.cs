// <fileheader>

// KoreRelocatableXYZMoverNode:
// - Keyboard and mouse controlled XYZ movement/rotation node for Godot.
// - Moves a position relative the Kore relocatable zero-point.
// - Does NOT use the left click, that is reserved for interactions.

using Godot;
using System.Collections.Generic;

using KoreCommon;

public partial class KoreRelocatableXYZMoverNode : Node3D
{
    public bool IsEnabled = true;

    Vector3 CamDirection = new Vector3();
    Vector3 CamRotation = new Vector3();
    Vector3 MouseMovement = new Vector3(); // Accumulated mouse movement for this frame
    public float RotateSpeedDegsPerSec = 1.0f;
    public float MoveSpeedUnitsPerSec = 5.0f;

    KoreXYZVector RwPosition = KoreXYZVector.Zero;  // Real-world position in meters
    KoreXYZVector RwOrientation = KoreXYZVector.Zero; // Real-world orientation (pitch, yaw, roll)

    // Mouse controls
    public float MouseSensitivity = 0.002f; // Radians per pixel
    public float MouseWheelSensitivity = 1.5f; // Units per wheel step
    public float MouseMovementSensitivity = 0.1f; // Units per pixel for movement

    private bool _isRightMouseDown = false;
    private bool _isMiddleMouseDown = false;
    private Vector2 _lastMousePosition = Vector2.Zero;
    private Vector2 _accumulatedMouseDelta = Vector2.Zero; // Accumulated mouse movement for this frame

    // Event-based key tracking (replaces global Input singleton polling)
    private HashSet<Key> _currentKeys = new HashSet<Key>();

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Capture mouse input
        SetProcessInput(true);

        // Ensure mouse is always visible
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _Process(double delta)
    {
        if (!IsEnabled)
            return;

        UpdateKeyboardInput();
        ProcessMouseInput();
        ProcessMovement(delta);
        ProcessRotation(delta);
        UpdateLocalPosition();
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEnabled)
            return;

        // Handle keyboard events for focus-aware input
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed)
                _currentKeys.Add(keyEvent.Keycode);
            else
                _currentKeys.Remove(keyEvent.Keycode);
        }

        // Handle mouse button press/release
        if (@event is InputEventMouseButton mouseButton)
        {
            // Right mouse button behavior - just track if it's down
            if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                if (mouseButton.Pressed)
                {
                    _lastMousePosition = mouseButton.Position;
                    _isRightMouseDown = true;
                }
                else
                {
                    // Release mouse tracking
                    _isRightMouseDown = false;
                }
            }

            // Middle mouse button (wheel click) behavior - track as movement modifier and standalone translation
            if (mouseButton.ButtonIndex == MouseButton.Middle)
            {
                if (mouseButton.Pressed)
                {
                    _lastMousePosition = mouseButton.Position;
                    _isMiddleMouseDown = true;
                }
                else
                {
                    _isMiddleMouseDown = false;
                }
            }

            // Handle mouse wheel for zoom (forward/backward movement)
            if (mouseButton.ButtonIndex == MouseButton.WheelUp)
            {
                Vector3 forwardMovement = -GlobalTransform.Basis.Z * MouseWheelSensitivity;
                MouseMovement += forwardMovement;
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                Vector3 backwardMovement = GlobalTransform.Basis.Z * MouseWheelSensitivity;
                MouseMovement += backwardMovement;
            }
        }

        // Handle mouse motion
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // Just accumulate mouse delta - processing happens in _Process
            if (_isRightMouseDown || _isMiddleMouseDown)
            {
                _accumulatedMouseDelta += mouseMotion.Relative;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Movement and Position Updates
    // --------------------------------------------------------------------------------------------

    void ProcessMovement(double delta)
    {
        // Combine keyboard and mouse movement
        Vector3 totalMovement = CamDirection + MouseMovement;

        // Convert local movement direction into world space based on current rotation
        Vector3 worldMovement = GlobalTransform.Basis * totalMovement;
        KoreXYZVector movementVector = new KoreXYZVector(worldMovement.X, worldMovement.Y, worldMovement.Z);

        // Apply movement to real-world position
        RwPosition += movementVector * MoveSpeedUnitsPerSec * delta;

        // Clear mouse movement for next frame
        MouseMovement = Vector3.Zero;
    }

    void ProcessMouseInput()
    {
        // Process accumulated mouse input from _Input
        if ((_isRightMouseDown || _isMiddleMouseDown) && _accumulatedMouseDelta.LengthSquared() > 0)
        {
            // Determine the mode based on which buttons are pressed
            bool isRightMouseOnly = _isRightMouseDown && !_isMiddleMouseDown;
            bool isMiddleMouseInvolved = _isMiddleMouseDown;

            if (isRightMouseOnly)
            {
                // Right mouse only - check shift key to determine mode
                bool shiftPressed = _currentKeys.Contains(Key.Shift);

                if (shiftPressed)
                {
                    // Shift + Right mouse drag for movement
                    ApplyMouseMovement(_accumulatedMouseDelta);
                }
                else
                {
                    // Right mouse drag for rotation
                    ApplyMouseRotation(_accumulatedMouseDelta);
                }
            }
            else if (isMiddleMouseInvolved)
            {
                // Middle mouse (alone or with right mouse) always means movement
                ApplyMouseMovement(_accumulatedMouseDelta);
            }
        }

        // Clear accumulated mouse delta for next frame
        _accumulatedMouseDelta = Vector2.Zero;
    }

    void ProcessRotation(double delta)
    {
        // Apply rotation to real-world orientation
        RwOrientation += new KoreXYZVector(CamRotation.X, CamRotation.Y, CamRotation.Z) * RotateSpeedDegsPerSec * delta;

        // Apply to Godot rotation
        Rotation += CamRotation * (float)delta * RotateSpeedDegsPerSec;

        // Clamp pitch to avoid flipping
        Rotation = new Vector3(
            Mathf.Clamp(Rotation.X, -Mathf.Pi / 2 + 0.1f, Mathf.Pi / 2 - 0.1f),
            Rotation.Y,
            Rotation.Z
        );
    }

    void UpdateKeyboardInput()
    {
        CamDirection = Vector3.Zero;
        CamRotation = Vector3.Zero;

        // Use event-based key tracking instead of global Input singleton
        bool shift = _currentKeys.Contains(Key.Shift);
        bool alt   = _currentKeys.Contains(Key.Alt);

        if (alt)
        {
            // Alt + WASD for rotation
            CamRotation.X = _currentKeys.Contains(Key.W) ? +1f :
                            _currentKeys.Contains(Key.S) ? -1f : 0f;

            CamRotation.Y = _currentKeys.Contains(Key.A) ? +1f :
                            _currentKeys.Contains(Key.D) ? -1f : 0f;
        }
        else if (shift)
        {
            // Shift + WASD for forward/backward movement
            CamDirection.Z = _currentKeys.Contains(Key.W) ? -1f :
                             _currentKeys.Contains(Key.S) ? +1f : 0f;
        }
        else
        {
            // Regular WASD for up/down and left/right movement
            CamDirection.Y = _currentKeys.Contains(Key.W) ? +1f :
                             _currentKeys.Contains(Key.S) ? -1f : 0f;

            CamDirection.X = _currentKeys.Contains(Key.A) ? -1f :
                             _currentKeys.Contains(Key.D) ? +1f : 0f;
        }

        // Debug output when movement is detected
        if (CamDirection.LengthSquared() > 0)
        {
            GD.Print($"CamDirection: {CamDirection} // Keys: {string.Join(",", _currentKeys)}");
        }
    }

    void UpdateLocalPosition()
    {
        // Calculate offset from zero node using relocatable system
        Vector3 offset = KoreRelocateOps.RWtoGE(RwPosition);

        // Debug the relocatable system
        //var zeroPos = KoreZeroOffset.AppliedZeroPosXYZ;
        // GD.Print($"RwPos: {RwPosition.X:F2},{RwPosition.Y:F2},{RwPosition.Z:F2}");
        // GD.Print($"ZeroPos: {zeroPos.X:F2},{zeroPos.Y:F2},{zeroPos.Z:F2}");
        // GD.Print($"Offset: {offset.X:F2},{offset.Y:F2},{offset.Z:F2}");
        // GD.Print("---");

        // TEMPORARY: For testing, apply RwPosition directly (scaled down for reasonable movement)
        // Remove this once relocatable system is working
        // Position = new Vector3((float)RwPosition.X, (float)RwPosition.Y, (float)RwPosition.Z);
        Position = offset;

        // Apply offset as local position (comment out for testing)
        // Position = offset;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mouse Helper Methods
    // --------------------------------------------------------------------------------------------

    private void ApplyMouseMovement(Vector2 mouseDelta)
    {
        // Convert mouse movement to world movement
        // Horizontal mouse movement = left/right (X axis)
        // Vertical mouse movement = up/down (Y axis)
        Vector3 mouseMovementDelta = new Vector3(
            -mouseDelta.X * MouseMovementSensitivity,  // Left/Right
            mouseDelta.Y * MouseMovementSensitivity,   // Up/Down
            0f                                         // No Z movement from mouse
        );

        // Accumulate mouse movement for ProcessMovement to handle
        MouseMovement += mouseMovementDelta;
    }

    private void ApplyMouseRotation(Vector2 mouseDelta)
    {
        // Apply mouse rotation
        // Horizontal mouse movement rotates around Y axis (yaw)
        // Vertical mouse movement rotates around X axis (pitch)
        Vector3 mouseRotation = new Vector3(
            mouseDelta.Y * MouseSensitivity, // Pitch (up/down)
            mouseDelta.X * MouseSensitivity, // Yaw (left/right)
            0f
        );

        Rotation += mouseRotation;

        // Clamp pitch to avoid flipping
        Rotation = new Vector3(
            Mathf.Clamp(Rotation.X, -Mathf.Pi / 2 + 0.1f, Mathf.Pi / 2 - 0.1f),
            Rotation.Y,
            Rotation.Z
        );
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------

    // Stop all mouse interactions (useful for debugging or UI interaction)
    public void StopMouseInteraction()
    {
        _isRightMouseDown = false;
        _isMiddleMouseDown = false;
    }

    // Clear all keyboard state (useful when losing focus)
    public void ClearKeyboardState()
    {
        _currentKeys.Clear();
        CamDirection = Vector3.Zero;
        CamRotation = Vector3.Zero;
    }

    // Call this to reset rotation (useful for debugging)
    public void ResetRotation()
    {
        Rotation = Vector3.Zero;
    }

    // Check if currently performing mouse operations (useful for other systems)
    public bool IsMouseActive()
    {
        return _isRightMouseDown || _isMiddleMouseDown;
    }

    // Check if middle mouse button is currently pressed
    public bool IsMiddleMousePressed()
    {
        return _isMiddleMouseDown;
    }

    // Get current mouse operation mode
    public string GetMouseMode()
    {
        if (!_isRightMouseDown && !_isMiddleMouseDown) return "None";

        if (_isMiddleMouseDown)
        {
            return "Moving"; // Middle mouse always means movement
        }

        if (_isRightMouseDown)
        {
            bool shiftPressed = _currentKeys.Contains(Key.Shift);
            return shiftPressed ? "Moving" : "Rotating";
        }

        return "None";
    }
}
