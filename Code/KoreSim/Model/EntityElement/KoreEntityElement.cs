using System;

// KoreEntityElement: An item added to a Entity that has some amount of additinoal functionality to be cmodelled and/or displayed.

using KoreCommon;

public class KoreEntityElement
{
    public string Name { set; get; } = "Unnamed";

    // Overridable type for the element
    public virtual string Type { set; get; } = "Unknown";

    public bool Enabled { get; set; } = false;

    public KoreStringDictionary Properties { get; set; } = new KoreStringDictionary();

    // A virtual functino for all element child classes to output a one-line report of their contents.
    public virtual string Report()
    {
        return $"Element: {Name} ({Type})";
    }
}
