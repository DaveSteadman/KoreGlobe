using Godot;
using KoreCommon;
using System.Collections.Generic;

public partial class KoreWorldMoverNode2 : Node3D
{
    public bool IsEnabled = true;

    // Speeds (real-world meters/second and radians/second)
    public float MoveSpeedMps = 25.0f;
    public float RotateSpeedRadsPerSec = 1.0f;

    // Mouse
    public float MouseSensitivityRadsPerPx = 0.002f;
    public float MouseWheelStepM = 5000.0f;
    public float MouseMoveMetersPerPx = 50f;

    private KoreNumeric1DMappedRange<double> MouseDragSpeedToAlt = new KoreNumeric1DMappedRange<double>();
    private KoreNumeric1DMappedRange<double> MouseWheelSpeedToAlt = new KoreNumeric1DMappedRange<double>();


    // State
    public KoreLLAPoint CurrLLA = new KoreLLAPoint() { LatDegs = 50, LonDegs = 0, AltMslM = 3000 };
    public KoreAzEl CurrAim = KoreAzEl.Zero; // Az: yaw (0=north, +east), El: pitch (0=horizon,+up)

    public KoreLLAPoint CurrAimLLA = new KoreLLAPoint() { LatDegs = 50, LonDegs = 0, AltMslM = 3000 };


    // Input state
    private bool _isRightMouseDown = false;
    private bool _isMiddleMouseDown = false;
    private HashSet<Key> _currentKeys = new();
    private const double PitchLimit = 1.55334; // ~89.0 deg to avoid gimbal weirdness

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        SetProcessInput(true);
        Input.MouseMode = Input.MouseModeEnum.Visible;
        UpdateTransformFromLLAAndAim();

        MouseDragSpeedToAlt.Add(0.0, 5);
        MouseDragSpeedToAlt.Add(100000, 5000);

        MouseWheelSpeedToAlt.Add(0.0, 500);
        MouseWheelSpeedToAlt.Add(1000000, 500000);

    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        if (!IsEnabled) return;

        // Keyboard
        UpdateKeyboardMovement(delta);
        UpdateKeyboardRotation(delta);

        // Apply limits
        ApplyPosLimits();

        // Keep transform in sync with CurrLLA/CurrAim
        UpdateTransformFromLLAAndAim();
        
        // Update a position on the ground where the camera is looking
        UpdateProjectedGroundPoint();
    }

    // --------------------------------------------------------------------------------------------

    public override void _Input(InputEvent @event)
    {
        if (!IsEnabled) return;

        // Key press/release tracking
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Pressed)
            {
                _currentKeys.Add(keyEvent.Keycode);
                //GD.Print($"Key pressed: {keyEvent.Keycode}");
            }
            else
            {
                _currentKeys.Remove(keyEvent.Keycode);
                //GD.Print($"Key released: {keyEvent.Keycode}");
            }
        }

        if (@event is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Right) _isRightMouseDown = mb.Pressed;
            if (mb.ButtonIndex == MouseButton.Middle) _isMiddleMouseDown = mb.Pressed;



            double currMouseWheelSpeed = MouseWheelSpeedToAlt.GetOutput(CurrLLA.AltMslM);

            if (mb.ButtonIndex == MouseButton.WheelUp && mb.Pressed)
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Forward, currMouseWheelSpeed);
            if (mb.ButtonIndex == MouseButton.WheelDown && mb.Pressed)
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Backward, currMouseWheelSpeed);
        }

        if (@event is InputEventMouseMotion mm)
        {
            // Middle-click movement tweak:
            // - Default: Y mouse moves Forward/Backward at current altitude scale
            // - With Shift pressed: Y mouse performs vertical Up/Down (previous behavior)
            if (_isMiddleMouseDown)
            {
                double metersPerPx = MouseDragSpeedToAlt.GetOutput(CurrLLA.AltMslM);
                var dx = mm.Relative.X * metersPerPx;
                var dy = mm.Relative.Y * metersPerPx;

                bool shift = _currentKeys.Contains(Key.Shift);
                bool alt = _currentKeys.Contains(Key.Alt);

                // X => strafe Left/Right
                if (System.Math.Abs(dx) > 0.0)
                    CurrLLA = KoreLLAPointOps.MoveWithAim(
                        CurrLLA, CurrAim,
                        dx > 0 ? KoreLLAPointOps.MoveDirection.Right : KoreLLAPointOps.MoveDirection.Left,
                        System.Math.Abs(dx));

                if (System.Math.Abs(dy) > 0.0)
                {
                    if (alt)
                    {
                        // Alt: All movement at same elevation (horizontal plane)
                        // Create a flattened aim with no elevation component
                        KoreAzEl flatAim = new KoreAzEl() { AzRads = CurrAim.AzRads, ElRads = 0.0 };
                        CurrLLA = KoreLLAPointOps.MoveWithAim(
                            CurrLLA, flatAim,
                            dy > 0 ? KoreLLAPointOps.MoveDirection.Forward : KoreLLAPointOps.MoveDirection.Backward,
                            System.Math.Abs(dy));
                    }
                    else if (shift)
                    {
                        // Vertical movement (legacy behavior)
                        CurrLLA = KoreLLAPointOps.MoveWithAim(
                            CurrLLA, CurrAim,
                            dy < 0 ? KoreLLAPointOps.MoveDirection.Down : KoreLLAPointOps.MoveDirection.Up,
                            System.Math.Abs(dy));
                    }
                    else
                    {
                        // Forward/Backward movement
                        CurrLLA = KoreLLAPointOps.MoveWithAim(
                            CurrLLA, CurrAim,
                            dy < 0 ? KoreLLAPointOps.MoveDirection.Forward : KoreLLAPointOps.MoveDirection.Backward,
                            System.Math.Abs(dy));
                    }
                }
            }
            else if (_isRightMouseDown && _currentKeys.Contains(Key.Shift))
            {
                // Right mouse + Shift: keep existing translate mode (strafe + vertical)
                double metersPerPx = MouseDragSpeedToAlt.GetOutput(CurrLLA.AltMslM);
                var dx = mm.Relative.X * metersPerPx;
                var dy = mm.Relative.Y * metersPerPx;

                if (System.Math.Abs(dx) > 0.0)
                    CurrLLA = KoreLLAPointOps.MoveWithAim(
                        CurrLLA, CurrAim,
                        dx > 0 ? KoreLLAPointOps.MoveDirection.Right : KoreLLAPointOps.MoveDirection.Left,
                        System.Math.Abs(dx));

                if (System.Math.Abs(dy) > 0.0)
                    CurrLLA = KoreLLAPointOps.MoveWithAim(
                        CurrLLA, CurrAim,
                        dy < 0 ? KoreLLAPointOps.MoveDirection.Down : KoreLLAPointOps.MoveDirection.Up,
                        System.Math.Abs(dy));
            }
            else if (_isRightMouseDown)
            {
                // Look: yaw/pitch
                CurrAim.AzRads += mm.Relative.X * MouseSensitivityRadsPerPx * -1.0;
                CurrAim.ElRads += mm.Relative.Y * MouseSensitivityRadsPerPx * 1.0; // invert to feel natural

                CurrAim.AzRads = WrapAngle(CurrAim.AzRads);
                CurrAim.ElRads = Mathf.Clamp((float)CurrAim.ElRads, (float)-PitchLimit, (float)PitchLimit);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Movement / Rotation from keyboard
    // --------------------------------------------------------------------------------------------
    private void UpdateKeyboardMovement(double delta)
    {
        bool shift = _currentKeys.Contains(Key.Shift);
        bool alt = _currentKeys.Contains(Key.Alt);
        if (alt) return; // alt is reserved for rotate in your scheme

        double mouseSpeedForAlt = MouseDragSpeedToAlt.GetOutput(CurrLLA.AltMslM);
        double step = mouseSpeedForAlt * delta;
        //double step = MoveSpeedMps * delta;

        if (shift)
        {
            // Shift + WASD => forward/back only (W/S)
            if (_currentKeys.Contains(Key.W))
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Forward, step);
            if (_currentKeys.Contains(Key.S))
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Backward, step);
        }
        else
        {
            // Plain WASD => up/down (W/S) and strafe (A/D)
            if (_currentKeys.Contains(Key.W))
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Up, step);
            if (_currentKeys.Contains(Key.S))
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Down, step);
            if (_currentKeys.Contains(Key.A))
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Left, step);
            if (_currentKeys.Contains(Key.D))
                CurrLLA = KoreLLAPointOps.MoveWithAim(CurrLLA, CurrAim, KoreLLAPointOps.MoveDirection.Right, step);
        }
    }

    private void UpdateKeyboardRotation(double delta)
    {
        bool alt = _currentKeys.Contains(Key.Alt);
        if (!alt) return;

        double yawRate = RotateSpeedRadsPerSec * delta;
        double pitRate = RotateSpeedRadsPerSec * delta;

        if (_currentKeys.Contains(Key.A)) CurrAim.AzRads += yawRate;
        if (_currentKeys.Contains(Key.D)) CurrAim.AzRads -= yawRate;
        if (_currentKeys.Contains(Key.W)) CurrAim.ElRads += pitRate;
        if (_currentKeys.Contains(Key.S)) CurrAim.ElRads -= pitRate;

        CurrAim.AzRads = WrapAngle(CurrAim.AzRads);
        CurrAim.ElRads = Mathf.Clamp((float)CurrAim.ElRads, (float)-PitchLimit, (float)PitchLimit);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Transform build from LLA + Aim
    // --------------------------------------------------------------------------------------------

    private void UpdateTransformFromLLAAndAim()
    {
        // GE-space frame at CurrLLA
        var posStruct = KoreGeoConvOps.RwToGeStruct(CurrLLA);
        Vector3 up = posStruct.VecUp.Normalized();        // surface normal (already in GE space)
        Vector3 north = posStruct.VecNorth.Normalized();     // +lat direction (GE space)

        // East from N×U (works with your Z-inverted GE space)
        Vector3 east = north.Cross(up).Normalized();

        // Aim
        float yaw = (float)CurrAim.AzRads;  // 0 = north, +ve turns toward east
        float pitch = (float)CurrAim.ElRads;  // 0 = horizon, +up

        // Yaw-only forward on tangent plane
        Vector3 fFlat = (north * Mathf.Cos(yaw) + east * Mathf.Sin(yaw)).Normalized();

        // Full forward (pitch relative to horizon)
        Vector3 fwd = (fFlat * Mathf.Cos(pitch) + up * Mathf.Sin(pitch)).Normalized();

        // Right derived from yaw (stable even when pitch ≈ ±90°)
        Vector3 right = (east * Mathf.Cos(yaw) - north * Mathf.Sin(yaw)).Normalized();

        // Re-orthonormalize up to kill drift (right-handed: up = fwd × right)
        Vector3 upOrtho = fwd.Cross(right).Normalized();

        // Position in GE space (already applies Z flip internally)
        Vector3 v3Pos = KoreGeoConvOps.RwToOffsetGe(CurrLLA);

        // Godot basis columns: X=right, Y=up, Z=forwardAxis
        // We want local -Z to look along world fwd ⇒ put -fwd in Z.
        // Because GE uses Z-inverted coordinates, flip Y to keep view upright.
        Basis basis = new Basis(right, -upOrtho, -fwd).Orthonormalized();

        GlobalTransform = new Transform3D(basis, v3Pos);
    }

    // --------------------------------------------------------------------------------------------

    private void UpdateProjectedGroundPoint()
    {
        double offsetRange = CurrLLA.AltMslM;
        if (offsetRange < 1.0) offsetRange = 1.0;
        if (offsetRange > 100_000) offsetRange = 100_000;
        KoreRangeBearing offset = new KoreRangeBearing() { BearingRads = CurrAim.AzRads, RangeM =  offsetRange};

        CurrAimLLA = CurrLLA.PlusRangeBearing(offset);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utilities
    // --------------------------------------------------------------------------------------------
    private static double WrapAngle(double a)
    {
        // Wrap to [-PI, PI)
        a = System.Math.IEEERemainder(a, Mathf.Tau);
        if (a < -Mathf.Pi) a += Mathf.Tau;
        if (a >= Mathf.Pi) a -= Mathf.Tau;
        return a;
    }

    // Public helpers (unchanged semantics from your original)
    public void StopMouseInteraction() { _isRightMouseDown = false; _isMiddleMouseDown = false; }
    public void ClearKeyboardState() { _currentKeys.Clear(); }
    public void ResetRotation() { CurrAim = KoreAzEl.Zero; }
    public bool IsMouseActive() { return _isRightMouseDown || _isMiddleMouseDown; }
    public bool IsMiddleMousePressed() { return _isMiddleMouseDown; }
    public string GetMouseMode()
    {
        if (!_isRightMouseDown && !_isMiddleMouseDown) return "None";
        if (_isMiddleMouseDown) return "Moving";
        bool shift = _currentKeys.Contains(Key.Shift);
        return shift ? "Moving" : "Rotating";
    }

    // --------------------------------------------------------------------------------------------

    private void ApplyPosLimits()
    {
        // Example: Clamp altitude to a realistic range
        if (CurrLLA.AltMslM < 500) CurrLLA.AltMslM = 500; // No negative altitudes
        if (CurrLLA.AltMslKm > 10000) CurrLLA.AltMslKm = 10000; // Arbitrary upper limit
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    // Turn the camera position to and from a string we can serialise

    public string GetMoverString()
    {
        return $"[{CurrLLA.LatDegs:00.0000}, {CurrLLA.LonDegs:000.0000}, {CurrLLA.AltMslM:F0}, {CurrAim.AzDegs:000.00}, {CurrAim.ElDegs:00.00}]";
    }

    public void SetMoverString(string s)
    {
        // Expecting format: [lat, lon, alt]
        s = s.Trim();
        if (!s.StartsWith("[") || !s.EndsWith("]"))
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid mover string format (missing brackets): {s}");
            return;
        }
        s = s[1..^1]; // Remove brackets

        var parts = s.Split(',');
        if (parts.Length != 5)
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid mover string format (expected 5 parts): {s}");
            return;
        }

        if (!double.TryParse(parts[0].Trim(), out double lat))
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid latitude in mover string: {parts[0]}");
            return;
        }
        if (!double.TryParse(parts[1].Trim(), out double lon))
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid longitude in mover string: {parts[1]}");
            return;
        }
        if (!double.TryParse(parts[2].Trim(), out double alt))
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid altitude in mover string: {parts[2]}");
            return;
        }
        if (!double.TryParse(parts[3].Trim(), out double az))
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid azimuth in mover string: {parts[3]}");
            return;
        }
        if (!double.TryParse(parts[4].Trim(), out double el))
        {
            GD.PrintErr($"KoreWorldMoverNode2: Invalid elevation in mover string: {parts[4]}");
            return;
        }

        CurrLLA.LatDegs = lat;
        CurrLLA.LonDegs = lon;
        CurrLLA.AltMslM = alt;
        CurrAim.AzDegs = az;
        CurrAim.ElDegs = el;

        // Apply limits
        ApplyPosLimits();

        // Update transform
        UpdateTransformFromLLAAndAim();
    }

}
