using Godot;
using System;
using System.Runtime;

using KoreCommon;
using KoreSim;
using System.Collections.Generic;

#nullable enable

// Kore3DRelocatableSceneObjects: Utility class of useful objects passed around the scene functionality
// - Avoids the need for singletons/globals

public class Kore3DRelocatableSceneObjects
{
    // MainScene Control
    public KoreZeroNode?                ZeroNode           { get; set; } = null;
    public KoreRelocatableXYZMoverNode? WorldCameraMount   { get; set; } = null;
    public KoreZeroNodeSphere?          ZeroNodeSphere     { get; set; } = null;

    // Debug Objects
    public Kore3DFrameworkObjectManager?  FrameworkManager { get; set; } = null;

    // Map Managers
    // public KoreQuadZNMapManager?        QuadZNMapManager   { get; set; } = null;
    //public KoreZeroNodeMapManager?      ZeroNodeMapManager { get; set; } = null;
    public KoreFlatMapManager?         FlatMapManager     { get; set; } = null;

}
