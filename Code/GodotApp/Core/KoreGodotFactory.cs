
// GloAppFactory is a central global class managing access to internal modelling element.
// KoreGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;
using System.IO;

using Godot;

using KoreCommon;
using KoreSim;

#nullable enable

public class KoreGodotFactory
{
    public Node3D?                 SceneRootNode      { get; private set; }
    public KoreZeroNode?           ZeroNode           { get; private set; }
    // public GloMapManager         EarthCoreNode      { get; private set; }  // KoreGodotFactory.Instance.EarthCodeNode

    //public GloCameraMoverWorld WorldCamNode         { get; private set; }

    // Configuration settings - KoreGodotFactory.Config
    public KoreStringDictionary Config { get; private set; } = new KoreStringDictionary();

    //public KoreGodotEntityManager GodotEntityManager { get; private set; }
    public KoreZeroNodeMapManager? ZeroNodeMapManager { get; private set; }
    //public GloTextureLoader       TextureLoader      { get; private set; }
    //public Kore3DModelLibrary     ModelLibrary       { get; private set; } // KoreGodotFactory.Instance.ModelLibrary
    //public GloUIState             UIState            { get; private set; }

    // Singleton pattern
    private static readonly object   lockObject        = new object();
    private static KoreGodotFactory? SingletonInstance = null;
    private static bool              IsInitialised     = false;
    private static bool              IsCreating        = false;

    // Threadsafe message queue, for messages that need to be handled on the main thread
    //public JSONThreadsafeMessageFIFO UIMsgQueue       { get; private set; }
    //private GloUIMessageManager      UIMessageManager { get; set; }

    // Usage:
    // - KoreGodotFactory.Instance.UIMsgQueue.EnqueueMessage
    // - KoreGodotFactory.Instance.UIMsgQueue.TryDequeueMessage

    // --------------------------------------------------------------------------------------------
    // MARK: Singleton Pattern
    // --------------------------------------------------------------------------------------------

    // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    public static KoreGodotFactory Instance
    {
        get
        {
            //GD.Print("KoreGodotFactory.Instance");
            lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
            {
                if (IsCreating)
                {
                    throw new InvalidOperationException("KoreGodotFactory instance is being initialized and cannot be accessed recursively.");
                }

                if (SingletonInstance == null)
                {
                    IsCreating = true;
                    try
                    {
                        SingletonInstance = new KoreGodotFactory();
                    }
                    finally
                    {
                        IsCreating = false;
                    }
                }
                return SingletonInstance;
            }
        }
    }

    // Private constructor
    private KoreGodotFactory()
    {
    }

    // Function to check if the instance has been created, and if its not there yet, create it
    public static void TriggerInstance()
    {
        if (SingletonInstance == null)
        {
            GD.Print("KoreGodotFactory.TriggerInstance");
            lock (lockObject)
            {
                if (SingletonInstance == null)
                {
                    SingletonInstance = new KoreGodotFactory();
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Initialization
    // --------------------------------------------------------------------------------------------

    public void CreateObjects(Node3D sceneRootNode)
    {
        GD.Print($"4");
        lock (lockObject)
        {
            GD.Print($"5");
            if (!IsInitialised)
            {
                GD.Print($"6");
                // Doing a chunk of object creation, wrapping in an excpetion handler
                try
                {



                    // string configPath = "Config.json";
                    // string configJson = File.ReadAllText(configPath);
                    // string fullConfigJson = Path.Combine(Directory.GetCurrentDirectory(), configJson);

                    // GD.Print($"- KoreGodotFactory Import Config Json ({fullConfigJson}) - - - -");

                    // Config.ImportJson(configJson);

                    GD.Print("- KoreGodotFactory CreateObjects - - - -");

                    SceneRootNode = sceneRootNode;


                    if (SceneRootNode == null)
                    {
                        GD.PrintErr("KoreGodotFactory.CreateObjects: SceneRootNode is null!");
                        return;
                    }
                    else
                    {
                        ZeroNode = new KoreZeroNode();

                        SceneRootNode.Name = "SceneRootNode";
                        SceneRootNode.AddChild(ZeroNode);

                        ZeroNodeMapManager = new KoreZeroNodeMapManager(ZeroNode);
                        SceneRootNode.AddChild(ZeroNodeMapManager);

                    }
                    

                    // GodotEntityManager = new KoreGodotEntityManager();
                    // ZeroNode.AddChild(GodotEntityManager);

                    // ZeroNodeMapManager = new KoreZeroNodeMapManager(ZeroNode);
                    // ZeroNode.AddChild(ZeroNodeMapManager);

                    // ImageManager = new();
                    // SceneRootNode.AddChild(ImageManager);


                    // TextureLoader = new KoreTextureLoader();
                    // ModelLibrary  = new Kore3DModelLibrary();
                    // UIState       = new KoreUIState();

                    // WorldCamNode = new KoreCameraMoverWorld() { Name = "WorldCamBase" };
                    // SceneRootNode.AddChild(WorldCamNode);
                    // WorldCamNode.CamNode.Current = true;
                    // WorldCamNode.CamPosFromString(UIState.CameraMemory);


                    // UIMsgQueue    = new JSONThreadsafeMessageFIFO();


                    // UIMessageManager = new GloUIMessageManager();
                    // SceneRootNode.AddChild(UIMessageManager);

                    IsInitialised = true;

                    KoreCentralLog.AddEntry("KoreGodotFactory CreateObjects Done");
                    GD.Print("- KoreGodotFactory CreateObjects Done - - - - ");

                    // TestPoint();
                }
                catch (Exception ex)
                {
                    KoreCentralLog.AddEntry($"KoreGodotFactory.CreateObjects: Exception: {ex.Message}");
                }
            }
            else
            {
                throw new InvalidOperationException("KoreGodotFactory.CreateObjects called twice.");
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Helpers
    // --------------------------------------------------------------------------------------------

    private void TestPoint()
    {
        // GloNumeric2DArray<float> testPattern =
        //     GloNumeric2DArray<float>.LLATestPattern(360, 180,
        //         new GloLLBox() {MinLonDegs = -180.0, MinLatDegs = -90.0, MaxLonDegs = 180.0, MaxLatDegs = 90.0 });

        // GloNumeric2DArrayIO<float>.SaveToCSVFile(testPattern, "C:/Util/Godot/Globe4-Maps/ElePrep/testPattern.csv", 2);
    }


}
