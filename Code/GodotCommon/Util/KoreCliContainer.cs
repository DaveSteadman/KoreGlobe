using Godot;
using System;
using System.Collections.Generic;

public partial class KoreCliContainer : MarginContainer
{
    private Label CommandResponseLabel;
    private LineEdit CommandEntryEdit;
    private Window? ParentWindow = null;

    private List<string> CommandHistory = new();
    private const int invalidIndex = -1;
    private int historyIndex = invalidIndex;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Fetch child nodes
        CommandResponseLabel = GetNode<Label>("VBoxContainer/CommandResponseLabel");
        CommandEntryEdit     = GetNode<LineEdit>("VBoxContainer/CommandEntryEdit");

        if (CommandResponseLabel == null || CommandEntryEdit == null)
        {
            GD.PrintErr("KoreCliContainer: Failed to find child nodes.");
            return;
        }

        // LineEdit settings
        CommandEntryEdit.KeepEditingOnTextSubmit = true;
        // CommandEntryEdit.GrabFocus();

        // Signal connections
        // CommandEntryEdit.Connect("text_submitted", new Callable(this, nameof(OnCommandSubmitted)));
        // CommandEntryEdit.Connect("text_changed", new Callable(this, nameof(OnTextChanged)));
        
        CommandEntryEdit.Connect("focus_entered", new Callable(this, nameof(OnFocusEntered)));
        CommandEntryEdit.Connect("focus_exited", new Callable(this, nameof(OnFocusExited)));

        // get the parent window node
        ParentWindow = GetParent<Window>();
        if (ParentWindow != null)
        {
            ParentWindow.Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));
        }

        // Ensure this container can receive focus
        SetFocusMode(Control.FocusModeEnum.All);

        GD.Print("KoreCliContainer _Ready - CLI container initialized");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Input 
    // --------------------------------------------------------------------------------------------

    // public override void _Input(InputEvent @event)
    // {
    //     if (@event is InputEventKey keyEvent && keyEvent.Pressed)
    //     {
    //         GD.Print($"KoreCliContainer: _Input Key pressed: {keyEvent.Keycode} (Ctrl: {keyEvent.CtrlPressed}, Shift: {keyEvent.ShiftPressed})");
    //         //GetViewport().SetInputAsHandled();
    //     }
    //     base._Input(@event);
    // }

    // public override void _GuiInput(InputEvent @event)
    // {
    //     if (@event is InputEventKey keyEvent && keyEvent.Pressed)
    //     {
    //         // Only intercept input when LineEdit has focus
    //         // if (CommandEntryEdit.HasFocus())
    //         // {

    //         // Print keystroke for debugging
    //         GD.Print($"KoreCliContainer: _GuiInput Key pressed: {keyEvent.Keycode} (Ctrl: {keyEvent.CtrlPressed}, Shift: {keyEvent.ShiftPressed})");

    //         // Handle history navigation with arrow keys (or Ctrl+Up/Down)
    //         if (keyEvent.Keycode == Key.Up || (keyEvent.CtrlPressed && keyEvent.Keycode == Key.Up))
    //         {
    //             NavigateHistoryBackward();
    //             GetViewport().SetInputAsHandled();
    //             return;
    //         }
    //         else if (keyEvent.Keycode == Key.Down || (keyEvent.CtrlPressed && keyEvent.Keycode == Key.Down))
    //         {
    //             NavigateHistoryForward();
    //             GetViewport().SetInputAsHandled();
    //             return;
    //         }

    //         // ✅ Consume ALL keyboard events to prevent propagation to other scenes
    //         GetViewport().SetInputAsHandled();
    //         // }
    //     }

    //     //base._GuiInput(@event); // Let normal GUI input proceed
    // }

    // // Unhandled input processing
    // public override void _UnhandledInput(InputEvent @event)
    // {
    //     // Additional safety net: consume any unhandled key input when CLI has focus
    //     if (@event is InputEventKey keyEvent && keyEvent.Pressed)
    //     {
    //         GD.Print($"KoreCliContainer: _UnhandledInput: {keyEvent.Keycode}");
    //         GetViewport().SetInputAsHandled();
    //     }
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Actions
    // --------------------------------------------------------------------------------------------

    private void OnCommandSubmitted(string text)
    {
        GD.Print("Command submitted: " + text);
        CommandResponseLabel.Text += "\nResponse: " + text;

        if (!string.IsNullOrWhiteSpace(text))
        {
            // Remove duplicates, then append
            CommandHistory.RemoveAll(entry => entry == text);
            CommandHistory.Add(text);
        }

        CommandEntryEdit.Text = "";
        historyIndex = invalidIndex;

        // Keep history size capped
        if (CommandHistory.Count > 100)
            CommandHistory.RemoveRange(0, CommandHistory.Count - 100);
    }

    private void NavigateHistoryBackward()
    {
        if (CommandHistory.Count == 0) return;

        if (historyIndex == invalidIndex)
            historyIndex = CommandHistory.Count - 1;
        else if (historyIndex > 0)
            historyIndex--;

        CommandEntryEdit.Text = CommandHistory[historyIndex];
    }

    private void NavigateHistoryForward()
    {
        if (historyIndex == invalidIndex) return;

        if (historyIndex < CommandHistory.Count - 1)
        {
            historyIndex++;
            CommandEntryEdit.Text = CommandHistory[historyIndex];
        }
        else
        {
            historyIndex = invalidIndex;
            CommandEntryEdit.Text = "";
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Event Handlers
    // --------------------------------------------------------------------------------------------

    private void OnFocusEntered()
    {
        GD.Print("KoreCliContainer: LineEdit gained focus");
    }

    private void OnFocusExited()
    {
        GD.Print("KoreCliContainer: LineEdit lost focus");
        // Optionally clear history index when focus is lost

    }




    private void OnTextChanged(string newText)
    {
        // Reset history index when user types manually
        if (historyIndex != invalidIndex)
        {
            historyIndex = invalidIndex;
        }
    }
    
    private void OnCloseRequested()
    {
        GD.Print("KoreCliContainer: Close requested");
        ParentWindow?.QueueFree(); // This will close and destroy the CLI container
    }
}
