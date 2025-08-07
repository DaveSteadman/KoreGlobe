using Godot;
using System;

public partial class KoreAppVersionLabel : Label
{
    public override void _Ready()
    {
        // Set the text to the current KoreApp version
        Text = KoreGlobals.VersionString;

        // Optionally, set the theme font size or other properties
        // ThemeFontSize = 16; // Example size, adjust as needed
    }
}
