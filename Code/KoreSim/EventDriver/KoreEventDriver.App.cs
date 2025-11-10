
using System;


using KoreCommon;

namespace KoreSim;

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class KoreEventDriver
{
    // --------------------------------------------------------------------------------------------
    // Command Execution
    // --------------------------------------------------------------------------------------------

    public static void ExitApplication()
    {
        KoreSimFactory.Instance.ConsoleInterface.Stop();
        KoreSimFactory.Instance.MessageManager.EndThread();
        KoreSimFactory.Instance.NetworkHub.EndAllConnections();
        //KoreSimFactory.Instance.NetworkHub.EndAllThread();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Config
    // --------------------------------------------------------------------------------------------

    public static string ConfigReport()
    {
        // Get the config report from the KoreCentralConfig
        return KoreSimFactory.Instance.KoreConfig.Report();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Sidebar view settings
    // --------------------------------------------------------------------------------------------

    // Usage: KoreEventDriver.SidebarSetBeamVisibility(true, true);

    // public void SidebarSetBeamVisibility(bool rxVisible, bool txVisible)
    // {
    //     // Set the UI State values
    //     GloGodotFactory.Instance.UIState.ShowRx = rxVisible;
    //     GloGodotFactory.Instance.UIState.ShowTx = txVisible;
    // }

    // --------------------------------------------------------------------------------------------

    // Usage: KoreEventDriver.SidebarSetRouteVisibility(true);
    //        KoreEventDriver.SidebarSetEmitterVisibility(true);
    //        KoreEventDriver.SidebarSetAntennaPatternVisibility(true);

    // public void SidebarSetRouteVisibility(bool routeVisible)                   => GloGodotFactory.Instance.UIState.ShowRoutes          = routeVisible;
    // public void SidebarSetEmitterVisibility(bool emitterVisible)               => GloGodotFactory.Instance.UIState.ShowEmitters        = emitterVisible;
    // public void SidebarSetAntennaPatternVisibility(bool antennaPatternVisible) => GloGodotFactory.Instance.UIState.ShowAntennaPatterns = antennaPatternVisible;

    // public void SetCameraModeWorld()    => GloGodotFactory.Instance.UIState.SetCameraModeWorld();
    // public void SetCameraModeChaseCam() => GloGodotFactory.Instance.UIState.SetCameraModeChaseCam();
    // public void SetCameraModeAlignCam() => GloGodotFactory.Instance.UIState.SetCameraModeAlignCam();


    // --------------------------------------------------------------------------------------------

}

