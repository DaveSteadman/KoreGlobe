// KoreNodeMoverPlus: Extended version of KoreNodeMover with mouse support
// - Keyboard controls same as KoreNodeMover
// - Mouse drag to rotate (left mouse button)
// - Mouse wheel to move forward/backward

using Godot;

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
    
    private bool _isMouseDragging = false;
    private bool _isMouseRotating = false;
    private Vector2 _lastMousePosition = Vector2.Zero;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D 
    // --------------------------------------------------------------------------------------------
    public override void _Ready()
    {
        // Capture mouse input
        SetProcessInput(true);
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

        // Handle mouse button press/release
        if (@event is InputEventMouseButton mouseButton)
        {
            // Left mouse button for movement
            if (mouseButton.ButtonIndex == MouseButton.Left)
            {
                if (mouseButton.Pressed)
                {
                    _isMouseDragging = true;
                    _lastMousePosition = mouseButton.Position;
                    Input.MouseMode = Input.MouseModeEnum.Captured; // Hide and capture mouse
                }
                else
                {
                    _isMouseDragging = false;
                    Input.MouseMode = Input.MouseModeEnum.Visible; // Show mouse again
                }
            }
            
            // Right mouse button for rotation
            if (mouseButton.ButtonIndex == MouseButton.Right)
            {
                if (mouseButton.Pressed)
                {
                    _isMouseRotating = true;
                    _lastMousePosition = mouseButton.Position;
                    Input.MouseMode = Input.MouseModeEnum.Captured; // Hide and capture mouse
                }
                else
                {
                    _isMouseRotating = false;
                    Input.MouseMode = Input.MouseModeEnum.Visible; // Show mouse again
                }
            }
            
            // Handle mouse wheel for forward/backward movement
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
            // Left mouse drag for movement (like WASD)
            if (_isMouseDragging)
            {
                Vector2 mouseDelta = mouseMotion.Relative;
                
                // Convert mouse movement to world movement
                // Horizontal mouse movement = left/right (X axis)
                // Vertical mouse movement = forward/backward (Z axis)
                Vector3 mouseMovement = new Vector3(
                    -mouseDelta.X * MouseMovementSensitivity,  // Left/Right
                    mouseDelta.Y * MouseMovementSensitivity,
                    0f                                       // No Y movement from mouse
                       // Forward/Backward
                );
                
                // Convert to world space and apply
                Vector3 worldMovement = GlobalTransform.Basis * mouseMovement;
                Position += worldMovement;
            }
            
            // Right mouse drag for rotation
            if (_isMouseRotating)
            {
                Vector2 mouseDelta = mouseMotion.Relative;
                
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
        }
    }

    void UpdateKeyboardInput()
    {
        CamDirection = Vector3.Zero;
        CamRotation  = Vector3.Zero;

        bool shift = Input.IsKeyPressed(Key.Shift);
        bool alt   = Input.IsKeyPressed(Key.Alt);

        if (alt)
        {
            CamRotation.X = Input.IsKeyPressed(Key.W) ? +1f :
                            Input.IsKeyPressed(Key.S) ? -1f : 0f;

            CamRotation.Y = Input.IsKeyPressed(Key.A) ? +1f :
                            Input.IsKeyPressed(Key.D) ? -1f : 0f;
        }
        else if (shift)
        {
            CamDirection.Z = Input.IsKeyPressed(Key.W) ? -1f :
                            Input.IsKeyPressed(Key.S) ? +1f : 0f;
        }
        else
        {
            CamDirection.Y = Input.IsKeyPressed(Key.W) ? +1f :
                            Input.IsKeyPressed(Key.S) ? -1f : 0f;

            CamDirection.X = Input.IsKeyPressed(Key.A) ? -1f :
                            Input.IsKeyPressed(Key.D) ? +1f : 0f;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------
    
    // Call this to release mouse capture (useful for debugging or UI interaction)
    public void ReleaseMouse()
    {
        _isMouseDragging = false;
        _isMouseRotating = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    
    // Call this to reset rotation (useful for debugging)
    public void ResetRotation()
    {
        Rotation = Vector3.Zero;
    }
}
