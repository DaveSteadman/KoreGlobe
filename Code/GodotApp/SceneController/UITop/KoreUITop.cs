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

    private Button?          DbgButton = null;
    private KoreSandbox3DWindow? Sandbox3DWindow = null;

    private Button?          MeshButton = null;
    private ModelEditWindow?  MeshWindow = null;

    private Button?          EntityButton = null;
    private KoreEntityWindow?    EntityWindow = null;

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
            UpdateSandbox3DWindowStates();
            UpdateMeshWindowStates();
            UpdateEntityWindowStates();
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

        DbgButton = (Button)FindChild("DbgButton");
        if (DbgButton == null) GD.PrintErr("KoreUITop: DbgButton node not found.");
        DbgButton?.Connect("pressed", new Callable(this, "OnDbgButtonPressed"));

        MeshButton = (Button)FindChild("MeshButton");
        if (MeshButton == null) GD.PrintErr("KoreUITop: MeshButton node not found.");
        MeshButton?.Connect("pressed", new Callable(this, "OnMeshButtonPressed"));

        EntityButton = (Button)FindChild("EntityButton");
        if (EntityButton == null) GD.PrintErr("KoreUITop: EntityButton node not found.");
        EntityButton?.Connect("pressed", new Callable(this, "OnEntityButtonPressed"));
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

    private void UpdateSandbox3DWindowStates()
    {
        if (Sandbox3DWindow != null)
        {
            // Disable the Dbg button if the Sandbox 3D window is open
            DbgButton!.Disabled = true;

            // If the window is valid and has it "Close Me" state set, clear it away and reset the states
            if (Sandbox3DWindow != null && Sandbox3DWindow.ToClose)
            {
                Sandbox3DWindow.QueueFree(); // Clean up the Sandbox 3D window
                Sandbox3DWindow = null!; // Reset the reference
                DbgButton.Disabled = false; // Enable if the Sandbox 3D window is closed
            }
        }
        else
        {
            DbgButton!.Disabled = false; // Enable if no Sandbox 3D window is open
        }
    }

    private void UpdateMeshWindowStates()
    {
        if (MeshWindow != null)
        {
            // Disable the Mesh button if the Mesh Editor window is open
            MeshButton!.Disabled = true;

            // If the window is valid and has it "Close Me" state set, clear it away and reset the states
            if (MeshWindow != null && MeshWindow.ToClose)
            {
                MeshWindow.QueueFree(); // Clean up the Mesh Editor window
                MeshWindow = null!; // Reset the reference
                MeshButton.Disabled = false; // Enable if the Mesh Editor window is closed
            }
        }
        else
        {
            MeshButton!.Disabled = false; // Enable if no Mesh Editor window is open
        }
    }

    private void UpdateEntityWindowStates()
    {
        if (EntityWindow != null)
        {
            // Disable the Entity button if the Entity window is open
            EntityButton!.Disabled = true;

            // If the window is valid and has it "Close Me" state set, clear it away and reset the states
            if (EntityWindow != null && EntityWindow.ToClose)
            {
                EntityWindow.QueueFree(); // Clean up the Entity window
                EntityWindow = null!; // Reset the reference
                EntityButton.Disabled = false; // Enable if the Entity window is closed
            }
        }
        else
        {
            EntityButton!.Disabled = false; // Enable if no Entity window is open
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

    // ----------------------------------------------------------------------------------------------

    private void OnNetworkButtonPressed()
    {
        GD.Print("KoreUITop: Network Button Pressed");

        // Load the Network Settings window scene and display it
        var networkWindowScene = GD.Load<PackedScene>("res://Scenes/GodotApp/NetworkControlWindow.tscn");
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

    // ----------------------------------------------------------------------------------------------

    private void OnDbgButtonPressed()
    {
        GD.Print("KoreUITop: Dbg Button Pressed");

        // Load the Test 3D scene and display it
        var test3DScene = GD.Load<PackedScene>("res://Scenes/GodotCommon/Sandbox3DScene.tscn");
        if (test3DScene == null)
        {
            GD.PrintErr("KoreUITop: Failed to load KoreSandbox3DScene.");
            return;
        }

        // Instance the Test 3D scene


        // GetTree().ChangeSceneToPacked(test3DScene);


        Sandbox3DWindow = test3DScene.Instantiate<KoreSandbox3DWindow>();
        if (Sandbox3DWindow == null)
        {
            GD.PrintErr("KoreUITop: Failed to instantiate KoreSandbox3DWindow.");
            return;
        }
        AddChild(Sandbox3DWindow);

        UpdateSandbox3DWindowStates();
    }

    // ----------------------------------------------------------------------------------------------

    private void OnMeshButtonPressed()
    {
        GD.Print("KoreUITop: Mesh Button Pressed");

        // Load the Mesh Editor window scene and display it
        var meshWindowScene = GD.Load<PackedScene>("res://Scenes/GodotCommon/ModelEdit.tscn");
        if (meshWindowScene == null)
        {
            GD.PrintErr("KoreUITop: Failed to load ModelEditWindow scene.");
            return;
        }

        // Instance the Mesh Editor window scene
        MeshWindow = meshWindowScene.Instantiate<ModelEditWindow>();
        if (MeshWindow == null)
        {
            GD.PrintErr("KoreUITop: Failed to instantiate ModelEditWindow.");
            return;
        }
        AddChild(MeshWindow);

        UpdateMeshWindowStates();
    }

    private void OnEntityButtonPressed()
    {
        GD.Print("KoreUITop: Entity Button Pressed");

        // Load the Entity window scene and display it
        var entityWindowScene = GD.Load<PackedScene>("res://Scenes/GodotApp/EntityWindow.tscn");
        if (entityWindowScene == null)
        {
            GD.PrintErr("KoreUITop: Failed to load KoreEntityWindow scene.");
            return;
        }

        // Instance the Entity window scene
        EntityWindow = entityWindowScene.Instantiate<KoreEntityWindow>();
        if (EntityWindow == null)
        {
            GD.PrintErr("KoreUITop: Failed to instantiate KoreEntityWindow.");
            return;
        }
        AddChild(EntityWindow);

        UpdateEntityWindowStates(); // Update the Entity window states after adding the window
    }

    // ----------------------------------------------------------------------------------------------

    private void OnCloseRequested()
    {
        GD.Print("KoreUITop: Close requested");
        QueueFree(); // This will close and destroy the UI Top node
    }

}
