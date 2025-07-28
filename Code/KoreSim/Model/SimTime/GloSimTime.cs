using System;


// Class to control the running clock of a simulation.

// Public:
// - StartTime: A system clock of the start event
// - IsRunning: Flag to indicate if the simulation is running
// - SimRate: The multiplier rate at which the simulation is running
// - CurrSimTime: Current runtime in seconds

// Private:
// - AccumulatedTime: Record of previous time-chunks, between edits to state
// - ChunkStartTime: A system clock of the start of the current time-chunk
// - MarkedTime: A stored time value to reference an "elapsed seconds" value to.

// Uses UTC Time to avoid issues like daylight saving shifts.

using KoreCommon;

public class KoreSimTime
{
    public bool IsRunning { get; private set; } = false;
    public double SimRate { get; private set; } = 1.0;

    private DateTime StartTime = DateTime.MinValue; // Set to an obvious zero default value;
    private DateTime ChunkStartTime = DateTime.MinValue;
    private TimeSpan AccumulatedTime = TimeSpan.Zero;
    private double MarkedTime = 0;

    public double SimTime
    {
        get
        {
            if (IsRunning)
            {
                double CurrChunkTime = (DateTime.UtcNow - ChunkStartTime).TotalSeconds * SimRate;
                return AccumulatedTime.TotalSeconds + CurrChunkTime;
            }
            else
            {
                return AccumulatedTime.TotalSeconds;
            }
        }
        private set { } // This is a read-only property
    }

    public string SimTimeHMS
    {
        get
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(SimTime);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
        set
        {
            // We have a fixed accumulated time, and a start for the current rate. An act of setting the time
            // fixed the AccumulatedTime to the new value, and the ChunkStartTime to o for the current time.

            // Temp store the running state, so we don't disrupt that
            bool isRunning = IsRunning;

            if (isRunning) Stop();
            int seconds = KoreStringOps.TimeHmsToSeconds(value);

            KoreCentralLog.AddEntry($"SimTimeHMS: {value} -> {seconds} seconds");

            AccumulatedTime = TimeSpan.FromSeconds(seconds);
            if (isRunning) Start();
        }
    }

    // --------------------------------------------------------------------------------------------

    public KoreSimTime()
    {
        Reset();
    }

    // --------------------------------------------------------------------------------------------

    // This method now encapsulates what was previously intended for the SimRate setter.
    public void SetSimRate(double newSimRate)
    {
        // Only act if the value is different and within the desired bounds.
        newSimRate = KoreValueUtils.ClampD(newSimRate, 0.1, 10);
        if (!KoreValueUtils.EqualsWithinTolerance(newSimRate, SimRate))
        {
            // If running, "accumulate" the current chunk of time under the current simrate.
            if (IsRunning)
            {
                AccumulatedTime += (DateTime.UtcNow - ChunkStartTime) * SimRate;
                ChunkStartTime = DateTime.UtcNow;
            }

            // Directly update the simulation rate after clamping it within bounds.
            SimRate = newSimRate;
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Start()
    {
        if (!IsRunning)
        {
            StartTime = DateTime.UtcNow;
            ChunkStartTime = DateTime.UtcNow;
            IsRunning = true;
        }
    }

    public void Stop()
    {
        if (IsRunning)
        {
            AccumulatedTime += (DateTime.UtcNow - ChunkStartTime) * SimRate; // Correctly update accumulated time on stop
            IsRunning = false;
        }
    }

    public void Reset()
    {
        StartTime = DateTime.MinValue;
        ChunkStartTime = DateTime.MinValue;
        AccumulatedTime = TimeSpan.Zero;
        IsRunning = false;
        SetSimRate(1.0);
    }

    // --------------------------------------------------------------------------------------------

    public void MarkTime()
    {
        MarkedTime = SimTime;
    }
    public double ElapsedTimeSinceMark()
    {
        return SimTime - MarkedTime;
    }

    // --------------------------------------------------------------------------------------------
}
