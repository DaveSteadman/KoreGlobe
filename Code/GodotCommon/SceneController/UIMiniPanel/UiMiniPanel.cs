using Godot;
using System;

using KoreCommon;
using System.Data;

#nullable enable

public partial class UiMiniPanel : Control
{
    private Label? LabelError = null;
    private Label? LabelWarn = null;
    private Label? LabelInfo = null;
    private Label? LabelUPS = null;

    // UI Timers
    private float UITimer = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms
    private float UI1HzTimer = 0.0f;
    private float UI1HzTimerInterval = 1.0f; // 1 second
    
    private int UPSRunningCount = 0; // UPS Updates Per Second
    private int UPSCount = 0; // UPS Updates Per Second

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("ModelEditWindow _Ready");
        AttachControls();
        SetupAnchoring();
    }

    public override void _Process(double delta)
    {
        UPSRunningCount++;
        if (KoreCentralTime.CheckTimer(ref UI1HzTimer, UI1HzTimerInterval))
        {
            UPSCount = UPSRunningCount;
            UPSRunningCount = 0;
        }

        if (KoreCentralTime.CheckTimer(ref UITimer, UITimerInterval))
        {
            UpdateUI();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void SetupAnchoring()
    {
        // Set anchors to bottom-right corner
        SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomRight);

        // Add 10px margin from edges
        OffsetLeft = -10 - Size.X;   // 10px from right edge
        OffsetTop = -10 - Size.Y;    // 10px from bottom edge
    }

    private void AttachControls()
    {
        LabelError = (Label)FindChild("LabelError");
        LabelWarn = (Label)FindChild("LabelWarn");
        LabelInfo = (Label)FindChild("LabelInfo");
        LabelUPS = (Label)FindChild("LabelUPS");

        if (LabelError == null || LabelWarn == null || LabelInfo == null || LabelUPS == null)
        {
            KoreCentralLog.AddEntry("UiMiniPanel: One or more labels not found");
        }
    }

    private void UpdateUI()
    {
        LabelError?.SetText("❌ ---");
        LabelWarn?.SetText("⚠️ ---");
        LabelInfo?.SetText($"ℹ️ {KoreCentralLog.GetLogEntryCount():0000}");
        LabelUPS?.SetText($"⚡ {UPSCount:000}");
    }

}
