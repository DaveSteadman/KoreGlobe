using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using KoreCommon;
using KoreSim;
using KoreGIS;

#nullable enable

public partial class KoreEntityWindowPositionTab : Panel
{
    private MarginContainer? OutlineContainer = null;
    private VBoxContainer? MainVertContainer = null;

    // Latitude Controls
    private HBoxContainer? LatitudeRow = null;
    private Label? LatNameLabel = null;
    private LineEdit? LatValueInput = null;
    private Label? LatUnitLabel = null;

    // Longitude Controls
    private HBoxContainer? LongitudeRow = null;
    private Label? LonNameLabel = null;
    private LineEdit? LonValueInput = null;
    private Label? LonUnitLabel = null;

    // Altitude Controls
    private HBoxContainer? AltitudeRow = null;
    private Label? AltNameLabel = null;
    private LineEdit? AltValueInput = null;
    private Label? AltUnitLabel = null;

    // Separator
    private HSeparator? HSep1 = null;

    // Course Controls
    private HBoxContainer? CourseRow = null;
    private Label? CourseNameLabel = null;
    private LineEdit? CourseValueInput = null;
    private Label? CourseUnitLabel = null;

    // Speed Controls
    private HBoxContainer? SpeedRow = null;
    private Label? SpeedNameLabel = null;
    private LineEdit? SpeedValueInput = null;
    private Label? SpeedUnitLabel = null;

    // Selected Entity
    public string? SelectedEntityName { get; set; } = null;

    // UI Update Timer
    private float UIUpdateTimer = 0.0f;
    private float UIUpdateTimerIncrementSecs = 0.5f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("KoreEntityWindowPositionTab _Ready");

        CreateControls();
    }

    public override void _Process(double delta)
    {
        // Update UI every few seconds
        if (KoreCentralTime.CheckTimer(ref UIUpdateTimer, UIUpdateTimerIncrementSecs))
        {
            UpdateUIValues();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void CreateControls()
    {
        // Create the margin around the edge of the whole panel
        OutlineContainer = new MarginContainer();
        OutlineContainer.AddThemeConstantOverride("margin_left", 10);
        OutlineContainer.AddThemeConstantOverride("margin_right", 10);
        OutlineContainer.AddThemeConstantOverride("margin_top", 10);
        OutlineContainer.AddThemeConstantOverride("margin_bottom", 10);
        AddChild(OutlineContainer);

        // Create the vertical arrangement of rows in the panel
        MainVertContainer = new VBoxContainer();
        OutlineContainer.AddChild(MainVertContainer);
        MainVertContainer.AddThemeConstantOverride("separation", 5);

        // Create the latitude row
        LatitudeRow = new HBoxContainer();
        LatitudeRow.Alignment = BoxContainer.AlignmentMode.Center; // Center align all children vertically
        LatitudeRow.AddThemeConstantOverride("separation", 15);
        MainVertContainer.AddChild(LatitudeRow);
        {
            LatNameLabel = new Label() { Name = "LatNameLabel" };
            LatNameLabel.Text = "Latitude";
            LatNameLabel.CustomMinimumSize = new Vector2(160, 50);
            LatNameLabel.VerticalAlignment = VerticalAlignment.Center;
            LatitudeRow.AddChild(LatNameLabel);

            LatValueInput = new LineEdit() { Name = "LatValueInput" };
            LatValueInput.Text = "-";
            LatValueInput.CustomMinimumSize = new Vector2(160, 50);
            LatitudeRow.AddChild(LatValueInput);

            LatUnitLabel = new Label() { Name = "LatUnitLabel" };
            LatUnitLabel.Text = "Degrees";
            LatUnitLabel.CustomMinimumSize = new Vector2(100, 50);
            LatUnitLabel.VerticalAlignment = VerticalAlignment.Center;
            LatitudeRow.AddChild(LatUnitLabel);
        }

        // Create the longitude row
        LongitudeRow = new HBoxContainer();
        LongitudeRow.Alignment = BoxContainer.AlignmentMode.Center; // Center align all children vertically
        LongitudeRow.AddThemeConstantOverride("separation", 15);
        MainVertContainer.AddChild(LongitudeRow);
        {
            LonNameLabel = new Label() { Name = "LonNameLabel" };
            LonNameLabel.Text = "Longitude";
            LonNameLabel.CustomMinimumSize = new Vector2(160, 50);
            LonNameLabel.VerticalAlignment = VerticalAlignment.Center;
            LongitudeRow.AddChild(LonNameLabel);

            LonValueInput = new LineEdit() { Name = "LonValueInput" };
            LonValueInput.CustomMinimumSize = new Vector2(160, 50);
            LongitudeRow.AddChild(LonValueInput);

            LonUnitLabel = new Label() { Name = "LonUnitLabel" };
            LonUnitLabel.Text = "Degrees";
            LonUnitLabel.CustomMinimumSize = new Vector2(100, 50);
            LonUnitLabel.VerticalAlignment = VerticalAlignment.Center;
            LongitudeRow.AddChild(LonUnitLabel);
        }

        // Create the altitude row
        AltitudeRow = new HBoxContainer();
        AltitudeRow.Alignment = BoxContainer.AlignmentMode.Center; // Center align all children vertically
        AltitudeRow.AddThemeConstantOverride("separation", 15);
        MainVertContainer.AddChild(AltitudeRow);
        {
            AltNameLabel = new Label() { Name = "AltNameLabel" };
            AltNameLabel.Text = "Altitude";
            AltNameLabel.CustomMinimumSize = new Vector2(160, 50);
            AltNameLabel.VerticalAlignment = VerticalAlignment.Center;
            AltitudeRow.AddChild(AltNameLabel);

            AltValueInput = new LineEdit() { Name = "AltValueInput" };
            AltValueInput.CustomMinimumSize = new Vector2(160, 50);
            AltitudeRow.AddChild(AltValueInput);

            AltUnitLabel = new Label() { Name = "AltUnitLabel" };
            AltUnitLabel.Text = "Meters MSL";
            AltUnitLabel.CustomMinimumSize = new Vector2(100, 50);
            AltUnitLabel.VerticalAlignment = VerticalAlignment.Center;
            AltitudeRow.AddChild(AltUnitLabel);
        }

        // Create the separator
        HSep1 = new HSeparator();
        MainVertContainer.AddChild(HSep1);

        // Create the course row
        CourseRow = new HBoxContainer();
        CourseRow.Alignment = BoxContainer.AlignmentMode.Center; // Center align all children vertically
        CourseRow.AddThemeConstantOverride("separation", 15);
        MainVertContainer.AddChild(CourseRow);
        {
            CourseNameLabel = new Label() { Name = "CourseNameLabel" };
            CourseNameLabel.Text = "Course";
            CourseNameLabel.CustomMinimumSize = new Vector2(160, 50);
            CourseNameLabel.VerticalAlignment = VerticalAlignment.Center;
            CourseRow.AddChild(CourseNameLabel);

            CourseValueInput = new LineEdit() { Name = "CourseValueInput" };
            CourseValueInput.CustomMinimumSize = new Vector2(160, 50);
            CourseRow.AddChild(CourseValueInput);

            CourseUnitLabel = new Label() { Name = "CourseUnitLabel" };
            CourseUnitLabel.Text = "Degrees";
            CourseUnitLabel.CustomMinimumSize = new Vector2(100, 50);
            CourseUnitLabel.VerticalAlignment = VerticalAlignment.Center;
            CourseRow.AddChild(CourseUnitLabel);
        }

        // Create the speed row
        SpeedRow = new HBoxContainer();
        SpeedRow.Alignment = BoxContainer.AlignmentMode.Center; // Center align all children vertically
        SpeedRow.AddThemeConstantOverride("separation", 15);
        MainVertContainer.AddChild(SpeedRow);
        {
            SpeedNameLabel = new Label() { Name = "SpeedNameLabel" };
            SpeedNameLabel.Text = "Speed";
            SpeedNameLabel.CustomMinimumSize = new Vector2(160, 50);
            SpeedNameLabel.VerticalAlignment = VerticalAlignment.Center;
            SpeedRow.AddChild(SpeedNameLabel);

            SpeedValueInput = new LineEdit() { Name = "SpeedValueInput" };
            SpeedValueInput.CustomMinimumSize = new Vector2(160, 50);
            SpeedRow.AddChild(SpeedValueInput);

            SpeedUnitLabel = new Label() { Name = "SpeedUnitLabel" };
            SpeedUnitLabel.Text = "KPH";
            SpeedUnitLabel.CustomMinimumSize = new Vector2(100, 50);
            SpeedUnitLabel.VerticalAlignment = VerticalAlignment.Center;
            SpeedRow.AddChild(SpeedUnitLabel);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    public void UpdateUIValues()
    {
        GD.Print("KoreEntityWindowPositionTab UpdateUIValues");

        if (!string.IsNullOrEmpty(SelectedEntityName))
        {
            KoreLLAPoint entPos = KoreEventDriver.GetEntityPosition(SelectedEntityName);
            KoreCourse entCourse = KoreEventDriver.GetEntityCourse(SelectedEntityName);

            GD.Print($"Entity: {SelectedEntityName}, Position: {entPos}");

            // Update latitude and longitude values
            if (LatValueInput != null) LatValueInput.Text = $"{entPos.LatDegs:F3}";
            if (LonValueInput != null) LonValueInput.Text = $"{entPos.LonDegs:F3}";
            if (AltValueInput != null) AltValueInput.Text = $"{entPos.AltMslM:F2}";

            // Update course values
            if (CourseValueInput != null) CourseValueInput.Text = $"{entCourse.HeadingDegs:F2}";
            if (SpeedValueInput != null) SpeedValueInput.Text = $"{entCourse.SpeedKph:F2}";
        }
        else
        {
            // Update latitude and longitude values
            if (LatValueInput != null) LatValueInput.Text = "-";
            if (LonValueInput != null) LonValueInput.Text = "-";
            if (AltValueInput != null) AltValueInput.Text = "-";

            // Update course values
            if (CourseValueInput != null) CourseValueInput.Text = "-";
            if (SpeedValueInput != null) SpeedValueInput.Text = "-";
        }
    }
}

