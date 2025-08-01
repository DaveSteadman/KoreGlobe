using Godot;
using System;

#nullable enable

public partial class KoreGodotAppRoot : Node
{
    private Control? RootUI = null;
    private Node3D? Root3D = null;
    private Node2D? Root2D = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialize the application root node
        GD.Print("KoreAppRoot _Ready");

        RootUI = (Control)FindChild("RootUI");
        Root2D = (Node2D)FindChild("Root2D");
        Root3D = (Node3D)FindChild("Root3D");
        if (RootUI == null) GD.PrintErr("KoreAppRoot: RootUI node not found.");
        if (Root2D == null) GD.PrintErr("KoreAppRoot: Root2D node not found.");
        if (Root3D == null) GD.PrintErr("KoreAppRoot: Root3D node not found.");

        RootUI!.TopLevel = true;
        Root3D!.TopLevel = true;
        Root2D!.TopLevel = true;

        // Load the main scene into the Root3D node
        var mainScene = GD.Load<PackedScene>("res://Scenes/MainScene.tscn");
        //var mainScene = GD.Load<PackedScene>("res://Scenes/3DSandbox.tscn");
        if (mainScene == null)
        {
            GD.PrintErr("KoreAppRoot: Failed to load MainScene.");
            return;
        }
        GD.Print("KoreAppRoot: MainScene loaded successfully.");

        Root3D!.AddChild(mainScene.Instantiate<Node>());





        // Load the UI Top scene first (so it appears underneath)
        PackedScene uiTopScene = GD.Load<PackedScene>("res://Scenes/UITop.tscn");
        Node uiTop = uiTopScene.Instantiate();
        RootUI!.AddChild(uiTop);

        var splashScene = GD.Load<PackedScene>("res://Scenes/SplashScreen.tscn");
        if (splashScene == null)
        {
            GD.PrintErr("KoreAppRoot: Failed to load SplashScreen.");
            return;
        }
        GD.Print("KoreAppRoot: SplashScreen loaded successfully.");
        RootUI!.AddChild(splashScene.Instantiate<Node>());


        // var mainInstance = mainScene.Instantiate<Node>();
        // if (mainInstance == null)
        // {
        //     GD.PrintErr("KoreAppRoot: Failed to instantiate MainScene.");
        //     return;
        // }

        // // Add the main instance to the scene tree
        // GetTree().Root.AddChild(mainInstance);

        // GD.Print("KoreAppRoot: MainScene instance added to the scene tree.  ");
    }
}
