using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using KoreCommon;
using KoreSim;
using System.CodeDom;

#nullable enable

public partial class KoreEntityWindow : Window
{
    // Controls
    public Tree? EntityTreeList = null;
    private Button? CloseButton = null;
    private KoreEntityWindowPositionTab? PositionTab = null;


    // UI Timers
    private float UITimer = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms

    private float UISlowTimer = 0.0f;
    private float UISlowTimerInterval = 2.5f;

    public bool ToClose { get; set; } = false;

    // selected item
    private string? SelectedEntityName { get; set; } = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("ModelEditWindow _Ready");

        // var sv = GetNode<SubViewport>("SubViewport");
        // sv.OwnWorld3D = true;
        // sv.World3D = new World3D();
        //WindowMeshData = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.DefaultMaterial);
        // KoreMeshDataEditOps.IsolateAllTriangles(WindowMeshData);
        // KoreMeshDataEditOps.CalcNormalsForAllTriangles();

        AttachControls();
    }

    public override void _Process(double delta)
    {
        if (KoreCentralTime.CheckTimer(ref UITimer, UITimerInterval))
        {
            // UpdateUI();
        }

        if (KoreCentralTime.CheckTimer(ref UISlowTimer, UISlowTimerInterval))
        {
            UpdateTreeView();
        }
    }




    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void AttachControls()
    {
        EntityTreeList = (Tree)FindChild("EntityTreeList");
        CloseButton = (Button)FindChild("CloseButton");
        PositionTab = (KoreEntityWindowPositionTab)FindChild("Position");

        if (EntityTreeList == null) GD.PrintErr("ModelEditWindow: EntityTreeList node not found.");
        if (CloseButton == null) GD.PrintErr("ModelEditWindow: CloseButton node not found.");
        if (PositionTab == null) GD.PrintErr("ModelEditWindow: PositionTab node not found.");

        // Connect tree selection events
        if (EntityTreeList != null)
        {
            EntityTreeList.Connect("item_selected", new Callable(this, nameof(OnTreeItemSelected)));
            EntityTreeList.Connect("item_activated", new Callable(this, nameof(OnTreeItemActivated))); // Double-click
        }

        // Link up the X button to close the window
        Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));
    }

    private void OnCloseRequested()
    {
        GD.Print("KoreSandbox3DWindow: Close button pressed");
        ToClose = true;
        // QueueFree();
    }

    private void OnTreeItemSelected()
    {
        string? selectedEntity = GetSelectedEntityName();
        if (selectedEntity != null)
        {
            GD.Print($"Entity selected: {selectedEntity}");
            // TODO: Handle entity selection (show details, etc.)


        }

        // Assign the value, even if it is null (as that clears a selection)
        if (PositionTab != null)
        {
            PositionTab.SelectedEntityName = selectedEntity!;
            PositionTab.UpdateUIValues();
        }
    }

    private void OnTreeItemActivated()
    {
        string? selectedEntity = GetSelectedEntityName();
        if (selectedEntity != null)
        {
            GD.Print($"Entity activated: {selectedEntity}");
            // TODO: Handle entity activation (focus camera, edit mode, etc.)
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Data
    // --------------------------------------------------------------------------------------------

    // Get the currently selected entity name, or null if none selected
    public string? GetSelectedEntityName()
    {
        if (EntityTreeList == null) return null;

        TreeItem? selected = EntityTreeList.GetSelected();
        if (selected == null) return null;

        // Don't return the root item name
        string name = selected.GetText(0);
        if (name == "Entities") return null;

        return name;
    }

    // UpdateTreeView
    // - endeavours to maintain any selected item by adding and removing changes without
    //   disrupting unchanging items

    private void UpdateTreeView()
    {
        if (EntityTreeList == null) return;

        var currentEntityNames = KoreEventDriver.EntityNameList().ToHashSet();

        // Get or create root
        TreeItem? root = EntityTreeList.GetRoot();
        if (root == null)
        {
            root = EntityTreeList.CreateItem();
            root.SetText(0, "Entities");
            root.SetCollapsed(false);
        }

        // Collect existing items
        var existingItems = new Dictionary<string, TreeItem>();
        TreeItem? child = root.GetFirstChild();
        while (child != null)
        {
            string name = child.GetText(0);
            existingItems[name] = child;
            child = child.GetNext();
        }

        // Remove items that no longer exist
        var itemsToRemove = new List<TreeItem>();
        foreach (var kvp in existingItems)
        {
            if (!currentEntityNames.Contains(kvp.Key))
            {
                itemsToRemove.Add(kvp.Value);
            }
        }

        foreach (var item in itemsToRemove)
        {
            item.Free(); // Remove from tree
        }

        // Add new items
        foreach (var name in currentEntityNames)
        {
            if (!existingItems.ContainsKey(name))
            {
                TreeItem item = root.CreateChild();
                item.SetText(0, name);
            }
        }

    }

}
