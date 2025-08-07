
using KoreCommon;

namespace KoreSim;

public static class KoreSimCommands
{
    
    // Usage: KoreSimCommands.RegisterCommands(console)
    public static void RegisterCommands(KoreConsole console)
    {

        // Register commands and their handlers here
        KoreCentralLog.AddEntry("KoreConsole: Initializing commands...");

        // General app control commands
        console.AddCommandHandler(new KoreCommandExit());
        console.AddCommandHandler(new KoreCommandConfigReport());
        console.AddCommandHandler(new KoreCommandConfigSet());

        // Sim control
        console.AddCommandHandler(new KoreCommandSimClock());
        console.AddCommandHandler(new KoreCommandSimStart());
        console.AddCommandHandler(new KoreCommandSimStop());
        console.AddCommandHandler(new KoreCommandSimReset());
        console.AddCommandHandler(new KoreCommandSimPause());
        console.AddCommandHandler(new KoreCommandSimResume());

        // Network
        console.AddCommandHandler(new KoreCommandNetworkReport());
        console.AddCommandHandler(new KoreCommandNetworkInjectIncoming());
        console.AddCommandHandler(new KoreCommandNetworkEndConnection());

        //console.AddCommandHandler(new KoreCommandModelJsonRead());
        //console.AddCommandHandler(new KoreCommandModelJsonWrite());

        // Entity control
        console.AddCommandHandler(new KoreCommandEntityTestScenario());
        console.AddCommandHandler(new KoreCommandEntityAdd());
        console.AddCommandHandler(new KoreCommandEntityDelete());
        console.AddCommandHandler(new KoreCommandEntityDeleteAll());

        // Entity details
        console.AddCommandHandler(new KoreCommandEntityPosition());
        console.AddCommandHandler(new KoreCommandEntityCourse());
        console.AddCommandHandler(new KoreCommandEntityCourseDelta());

        // Entity Report
        console.AddCommandHandler(new KoreCommandEntityReportElem());
        console.AddCommandHandler(new KoreCommandEntityReportPos());

        // Element Control
        //console.AddCommandHandler(new KoreCommandEntityDeleteAllEmitters());

        // MapServer
        console.AddCommandHandler(new KoreCommandElePrep());
        console.AddCommandHandler(new KoreCommandEleLoadArc());
        console.AddCommandHandler(new KoreCommandEleSaveTile());
        console.AddCommandHandler(new KoreCommandEleSaveTileSet());
        console.AddCommandHandler(new KoreCommandEleLoadTile());
        console.AddCommandHandler(new KoreCommandEleForPos());
        console.AddCommandHandler(new KoreCommandEleReport());
        console.AddCommandHandler(new KoreCommandElePatchLoad());
        console.AddCommandHandler(new KoreCommandElePatchSave());

        // Tile Images
        console.AddCommandHandler(new KoreCommandSatCollate());
        console.AddCommandHandler(new KoreCommandSatDivide());
        console.AddCommandHandler(new KoreCommandSatDivideTo());


    }
}