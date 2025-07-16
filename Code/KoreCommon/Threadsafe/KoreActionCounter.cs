using System;
using System.Threading;

namespace KoreCommon;


/*
KoreActionCounter: An approach that uses atomic operations with Interlocked to ensure thread safety
without a full lock. In this example, the TryConsume method uses a CAS (compare-and-swap) loop to
safely decrement the counter only if it’s positive, and Refresh atomically sets the new total

TryConsume: The method reads the current count, and if it's greater than zero, attempts to decrement
it. The CAS loop ensures that if another thread updates the count simultaneously, it will retry until
it succeeds.

Refresh: This method atomically sets the counter to a new value using Interlocked.Exchange, ensuring
any ongoing operations see a consistent update.

This design provides a simple, thread-safe counter suitable for use cases like token-based load balancing.
*/

public class KoreActionCounter
{
    private int _count;

    public KoreActionCounter(int initialCount)
    {
        _count = initialCount;
    }

    // Tries to decrement the counter. Returns true if a token was available and consumed.
    public bool TryConsume()
    {
        int initialValue, newValue;
        do
        {
            initialValue = _count;
            if (initialValue <= 0)
                return false;
            newValue = initialValue - 1;
        }
        while (Interlocked.CompareExchange(ref _count, newValue, initialValue) != initialValue);

        return true;
    }

    // Refresh operation: sets the counter back to a new total.
    public void Refresh(int newTotal)
    {
        Interlocked.Exchange(ref _count, newTotal);
    }

    // Optionally expose the current count.
    public int CurrentCount => Volatile.Read(ref _count);
}
