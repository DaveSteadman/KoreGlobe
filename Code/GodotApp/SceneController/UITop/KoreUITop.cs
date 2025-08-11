using System;

using Godot;
using KoreCommon;

#nullable enable

// KoreUITop: The top-level UI node, controlling the management of the UI Frame presented to the user, and the spawning of
// various windows/controls within it.

public partial class KoreUITop : Control
{   
    private Button?          CliButton = null;
    private KoreUICLIWindow? CliWindow = null;
    
    private Button?          NetworkButton = null;
    private KoreNetworkSettingsWindow? KoreNetworkSettingsWindow = null;

    private float UIEventTimer = 0.0f;
    private float UIEventTimerInterval = 0.1f;

    // ---------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // ---------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("KoreUITop _Ready");

        AttachControls();
    }

    public override void _Process(double delta)
    {
        if (UIEventTimer < KoreCentralTime.RuntimeSecs)
        {
            UIEventTimer = KoreCentralTime.RuntimeSecs + UIEventTimerInterval;

            UpdateCLIStates();
            UpdateNetworkWindowStates();
        }
    }

    // ----------------------------------------------------------------------------------------------
    // MARK: Setup
    // ----------------------------------------------------------------------------------------------

    private void AttachControls()
    {
        CliButton = (Button)FindChild("CLIButton");
        if (CliButton == null) GD.PrintErr("KoreUITop: CliButton node not found.");

        NetworkButton = (Button)FindChild("NetworkButton");
        if (NetworkButton == null) GD.PrintErr("KoreUITop: NetworkButton node not found.");

        CliButton?.Connect("pressed", new Callable(this, "OnCliButtonPressed"));
        NetworkButton?.Connect("pressed", new Callable(this, "OnNetworkButtonPressed"));

    }

    // ----------------------------------------------------------------------------------------------

    private void UpdateCLIStates()
    {
        if (CliWindow != null)
        {
            // Disable the CLI button if the CLI window is open
            CliButton!.Disabled = true;

            // If the window is valid and has it "Close Me" state set, clear it away and reset the states
            if (CliWindow != null && CliWindow.ToClose)
            {
                CliWindow.QueueFree(); // Clean up the CLI window
                CliWindow = null!; // Reset the reference
                CliButton.Disabled = false; // Enable if the CLI window is closed
            }
        }
        else
        {
            CliButton!.Disabled = false; // Enable if no CLI window is open
        }

    }

    private void UpdateNetworkWindowStates()
    {
        if (KoreNetworkSettingsWindow != null)
        {
            // Disable the Network button if the Network Settings window is open
            NetworkButton!.Disabled = true;

            // If the window is valid and has it "Close Me" state set, clear it away and reset the states
            if (KoreNetworkSettingsWindow != null && KoreNetworkSettingsWindow.ToClose)
            {
                KoreNetworkSettingsWindow.QueueFree(); // Clean up the Network Settings window
                KoreNetworkSettingsWindow = null!; // Reset the reference
                NetworkButton.Disabled = false; // Enable if the Network Settings window is closed
            }
        }
        else
        {
            NetworkButton!.Disabled = false; // Enable if no Network Settings window is open
        }
    }

    // ----------------------------------------------------------------------------------------------
    // MARK: Actions
    // ----------------------------------------------------------------------------------------------

    private void OnCliButtonPressed()
    {
        GD.Print("KoreUITop: CLI Button Pressed");

        // Load the CLI window scene and display it
        var cliWindowScene = GD.Load<PackedScene>("res://Scenes/GodotCommon/UICommandLineWindow.tscn");
        if (cliWindowScene == null)
        {
            GD.PrintErr("KoreUITop: Failed to load KoreUICliWindow scene.");
            return;
        }

        // Instance the CLI window scene
        CliWindow = cliWindowScene.Instantiate<KoreUICLIWindow>();
        if (CliWindow == null)
        {
            GD.PrintErr("KoreUITop: Failed to instantiate KoreUICliWindow.");
            return;
        }
        AddChild(CliWindow);
        
        UpdateCLIStates(); // Update the CLI states after adding the window
    }

    private void OnNetworkButtonPressed()
    {
        GD.Print("KoreUITop: Network Button Pressed");

        // Load the Network Settings window scene and display it
        var networkWindowScene = GD.Load<PackedScene>("res://Scenes/GodotApp/NetworkControl.tscn");
        if (networkWindowScene == null)
        {
            GD.PrintErr("KoreUITop: Failed to load KoreNetworkSettingsWindow scene.");
            return;
        }

        // Instance the Network Settings window scene
        var networkInstance = networkWindowScene.Instantiate();
        KoreNetworkSettingsWindow = networkInstance as KoreNetworkSettingsWindow;
        if (KoreNetworkSettingsWindow == null)
        {
            GD.PrintErr($"KoreUITop: Failed to cast instantiated scene to KoreNetworkSettingsWindow. Actual type: {networkInstance.GetType().Name}");
            networkInstance?.QueueFree();
            return;
        }
        AddChild(KoreNetworkSettingsWindow);

        UpdateNetworkWindowStates(); // Update the Network window states after adding the window
    }
    

    private void OnCloseRequested()
    {
        GD.Print("KoreUITop: Close requested");
        QueueFree(); // This will close and destroy the UI Top node
    }

}
