using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

public partial class KoreSandbox3DScene : Node3D
{
    // Sandbox3DScene.UIMount
    public static Control? UIMount = null;

    // UI Timers
    private float UITimer = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms

    private float UISlowTimer = 0.0f;
    private float UISlowTimerInterval = 2.5f;

    public bool ToClose { get; set; } = false;

    private Button? CloseButton = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("KoreSandbox3DScene _Ready");

        AttachControls();
    }

    public override void _Process(double delta)
    {
        if (UITimer > KoreCentralTime.RuntimeSecs)
        {
            UITimer = KoreCentralTime.RuntimeSecs + UITimerInterval;
            // UpdateUI();
        }

        if (UISlowTimer > KoreCentralTime.RuntimeSecs)
        {
            UISlowTimer = KoreCentralTime.RuntimeSecs + UISlowTimerInterval;
            // UpdateUISlow();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void AttachControls()
    {
        CloseButton = (Button)FindChild("CloseButton");
        if (CloseButton == null) GD.PrintErr("KoreSandbox3DScene: CloseButton node not found.");

        CloseButton?.Connect("pressed", new Callable(this, nameof(OnCloseButtonPressed)));
    }

    private void OnCloseButtonPressed()
    {
        GD.Print("KoreSandbox3DScene: Close button pressed");
        ToClose = true;
        QueueFree();
    }

}
