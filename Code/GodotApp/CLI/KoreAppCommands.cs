
using KoreCommon;

public static class KoreAppCommands
{
    // Usage: KoreAppCommands.RegisterCommands(console)
    public static void RegisterCommands(KoreCommandHandler console)
    {
        // Register commands and their handlers here
        KoreCentralLog.AddEntry("KoreAppCommands: Initializing APP commands...");

        // General app control commands
        console.AddCommandHandler(new KoreCommandVersion());
    }
}