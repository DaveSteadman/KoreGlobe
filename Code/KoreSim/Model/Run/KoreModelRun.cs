using System;
using System.Threading;

using KoreCommon;

#nullable enable

namespace KoreSim;

// KoreModelRun: Class to control the running of the model, starting and stopping the clock and calling the model for its cyclic updates.
// This class exists underneath the KoreEventDriver, so it is called by the interface to perform the actual start and stop of the model.
public class KoreModelRun
{
    private Thread? modelThread = null;
    private float TargetUpdateIntervalSecs = 0.020f; // 50Hz target (20ms interval)

    // --------------------------------------------------------------------------------------------

    public void Start()
    {
        bool running = KoreSimFactory.Instance.SimClock.IsRunning;

        if (!running)
        {
            KoreSimFactory.Instance.EntityManager.Reset();
            KoreSimFactory.Instance.SimClock.Start();
            KoreSimFactory.Instance.SimClock.MarkTime();

            // Start the model running thread
            modelThread = new Thread(new ThreadStart(RunModel));
            modelThread.Start();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Pause()
    {
        KoreSimFactory.Instance.SimClock.Stop();

        // Stop the model running thread
        if (modelThread != null && modelThread.IsAlive)
        {
            modelThread.Join();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Resume()
    {
        bool running = KoreSimFactory.Instance.SimClock.IsRunning;

        if (!running)
        {
            KoreSimFactory.Instance.SimClock.Start();
            KoreSimFactory.Instance.SimClock.MarkTime();

            // Start the model running thread
            modelThread = new Thread(new ThreadStart(RunModel));
            modelThread.Start();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Stop()
    {
        KoreSimFactory.Instance.SimClock.Stop();

        // Stop the model running thread
        if (modelThread != null && modelThread.IsAlive)
        {
            modelThread.Join();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Reset()
    {
        Stop();
        KoreSimFactory.Instance.SimClock.Reset();
    }

    // --------------------------------------------------------------------------------------------

    private void RunModel()
    {
        bool running = KoreSimFactory.Instance.SimClock.IsRunning;

        while (running)
        {
            float startCycleTime = KoreCentralTime.RuntimeSecs;
            Update();
            float endCycleTime = KoreCentralTime.RuntimeSecs;

            float processingTime = endCycleTime - startCycleTime;
            if (processingTime > TargetUpdateIntervalSecs)
            {
                KoreCentralLog.AddEntry($"KoreModelRun: Processing time exceeded target - {processingTime * 1000.0f:F1}ms");
            }
            else
            {
                float sleepTimeMs = (TargetUpdateIntervalSecs - processingTime) * 1000.0f;
                sleepTimeMs = KoreValueUtils.Clamp(sleepTimeMs, 10, 1000);
                Thread.Sleep((int)sleepTimeMs);
            }

            running = KoreSimFactory.Instance.SimClock.IsRunning;
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Update()
    {
        // Call the model to update
        // Assuming the model update logic is handled within this method
        KoreSimFactory.Instance.EntityManager.UpdateKinetics();
    }

    // --------------------------------------------------------------------------------------------
}
