using System;
using System.Collections.Generic;

#nullable enable

// ------------------------------------------------------------------------------------------------

public class KoreScorableObject : IComparable<KoreScorableObject>
{
    public float Score { get; set; } = 0; // higher is better

    private bool AboveThreshold { get; set; } = false;
    private bool IsDirty { get; set; } = false;

    // Scoring - set values
    public void SetScore(float score)
    {
        Score = score;
        IsDirty = true;
    }
    
    // Marking - assessing the score
    public void Mark(bool newValue)
    {
        if (AboveThreshold != newValue)
        {
            AboveThreshold = newValue;
            IsDirty = true;
        }
    }

    // Consuming - checking the mark and clearing the dirty flag
    public bool IsMarked() { return AboveThreshold; }
    public void ClearDirty() { IsDirty = false; }


    // Utility functions
    
    // Implement IComparable for ranking (higher scores first)
    public int CompareTo(KoreScorableObject? other)
    {
        if (other == null) return 1; // This object is greater than null

        // Reverse comparison so higher scores come first
        return other.Score.CompareTo(this.Score);
    }
}

// ------------------------------------------------------------------------------------------------

public class KoreScorableObjectManager
{
    public List<KoreScorableObject> Objects { get; } = new List<KoreScorableObject>();

    // --------------------------------------------------------------------------------------------
    // MARK: Create Delete
    // --------------------------------------------------------------------------------------------

    public KoreScorableObject AllocateNewObject()
    {
        var obj = new KoreScorableObject();
        Objects.Add(obj);
        return obj;
    }

    public void RemoveObject(KoreScorableObject obj)
    {
        Objects.Remove(obj);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mark
    // --------------------------------------------------------------------------------------------

    public void MarkCount(int numberToMark)
    {
        // First handle some basic cases
        if (Objects.Count == 0) return;

        // If the number to mark is greater than or equal to the total, mark all
        if (numberToMark >= Objects.Count)
        {
            // Mark all as above threshold
            foreach (var obj in Objects)
                obj.Mark(true);
            return;
        }

        // Sort by score (highest first)
        Objects.Sort();

        // Mark the top N objects as above threshold
        int markedCount = numberToMark;
        bool newMark = true;
        foreach (KoreScorableObject obj in Objects)
        {
            // Count down the number of marks to apply, then set the result to false
            if (markedCount <= 0) newMark = false;
            markedCount--;
            obj.Mark(newMark);
        }
    }

    public void MarkThreshold(float threshold)
    {
        // First handle some basic cases
        if (threshold <= 0 || Objects.Count == 0) return;

        // Sort by score (highest first)
        Objects.Sort();

        // Mark objects based on threshold - all objects, some may previously have scored above threshold
        foreach (var obj in Objects)
            obj.Mark(obj.Score >= threshold);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utility
    // --------------------------------------------------------------------------------------------

    // Add this method for reusing the same objects across frames
    public void ClearAllMarks()
    {
        foreach (var obj in Objects)
            obj.Mark(false); // This will set IsDirty if changing from true to false
    }
    
    public void ClearAllDirtyFlags()
    {
        foreach (var obj in Objects)
            obj.ClearDirty();
    }
    
}

// ------------------------------------------------------------------------------------------------

