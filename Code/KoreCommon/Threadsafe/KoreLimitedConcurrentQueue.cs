// <fileheader>

using System;
using System.Collections.Generic;

// KoreLimitedConcurrentQueue Class to store new values up to a set capacity, then remove oldest element on the arrival of new ones.
// Can return the list population count, list and latest values.

namespace KoreCommon;


public class KoreLimitedConcurrentQueue<T>
{
    private readonly LinkedList<T> List = new LinkedList<T>();
    private readonly object Lock = new object();
    private readonly int Capacity;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreLimitedConcurrentQueue(int capacity)
    {
        if (capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
        Capacity = capacity;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Setter
    // --------------------------------------------------------------------------------------------

    public void Enqueue(T item)
    {
        lock (Lock)
        {
            List.AddLast(item);
            if (List.Count > Capacity)
            {
                List.RemoveFirst();
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Accessors
    // --------------------------------------------------------------------------------------------

    public int Count
    {
        get
        {
            lock (Lock)
            {
                return List.Count;
            }
        }
    }

    public List<T> ToList()
    {
        lock (Lock)
        {
            return new List<T>(List);
        }
    }

    public T GetMostRecent()
    {
        lock (Lock)
        {
            if (List.Last == null) throw new InvalidOperationException("The queue is empty.");
            return List.Last.Value;
        }
    }

    public void Clear()
    {
        lock (Lock)
        {
            List.Clear();
        }
    }
}
