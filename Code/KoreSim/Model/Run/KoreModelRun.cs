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
            Update();
            // Assuming the update should happen at regular intervals
            // Sleep for a while to create the cyclic update
            Thread.Sleep(20); // Adjust the interval as needed - running the model at 50Hz for now.
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
