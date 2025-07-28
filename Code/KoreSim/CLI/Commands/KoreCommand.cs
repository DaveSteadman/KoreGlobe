using System;
using System.Collections.Generic;

// KoreCommand: The console can execute commands. This base class defines a signature for each one with some functionality
// around that, and then a main execute command. Child classes may also need to access additional resouces in order
// to perform their tasks, these will be added to their specific constructor when they are added to the command list.

public abstract class KoreCommand
{
    protected List<string> Signature { get; set; } = new List<string>();
    public string SignatureString { get => string.Join(" ", Signature); }

    // Virtual property to provide help text - and the virtual keyword allows us room to add parameters.
    public virtual string HelpString => SignatureString;

    // Public property to expose the count of signature parameters
    public int SignatureCount => Signature.Count;

    public abstract string Execute(List<string> parameters);

    // Look through the signature and check it matches the leading terms of the input line.
    public bool Matches(List<string> inputParts)
    {
        if (Signature.Count > inputParts.Count) return false;
        for (int i = 0; i < Signature.Count; i++)
        {
            if (!string.Equals(Signature[i], inputParts[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }
}
