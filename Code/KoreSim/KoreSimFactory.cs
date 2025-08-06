using System;

using KoreCommon;


// Class to construct the main classes in the application and link them together, in lieu
// of a globals file that would publicly expose the main classes.

#nullable enable

namespace KoreSim;

public class KoreSimFactory
{
    // Singleton instance created as soon as the class is loaded.
    public static readonly KoreSimFactory Instance = new KoreSimFactory();

    private static readonly object lockObject = new object();

    public static readonly string ConfigPath = "AppConfig.json";

    // --------------------------------------------------------------------------------------------

    // Outside EventDriver
    public KoreConsole          ConsoleInterface { get; private set; }
    public KoreNetworkHub       NetworkHub       { get; private set; }
    public KoreMessageManager   MessageManager   { get; private set; }

    // Inside EventDriver
    public KoreEntityManager    EntityManager    { get; private set; }
    public KoreSimTime          SimClock         { get; private set; }
    public KoreModelRun         ModelRun         { get; private set; }
    public KoreElevationManager EleManager       { get; private set; }
    public KoreEventRegister    EventRegister    { get; private set; }

    // Usage: KoreStringDictionary kc = KoreSimFactory.Instance.KoreConfig;
    //        //        kc.SetParam("Key", "Value");
    public KoreStringDictionary KoreConfig { get; private set; } = new KoreStringDictionary();

    // --------------------------------------------------------------------------------------------

    // Instance control, with a lock, hidden constructor, and a singleton instance.

    // private static readonly object lockObject = new object();

    // private static KoreSimFactory? SingletonInstance = null;
    // private static bool           IsInitializing    = false;

    // // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    // public static KoreSimFactory Instance
    // {
    //     get
    //     {
    //         //GD.Print("KoreSimFactory.Instance");
    //         lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
    //         {
    //             if (IsInitializing)
    //             {
    //                 throw new InvalidOperationException("KoreSimFactory instance is being initialized and cannot be accessed recursively.");
    //             }

    //             if (SingletonInstance == null)
    //             {
    //                 SingletonInstance = new KoreSimFactory();
    //             }
    //             IsInitializing = false;

    //             return SingletonInstance;
    //         }
    //     }
    // }

    // --------------------------------------------------------------------------------------------

    // Private constructor

    private KoreSimFactory()
    {
        // Create the objects
        KoreCentralLog.AddEntry("Creating KoreSimFactory objects");

        ConsoleInterface = new KoreConsole();
        NetworkHub       = new KoreNetworkHub();
        MessageManager   = new KoreMessageManager();

        EventRegister    = new KoreEventRegister();
        EntityManager    = new KoreEntityManager();
        SimClock         = new KoreSimTime();
        ModelRun         = new KoreModelRun();
        EleManager       = new KoreElevationManager();


        // Read the logfile path from the config and update the centralised logger with it.
        LoadConfig("AppConfig.json");

        KoreSimCommands.RegisterCommands(ConsoleInterface);
        ConsoleInterface.Start();
        
        MessageManager.Start();

        KoreCentralLog.AddEntry("KoreSimFactory Construction - Done");

        // KoreTestCenter.RunAdHocTests();
    }


    // --------------------------------------------------------------------------------------------

    // Call to trigger constuctor 
    // This will trigger the constructor to run, if it hasn't already.
   
    
    // Usage: KoreSimFactory.TriggerInstance();

    public static void TriggerInstance()
    {
        // This will trigger the constructor to run, if it hasn't already.
        var _ = Instance;
    }

    // --------------------------------------------------------------------------------------------

    // // point to start the services, called after the main constructors
    // public void CallStart()
    // {

    //     //ConsoleInterface.Start();
    //     //MessageManager.Start();

    //     // co-ordinate some config (avoid directly coupling the classes)
    //     KoreCentralLog.LoggingActive = KoreCentralConfig.Instance.GetParam<bool>("LoggingActive");
    //     //KoreCentralLog.UpdatePath(KoreCentralConfig.Instance.GetParam<string>("LogPath"));

    //     string logPath = KoreCentralConfig.Instance.GetParam<string>("LogPath");
    //     if (!String.IsNullOrWhiteSpace(logPath))
    //     {
    //         KoreCentralLog.UpdatePath(logPath);
    //     }
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Config
    // ----------------------------------------------------------------------------------------------

    // Load the config:
    // - Read a file into a tring, then parse the string as JSON into the dictionary

    public void LoadConfig(string configFilePath)
    {
        KoreCentralLog.AddEntry($"Loading config from: {configFilePath}");

        // Readthe string
        if (!System.IO.File.Exists(configFilePath))
        {
            KoreCentralLog.AddEntry($"Config file not found: {configFilePath}");
            return;
        }
        // Read the file into a string
        string configFileContent = System.IO.File.ReadAllText(configFilePath);

        // Read teh content (dictionary is cleared as an init step)
        KoreConfig.ImportJson(configFileContent);

        // Log how many items were loaded
        KoreCentralLog.AddEntry($"Config loaded with {KoreConfig.Count} items"); // Assuming Count property exists

    }

    // --------------------------------------------------------------------------------------------

    // Save the config:
    // Usage: KoreSimFactory.Instance.SaveConfig(KoreSimFactory.ConfigPath);
    public void SaveConfig(string configFilePath)
    {
        try
        {

            KoreCentralLog.AddEntry($"Saving config to: {configFilePath}");

            // Export the dictionary to a JSON string
            string jsonContent = KoreConfig.ExportJson();

            // Write the string to the file
            System.IO.File.WriteAllText(configFilePath, jsonContent);
        }
        catch (Exception ex)
        {
            KoreCentralLog.AddEntry($"Error saving config: {ex.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------

    public void UpdateFromConfig()
    {
        // Update the objects from the config
        KoreCentralLog.AddEntry("Updating KoreSimFactory from config");

        // Example: ConsoleInterface.UpdateFromConfig(KoreConfig);
        // Example: EntityManager.UpdateFromConfig(KoreConfig);
        // Example: NetworkHub.UpdateFromConfig(KoreConfig);
        // Example: SimClock.UpdateFromConfig(KoreConfig);
        // Example: ModelRun.UpdateFromConfig(KoreConfig);
        // Example: MessageManager.UpdateFromConfig(KoreConfig);
        // Example: EleManager.UpdateFromConfig(KoreConfig);

        // Set logging path
        //string logPath = ;
        if (KoreConfig.Has("LogPath"))
        {
            string logPath = KoreConfig.Get("LogPath");

            if (!String.IsNullOrWhiteSpace(logPath))
                KoreCentralLog.SetFilename(logPath);
        }

        KoreCentralLog.AddEntry("KoreSimFactory updated from config");
    }


}
