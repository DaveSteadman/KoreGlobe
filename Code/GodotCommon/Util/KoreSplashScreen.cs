// KoreSplashScreen.cs
using Godot;
using KoreCommon;
using System;

#nullable enable

public partial class KoreSplashScreen : CanvasLayer
{
    Control? _visualRoot;
    public override void _Ready()
    {
        _visualRoot = GetNode<Control>("VisualRoot");
        if (_visualRoot == null)
        {
            GD.PrintErr("KoreSplashScreen: VisualRoot node not found.");
            return;
        }

        // _visualRoot.AnchorLeft = -0.5f;
        // _visualRoot.AnchorTop = 0.5f;
        // _visualRoot.AnchorRight = 0.5f;
        // _visualRoot.AnchorBottom = 0.5f;

        // _visualRoot.OffsetLeft = -_visualRoot.Size.X / 2f;
        // _visualRoot.OffsetTop = -_visualRoot.Size.Y / 2f;
        // _visualRoot.OffsetRight = _visualRoot.Size.X / 2f;
        // _visualRoot.OffsetBottom = _visualRoot.Size.Y / 2f;
    }

    public override void _PhysicsProcess(double delta)
    {
        const float startTime = 0.8f;
        const float animDuration = 0.4f;
        const float endTime = startTime + animDuration;

        if (KoreCentralTime.RuntimeSecs > startTime)
        {
            float animPos = KoreCentralTime.RuntimeSecs - startTime;
            float animAlpha = KoreValueUtils.ScaleVal(animPos, 0, animDuration, 1, 0);
            float animScale = KoreValueUtils.ScaleVal(animPos, 0, animDuration, 1, 3);

            if (_visualRoot != null)
            {
                _visualRoot.Modulate = new Color(1, 1, 1, animAlpha);
                _visualRoot.Scale = new Vector2(animScale, animScale);

                // Get the _visualroot size and reposition it to center
                Vector2 size = _visualRoot.Size;
                Vector2 screenCenter = GetViewport().GetVisibleRect().Size / 2f;
                _visualRoot.Position = screenCenter - (size * _visualRoot.Scale / 2f);


            }
        }

        if (KoreCentralTime.RuntimeSecs > endTime)
        {
            GD.Print("KoreSplashScreen.PhysicsProcess: RuntimeIntSecs > 2, removing splash screen.");
            // Do something after 3 seconds
            QueueFree(); // Remove the splash screen
        }
    }

}