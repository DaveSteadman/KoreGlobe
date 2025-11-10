// <fileheader>

using System;

namespace KoreCommon;

// KoreMovingDouble:
// - We have a current value, and a truth value.
// - We increment the current value towards the truth value over a number of steps,
//   to smooth out jumps, likely from maths and messaging update periods.
// - AKA - low-tech smoothing functionality

public class KoreMovingDouble
{
    public double CurrentValue { get; private set; }
    public double CommandedValue { get; private set; }
    public double MaxMovementPerTick { get; set; }

    public KoreMovingDouble(double initialValue, double maxMovementPerTick)
    {
        CurrentValue = initialValue;
        MaxMovementPerTick = maxMovementPerTick;
        CommandedValue = initialValue; // Start with commanded value equal to initial value
    }

    // Allows gradual movement towards the commanded value
    public void SetCommandedValue(double newCommandedValue)
    {
        CommandedValue = newCommandedValue;
    }

    // Instantly forces the CurrentValue to the CommandedValue
    public void ForceToCommandedValue()
    {
        CurrentValue = CommandedValue;
    }

    // Tick function that performs gradual movement towards the commanded value
    public void Tick()
    {
        // Calculate the difference between the commanded and current value
        double difference = CommandedValue - CurrentValue;

        // Determine the maximum allowed movement for this tick
        double movement = Math.Clamp(difference, -MaxMovementPerTick, MaxMovementPerTick);

        // Update the current value
        CurrentValue += movement;
    }
}


