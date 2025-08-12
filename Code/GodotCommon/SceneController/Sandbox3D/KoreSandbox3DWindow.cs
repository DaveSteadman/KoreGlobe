using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

public partial class KoreSandbox3DWindow : Window
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
        GD.Print("KoreSandbox3DWindow _Ready");

        // var sv = GetNode<SubViewport>("SubViewport");
        // sv.OwnWorld3D = true;
        // sv.World3D = new World3D();

        AttachControls();
    }

    public override void _Process(double delta)
    {
        if (KoreCentralTime.CheckTimer(ref UITimer, UITimerInterval))
        {
            // UpdateUI();
        }

        if (KoreCentralTime.CheckTimer(ref UISlowTimer, UISlowTimerInterval))
        {
            // UpdateUISlow();
        }
    }




    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void AttachControls()
    {
        CloseButton = (Button)FindChild("CloseButton");
        if (CloseButton == null) GD.PrintErr("KoreSandbox3DWindow: CloseButton node not found.");

        CloseButton?.Connect("pressed", new Callable(this, nameof(OnCloseRequested)));

        // Link up the X button to close the window
        Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));
    }

    private void OnCloseRequested()
    {
        GD.Print("KoreSandbox3DWindow: Close button pressed");
        ToClose = true;
        // QueueFree();
    }

}
