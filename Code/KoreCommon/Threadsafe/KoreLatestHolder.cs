// KoreLatestHolder:
// - A simple thread-safe holder for a single value of type T. It uses a lock to ensure that the latest value can be safely updated and read from multiple threads.
// - Use Case: The background thread updates the value, and the main thread gets the latest value regardless of when it was last updated.

namespace KoreCommon;

using System;
using System.Threading;

public class KoreLatestHolder<T> where T : class
{
    private T _latestValue;

    public KoreLatestHolder(T initialValue)
    {
        _latestValue = initialValue ?? throw new ArgumentNullException(nameof(initialValue));
    }

    public KoreLatestHolder()
    {
        throw new InvalidOperationException("Default constructor is not supported without an initial value.");
    }

    public T LatestValue
    {
        get => Interlocked.CompareExchange(ref _latestValue, null, null);
        set => Interlocked.Exchange(ref _latestValue, value);
    }
}

