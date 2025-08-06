using Godot;
using System;
using System.Collections.Generic;

#nullable enable

public partial class KoreGodot2DCanvas : CanvasLayer
{
    private readonly Dictionary<string, KoreGodot2DBox> _boxMap = new();

    public void AddBox(string name, Rect2 rect, Color color, float lineWidth = 2.0f, bool filled = false)
    {
        if (_boxMap.ContainsKey(name))
            return; // Already exists

        KoreGodot2DBox box = new KoreGodot2DBox
        {
            Name = name,
            ScreenRect = rect,
            LineColor = color,
            LineWidth = lineWidth,
            Filled = filled
        };

        _boxMap[name] = box;
        AddChild(box);
        box.Invalidate();
    }

    public void UpdateBox(string name, Rect2 rect, Color color, float lineWidth = 2.0f, bool filled = false)
    {
        if (_boxMap.TryGetValue(name, out KoreGodot2DBox? box))
        {
            if (box.ScreenRect == rect &&
                box.LineColor == color &&
                box.LineWidth == lineWidth &&
                box.Filled == filled)
                return;

            box.ScreenRect = rect;
            box.LineColor = color;
            box.LineWidth = lineWidth;
            box.Filled = filled;
            box.Invalidate();
        }
        else
        {
            // If it doesn't exist, create a new one
            AddBox(name, rect, color, lineWidth, filled);
        }
    }

    public void DelBox(string name)
    {
        if (_boxMap.TryGetValue(name, out KoreGodot2DBox? box))
        {
            _boxMap.Remove(name);
            box.QueueFree();
        }
    }

    public void ClearBoxes()
    {
        foreach (var box in _boxMap.Values)
            box.QueueFree();

        _boxMap.Clear();
    }
}
