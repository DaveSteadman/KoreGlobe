using Godot;
using KoreCommon;

#nullable enable

public partial class KoreViewportAutosize : SubViewport
{
    [Export]
    public float ViewportCustom3DScale { get; set; } = 0.2f;

    private float ProcessTimer = 0.0f;
    private float ProcessTimerInterval = 0.1f;

    public override void _Ready()
    {
        // Do an initial sync
        SyncToParent();
    }

    public override void _Process(double delta)
    {
        if (KoreCentralTime.CheckTimer(ref ProcessTimer, ProcessTimerInterval))
        {
            // Sync the viewport size to the parent control
            SyncToParent();
        }
    }

    private void SyncToParent()
    {
        if (GetParent() is Control parentCtrl)
        {
            Vector2 size = parentCtrl.Size;
            Vector2I newSize = new Vector2I((int)size.X, (int)size.Y);

            if (Size != newSize)
                Size = newSize;
        }

        ViewportCustom3DScale = KoreValueUtils.Clamp(ViewportCustom3DScale, 0.2f, 1.0f);
        Scaling3DScale = ViewportCustom3DScale;
    }
}