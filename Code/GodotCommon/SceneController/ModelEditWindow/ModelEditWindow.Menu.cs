using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

// Structure of the flags, set/get by the menu items, and queried in an update poll to show/hide 3D elements.
public struct KoreViewSelection
{
    public bool ShowPoints { get; set; }
    public bool ShowLines { get; set; }
    public bool ShowNormals { get; set; }
    public bool ShowFaces { get; set; }
}

public partial class ModelEditWindow : Window
{
    // Menu item IDs
    private enum MenuId
    {
        // File menu
        FileNew = 1,
        FileOpen = 2,
        FileExit = 99,


        // Edit menu (for future expansion)
        EditUndo = 10,
        EditRedo = 11,
        EditCut = 12,
        EditCopy = 13,
        EditPaste = 14,

        EditFlipTriangles = 1001,
        EditFlipUV_Vertical = 1002,
        EditFlipUV_Horizontal = 1003,

        ImportObjMtl = 2001,

        ViewPoints = 3001,
        ViewLines = 3002,
        ViewNormals = 3003,
        ViewFaces = 3004,


        // View menu (for future expansion)  
        ViewZoomIn = 20,
        ViewZoomOut = 21,
        ViewResetView = 22
    }

    KoreViewSelection ViewSelection = new KoreViewSelection()
    {
        ShowPoints = true,
        ShowLines = true,
        ShowNormals = true,
        ShowFaces = true
    };

    private void PopulateMenuBar()
    {
        if (ModelMenuBar == null) { GD.PrintErr("ModelEditWindow: ModelMenuBar null"); return; }

        PopupMenu _importMenu = new PopupMenu();
        _importMenu.Name = "Import";
        _importMenu.AddItem("Import OBJ/MTL", (int)MenuId.ImportObjMtl);

        PopupMenu _editMenu = new PopupMenu();
        _editMenu.Name = "Edit";
        _editMenu.AddItem("Flip Triangles", (int)MenuId.EditFlipTriangles);
        _editMenu.AddSeparator();
        _editMenu.AddItem("FlipUV - Vertical", (int)MenuId.EditFlipUV_Vertical);
        _editMenu.AddItem("FlipUV - Horizontal", (int)MenuId.EditFlipUV_Horizontal);

        PopupMenu _viewMenu = new PopupMenu();
        _viewMenu.Name = "View";
        _viewMenu.AddItem("Points", (int)MenuId.ViewPoints);
        _viewMenu.AddItem("Lines", (int)MenuId.ViewLines);
        _viewMenu.AddItem("Normals", (int)MenuId.ViewNormals);
        _viewMenu.AddItem("Faces", (int)MenuId.ViewFaces);

        ModelMenuBar.AddChild(_importMenu);              // attach to MenuBar
        ModelMenuBar.AddChild(_editMenu);                // attach to MenuBar
        ModelMenuBar.AddChild(_viewMenu);                // attach to MenuBar

        // Connect all menu event handlers
        _importMenu.IdPressed += OnMenuClicked;
        _editMenu.IdPressed += OnMenuClicked;
        _viewMenu.IdPressed += OnMenuClicked;

        // Make view menu items checkable and set initial states from ViewSelection
        int pointsIdx = _viewMenu.GetItemIndex((int)MenuId.ViewPoints);
        int linesIdx = _viewMenu.GetItemIndex((int)MenuId.ViewLines);
        int normalsIdx = _viewMenu.GetItemIndex((int)MenuId.ViewNormals);
        int facesIdx = _viewMenu.GetItemIndex((int)MenuId.ViewFaces);

        _viewMenu.SetItemAsCheckable(pointsIdx, true);
        _viewMenu.SetItemAsCheckable(linesIdx, true);
        _viewMenu.SetItemAsCheckable(normalsIdx, true);
        _viewMenu.SetItemAsCheckable(facesIdx, true);

        // Sync menu checkboxes with ViewSelection initial state
        _viewMenu.SetItemChecked(pointsIdx, ViewSelection.ShowPoints);
        _viewMenu.SetItemChecked(linesIdx, ViewSelection.ShowLines);
        _viewMenu.SetItemChecked(normalsIdx, ViewSelection.ShowNormals);
        _viewMenu.SetItemChecked(facesIdx, ViewSelection.ShowFaces);

    }

    private void OnMenuClicked(long id)
    {
        GD.Print($"ModelEditWindow: Menu item pressed - {id}");

        switch ((MenuId)id)
        {
            case MenuId.FileNew:
                HandleFileNew();
                break;

            case MenuId.FileOpen:
                HandleFileOpen();
                break;

            case MenuId.FileExit:
                HandleFileExit();
                break;

            case MenuId.ImportObjMtl:
                HandleImportObjMtl();
                break;

            case MenuId.EditFlipTriangles:
                HandleEditFlipTriangles();
                break;

            case MenuId.EditFlipUV_Vertical:
                HandleEditFlipUVVertical();
                break;

            case MenuId.EditFlipUV_Horizontal:
                HandleEditFlipUVHorizontal();
                break;

            case MenuId.ViewPoints:
                HandleViewToggle(MenuId.ViewPoints);
                break;

            case MenuId.ViewLines:
                HandleViewToggle(MenuId.ViewLines);
                break;

            case MenuId.ViewNormals:
                HandleViewToggle(MenuId.ViewNormals);
                break;

            case MenuId.ViewFaces:
                HandleViewToggle(MenuId.ViewFaces);
                break;

            default:
                GD.PrintErr($"ModelEditWindow: Unknown menu ID - {id}");
                break;
        }
    }

    private void HandleFileNew()
    {
        GD.Print("ModelEditWindow: File -> New");
        // TODO: Implement new file functionality
    }

    private void HandleFileOpen()
    {
        GD.Print("ModelEditWindow: File -> Open");
        // TODO: Implement file open functionality
    }

    private void HandleFileExit()
    {
        GD.Print("ModelEditWindow: File -> Exit");
        OnCloseRequested();
    }

    private void HandleImportObjMtl()
    {
        GD.Print("ModelEditWindow: Import -> OBJ/MTL");
        // TODO: Implement OBJ/MTL import functionality
    }

    private void HandleEditFlipTriangles()
    {
        GD.Print("ModelEditWindow: Edit -> Flip Triangles");
        
        // Code to Mesh
        JSONToMesh();
        WindowMeshData?.FlipAllTriangleWindings();
        MeshToJSON();
        JSONToMesh();
    }

    private void HandleEditFlipUVVertical()
    {
        GD.Print("ModelEditWindow: Edit -> Flip UV Vertical");
        JSONToMesh();
        WindowMeshData?.FlipAllUVsVertical();
        MeshToJSON();
        JSONToMesh();
    }

    private void HandleEditFlipUVHorizontal()
    {
        GD.Print("ModelEditWindow: Edit -> Flip UV Horizontal");
        JSONToMesh();
        WindowMeshData?.FlipAllUVsHorizontal();
        MeshToJSON();
        JSONToMesh();
    }

    private void HandleViewToggle(MenuId viewItem)
    {
        // Find the view menu (assuming it's the 3rd child - adjust index if needed)
        PopupMenu? viewMenu = null;
        if (ModelMenuBar != null)
        {
            foreach (Node child in ModelMenuBar.GetChildren())
            {
                if (child is PopupMenu menu && menu.Name == "View")
                {
                    viewMenu = menu;
                    break;
                }
            }
        }

        if (viewMenu == null)
        {
            GD.PrintErr("ModelEditWindow: Could not find View menu");
            return;
        }

        int itemIndex = viewMenu.GetItemIndex((int)viewItem);
        bool currentState = viewMenu.IsItemChecked(itemIndex);
        bool newState = !currentState;

        viewMenu.SetItemChecked(itemIndex, newState);

        string itemName = viewItem.ToString().Replace("View", "");
        GD.Print($"ModelEditWindow: View -> {itemName} = {(newState ? "ON" : "OFF")}");

        // Apply the view change to your 3D model display
        ApplyViewState(viewItem, newState);
    }

    private void ApplyViewState(MenuId viewItem, bool isVisible)
    {
        // Update the ViewSelection struct to maintain state
        switch (viewItem)
        {
            case MenuId.ViewPoints:
                ViewSelection.ShowPoints = isVisible;
                GD.Print($"  -> Setting points visibility: {isVisible}");
                break;

            case MenuId.ViewLines:
                ViewSelection.ShowLines = isVisible;
                GD.Print($"  -> Setting lines visibility: {isVisible}");
                break;

            case MenuId.ViewNormals:
                ViewSelection.ShowNormals = isVisible;
                GD.Print($"  -> Setting normals visibility: {isVisible}");
                break;

            case MenuId.ViewFaces:
                ViewSelection.ShowFaces = isVisible;
                GD.Print($"  -> Setting faces visibility: {isVisible}");
                break;
        }

        JSONToMesh();
        
        // TODO: Apply actual 3D model view changes using ViewSelection state
        // For example: UpdateMeshDisplay(ViewSelection);
    }

    // Helper methods to access ViewSelection state
    public KoreViewSelection GetViewSelection()
    {
        return ViewSelection;
    }
    
    public void SetViewSelection(KoreViewSelection newSelection)
    {
        ViewSelection = newSelection;
        
        // Update menu checkboxes to match the new selection
        UpdateMenuFromViewSelection();
    }
    
    private void UpdateMenuFromViewSelection()
    {
        if (ModelMenuBar == null) return;
        
        // Find the view menu
        PopupMenu? viewMenu = null;
        foreach (Node child in ModelMenuBar.GetChildren())
        {
            if (child is PopupMenu menu && menu.Name == "View")
            {
                viewMenu = menu;
                break;
            }
        }
        
        if (viewMenu == null) return;
        
        // Update checkbox states to match ViewSelection
        int pointsIdx = viewMenu.GetItemIndex((int)MenuId.ViewPoints);
        int linesIdx = viewMenu.GetItemIndex((int)MenuId.ViewLines);
        int normalsIdx = viewMenu.GetItemIndex((int)MenuId.ViewNormals);
        int facesIdx = viewMenu.GetItemIndex((int)MenuId.ViewFaces);
        
        viewMenu.SetItemChecked(pointsIdx, ViewSelection.ShowPoints);
        viewMenu.SetItemChecked(linesIdx, ViewSelection.ShowLines);
        viewMenu.SetItemChecked(normalsIdx, ViewSelection.ShowNormals);
        viewMenu.SetItemChecked(facesIdx, ViewSelection.ShowFaces);
    }

}
