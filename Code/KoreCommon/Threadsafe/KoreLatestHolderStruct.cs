// KoreLatestHolderClass:
// - A simple thread-safe holder for a single value of type T. It uses a lock to ensure that the latest value can be safely updated and read from multiple threads.
// - Use Case: The background thread updates the value, and the main thread gets the latest value regardless of when it was last updated.

namespace KoreCommon;

using System;
using System.Threading;

#nullable enable

public class KoreLatestHolderStruct<T>
{
    private T _latestValue;
    private readonly object _lock = new object();

    // --------------------------------------------------------------------------------------------

    public KoreLatestHolderStruct(T initialValue)
    {
        _latestValue = initialValue;
    }

    // --------------------------------------------------------------------------------------------

    public T LatestValue
    {
        get
        {
            lock (_lock)
            {
                return _latestValue;
            }
        }
        set
        {
            lock (_lock)
            {
                _latestValue = value;
            }
        }
    }
}

