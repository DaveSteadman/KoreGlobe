using Godot;
using System;

public partial class CommandEntryEdit : LineEdit
{
    public override void _Ready()
    {
        GD.Print("CommandEntryEdit _Ready");

        // Set up the LineEdit properties
        this.KeepEditingOnTextSubmit = true;
        this.GrabFocus();

        // Connect signals if needed
        this.Connect("text_submitted", new Callable(this, nameof(OnTextSubmitted)));
    }
    private void OnTextSubmitted(string text)
    {
        GD.Print($"CommandEntryEdit: Text submitted: {text}");
        // Handle the submitted text here, e.g., send it to a command processor
    }


    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            GD.Print($"CommandEntryEdit: _Input Key pressed: {keyEvent.Keycode} (Ctrl: {keyEvent.CtrlPressed}, Shift: {keyEvent.ShiftPressed})");
            GetViewport().SetInputAsHandled();
        }
    }

}
