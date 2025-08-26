using System;
using System.Collections.Generic;
using System.Text;

using Godot;

using KoreCommon;
using KoreSim;

#nullable enable

public partial class KoreGodotEntityManager : Node3D
{
    List<KoreGodotEntity> EntityList = new List<KoreGodotEntity>();

    public Node3D EntityRootNode = new Node3D() { Name = "EntityRootNode" };
    public Node3D UnlinkedRootNode = new Node3D() { Name = "UnlinkedRootNode" };

    private float TimerModelCheck = 0.0f;
    private float TimerModelInterval = 1.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "EntityManager";

        // Setup the Root Nodes.
        AddChild(EntityRootNode);
        AddChild(UnlinkedRootNode);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (KoreCentralTime.CheckTimer(ref TimerModelCheck, TimerModelInterval))
        {
            MatchEntities();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Entities
    // --------------------------------------------------------------------------------------------

    public void MatchEntities()
    {
        // Get the string list of entity node names.
        // Get the list of model entity names.
        // Create a list of nodes to create and nodes to delete, then call functions to action those.

        List<string> nodeNames = EntityNodeNames();
        List<string> modelNames = KoreEventDriver.EntityNameList();

        List<string> entitiesWithoutNodes = KoreStringListOps.ListOmittedInSecond(modelNames, nodeNames);
        List<string> nodesWithoutEntities = KoreStringListOps.ListOmittedInSecond(nodeNames, modelNames);

        // Create and delete entities as needed.
        CreateEntities(entitiesWithoutNodes);
        DeleteEntities(nodesWithoutEntities);
    }

    // --------------------------------------------------------------------------------------------

    public void CreateEntities(List<string> entityNames)
    {
        foreach (string entityName in entityNames)
        {
            AddEntity(entityName);
        }
    }
    
    public void AddEntity(string entityName)
    {
        KoreGodotEntity newEntity = new KoreGodotEntity() { EntityName = entityName, Name = entityName };
        EntityRootNode.AddChild(newEntity);
    }
   
    // --------------------------------------------------------------------------------------------

    public void DeleteEntities(List<string> entityNames)
    {
        foreach (string entityName in entityNames)
        {
            RemoveEntityNode(entityName);
        }
    }

    public void RemoveEntityNode(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
            {
                currNode.QueueFree();
                // RemoveUnlinkedPlatform(entityName);
                return;
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public KoreGodotEntity? GetEntityNode(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return currNode as KoreGodotEntity;
        }

        return null;
    }

    // --------------------------------------------------------------------------------------------

    public List<string> EntityNodeNames()
    {
        List<string> names = new List<string>();

        foreach (Node3D currNode in EntityRootNode.GetChildren())
            names.Add(currNode.Name);

        return names;
    }

    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Create
    //     // --------------------------------------------------------------------------------------------

    //     // Create and delete entities to keep uptodate with the model.

    //     // public void MatchModelPlatforms()
    //     // {
    //     //     // Get the model
    //     //     List<string> platNames = KoreAppFactory.Instance.EventDriver.PlatformNames();
    //     //     List<string> godotEntityNames = EntityNames();

    //     //     // Compare the two lists, to find the new, the deleted and the consistent
    //     //     List<string> omittedInPresentation = KoreStringListOperations.ListOmittedInSecond(platNames, godotEntityNames);
    //     //     List<string> noLongerInModel = KoreStringListOperations.ListOmittedInSecond(godotEntityNames, platNames);
    //     //     List<string> maintainedEnitites = KoreStringListOperations.ListInBoth(platNames, godotEntityNames);

    //     //     bool addInfographicScale = KoreGodotFactory.Instance.UIState.IsRwScale;
    //     //     float scaleModifier = KoreValueUtils.Clamp(KoreGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);

    //     //     // Loop through the list of platform names, and the EntityList, match them up.
    //     //     foreach (string currModelName in omittedInPresentation)
    //     //     {
    //     //         AddEntity(currModelName);
    //     //         // AddChaseCam(currModelName);
    //     //     }

    //     //     foreach (string currModelName in noLongerInModel)
    //     //         RemoveEntity(currModelName);

    //     //     foreach (string currModelName in maintainedEnitites)
    //     //     {
    //     //         MatchModelPlatformElements(currModelName);
    //     //         MatchModelPlatform3DModel(currModelName);

    //     //         // Set the scale of the model
    //     //         string platformType = KoreAppFactory.Instance.EventDriver.PlatformType(currModelName) ?? "default";

    //     //         SetModelScale(currModelName, platformType, addInfographicScale, scaleModifier);

    //     //         //CheckAABB(currModelName);
    //     //     }
    //     // }

    //     // // --------------------------------------------------------------------------------------------
    //     // // MARK: Update - Add Elements
    //     // // --------------------------------------------------------------------------------------------

    //     // private void MatchModelPlatformElements(string platName)
    //     // {
    //     //     List<string> modelElementNames = KoreAppFactory.Instance.EventDriver.PlatformElementNames(platName);

    //     //     // List the unlinked elements for the platform.
    //     //     List<string> unlinkedElementNames = UnlinkedElementNames(platName);
    //     //     List<string> linkedElementNames = LinkedElementNames(platName);

    //     //     // Filter out some keyword elements
    //     //     linkedElementNames = KoreStringListOperations.FilterOut(linkedElementNames, "aabb");
    //     //     linkedElementNames = KoreStringListOperations.FilterOut(linkedElementNames, "model");

    //     //     {
    //     //         // Debug print out all the platforms and element names
    //     //         string allUnlinkedNames = string.Join(", ", unlinkedElementNames);
    //     //         string allLinkedNames = string.Join(", ", linkedElementNames);
    //     //         string matchDebug = $"MatchModelPlatformElements: {platName} Unlinked: {allUnlinkedNames} Linked: {allLinkedNames}";
    //     //         KoreCentralLog.AddEntry(matchDebug);

    //     //         string allModelNames = string.Join(", ", modelElementNames);
    //     //         KoreCentralLog.AddEntry($"MatchModelPlatformElements: {platName} Model: {allModelNames}");
    //     //     }

    //     //     // Join the two lists
    //     //     List<string> allElementNames = new List<string>();
    //     //     allElementNames.AddRange(unlinkedElementNames);
    //     //     allElementNames.AddRange(linkedElementNames);

    //     //     // Loop through the list of model elements, and the EntityList, match them up.
    //     //     List<string> omittedInPresentation = KoreStringListOperations.ListOmittedInSecond(modelElementNames, allElementNames);
    //     //     List<string> noLongerInModel = KoreStringListOperations.ListOmittedInSecond(allElementNames, modelElementNames);
    //     //     List<string> maintainedEnitites = KoreStringListOperations.ListInBoth(allElementNames, modelElementNames);

    //     //     foreach (string currElemName in omittedInPresentation)
    //     //     {
    //     //         KorePlatformElement? element = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
    //     //         if (element != null)
    //     //         {
    //     //             if (element is KorePlatformElementRoute) AddPlatformElementRoute(platName, currElemName);
    //     //             if (element is KorePlatformElementBeam) AddPlatformElementBeam(platName, currElemName);
    //     //             if (element is KorePlatformElementAntennaPatterns) AddPlatformElementAntennaPatterns(platName, currElemName);

    //     //             // Contrail
    //     //             // if (element is KorePlatformElementDome)            AddPlatformElementDome(platName, currElemName);
    //     //         }
    //     //     }

    //     //     // Process each of the maintained elements - updating for differences in the model data.
    //     //     foreach (string currElemName in maintainedEnitites)
    //     //     {
    //     //         KorePlatformElement? element = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
    //     //         if (element != null)
    //     //         {
    //     //             if (element is KorePlatformElementRoute) UpdatePlatformElementRoute(platName, currElemName);
    //     //             if (element is KorePlatformElementBeam) UpdatePlatformElementBeam(platName, currElemName);
    //     //             if (element is KorePlatformElementAntennaPatterns) AddPlatformElementAntennaPatterns(platName, currElemName);
    //     //         }
    //     //     }

    //     //     foreach (string currElemName in noLongerInModel)
    //     //     {
    //     //         GD.Print($"DELETING ELEMENT: {platName} {currElemName}");

    //     //         RemoveLinkedElement(platName, currElemName);
    //     //         RemoveUnlinkedElement(platName, currElemName);
    //     //     }
    //     // }

    //     // // --------------------------------------------------------------------------------------------
    //     // // MARK: Routes
    //     // // --------------------------------------------------------------------------------------------

    //     // public void AddPlatformElementRoute(string platName, string currElemName)
    //     // {
    //     //     // Get the Route Details
    //     //     KorePlatformElementRoute? route = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as KorePlatformElementRoute;

    //     //     KoreGodotPlatformElementRoute newRoute = new KoreGodotPlatformElementRoute();

    //     //     if ((route != null) && (newRoute != null))
    //     //     {
    //     //         newRoute.Name = currElemName;
    //     //         newRoute.SetRoutePoints(route!.RoutePoints);

    //     //         // Update visibility based on the UI state
    //     //         newRoute.SetVisibility(KoreGodotFactory.Instance.UIState.ShowRoutes);

    //     //         // Add the route to the entity and scene tree
    //     //         AddUnlinkedElement(platName, newRoute);

    //     //         KoreCentralLog.AddEntry($"Added route element {currElemName} to {platName}");
    //     //     }
    //     // }

    //     // public void UpdatePlatformElementRoute(string platName, string currElemName)
    //     // {
    //     //     // Get the Route Details
    //     //     KorePlatformElementRoute? route = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as KorePlatformElementRoute;

    //     //     // Get the godot route we'll update
    //     //     KoreGodotPlatformElementRoute? routeNode = GetUnlinkedElement(platName, currElemName) as KoreGodotPlatformElementRoute;

    //     //     if ((route != null) && (routeNode != null))
    //     //     {
    //     //         // Update visibility based on the UI state
    //     //         routeNode!.SetVisibility(KoreGodotFactory.Instance.UIState.ShowRoutes);
    //     //         routeNode!.SetRoutePoints(route.RoutePoints);
    //     //     }
    //     // }

    //     // // --------------------------------------------------------------------------------------------
    //     // // MARK: Beams
    //     // // --------------------------------------------------------------------------------------------

    //     // public void AddPlatformElementBeam(string platName, string currElemName)
    //     // {
    //     //     // Get the Beam details: shape, range, etc.
    //     //     KorePlatformElementBeam? beam = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as KorePlatformElementBeam;
    //     //     KoreGodotEntity? ent = GetEntity(platName);

    //     //     if ((ent != null) && (beam != null))
    //     //     {
    //     //         // Get the shape: important to determine the type of element to create

    //     //         KorePlatformElementBeam.ScanPatternShape shape = beam.GetScanPatternShape();

    //     //         if (shape == KorePlatformElementBeam.ScanPatternShape.Wedge)
    //     //         {
    //     //             KoreAzElBox azElBox = new KoreAzElBox() { MinAzDegs = -10, MaxAzDegs = 10, MinElDegs = -10, MaxElDegs = 10 };

    //     //             KoreGodotPlatformElementWedge newBeam = new KoreGodotPlatformElementWedge();

    //     //             newBeam.Name = currElemName;
    //     //             newBeam.AzElBox = beam.AzElBox;
    //     //             newBeam.ElementAttitude = beam.PortAttitude;
    //     //             newBeam.TxDistanceM = (float)(beam.DetectionRangeTxM);
    //     //             newBeam.RxDistanceM = (float)(beam.DetectionRangeRxM);

    //     //             GD.Print($"AddPlatformElementBeam: {platName} {currElemName} {newBeam.ElementAttitude}");

    //     //             AddLinkedElement(platName, newBeam);

    //     //             // rotate (after adding to parent) to accomodate the -ve Z axis
    //     //             newBeam.Rotation = new Vector3(0, (float)KoreValueUtils.DegsToRads(180), 0);
    //     //         }

    //     //         if (shape == KorePlatformElementBeam.ScanPatternShape.Dome)
    //     //         {
    //     //             KoreGodotPlatformElementDome newDome = new KoreGodotPlatformElementDome();
    //     //             newDome.Name = currElemName;
    //     //             newDome.TxDistanceM = (float)(beam.DetectionRangeTxM);
    //     //             newDome.RxDistanceM = (float)(beam.DetectionRangeRxM);
    //     //             newDome.RotateDegsPerSec = 360f / beam.PeriodSecs;
    //     //             AddLinkedElement(platName, newDome);
    //     //         }

    //     //         if (shape == KorePlatformElementBeam.ScanPatternShape.Cone)
    //     //         {
    //     //             KoreGodotPlatformElementCone newCone = new KoreGodotPlatformElementCone();
    //     //             newCone.Name = currElemName;
    //     //             newCone.TxConeLengthM = (float)(beam.DetectionRangeTxM);
    //     //             newCone.RxConeLengthM = (float)(beam.DetectionRangeRxM);
    //     //             newCone.ConeAzDegs = (float)beam.AzElBox.HalfArcAzDegs;
    //     //             newCone.SourcePlatformName = platName;
    //     //             newCone.TargetPlatformName = beam.TargetPlatName;
    //     //             newCone.Targeted = beam.Targeted;
    //     //             AddLinkedElement(platName, newCone);

    //     //             // rotate (after adding to parent) to accomodate the -ve Z axis
    //     //             newCone.Rotation = new Vector3(0, (float)KoreValueUtils.DegsToRads(180), 0);
    //     //         }

    //     //         if (shape == KorePlatformElementBeam.ScanPatternShape.DomeSector)
    //     //         {
    //     //             KoreGodotPlatformElementDomeSector newDomeSector = new KoreGodotPlatformElementDomeSector();
    //     //             newDomeSector.Name = currElemName;
    //     //             newDomeSector.TxDistanceM = (float)(beam.DetectionRangeTxM);
    //     //             newDomeSector.RxDistanceM = (float)(beam.DetectionRangeRxM);
    //     //             newDomeSector.SectorAzElBox = beam.AzElBox;
    //     //             AddLinkedElement(platName, newDomeSector);
    //     //         }

    //     //         // KoreGodotPlatformElementDome newDome = new KoreGodotPlatformElementDome();
    //     //         // newDome.RxDistanceM = 50000f;
    //     //         // AddLinkedElement(platName, newDome);
    //     //     }
    //     // }

    //     // public void UpdatePlatformElementBeam(string platName, string currElemName)
    //     // {
    //     //     // Get the Beam details: shape, range, etc.
    //     //     KorePlatformElementBeam? beam = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as KorePlatformElementBeam;
    //     //     KoreGodotEntity? ent = GetEntity(platName);

    //     //     KoreCentralLog.AddEntry($"################### =====> UpdatePlatformElementBeam: {platName} {currElemName}");

    //     //     if ((ent != null) && (beam != null))
    //     //     {
    //     //         if (beam.ScanShape == KorePlatformElementBeam.ScanPatternShape.Wedge)
    //     //         {
    //     //             KoreGodotPlatformElementWedge? beamNode = GetLinkedElement(platName, currElemName) as KoreGodotPlatformElementWedge;
    //     //             if (beamNode != null)
    //     //             {
    //     //                 KoreAzElBox azElBox = beam.AzElBox;

    //     //                 beamNode.TxDistanceM = (float)(beam.DetectionRangeTxM);
    //     //                 beamNode.RxDistanceM = (float)(beam.DetectionRangeRxM);
    //     //                 beamNode.AzElBox = azElBox;

    //     //                 beamNode.SetVisibility(
    //     //                     KoreGodotFactory.Instance.UIState.ShowEmitters,
    //     //                     KoreGodotFactory.Instance.UIState.ShowRx,
    //     //                     KoreGodotFactory.Instance.UIState.ShowTx);
    //     //             }
    //     //         }
    //     //         if (beam.ScanShape == KorePlatformElementBeam.ScanPatternShape.Dome)
    //     //         {
    //     //             KoreGodotPlatformElementDome? beamNode = GetLinkedElement(platName, currElemName) as KoreGodotPlatformElementDome;
    //     //             beamNode?.SetVisibility(
    //     //                 KoreGodotFactory.Instance.UIState.ShowEmitters,
    //     //                 KoreGodotFactory.Instance.UIState.ShowRx,
    //     //                 KoreGodotFactory.Instance.UIState.ShowTx);
    //     //         }
    //     //         if (beam.ScanShape == KorePlatformElementBeam.ScanPatternShape.Cone)
    //     //         {
    //     //             KoreGodotPlatformElementCone? beamNode = GetLinkedElement(platName, currElemName) as KoreGodotPlatformElementCone;
    //     //             beamNode?.SetVisibility(
    //     //                 KoreGodotFactory.Instance.UIState.ShowEmitters,
    //     //                 KoreGodotFactory.Instance.UIState.ShowRx,
    //     //                 KoreGodotFactory.Instance.UIState.ShowTx);
    //     //         }
    //     //     }
    //     // }

    //     // // --------------------------------------------------------------------------------------------
    //     // // MARK: Antenna Patterns
    //     // // --------------------------------------------------------------------------------------------

    //     // public void AddPlatformElementAntennaPatterns(string platName, string currElemName)
    //     // {
    //     //     KoreCentralLog.AddEntry($"AddPlatformElementAntennaPatterns: {platName} {currElemName}");

    //     //     // Get the Antenna Pattern details - from the model
    //     //     KorePlatformElementAntennaPatterns? antPat = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as KorePlatformElementAntennaPatterns;

    //     //     // See if we can get the antenna patterns from the godot presentation
    //     //     KoreGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as KoreGodotPlatformElementAntennaPatterns;

    //     //     // Get the godot platform we'll attach the element to
    //     //     KoreGodotEntity? entNode = GetEntity(platName);

    //     //     // If the node and antenna pattern are not null, we have enough to proceed.
    //     //     if ((entNode != null) && (antPat != null))
    //     //     {
    //     //         // Create a new Antenna Pattern element if it was null
    //     //         if (antPatNode == null)
    //     //         {
    //     //             // Get the size and distance to the AP to fit with the platform.
    //     //             double rwLongestAABBOffset = entNode.ModelInfo.RwAABB.LongestOffset();
    //     //             double geApOffsetDist = KoreZeroOffset.RwToGeDistanceMultiplier * (rwLongestAABBOffset * 2);
    //     //             double geApMaxAmplitude = geApOffsetDist * 0.5;

    //     //             antPatNode = new KoreGodotPlatformElementAntennaPatterns();
    //     //             antPatNode.Name = currElemName;
    //     //             antPatNode.Name = currElemName;
    //     //             antPatNode.SetSizeAndDistance((float)geApMaxAmplitude, (float)geApOffsetDist);
    //     //             AddLinkedElement(platName, antPatNode);
    //     //         }

    //     //         // Now the task is to loop through the antenna patterns and add the ones we don't have
    //     //         // Lets start with lists of the pattern names
    //     //         List<string> modelNames = antPat.PatternNames();
    //     //         List<string> nodeNames = antPatNode.PatternsList();

    //     //         // Debug print out all the platforms and element names
    //     //         string allModelNames = string.Join(", ", modelNames);
    //     //         string allNodeNames = string.Join(", ", nodeNames);
    //     //         string matchDebug = $"AddPlatformElementAntennaPatterns: {platName} {currElemName} Model: {allModelNames} Node: {allNodeNames}";
    //     //         KoreCentralLog.AddEntry(matchDebug);

    //     //         // Loop through the model names, then the node names, and add the missing ones
    //     //         List<string> omittedInPresentation = KoreStringListOperations.ListOmittedInSecond(modelNames, nodeNames);
    //     //         List<string> noLongerInModel = KoreStringListOperations.ListOmittedInSecond(nodeNames, modelNames);
    //     //         List<string> maintainedEnitites = KoreStringListOperations.ListInBoth(modelNames, nodeNames);

    //     //         foreach (string currPatternName in omittedInPresentation)
    //     //         {
    //     //             KoreAntennaPattern? pattern = antPat.PatternForPortName(currPatternName);
    //     //             if (pattern != null)
    //     //             {
    //     //                 antPatNode.AddPattern(pattern);
    //     //             }
    //     //         }

    //     //         // Update the visbility
    //     //         UpdatePlatformElementAntennaPatterns(platName, currElemName);

    //     //         // Update the sclae
    //     //         float scaleModifier = KoreValueUtils.Clamp(KoreGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);
    //     //         if (KoreGodotFactory.Instance.UIState.IsRwScale)
    //     //             scaleModifier = 1f;
    //     //         SetAPScale(platName, currElemName, scaleModifier);
    //     //     }
    //     // }

    //     // public void UpdatePlatformElementAntennaPatterns(string platName, string currElemName)
    //     // {
    //     //     // Get the godot platform we'll attach the element to
    //     //     KoreGodotEntity? ent = GetEntity(platName);

    //     //     // Get the Antenna Pattern details
    //     //     KorePlatformElementAntennaPatterns? antPat = KoreAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as KorePlatformElementAntennaPatterns;

    //     //     // Get the linked element node for APs
    //     //     KoreGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as KoreGodotPlatformElementAntennaPatterns;

    //     //     if ((ent != null) && (antPat != null) && (antPatNode != null))
    //     //     {
    //     //         // Update the visibility of the APs based on the UI state
    //     //         antPatNode.SetVisibility(KoreGodotFactory.Instance.UIState.ShowAntennaPatterns);
    //     //     }
    //     // }

    //     // public void SetAPScale(string platName, string currElemName, float scaleModifier)
    //     // {
    //     //     scaleModifier = (float)InfographicScaleRange.GetValue(scaleModifier);

    //     //     // Get the godot platform we'll attach the element to
    //     //     KoreGodotEntity? ent = GetEntity(platName);

    //     //     // Get the Antenna Pattern details
    //     //     KoreGodotPlatformElementAntennaPatterns? antPatNode = GetLinkedElement(platName, currElemName) as KoreGodotPlatformElementAntennaPatterns;

    //     //     if (antPatNode != null)
    //     //         antPatNode.Scale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
    //     // }

    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Reporting
    //     // --------------------------------------------------------------------------------------------

    //     public string FullReport()
    //     {
    //         StringBuilder sb = new StringBuilder();


    //         List<string> namesList = EntityNames();

    //         sb.AppendLine($"Entity Count: {namesList.Count}");
    //         foreach (string currName in namesList)
    //         {
    //             sb.AppendLine($"Entity: {currName}");
    //         }

    //         return sb.ToString();
    //     }

    //     // --------------------------------------------------------------------------------------------
    //     // MARK: Update - Delete
    //     // --------------------------------------------------------------------------------------------

}


