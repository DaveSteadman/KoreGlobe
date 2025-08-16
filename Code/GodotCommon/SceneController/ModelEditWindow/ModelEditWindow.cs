using Godot;
using System;

using KoreCommon;
using System.Collections.Generic;

#nullable enable

public partial class ModelEditWindow : Window
{
    // Sandbox3DScene.UIMount
    public Button? CodeToMeshButton = null;
    public Button? MeshToCodeButton = null;
    public Button? MeshToObjButton = null;
    public Button? ObjToCodeButton = null;
    public SubViewport? ModelViewport = null;
    public Node3D? MountRoot = null;
    public CodeEdit? MeshJsonEdit = null;

    // Mesh
    private KoreMeshData? WindowMeshData = null;

    // UI Timers
    private float UITimer = 0.0f;
    private float UITimerInterval = 0.1f; // 100ms

    private float UISlowTimer = 0.0f;
    private float UISlowTimerInterval = 2.5f;

    public bool ToClose { get; set; } = false;

    private Button? CloseButton = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GD.Print("ModelEditWindow _Ready");

        // var sv = GetNode<SubViewport>("SubViewport");
        // sv.OwnWorld3D = true;
        // sv.World3D = new World3D();
        WindowMeshData = KoreMeshDataPrimitives.IsolatedCube(0.5f, KoreMeshMaterialPalette.DefaultMaterial);
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
            // UpdateUISlow();
        }
    }




    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void AttachControls()
    {
        CodeToMeshButton = (Button)FindChild("CodeToMeshButton");
        MeshToCodeButton = (Button)FindChild("MeshToCodeButton");
        MeshToObjButton = (Button)FindChild("MeshToObjButton");
        ObjToCodeButton = (Button)FindChild("ObjToCodeButton");

        if (CodeToMeshButton == null) GD.PrintErr("ModelEditWindow: CodeToMeshButton node not found.");
        if (MeshToCodeButton == null) GD.PrintErr("ModelEditWindow: MeshToCodeButton node not found.");
        if (MeshToObjButton == null) GD.PrintErr("ModelEditWindow: MeshToObjButton node not found.");
        if (ObjToCodeButton == null) GD.PrintErr("ModelEditWindow: ObjToCodeButton node not found.");

        ModelViewport = (SubViewport)FindChild("ModelViewport");
        MountRoot = (Node3D)FindChild("MountRoot");
        MeshJsonEdit = (CodeEdit)FindChild("MeshJsonEdit");

        if (ModelViewport == null) GD.PrintErr("ModelEditWindow: ModelViewport node not found.");
        if (MountRoot == null) GD.PrintErr("ModelEditWindow: MountRoot node not found.");
        if (MeshJsonEdit == null) GD.PrintErr("ModelEditWindow: MeshJsonEdit node not found.");

        CodeToMeshButton?.Connect("pressed", new Callable(this, nameof(OnCodeToMeshRequested)));
        MeshToCodeButton?.Connect("pressed", new Callable(this, nameof(OnMeshToCodeRequested)));
        MeshToObjButton?.Connect("pressed", new Callable(this, nameof(OnMeshToObjRequested)));
        ObjToCodeButton?.Connect("pressed", new Callable(this, nameof(OnObjToCodeRequested)));

        // Link up the X button to close the window
        Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));
    }

    private void OnCloseRequested()
    {
        GD.Print("KoreSandbox3DWindow: Close button pressed");
        ToClose = true;
        // QueueFree();
    }

    private void OnCodeToMeshRequested()
    {
        GD.Print("ModelEditWindow: Code to Mesh button pressed");
        // Test OBJ export functionality
        JSONToMesh();
    }

    private void UpdateModelFromText()
    {
        // TODO: Implement parsing JSON from MeshJsonEdit and updating the 3D visualization
        GD.Print("UpdateModelFromText: Not yet implemented");
    }

    private void OnMeshToCodeRequested()
    {
        GD.Print("ModelEditWindow: Mesh to Code button pressed");
        // Implement the logic to convert mesh to code

        // OutputInitialJSON();

        MeshToJSON();
    }

    private void OnMeshToObjRequested()
    {
        GD.Print("ModelEditWindow: Mesh to OBJ button pressed");
        // Implement the logic to convert mesh to OBJ
        TestObjExport();
    }

    private void OnObjToCodeRequested()
    {
        GD.Print("ModelEditWindow: OBJ to Code button pressed");
        // Implement the logic to convert OBJ to code
        TestObjImport();
    }

}
