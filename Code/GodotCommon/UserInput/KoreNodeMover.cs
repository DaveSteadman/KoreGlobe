
// KoreNodeMover: A class to move the node around based on keyboard input

using Godot;

public partial class KoreNodeMover : Node3D
{
    public bool IsEnabled = true;
    
    Vector3 CamDirection = new Vector3();
    Vector3 CamRotation  = new Vector3();
    public float RotateSpeedDegsPerSec = 1.0f;
    public float MoveSpeedUnitsPerSec = 1.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D 
    // --------------------------------------------------------------------------------------------
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        if (!IsEnabled)
            return;

        UpdateInput();
            
        // Convert local movement direction into world space based on current rotation
        Vector3 worldMovement = GlobalTransform.Basis * CamDirection;

        Position += worldMovement * (float)delta * MoveSpeedUnitsPerSec;
        Rotation += CamRotation   * (float)delta * RotateSpeedDegsPerSec;
    }

    void UpdateInput()
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
            CamDirection.Y = Input.IsKeyPressed(Key.W) ? +1f :
                            Input.IsKeyPressed(Key.S) ? -1f : 0f;
        }
        else
        {
            CamDirection.Z = Input.IsKeyPressed(Key.W) ? -1f :
                            Input.IsKeyPressed(Key.S) ? +1f : 0f;

            CamDirection.X = Input.IsKeyPressed(Key.A) ? -1f :
                            Input.IsKeyPressed(Key.D) ? +1f : 0f;
        }
    }

}
