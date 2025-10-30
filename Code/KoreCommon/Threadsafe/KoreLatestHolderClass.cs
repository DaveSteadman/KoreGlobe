// <fileheader>

// KoreLatestHolderClass:
// - A simple thread-safe holder for a single value of type T. It uses a lock to ensure that the latest value can be safely updated and read from multiple threads.
// - Use Case: The background thread updates the value, and the main thread gets the latest value regardless of when it was last updated.

namespace KoreCommon;

using System;
using System.Threading;

#nullable enable

public class KoreLatestHolderClass<T> where T : class
{
    private T? _latestValue;

    public KoreLatestHolderClass(T? initialValue = null)
    {
        _latestValue = initialValue;
    }

    public T? LatestValue
    {
        get => Interlocked.CompareExchange(ref _latestValue, null, null);
        set => Interlocked.Exchange(ref _latestValue, value);
    }
    
    public bool HasValue => _latestValue != null;
}

