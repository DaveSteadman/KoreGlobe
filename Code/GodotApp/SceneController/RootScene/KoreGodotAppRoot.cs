using Godot;
using System;

#nullable enable

using KoreCommon;

public partial class KoreGodotAppRoot : Node
{
    private Control? RootUI = null;
    private Node2D? Root2D = null;
    private Node3D? Root3D = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialize the application root node
        GD.Print("KoreAppRoot _Ready");
        AppInitActions();

        RootUI = (Control)FindChild("RootUI");
        Root2D = (Node2D)FindChild("Root2D");
        Root3D = (Node3D)FindChild("Root3D");
        if (RootUI == null) GD.PrintErr("KoreAppRoot: RootUI node not found.");
        if (Root2D == null) GD.PrintErr("KoreAppRoot: Root2D node not found.");
        if (Root3D == null) GD.PrintErr("KoreAppRoot: Root3D node not found.");

        RootUI!.TopLevel = true;
        Root2D!.TopLevel = true;
        Root3D!.TopLevel = true;


        // --- LOAD UI Scenes ---


        // Load the UI Top scene first (so it appears underneath the splash screen)
        PackedScene uiTopScene = GD.Load<PackedScene>("res://Scenes/GodotApp/UITop.tscn");
        Node uiTop = uiTopScene.Instantiate();
        RootUI!.AddChild(uiTop);

        // Load the mini panel for status display
        PackedScene miniPanelScene = GD.Load<PackedScene>("res://Scenes/GodotCommon/UiMiniPanel.tscn");
        if (miniPanelScene != null)
        {
            Node miniPanel = miniPanelScene.Instantiate();
            RootUI!.AddChild(miniPanel);
            GD.Print("KoreAppRoot: UiMiniPanel loaded successfully.");
        }
        else
        {
            GD.PrintErr("KoreAppRoot: Failed to load UiMiniPanel.");
        }

        var splashScene = GD.Load<PackedScene>("res://Scenes/GodotCommon/SplashScreen.tscn");
        if (splashScene == null)
        {
            GD.PrintErr("KoreAppRoot: Failed to load SplashScreen.");
            return;
        }
        GD.Print("KoreAppRoot: SplashScreen loaded successfully.");
        RootUI!.AddChild(splashScene.Instantiate<Node>());


        // --- LOAD 2D Scenes ---


        // --- LOAD 3D Scenes ---


        // Load the main scene into the Root3D node
        var mainScene = GD.Load<PackedScene>("res://Scenes/GodotApp/MainScene.tscn");
        //var mainScene = GD.Load<PackedScene>("res://Scenes/3DSandbox.tscn");
        if (mainScene == null)
        {
            GD.PrintErr("KoreAppRoot: Failed to load MainScene.");
            return;
        }
        GD.Print("KoreAppRoot: MainScene loaded successfully.");

        Root3D!.AddChild(mainScene.Instantiate<Node>());
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Top Level App Functions
    // --------------------------------------------------------------------------------------------

    private void AppInitActions()
    {
        KoreCentralLog.AddEntry("AppInitActions");
        KoreCentralLog.SetFilename($"Log-{KoreCentralTime.RuntimeStartTimestampUTC}.log");
    }
}
