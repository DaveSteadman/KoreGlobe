using System;
using System.Collections.Generic;

using Godot;
using KoreSim;
using KoreCommon;

#nullable enable

public partial class KoreNetworkSettingsWindow : Window
{
    // A "Close Me" state to indicate the window should be closed. This is done so the parent can control associated 
    // states, such as the launching button and the window reference, more easily.
    public bool ToClose { get; private set; } = false;

    // Controls
    private Label? TCPServerAddressLabel;
    private Label? TCPServerPortLabel;
    private Label? TCPClientAddressLabel;
    private Label? TCPClientPortLabel;

    private LineEdit? TCPServerAddressEdit;
    private LineEdit? TCPServerPortEdit;
    private LineEdit? TCPClientAddressEdit;
    private LineEdit? TCPClientPortEdit;

    private Button? ServerConnectButton;
    private Button? ClientConnectButton;

    private TextEdit? CommandResponseTextEdit;
    private Button? CloseButton;
    private CheckBox? MaintainConnectionsCheckBox;

    // 1Hz processing, to slow down _Process
    private float CurrTimer = 0.0f;
    private float TimerInterval = 1.0f; // interval to poll for CLI output

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        FindControls();

        // Link up the X button to close the window
        Connect("close_requested", new Callable(this, nameof(OnCloseRequested)));

        CloseButton?.Connect("pressed", new Callable(this, nameof(OnCloseRequested)));
        ServerConnectButton?.Connect("pressed", new Callable(this, nameof(OnServerConnectPressed)));
        ClientConnectButton?.Connect("pressed", new Callable(this, nameof(OnClientConnectPressed)));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Support
    // --------------------------------------------------------------------------------------------

    private void FindControls()
    {
        TCPServerAddressLabel = (Label)FindChild("TCPServerAddressLabel");
        TCPServerPortLabel = (Label)FindChild("TCPServerPortLabel");
        TCPClientAddressLabel = (Label)FindChild("TCPClientAddressLabel");
        TCPClientPortLabel = (Label)FindChild("TCPClientPortLabel");

        if (TCPServerAddressLabel == null) { GD.PrintErr("TCPServerAddressLabel not found"); return; }
        if (TCPServerPortLabel == null) { GD.PrintErr("TCPServerPortLabel not found"); return; }
        if (TCPClientAddressLabel == null) { GD.PrintErr("TCPClientAddressLabel not found"); return; }
        if (TCPClientPortLabel == null) { GD.PrintErr("TCPClientPortLabel not found"); return; }

        TCPServerAddressEdit = (LineEdit)FindChild("TCPServerAddressEdit");
        TCPServerPortEdit = (LineEdit)FindChild("TCPServerPortEdit");
        TCPClientAddressEdit = (LineEdit)FindChild("TCPClientAddressEdit");
        TCPClientPortEdit = (LineEdit)FindChild("TCPClientPortEdit");

        if (TCPServerAddressEdit == null) { GD.PrintErr("TCPServerAddressEdit not found"); return; }
        if (TCPServerPortEdit == null) { GD.PrintErr("TCPServerPortEdit not found"); return; }
        if (TCPClientAddressEdit == null) { GD.PrintErr("TCPClientAddressEdit not found"); return; }
        if (TCPClientPortEdit == null) { GD.PrintErr("TCPClientPortEdit not found"); return; }

        ServerConnectButton = (Button)FindChild("ServerConnectButton");
        ClientConnectButton = (Button)FindChild("ClientConnectButton");

        if (ServerConnectButton == null) { GD.PrintErr("ServerConnectButton not found"); return; }
        if (ClientConnectButton == null) { GD.PrintErr("ClientConnectButton not found"); return; }

        MaintainConnectionsCheckBox = (CheckBox)FindChild("MaintainConnectionsCheckBox");
        CloseButton = (Button)FindChild("CloseButton");

        if (MaintainConnectionsCheckBox == null) { GD.PrintErr("MaintainConnectionsCheckBox not found"); return; }
        if (CloseButton == null) { GD.PrintErr("CloseButton not found"); return; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Actions
    // --------------------------------------------------------------------------------------------

    private void OnCloseRequested()
    {
        GD.Print("KoreNetworkSettingsWindow: Close requested");
        ToClose = true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Connections
    // --------------------------------------------------------------------------------------------

    private void OnServerConnectPressed()
    {
        string address = TCPServerAddressEdit?.Text ?? string.Empty;
        string port = TCPServerPortEdit?.Text ?? string.Empty;

        GD.Print($"ButtonPress: ServerConnect {address}:{port}");

        if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(port))
        {
            GD.PrintErr("Server address or port is empty.");
            return;
        }

        //KoreNetworkManager.Instance.ConnectToServer(address, port);
    }

    private void OnClientConnectPressed()
    {
        string address = TCPClientAddressEdit?.Text ?? string.Empty;
        string port = TCPClientPortEdit?.Text ?? string.Empty;

        GD.Print($"ButtonPress: ClientConnect {address}:{port}");

        if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(port))
        {
            GD.PrintErr("Client address or port is empty.");
            return;
        }

        //KoreNetworkManager.Instance.ConnectToClient(address, port);
    }

}
