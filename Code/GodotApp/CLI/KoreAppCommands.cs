
using KoreCommon;



public static class KoreAppCommands
{

    // Usage: KoreAppCommands.RegisterCommands(console)
    public static void RegisterCommands(KoreConsole console)
    {

        // Register commands and their handlers here
        KoreCentralLog.AddEntry("KoreConsole: Initializing APP commands...");

        // General app control commands
        console.AddCommandHandler(new KoreCommandVersion());

        // Zero Node
        console.AddCommandHandler(new KoreCliCmdZeroPosSet());
    }
}