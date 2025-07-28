using System.Collections.Generic;

// KoreCommandVersion

using KoreCommon;

namespace KoreSim;

public class KoreCommandExit : KoreCommand
{
    public KoreCommandExit()
    {
        Signature.Add("exit");
    }

    public override string Execute(List<string> parameters)
    {
        // Stopping the model run
        if (KoreSimFactory.Instance.SimClock.IsRunning)
        {
            KoreCentralLog.AddEntry("Stopping model run");
            KoreSimFactory.Instance.ModelRun.Stop();
        }

        // Exiting the application - ending the threads
        KoreCentralLog.AddEntry("Exiting the application");
        KoreEventDriver.ExitApplication();

        return "Exiting the application";
    }
}
