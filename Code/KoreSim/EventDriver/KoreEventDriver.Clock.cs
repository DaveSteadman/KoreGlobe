
using System;

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

using KoreCommon;

namespace KoreSim;

public static partial class KoreEventDriver
{

    // ---------------------------------------------------------------------------------------------
    // MARK: Simulation Clock
    // ---------------------------------------------------------------------------------------------
    // different to runtime clock and Time Functions, this is the in-sium time that can be paused etc.

    public static void SimClockStart() => KoreSimFactory.Instance.ModelRun.Start();
    public static void SimClockStop() => KoreSimFactory.Instance.ModelRun.Stop();
    public static void SimClockReset() => KoreSimFactory.Instance.ModelRun.Reset();
    public static void SimClockResume() => KoreSimFactory.Instance.ModelRun.Resume();

    public static void SetSimClockSeconds(double secs)
    {
    }

    public static void SetSimTimeHMS(string hms)
    {
        KoreSimFactory.Instance.SimClock.SimTimeHMS = hms;
    }

    public static int SimSeconds() => (int)KoreSimFactory.Instance.SimClock.SimTime;
    public static string SimTimeHMS() => KoreSimFactory.Instance.SimClock.SimTimeHMS;

    // ---------------------------------------------------------------------------------------------

    public static void ClockSetRate(double rate) => KoreSimFactory.Instance.SimClock.SetSimRate(rate);
    public static double ClockRate() => KoreSimFactory.Instance.SimClock.SimRate;
}

