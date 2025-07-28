
using Godot;
using KoreCommon;


public partial class KoreAppRoot : Node3D
{
    // This is the root node for the Kore application, which can be used to manage global state or settings.
    public static KoreAppRoot Instance { get; private set; }

    public override void _Ready()
    {
        // Ensure that there is only one instance of KoreAppRoot
        if (Instance != null)
        {
            GD.PrintErr("KoreAppRoot already initialized!");
            return;
        }

        Instance = this;
        Name = "KoreAppRoot";

        KoreCentralLog.SetFilename($"Log-{KoreCentralTime.RuntimeStartTimestampUTC}.log");

        KoreGodotFactory.TriggerInstance();



        // GD.Print("2!");
        KoreGodotFactory.Instance.CreateObjects(this);
        // GD.Print("33!");
        
        
        
        //KoreGodotFactory.Instance.ZeroNode = new KoreZeroNode();

        // Additional initialization code can go here
    }
}
