using Godot;
using System;

using KoreCommon;
using KoreSim;
//using KoreGodotCommon;

public partial class KoreAppVersionLabel : Label
{
    public override void _Ready()
    {
        // Set the text to the current KoreApp version
        //Text = $"{KoreAppConst.Version}\n{KoreCommonConst.Version}\n{KoreSimConst.Version}\n{KoreGodotCommonConst.Version}";

        Text = $"{KoreAppConst.Version}";
        Text += $"\n{KoreGodotCommonConst.Version}";
        Text += $"\n{KoreSimConst.Version}";
        Text += $"\n{KoreCommonConst.Version}";



        // Optionally, set the theme font size or other properties
        // ThemeFontSize = 16; // Example size, adjust as needed
    }
}
