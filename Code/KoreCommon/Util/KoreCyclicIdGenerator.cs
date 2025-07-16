using System;

namespace KoreCommon;


public class KoreCyclicIdGenerator
{
    private readonly int _maxEntries;
    private int _currentIndex;

    public KoreCyclicIdGenerator(int maxEntries)
    {
        if (maxEntries <= 0)
        {
            throw new ArgumentException("Max entries must be greater than zero.", nameof(maxEntries));
        }

        _maxEntries = maxEntries;
        _currentIndex = -1; // Start at -1 so the first call to GetNext returns the 0th entry
    }

    public string NextId()
    {
        _currentIndex = (_currentIndex + 1) % _maxEntries;

        string id = $"ID{_currentIndex:D4}"; // ID0000, ID0001, ..., ID000N
        //KoreCentralLog.AddEntry($"KoreCyclicIdGenerator.NextId: {id}");

        return id;
    }
}