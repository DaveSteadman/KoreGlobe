using System;

using Godot;

#nullable enable

// KoreUITop: The top-level UI node, controlling the management of the UI Frame presented to the user, and the spawning of
// various windows/controls within it.

public partial class KoreUITop : Control
{   
    private Button? CliButton = null;
    private KoreUICLIWindow? CliWindow = null;

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
        UpdateCLIStates();
    }

    // ----------------------------------------------------------------------------------------------
    // MARK: Setup
    // ----------------------------------------------------------------------------------------------

    private void AttachControls()
    {
        CliButton = (Button)FindChild("CLIButton");
        if (CliButton == null) GD.PrintErr("KoreUITop: CliButton node not found.");

        CliButton?.Connect("pressed", new Callable(this, "OnCliButtonPressed"));

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

    // ----------------------------------------------------------------------------------------------
    // MARK: Actions
    // ----------------------------------------------------------------------------------------------

    private void OnCliButtonPressed()
    {
        GD.Print("KoreUITop: CLI Button Pressed");

        // Load the CLI window scene and display it
        //var cliWindowScene = GD.Load<PackedScene>("res://Scenes/UITest.tscn");
        var cliWindowScene = GD.Load<PackedScene>("res://Scenes/UICommandLineWindow.tscn");
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
    }

    private void OnCloseRequested()
    {
        GD.Print("KoreUITop: Close requested");
        QueueFree(); // This will close and destroy the UI Top node
    }

}
