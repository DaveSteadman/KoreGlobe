using System;
using System.Collections.Generic;

using Godot;
using KoreSim;
using KoreCommon;

#nullable enable

public partial class KoreUICLIWindow : Window
{
    // A "Close Me" state to indicate the window should be closed. This is done so the parent can control associated 
    // states, such as the launching button and the window reference, more easily.
    public bool ToClose { get; private set; } = false;
    
    // Controls
    private TextEdit? CommandResponseTextEdit;
    private LineEdit? CommandEntryEdit;

    // Command history
    private List<string> CommandHistory = new();
    private const int invalidIndex = -1;
    private int historyIndex = invalidIndex;

    // 1Hz processing, to slow down _Process
    private float CurrTimer = 0.0f;
    private float TimerInterval = 1.0f; // interval to poll for CLI output

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Fetch child nodes - from some place in the hierarchy below this Window
        CommandResponseTextEdit = (TextEdit)FindChild("CommandResponseTextEdit");
        CommandEntryEdit = (LineEdit)FindChild("CommandEntryEdit");

        // Validate child nodes
        if (CommandResponseTextEdit == null) { GD.PrintErr("CommandResponseTextEdit not found"); return; }
        if (CommandEntryEdit == null) { GD.PrintErr("CommandEntryEdit not found"); return; }

        // LineEdit actions - Commented out as currently call from _Input
        // CommandEntryEdit.Connect("text_submitted", new Callable(this, nameof(OnCommandSubmitted)));

        // Link up the X button to close the window
        Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        // poll for new CLI output
        if (CurrTimer < KoreCentralTime.RuntimeSecs)
        {
            CurrTimer += TimerInterval; // reset the timer when it expires

            // Check the command line for new output
            if (KoreSimFactory.Instance.ConsoleInterface.HasOutput())
            {
                string output = KoreSimFactory.Instance.ConsoleInterface.GetOutput();
                CommandResponseTextEdit!.Text += "\n" + output;

                // scroll the TextEdit to the bottom
                CommandResponseTextEdit.ScrollVertical = CommandResponseTextEdit.GetLineCount() - 1;
            }
        }
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

                // Check the string basic validity, then submit as next command
                if (!string.IsNullOrEmpty(txt))
                    OnCommandSubmitted(txt);

                CommandEntryEdit!.Text = ""; // Clear input after submission
                historyIndex = invalidIndex; // Reset history index
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
        
        // Emit signal before closing
        ToClose = true;
        
        // QueueFree(); // Close the window
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edit Actions
    // --------------------------------------------------------------------------------------------

    private void OnCommandSubmitted(string text)
    {
        GD.Print("Command submitted: " + text);
        CommandResponseTextEdit!.Text += "\nResponse: " + text;

        // scroll the TextEdit to the bottom
        CommandResponseTextEdit.ScrollVertical = CommandResponseTextEdit.GetLineCount() - 1;

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

        // Call the CLI to process the new command
        if (KoreSimFactory.Instance.ConsoleInterface == null)
        {
            GD.PrintErr("KoreUICLIWindow: ConsoleInterface is null, cannot add input.");
            return;
        }
        else
            KoreSimFactory.Instance.ConsoleInterface.AddInput(text);
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

        // Set the cursor position to the end of the text
        CallDeferred(nameof(SetCaretToEnd));

        GD.Print($"KoreUICLIWindow: Navigated history backward to index {historyIndex}, text: {CommandEntryEdit.Text}, column: {CommandEntryEdit.CaretColumn}");
    }

    // --------------------------------------------------------------------------------------------

    private void NavigateHistoryForward()
    {
        if (historyIndex == invalidIndex) return;

        if (historyIndex < CommandHistory.Count - 1)
        {
            historyIndex++;
            CommandEntryEdit!.Text = CommandHistory[historyIndex];

            // Set the cursor position to the end of the text
            CallDeferred(nameof(SetCaretToEnd));
        }
        else
        {
            historyIndex = invalidIndex;
            CommandEntryEdit!.Text = "";
        }
    }

    // --------------------------------------------------------------------------------------------

    // We put the "move cursor to end" call into its own deferred method, so it can be processed correctly.
    private void SetCaretToEnd()
    {
        CommandEntryEdit!.CaretColumn = CommandEntryEdit.Text.Length;
    }

}
