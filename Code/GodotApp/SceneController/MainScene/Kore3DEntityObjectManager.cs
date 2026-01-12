using Godot;
using System;
using System.Runtime;

using KoreCommon;
using KoreSim;
using System.Collections.Generic;

#nullable enable

// Kore3DEntityObjectManager:
// - Adds and manages entity-related nodes in the 3D scene.
//     - They move themselves as they look up their positions
//     - They delete themselves when they can't find their entities.

public class Kore3DEntityObjectManager
{
    public Node3D? LocalZeroNode = null;

    // ---------------------------------------------------------------------------------------------

    public void CreateNodes(Node3D zn)
    {
        LocalZeroNode = zn;
    }

    // ---------------------------------------------------------------------------------------------

    public void UpdateNodes()
    {
        // Are we in an update cycle?
        if (!KoreMovingOrigin.IsChangePeriod()) return;

        // calc and place the nodes
        PlaceNodes();
    }

    public void PlaceNodes()
    {
    }


}


