// KoreNodeMoverPlus: Enhanced version with improved mouse behavior
// - Keyboard controls same as KoreNodeMover
// - Right mouse button drag: rotation (default) or movement (with shift or middle mouse)
// - Shift key or middle mouse button can be pressed/released mid-drag to switch modes dynamically
// - Mouse wheel for zoom (forward/backward)
// - Mouse cursor always visible and position unaffected

using Godot;
using System.Collections.Generic;

public partial class KoreNodeMoverPlus : Node3D
{
    public bool IsEnabled = true;
    
    Vector3 CamDirection = new Vector3();
    Vector3 CamRotation  = new Vector3();
    public float RotateSpeedDegsPerSec = 1.0f;
    public float MoveSpeedUnitsPerSec = 3.0f;
    
    // Mouse controls
    public float MouseSensitivity = 0.002f; // Radians per pixel
    public float MouseWheelSensitivity = 0.5f; // Units per wheel step
    public float MouseMovementSensitivity = 0.01f; // Units per pixel for movement
    
    private bool _isRightMouseDown = false;
    private bool _isMiddleMouseDown = false;
    private Vector2 _lastMousePosition = Vector2.Zero;

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
            
        // Convert local movement direction into world space based on current rotation
        Vector3 worldMovement = GlobalTransform.Basis * CamDirection;

        Position += worldMovement * (float)delta * MoveSpeedUnitsPerSec;
        Rotation += CamRotation   * (float)delta * RotateSpeedDegsPerSec;
    }

    public override void _Input(InputEvent @event)
    {
        if (!IsEnabled)
            return;

        // Handle keyboard events for focus-aware input
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed)
            {
                _currentKeys.Add(keyEvent.Keycode);
            }
            else
            {
                _currentKeys.Remove(keyEvent.Keycode);
            }
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
                Position += forwardMovement;
            }
            else if (mouseButton.ButtonIndex == MouseButton.WheelDown)
            {
                Vector3 backwardMovement = GlobalTransform.Basis.Z * MouseWheelSensitivity;
                Position += backwardMovement;
            }
        }
        
        // Handle mouse motion
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // Process mouse motion for either right mouse button or middle mouse button
            if (_isRightMouseDown || _isMiddleMouseDown)
            {
                Vector2 mouseDelta = mouseMotion.Relative;
                
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
                        ApplyMouseMovement(mouseDelta);
                    }
                    else
                    {
                        // Right mouse drag for rotation
                        ApplyMouseRotation(mouseDelta);
                    }
                }
                else if (isMiddleMouseInvolved)
                {
                    // Middle mouse (alone or with right mouse) always means movement
                    ApplyMouseMovement(mouseDelta);
                }
            }
        }
    }

    void UpdateKeyboardInput()
    {
        CamDirection = Vector3.Zero;
        CamRotation  = Vector3.Zero;

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
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mouse Helper Methods
    // --------------------------------------------------------------------------------------------
    
    private void ApplyMouseMovement(Vector2 mouseDelta)
    {
        // Convert mouse movement to world movement
        // Horizontal mouse movement = left/right (X axis)
        // Vertical mouse movement = up/down (Y axis)
        Vector3 mouseMovement = new Vector3(
            -mouseDelta.X * MouseMovementSensitivity,  // Left/Right
            mouseDelta.Y * MouseMovementSensitivity,   // Up/Down
            0f                                         // No Z movement from mouse
        );
        
        // Convert to world space and apply
        Vector3 worldMovement = GlobalTransform.Basis * mouseMovement;
        Position += worldMovement;
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
            Mathf.Clamp(Rotation.X, -Mathf.Pi/2 + 0.1f, Mathf.Pi/2 - 0.1f),
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
