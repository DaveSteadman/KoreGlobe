// KoreMoveBezierNode3D: Move a Node3D along a Bézier curve path with proper orientation
// - Moves at user-defined speed along the curve
// - Orients forward along the tangent direction
// - Maintains specified up vector for roll control

using Godot;
using System.Collections.Generic;
using KoreCommon;

public partial class KoreMoveBezierNode3D : Node3D
{
    // Path control points
    private List<KoreXYZVector> _controlPoints = new List<KoreXYZVector>();

    // Movement parameters
    [Export] public float Speed = 1.0f; // Units per second along the curve
    [Export] public bool IsMoving = true;
    [Export] public bool LoopPath = false;
    [Export] public Vector3 UpVector = Vector3.Up; // Overall up direction for orientation

    // Path calculation parameters
    [Export] public int PathDivisions = 100; // Resolution of the path

    // Internal state
    private List<KoreXYZVector> _pathPoints = new List<KoreXYZVector>();
    private List<float> _pathDistances = new List<float>(); // Cumulative distances along path
    private float _totalPathLength = 0.0f;
    private float _currentDistance = 0.0f; // Current position along path in world units
    private int _lastSegmentIndex = 0; // Optimization for path lookup

    // --------------------------------------------------------------------------------------------
    // MARK: Godot Lifecycle
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialize with default control points if none set
        if (_controlPoints.Count == 0)
        {
            SetDefaultPath();
        }

        // Add the points
        _pathPoints.Add(new KoreXYZVector(3, 1,3));
        _pathPoints.Add(new KoreXYZVector(5, 2, 3));
        _pathPoints.Add(new KoreXYZVector(7, 1, 3));

        // Draw the path for debugging
        DrawDebugPath();

        UpdatePathCalculations();
    }

    public override void _Process(double delta)
    {
        if (!IsMoving || _pathPoints.Count < 2) return;

        // Update position along path
        float deltaTime = (float)delta;
        _currentDistance += Speed * deltaTime;

        // Handle looping or clamping
        if (LoopPath)
        {
            if (_currentDistance >= _totalPathLength)
                _currentDistance -= _totalPathLength;
            else if (_currentDistance < 0)
                _currentDistance += _totalPathLength;
        }
        else
        {
            _currentDistance = Mathf.Clamp(_currentDistance, 0.0f, _totalPathLength);
        }

        // Update transform based on current position
        UpdateTransformFromPath();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Path Configuration
    // --------------------------------------------------------------------------------------------

    public void SetControlPoints(List<KoreXYZVector> controlPoints)
    {
        _controlPoints = new List<KoreXYZVector>(controlPoints);
        UpdatePathCalculations();
    }

    public void SetControlPoints(params Vector3[] points)
    {
        _controlPoints.Clear();
        foreach (var point in points)
        {
            _controlPoints.Add(new KoreXYZVector(point.X, point.Y, point.Z));
        }
        UpdatePathCalculations();
    }

    public void AddControlPoint(Vector3 point)
    {
        _controlPoints.Add(new KoreXYZVector(point.X, point.Y, point.Z));
        UpdatePathCalculations();
    }

    public void ClearControlPoints()
    {
        _controlPoints.Clear();
        _pathPoints.Clear();
        _pathDistances.Clear();
        _totalPathLength = 0.0f;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Movement Control
    // --------------------------------------------------------------------------------------------

    public void StartMovement() => IsMoving = true;
    public void StopMovement() => IsMoving = false;
    public void ToggleMovement() => IsMoving = !IsMoving;

    public void SetPositionOnPath(float normalizedPosition)
    {
        normalizedPosition = Mathf.Clamp(normalizedPosition, 0.0f, 1.0f);
        _currentDistance = normalizedPosition * _totalPathLength;
        UpdateTransformFromPath();
    }

    public void ResetToStart()
    {
        _currentDistance = 0.0f;
        _lastSegmentIndex = 0;
        UpdateTransformFromPath();
    }

    public float GetNormalizedPosition()
    {
        return _totalPathLength > 0 ? _currentDistance / _totalPathLength : 0.0f;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Internal Path Calculations
    // --------------------------------------------------------------------------------------------

    private void SetDefaultPath()
    {
        // Create a simple S-curve as default
        _controlPoints.Add(new KoreXYZVector(0, 0, 0));
        _controlPoints.Add(new KoreXYZVector(2, 2, 0));
        _controlPoints.Add(new KoreXYZVector(4, -2, 0));
        _controlPoints.Add(new KoreXYZVector(6, 0, 0));
    }

    private void UpdatePathCalculations()
    {
        if (_controlPoints.Count < 3) return;

        // Generate path points using the Bézier curve functionality
        _pathPoints = KoreMeshDataPrimitives.PointsListFromBezier(_controlPoints, PathDivisions);

        // Calculate cumulative distances along the path
        _pathDistances.Clear();
        _pathDistances.Add(0.0f);
        _totalPathLength = 0.0f;

        for (int i = 1; i < _pathPoints.Count; i++)
        {
            var prev = _pathPoints[i - 1];
            var curr = _pathPoints[i];
            float segmentLength = (float)prev.DistanceTo(curr);
            _totalPathLength += segmentLength;
            _pathDistances.Add(_totalPathLength);
        }

        // Reset position tracking
        _currentDistance = 0.0f;
        _lastSegmentIndex = 0;
    }

    private void UpdateTransformFromPath()
    {
        if (_pathPoints.Count < 2) return;

        // Find the current segment and interpolation factor
        var (segmentIndex, t) = FindSegmentAndInterpolation(_currentDistance);

        if (segmentIndex >= _pathPoints.Count - 1)
        {
            // At the end of the path
            var endPoint = _pathPoints[_pathPoints.Count - 1];
            Position = new Vector3((float)endPoint.X, (float)endPoint.Y, (float)endPoint.Z);
            return;
        }

        // Interpolate position
        var pointA = _pathPoints[segmentIndex];
        var pointB = _pathPoints[segmentIndex + 1];

        var currentPos = KoreXYZVectorOps.Lerp(pointA, pointB, t);
        Position = new Vector3((float)currentPos.X, (float)currentPos.Y, (float)currentPos.Z);

        // Calculate orientation based on tangent
        UpdateOrientationFromTangent(segmentIndex, t);
    }

    private (int segmentIndex, float t) FindSegmentAndInterpolation(float distance)
    {
        // Optimize by starting search from last known position
        int startIndex = Mathf.Max(0, _lastSegmentIndex);

        for (int i = startIndex; i < _pathDistances.Count - 1; i++)
        {
            if (distance <= _pathDistances[i + 1])
            {
                _lastSegmentIndex = i;
                float segmentStart = _pathDistances[i];
                float segmentEnd = _pathDistances[i + 1];
                float segmentLength = segmentEnd - segmentStart;

                float t = segmentLength > 0 ? (distance - segmentStart) / segmentLength : 0.0f;
                return (i, t);
            }
        }

        // If not found, we're at the end
        return (_pathDistances.Count - 2, 1.0f);
    }

    private void UpdateOrientationFromTangent(int segmentIndex, float t)
    {
        Vector3 forward = CalculateTangentAtPosition(segmentIndex, t);
        if (forward.LengthSquared() < 0.001f) return; // Avoid zero-length tangents

        forward = forward.Normalized();
        Vector3 up = UpVector.Normalized();

        // Ensure up is perpendicular to forward
        Vector3 right = forward.Cross(up).Normalized();
        up = right.Cross(forward).Normalized();

        // Create rotation from forward direction
        var basis = new Basis(right, up, -forward); // Negative forward for Godot's coordinate system
        Transform = new Transform3D(basis, Position);
    }

    private Vector3 CalculateTangentAtPosition(int segmentIndex, float t)
    {
        // Calculate tangent by looking at nearby points
        const float epsilon = 0.01f;

        // Get points slightly before and after current position
        float distance = _currentDistance;
        float dist1 = Mathf.Max(0, distance - epsilon);
        float dist2 = Mathf.Min(_totalPathLength, distance + epsilon);

        var (seg1, t1) = FindSegmentAndInterpolation(dist1);
        var (seg2, t2) = FindSegmentAndInterpolation(dist2);

        var point1 = KoreXYZVectorOps.Lerp(_pathPoints[seg1], _pathPoints[Mathf.Min(seg1 + 1, _pathPoints.Count - 1)], t1);
        var point2 = KoreXYZVectorOps.Lerp(_pathPoints[seg2], _pathPoints[Mathf.Min(seg2 + 1, _pathPoints.Count - 1)], t2);

        var tangentVector = point2 - point1;
        return new Vector3((float)tangentVector.X, (float)tangentVector.Y, (float)tangentVector.Z);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Debug and Utility
    // --------------------------------------------------------------------------------------------

    public void DrawDebugPath()
    {
        // This could be implemented to visualize the path in the editor
        // For now, just print path info
        GD.Print($"Path has {_controlPoints.Count} control points, {_pathPoints.Count} path points, total length: {_totalPathLength}");


        // // create the list of points to draw
        // List<KoreXYZVector> points = KoreMeshDataPrimitives.PointsListFromBezier(_pathPoints, 100);

        // List<KoreXYZVector> debugPoints2 = new List<KoreXYZVector>();
        // foreach (var point in points)
        // {
        //     debugPoints2.Add(new KoreXYZVector(point));
        // }

        // // Add the points to a mesh for visualization
        // KoreMeshData debugMesh = new KoreMeshData();
        // debugMesh.AddPolyLine(debugPoints2, KoreColorPalette.Colors["OliveGreen"]);

        // // Draw the mesh in the editor
        // var debugNode = new KoreMeshNode3D("BezierPathDebug", debugMesh);

    }
}
