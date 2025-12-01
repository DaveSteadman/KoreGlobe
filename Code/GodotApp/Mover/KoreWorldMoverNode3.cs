using Godot;
using KoreCommon;
using KoreGIS;
using System;

#nullable enable

// KoreWorldMoverNode3: Simplified world position controller with arrow key navigation
// - Left/Right arrows: Longitude movement
// - Up/Down arrows: Latitude movement
// - Always looks at zero node
// - No mouse input, no complex orientation

public partial class KoreWorldMoverNode3 : Node3D
{
    // Current world position
    public KoreLLAPoint CurrLLA = new KoreLLAPoint(50, 0, 5000); // Default: 50°N, 0°E, 5km altitude

    // Movement settings
    private double MovementSpeedDegsPerSec = 0.1; // Degrees per second for lat/lon movement
    private double AltitudeSpeedMPerSec = 100.0;  // Meters per second for altitude movement

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        Name = "WorldMoverNode3";
        UpdatePosition();
    }

    public override void _Process(double delta)
    {
        HandleInput(delta);
        UpdatePosition();
        LookAtZeroNode();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Input Handling
    // --------------------------------------------------------------------------------------------

    private void HandleInput(double delta)
    {
        double deltaMovement = MovementSpeedDegsPerSec * delta;
        double deltaAltitude = AltitudeSpeedMPerSec * delta;

        // Longitude movement (Left/Right arrows)
        if (Input.IsActionPressed("ui_left"))
        {
            CurrLLA.LonDegs -= deltaMovement;
        }
        if (Input.IsActionPressed("ui_right"))
        {
            CurrLLA.LonDegs += deltaMovement;
        }

        // Latitude movement (Up/Down arrows)
        if (Input.IsActionPressed("ui_up"))
        {
            CurrLLA.LatDegs += deltaMovement;
        }
        if (Input.IsActionPressed("ui_down"))
        {
            CurrLLA.LatDegs -= deltaMovement;
        }

        // Altitude movement (Page Up/Page Down)
        if (Input.IsActionPressed("ui_page_up"))
        {
            CurrLLA.AltMslM += deltaAltitude;
        }
        if (Input.IsActionPressed("ui_page_down"))
        {
            CurrLLA.AltMslM -= deltaAltitude;
            if (CurrLLA.AltMslM < 0) CurrLLA.AltMslM = 0; // Don't go below ground
        }

        // Clamp latitude to valid range
        CurrLLA.LatDegs = Math.Max(-90.0, Math.Min(90.0, CurrLLA.LatDegs));

        // Wrap longitude to valid range
        while (CurrLLA.LonDegs > 180.0) CurrLLA.LonDegs -= 360.0;
        while (CurrLLA.LonDegs < -180.0) CurrLLA.LonDegs += 360.0;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position Updates
    // --------------------------------------------------------------------------------------------

    private void UpdatePosition()
    {
        // // Convert LLA to XYZ relative to zero offset
        // KoreXYZVector worldXYZ = CurrLLA.ToXYZ();
        // KoreXYZVector zeroXYZ = KoreZeroOffset.AppliedZeroPosXYZ;
        // KoreXYZVector relativeXYZ = worldXYZ - zeroXYZ;

        // // Update Godot position
        // Position = new Vector3((float)relativeXYZ.X, (float)relativeXYZ.Y, (float)relativeXYZ.Z);
    }

    private void LookAtZeroNode()
    {
        // Always look at the zero node (origin)
        Vector3 targetPosition = Vector3.Zero;
        Vector3 currentPosition = Position;

        if (currentPosition.DistanceTo(targetPosition) > 0.1f)
        {
            LookAt(targetPosition, Vector3.Up);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility Methods
    // --------------------------------------------------------------------------------------------

    public string GetMoverString()
    {
        return $"LLA:{CurrLLA.LatDegs:F6},{CurrLLA.LonDegs:F6},{CurrLLA.AltMslM:F1}";
    }

    public void SetFromMoverString(string moverString)
    {
        try
        {
            if (moverString.StartsWith("LLA:"))
            {
                string[] parts = moverString.Substring(4).Split(',');
                if (parts.Length >= 3)
                {
                    CurrLLA.LatDegs = double.Parse(parts[0]);
                    CurrLLA.LonDegs = double.Parse(parts[1]);
                    CurrLLA.AltMslM = double.Parse(parts[2]);
                }
            }
        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"Error parsing mover string: {ex.Message}");
        }
    }
}