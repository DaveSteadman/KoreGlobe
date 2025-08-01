using System;
using System.Collections.Generic;

using Godot;

#nullable enable

public partial class KoreUICLIWindow : Window
{
    // Controls
    private Label? CommandResponseLabel;
    private LineEdit? CommandEntryEdit;

    // Command history
    private List<string> CommandHistory = new();
    private const int invalidIndex = -1;
    private int historyIndex = invalidIndex;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Fetch child nodes - from some place in the hierarchy below this Window
        CommandResponseLabel = (Label)FindChild("CommandResponseLabel");
        CommandEntryEdit = (LineEdit)FindChild("CommandEntryEdit");

        // Validate child nodes 
        if (CommandResponseLabel == null) { GD.PrintErr("CommandResponseLabel not found"); return; }
        if (CommandEntryEdit == null) { GD.PrintErr("CommandEntryEdit not found"); return; }

        // LineEdit actions - Commented out as currently call from _Input
        // CommandEntryEdit.Connect("text_submitted", new Callable(this, nameof(OnCommandSubmitted)));

        // Link up the X button to close the window
        Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));
    }

    // --------------------------------------------------------------------------------------------

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            // Print keystroke for debugging
            string currTxt = "null";
            if (CommandEntryEdit != null)
                currTxt = CommandEntryEdit.Text;
            GD.Print($"KoreUICLIWindow _Input // {keyEvent.Keycode} Ctrl:{keyEvent.CtrlPressed} Shift:{keyEvent.ShiftPressed} // Text:{currTxt}");

            // Handle history navigation with arrow keys (or Ctrl+Up/Down)
            if (keyEvent.Keycode == Key.Up)
            {
                NavigateHistoryBackward();
                return;
            }
            else if (keyEvent.Keycode == Key.Down)
            {
                NavigateHistoryForward();
                return;
            }
            
            // if ENTER is pressed, submit the command
            if (keyEvent.Keycode == Key.Enter)
            {
                string txt = CommandEntryEdit!.Text;
                
                // Simulate text submission
                OnCommandSubmitted(txt);
                CommandEntryEdit!.Text = ""; // Clear input after submission
                historyIndex = invalidIndex; // Reset history index
                //GetViewport().SetInputAsHandled();
                return;
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Window Actions
    // --------------------------------------------------------------------------------------------

    private void OnCloseRequested()
    {
        GD.Print("KoreUICLIWindow: Close requested");
        // Optionally, you can save state or perform cleanup here
        QueueFree(); // Close the window
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edit Actions
    // --------------------------------------------------------------------------------------------

    private void OnCommandSubmitted(string text)
    {
        GD.Print("Command submitted: " + text);
        CommandResponseLabel!.Text += "\nResponse: " + text;

        // Add to history if not empty input
        if (!string.IsNullOrWhiteSpace(text))
        {
            // Remove duplicates, then append
            CommandHistory.RemoveAll(entry => entry == text);
            CommandHistory.Add(text);
        }

        // Clear the command entry for the next input
        CommandEntryEdit!.Text = "";
        historyIndex = invalidIndex;
    
        // Keep history size capped
        if (CommandHistory.Count > 100)
            CommandHistory.RemoveRange(0, CommandHistory.Count - 100);
    }

    // --------------------------------------------------------------------------------------------

    private void NavigateHistoryBackward()
    {
        if (CommandHistory.Count == 0) return;

        if (historyIndex == invalidIndex)
            historyIndex = CommandHistory.Count - 1;
        else if (historyIndex > 0)
            historyIndex--;

        CommandEntryEdit!.Text = CommandHistory[historyIndex];
    }

    // --------------------------------------------------------------------------------------------

    private void NavigateHistoryForward()
    {
        if (historyIndex == invalidIndex) return;

        if (historyIndex < CommandHistory.Count - 1)
        {
            historyIndex++;
            CommandEntryEdit!.Text = CommandHistory[historyIndex];
        }
        else
        {
            historyIndex = invalidIndex;
            CommandEntryEdit!.Text = "";
        }
    }


}
