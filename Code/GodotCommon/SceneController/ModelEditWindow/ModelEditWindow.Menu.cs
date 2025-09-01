using Godot;
using System;
using System.IO;
using System.Linq;
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
        EditCalcNormals = 1004,

        ExportNativeJSON = 2000,
        ExportObjMtl = 2001,

        ImportNativeJSON = 2500,
        ImportObjMtl = 2501,

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

        PopupMenu _fileMenu = new PopupMenu();
        _fileMenu.Name = "File";
        _fileMenu.AddItem("New", (int)MenuId.FileNew);
        _fileMenu.AddItem("Open", (int)MenuId.FileOpen);
        _fileMenu.AddItem("Exit", (int)MenuId.FileExit);

        PopupMenu _exportMenu = new PopupMenu();
        _exportMenu.Name = "Export";
        _exportMenu.AddItem("Export Native JSON", (int)MenuId.ExportNativeJSON);
        _exportMenu.AddItem("Export OBJ/MTL", (int)MenuId.ExportObjMtl);

        PopupMenu _importMenu = new PopupMenu();
        _importMenu.Name = "Import";
        _importMenu.AddItem("Import Native JSON", (int)MenuId.ImportNativeJSON);
        _importMenu.AddItem("Import OBJ/MTL", (int)MenuId.ImportObjMtl);

        PopupMenu _editMenu = new PopupMenu();
        _editMenu.Name = "Edit";
        _editMenu.AddItem("Flip Triangles", (int)MenuId.EditFlipTriangles);
        _editMenu.AddSeparator();
        _editMenu.AddItem("FlipUV - Vertical", (int)MenuId.EditFlipUV_Vertical);
        _editMenu.AddItem("FlipUV - Horizontal", (int)MenuId.EditFlipUV_Horizontal);
        _editMenu.AddSeparator();
        _editMenu.AddItem("Calculate Normals", (int)MenuId.EditCalcNormals);
        
        PopupMenu _viewMenu = new PopupMenu();
        _viewMenu.Name = "View";
        _viewMenu.AddItem("Points", (int)MenuId.ViewPoints);
        _viewMenu.AddItem("Lines", (int)MenuId.ViewLines);
        _viewMenu.AddItem("Normals", (int)MenuId.ViewNormals);
        _viewMenu.AddItem("Faces", (int)MenuId.ViewFaces);

        ModelMenuBar.AddChild(_fileMenu);                // attach to MenuBar
        ModelMenuBar.AddChild(_exportMenu);              // attach to MenuBar
        ModelMenuBar.AddChild(_importMenu);              // attach to MenuBar
        ModelMenuBar.AddChild(_editMenu);                // attach to MenuBar
        ModelMenuBar.AddChild(_viewMenu);                // attach to MenuBar

        // Connect all menu event handlers
        _fileMenu.IdPressed += OnMenuClicked;
        _exportMenu.IdPressed += OnMenuClicked;
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
            // File
            case MenuId.FileNew:                   HandleFileNew(); break;
            case MenuId.FileOpen:                  HandleFileOpen(); break;
            case MenuId.FileExit:                  HandleFileExit(); break;

            // Export
            case MenuId.ExportNativeJSON:          HandleExportNativeJSON(); break;
            case MenuId.ExportObjMtl:              HandleExportObjMtl(); break;

            // Import
            case MenuId.ImportNativeJSON:          HandleImportNativeJSON(); break;
            case MenuId.ImportObjMtl:              HandleImportObjMtl(); break;

            // Edit
            case MenuId.EditFlipTriangles:         HandleEditFlipTriangles(); break;
            case MenuId.EditFlipUV_Vertical:       HandleEditFlipUVVertical(); break;
            case MenuId.EditFlipUV_Horizontal:     HandleEditFlipUVHorizontal(); break;
            case MenuId.EditCalcNormals:           HandleEditCalcNormals(); break;

            // View
            case MenuId.ViewPoints:                HandleViewToggle(MenuId.ViewPoints); break;
            case MenuId.ViewLines:                 HandleViewToggle(MenuId.ViewLines); break;
            case MenuId.ViewNormals:               HandleViewToggle(MenuId.ViewNormals); break;
            case MenuId.ViewFaces:                 HandleViewToggle(MenuId.ViewFaces); break;

            default:
                GD.PrintErr($"ModelEditWindow: Unknown menu ID - {id}");
                break;
        }
    }

    // --------------------------------------------------------------------------------------------

    private void HandleFileNew()
    {
        GD.Print("ModelEditWindow: File -> New");
        // TODO: Implement new file functionality

        // create a nominal cube and
        WindowMeshData = KoreMeshDataPrimitives.IsolatedCube(1.0f, KoreMeshMaterialPalette.Find("BlueGlass"));
        MeshJsonEdit!.SetText(KoreMeshDataIO.ToJson(WindowMeshData, dense: false));

        ClearMeshInstances();
        JSONToMesh();
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

    // --------------------------------------------------------------------------------------------

    // Export
    private void HandleExportNativeJSON()
    {
        GD.Print("ModelEditWindow: Export -> Native JSON");

        // Create and configure file dialog
        var fileDialog = new FileDialog();
        fileDialog.Title = "Save Native JSON";
        fileDialog.CurrentFile = "mesh.json";
        fileDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
        fileDialog.Access = FileDialog.AccessEnum.Filesystem; // Allow access to user filesystem (C:\, etc.)
        fileDialog.AddFilter("*.json", "JSON Files");

        // Connect the file selected signal
        fileDialog.FileSelected += OnExportNativeFileSelected;

        // Add to scene tree and show
        AddChild(fileDialog);
        fileDialog.PopupCentered(new Vector2I(800, 600));
    }

    private void HandleExportObjMtl()
    {
        GD.Print("ModelEditWindow: Export -> OBJ/MTL");
        // TODO: Implement OBJ/MTL export functionality
    }

    private void OnExportNativeFileSelected(string path)
    {
        GD.Print($"ModelEditWindow: Export -> Native JSON: {path}");

        // Clean up the file dialog
        foreach (Node child in GetChildren())
        {
            if (child is FileDialog fileDialog)
            {
                fileDialog.QueueFree();
                break;
            }
        }

        // TODO: Implement actual JSON export functionality
        // Example: ExportMeshToJSON(path);
        string jsonStr = KoreMeshDataIO.ToJson(WindowMeshData, dense: false);
        File.WriteAllText(path, jsonStr);
    }

    // --------------------------------------------------------------------------------------------

    // import

    private void HandleImportNativeJSON()
    {
        GD.Print("ModelEditWindow: Import -> Native JSON");

        // Create and configure file dialog
        var fileDialog = new FileDialog();
        fileDialog.Title = "Open Native JSON";
        fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        fileDialog.Access = FileDialog.AccessEnum.Filesystem; // Allow access to user filesystem (C:\, etc.)
        fileDialog.AddFilter("*.json", "JSON Files");

        // Connect the file selected signal
        fileDialog.FileSelected += OnImportNativeFileSelected;

        // Add to scene tree and show
        AddChild(fileDialog);
        fileDialog.PopupCentered(new Vector2I(800, 600));
    }

    private void OnImportNativeFileSelected(string path)
    {
        GD.Print($"ModelEditWindow: Import -> Native JSON: {path}");

        // Clean up the file dialog
        foreach (Node child in GetChildren())
        {
            if (child is FileDialog fileDialog)
            {
                fileDialog.QueueFree();
                break;
            }
        }

        // Import JSON file and load into mesh
        try
        {
            string jsonStr = File.ReadAllText(path);
            MeshJsonEdit!.SetText(jsonStr);
            WindowMeshData = KoreMeshDataIO.FromJson(jsonStr);
            JSONToMesh(); // Update the 3D display
            GD.Print($"Successfully imported mesh from: {path}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to import JSON file: {ex.Message}");
        }
    }

    private void HandleImportObjMtl()
    {
        GD.Print("ModelEditWindow: Import -> OBJ/MTL");
        // TODO: Implement OBJ/MTL import functionality
    }


    // --------------------------------------------------------------------------------------------



    private void HandleEditFlipTriangles()
    {
        GD.Print("ModelEditWindow: Edit -> Flip Triangles");

        // Load mesh, flip triangles, and save back
        JSONToMesh();

        if (WindowMeshData != null)
            KoreMeshDataEditOps.FlipAllTriangleWindings(WindowMeshData);

        MeshToJSON();
    }

    private void HandleEditFlipUVVertical()
    {
        GD.Print("ModelEditWindow: Edit -> Flip UV Vertical");
        JSONToMesh();
        if (WindowMeshData != null)
            KoreMeshDataEditOps.FlipAllUVsVertical(WindowMeshData);
        MeshToJSON();
        // Note: Don't call JSONToMesh() again - it causes disposal conflicts
    }

    private void HandleEditFlipUVHorizontal()
    {
        GD.Print("ModelEditWindow: Edit -> Flip UV Horizontal");
        JSONToMesh();
        if (WindowMeshData != null)
            KoreMeshDataEditOps.FlipAllUVsHorizontal(WindowMeshData);
        MeshToJSON();
        // Note: Don't call JSONToMesh() again - it causes disposal conflicts
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


    public void HandleEditCalcNormals()
    {
        GD.Print("ModelEditWindow: Edit -> Calculate Normals");
        JSONToMesh();
        if (WindowMeshData != null)
            KoreMeshDataEditOps.SetNormalsFromTriangles(WindowMeshData);
        MeshToJSON();
    }





    // ---------------------------------------------------------------------


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
