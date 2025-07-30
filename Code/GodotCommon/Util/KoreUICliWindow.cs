
using System;
using System.Timers;
using System.Text;
using System.Collections.Generic;

using Godot;

public partial class KoreUICliWindow : Window
{
    private Label CommandResponseLabel;
    private LineEdit CommandEntryEdit;

    private List<string> CommandHistory = new List<string>();

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Get references to the Label, LineEdit, and ScrollContainer

        GD.Print("KoreUICliWindow _Ready");

        AttachControls();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (CommandEntryEdit != null && CommandEntryEdit.HasFocus())
        {
            if (@event is InputEventKey keyEvent && keyEvent.Pressed)
            {
                if (keyEvent.Keycode == Key.Up)
                {
                    NavigateHistoryBackward();
                    GetViewport().SetInputAsHandled();

                }
                else if (keyEvent.Keycode == Key.Down)
                {
                    NavigateHistoryForward();
                    GetViewport().SetInputAsHandled();

                }
            }
        }
    }

    private void AttachControls()
    {
        // CommandResponseLabel = GetNode<Label>("CommandResponseLabel");
        // CommandEntryEdit = GetNode<LineEdit>("CommandEntryEdit");

        CommandResponseLabel = (Label)FindChild("CommandResponseLabel");
        CommandEntryEdit = (LineEdit)FindChild("CommandEntryEdit");

        if (CommandResponseLabel == null) GD.PrintErr("CommandResponseLabel not found!");
        if (CommandEntryEdit == null) GD.PrintErr("CommandEntryEdit not found!");

        CommandEntryEdit.KeepEditingOnTextSubmit = true;


        // Connect the LineEdit's text submitted signal to the command handler
        CommandEntryEdit.Connect("text_submitted", new Callable(this, "OnCommandSubmitted"));

        // Connect for the arrow keys to navigate command history
        CommandEntryEdit.Connect("focus_entered", new Callable(this, "OnFocusEntered"));
        CommandEntryEdit.Connect("text_changed", new Callable(this, "OnTextChanged"));

    }

    private void OnCommandSubmitted(string text)
    {
        GD.Print("Command submitted: " + text);
        CommandResponseLabel.Text += "\nResponse: " + text;

        CommandHistory.Add(text);
        CommandEntryEdit.Text = ""; // Clear the input field after submission

        // Reset to empty input state
        historyIndex = -1;

    }

    private int historyIndex = -1;  // -1 means current empty input state

    private void NavigateHistoryBackward()
    {
        if (CommandHistory.Count == 0) return;

        // Move to more recent command (or start from most recent if at empty state)
        if (historyIndex == -1)
            historyIndex = CommandHistory.Count - 1;  // Start with most recent
        else if (historyIndex > 0)
            historyIndex--;  // Move to more recent

        CommandEntryEdit.Text = CommandHistory[historyIndex];
        //CommandEntryEdit.CursorPosition = CommandEntryEdit.Text.Length;
    }

    private void NavigateHistoryForward()
    {
        if (historyIndex == -1) return;  // Already at empty state

        if (historyIndex < CommandHistory.Count - 1)
        {
            historyIndex++;  // Move to older command
            CommandEntryEdit.Text = CommandHistory[historyIndex];
        }
        else
        {
            historyIndex = -1;  // Return to empty state
            CommandEntryEdit.Text = "";
        }
        //CommandEntryEdit.CursorPosition = CommandEntryEdit.Text.Length;
    }
}