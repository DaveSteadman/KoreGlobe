using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using KoreCommon;
using KoreSim;

#nullable enable

public partial class KoreEntityWindowPositionTab : Panel
{
    private MarginContainer? OutlineContainer = null;
    private VBoxContainer? MainVertContainer = null;

    private HBoxContainer? LatitudeRow = null;
    private Label? LatNameLabel = null;
    private LineEdit? LatValueInput = null;
    private Label? LatUnitLabel = null;

    private HBoxContainer? LongitudeRow = null;
    private Label? LonNameLabel = null;
    private LineEdit? LonValueInput = null;
    private Label? LonUnitLabel = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("KoreEntityWindowPositionTab _Ready");

        CreateControls();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void CreateControls()
    {
        OutlineContainer = new MarginContainer();
        OutlineContainer.AddThemeConstantOverride("margin_left", 10);
        OutlineContainer.AddThemeConstantOverride("margin_right", 10);
        OutlineContainer.AddThemeConstantOverride("margin_top", 10);
        OutlineContainer.AddThemeConstantOverride("margin_bottom", 10);
        AddChild(OutlineContainer);

        MainVertContainer = new VBoxContainer();
        OutlineContainer.AddChild(MainVertContainer);
        MainVertContainer.AddThemeConstantOverride("separation", 5);

        LatitudeRow = new HBoxContainer();
        MainVertContainer.AddChild(LatitudeRow);
        {
            LatNameLabel = new Label();
            LatNameLabel.Text = "Latitude";
            LatitudeRow.AddChild(LatNameLabel);
            LatitudeRow.CustomMinimumSize = new Vector2(150, 0);

            LatValueInput = new LineEdit();
            LatitudeRow.AddChild(LatValueInput);

            LatUnitLabel = new Label();
            LatUnitLabel.Text = "Degrees";
            LatitudeRow.AddChild(LatUnitLabel);
        }
        
        LongitudeRow = new HBoxContainer();
        MainVertContainer.AddChild(LongitudeRow);
        {
            LonNameLabel = new Label();
            LonNameLabel.Text = "Longitude";
            LongitudeRow.AddChild(LonNameLabel);

            LonValueInput = new LineEdit();
            LongitudeRow.AddChild(LonValueInput);

            LonUnitLabel = new Label();
            LonUnitLabel.Text = "Degrees";
            LongitudeRow.AddChild(LonUnitLabel);
        }
    }
}
