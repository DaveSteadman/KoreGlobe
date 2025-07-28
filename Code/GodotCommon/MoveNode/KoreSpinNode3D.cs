

using Godot;

using KoreCommon;
using System;

// Class to provide random spinning functionality for a node, using publicly exposed (editor-editable) starting angles and rates
// and then using KoreCentralTime.RuntimeSecs
public partial class KoreSpinNode3D : Node3D
{
    // public editable values
    [Export]
    public float StartAngleXDegs = 0.0f;
    [Export]
    public float StartAngleYDegs = 0.0f;
    [Export]
    public float StartAngleZDegs = 0.0f;

    [Export]
    public float SpinRateXDegsPerSec = 0.0f;
    [Export]
    public float SpinRateYDegsPerSec = 0.0f;
    [Export]
    public float SpinRateZDegsPerSec = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Set the initial rotation based on the starting angles
        RotationDegrees = new Vector3(StartAngleXDegs, StartAngleYDegs, StartAngleZDegs);
    }

    public override void _Process(double delta)
    {
        // This method is called every frame, so we can update the rotation based on the elapsed time
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // determine the new angle, from the start, plus the spin rate times the elapsed time
        double elapsedSecs = (double)KoreCentralTime.RuntimeSecs;
        double newAngleX = StartAngleXDegs + SpinRateXDegsPerSec * elapsedSecs;
        double newAngleY = StartAngleYDegs + SpinRateYDegsPerSec * elapsedSecs;
        double newAngleZ = StartAngleZDegs + SpinRateZDegsPerSec * elapsedSecs;

        // Wrap the angles back to 0-360 degrees
        newAngleX = newAngleX % 360.0;
        newAngleY = newAngleY % 360.0;
        newAngleZ = newAngleZ % 360.0;

        // Set the new rotation
        //RotationDegrees = new Vector3(newAngleX, newAngleY, newAngleZ);

        // Apply the rotation
        // This will be called every frame, so the node will keep spinning
        Rotation = new Vector3(
            (float)KoreAngle.DegsToRads(newAngleX),
            (float)KoreAngle.DegsToRads(newAngleY),
            (float)KoreAngle.DegsToRads(newAngleZ)
        );


    }
}
    
