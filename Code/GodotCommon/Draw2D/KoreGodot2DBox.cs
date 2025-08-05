
using Godot;

public partial class KoreGodot2DBox : Node2D
{
    public Rect2 ScreenRect { get; set; } = new Rect2();
    public bool  IsValid    { get; set; } = false;

    public Color LineColor  { get; set; } = new Color(1, 1, 1, 1); // Default white color
    public float LineWidth  { get; set; } = 2.0f; // Default line width
    public bool  Filled     { get; set; } = false; // Default not filled

    // --------------------------------------------------------------------------------------------

    public override void _Draw()
    {
        if (IsValid)
        {
            DrawRect(ScreenRect, LineColor, filled: Filled, width: LineWidth);
        }
    }

    // --------------------------------------------------------------------------------------------
    
    // Trigger a redraw - including a deferred option for when not in the main thread
    public void Invalidate()
    {
        IsValid = false;
        QueueRedraw();
    }
    
    public void InvalidateDeferred()
    {
        CallDeferred(nameof(QueueRedraw));
    }
}
    