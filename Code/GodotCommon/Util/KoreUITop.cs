using System;

using Godot;

#nullable enable

public partial class KoreUITop : Control
{
    private Button? CliButton = null;

    // ---------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // ---------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("KoreUITop _Ready");

        AttachControls();
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
    // MARK: Actions
    // ----------------------------------------------------------------------------------------------

    private void OnCliButtonPressed()
    {
        GD.Print("KoreUITop: CLI Button Pressed");

        // Load the CLI window scene and display it
        var cliWindowScene = GD.Load<PackedScene>("res://Scenes/UICommandLineWindow.tscn");
        if (cliWindowScene == null)
        {
            GD.PrintErr("KoreUITop: Failed to load KoreUICliWindow scene.");
            return;
        }

        // Instance the CLI window scene
        var cliWindow = cliWindowScene.Instantiate<Window>();
        if (cliWindow == null)
        {
            GD.PrintErr("KoreUITop: Failed to instantiate KoreUICliWindow.");
            return;
        }

        cliWindow.SetExclusive(true);


        AddChild(cliWindow);
        // if (MainScene.UIMount != null)
        // {
        //     // Add the CLI window to the scene
        //     MainScene.UIMount.
        // }
    }

    private void OnCloseRequested()
    {
        GD.Print("KoreUITop: Close requested");
        QueueFree(); // This will close and destroy the UI Top node
    }

}
