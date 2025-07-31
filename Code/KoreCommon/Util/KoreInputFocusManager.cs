using Godot;

namespace KoreCommon;

/// <summary>
/// Global utility to check if any text input control has focus.
/// Use this in camera controls and other systems to avoid conflicts with text input.
/// </summary>
public static class KoreInputFocusManager
{
    /// <summary>
    /// Check if any text input control currently has focus in the scene tree.
    /// Returns true if keyboard input should be blocked from other systems.
    /// </summary>
    public static bool IsTextInputFocused(SceneTree sceneTree)
    {
        if (sceneTree?.CurrentScene == null)
            return false;

        // Get the current focused control
        var focusOwner = sceneTree.Root.GuiGetFocusOwner();
        
        if (focusOwner == null)
            return false;

        // Check if the focused control is a text input type
        return IsTextInputControl(focusOwner);
    }

    /// <summary>
    /// Check if a specific control is a text input control that should block keyboard shortcuts.
    /// </summary>
    private static bool IsTextInputControl(Control control)
    {
        // Check for common text input controls
        return control is LineEdit ||
               control is TextEdit ||
               control is CodeEdit ||
               // Add other text input types as needed
               control.GetType().Name.Contains("Edit") ||
               control.GetType().Name.Contains("Input");
    }

    /// <summary>
    /// Convenience method for camera controllers and other systems.
    /// Call this before processing keyboard input to avoid conflicts.
    /// </summary>
    public static bool ShouldBlockKeyboardInput(Node node)
    {
        return IsTextInputFocused(node.GetTree());
    }
}
