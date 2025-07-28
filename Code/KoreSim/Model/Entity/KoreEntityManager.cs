using System;
using System.Collections.Generic;
using System.Text;

using KoreCommon;
namespace KoreSim;

#nullable enable

// Class to provide the top level management of Entities in the system.
public partial class KoreEntityManager
{
    private List<KoreEntity> EntityList = new();

    // --------------------------------------------------------------------------------------------

    public KoreEntityManager()
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Entity List
    // --------------------------------------------------------------------------------------------

    // Considering the Entity list by index

    public int  NumEntities()       => EntityList.Count;
    public void DeleteAllEntities() => EntityList.Clear();

    // --------------------------------------------------------------------------------------------

    public KoreEntity? Add(string entityName)
    {
        // Check if Entity exists
        if (EntityForName(entityName) == null)
        {
            KoreEntity newEntity = new KoreEntity() { Name = entityName };
            EntityList.Add(newEntity);

            KoreSimFactory.Instance.EventRegister.AddEvent(KoreEventRegister.CreateEvent_CreateEntity(entityName));

            return newEntity;
        }
        return null;
    }

    // Looping through the list using the index, and in reverse order to more safely delete a Entity
    public void Delete(string entityName)
    {
        for (int i = EntityList.Count - 1; i >= 0; i--)
        {
            if (EntityList[i].Name == entityName)
            {
                EntityList.RemoveAt(i);
                KoreSimFactory.Instance.EventRegister.AddEvent(KoreEventRegister.CreateEvent_DeleteEntity(entityName));

            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public KoreEntity? EntityForName(string entityName)
    {
        foreach (KoreEntity currEntity in EntityList)
        {
            if (currEntity.Name == entityName)
                return currEntity;
        }
        return null;
    }

    public KoreEntity? EntityForIndex(int index)
    {
        if (index < 0 || index >= EntityList.Count)
            return null;

        return EntityList[index];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Entity name * index
    // --------------------------------------------------------------------------------------------

    public string EntityNameForIndex(int index)
    {
        if (index < 0 || index >= EntityList.Count)
            return string.Empty;

        return EntityList[index].Name;
    }

    public int EntityIndexForName(string entityName)
    {
        for (int i = 0; i < EntityList.Count; i++)
        {
            if (EntityList[i].Name == entityName)
                return i;
        }
        return -1;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: EntityIDs & Names
    // --------------------------------------------------------------------------------------------
    // Id being the 1-based user presented version of the list index

    public string EntityNameForId(int entityId)
    {
        int entityIndex = entityId - 1;
        if (entityIndex < 0 || entityIndex >= EntityList.Count)
            return "- - -";

        return EntityList[entityIndex].Name;
    }

    public string EntityIdForName(string entityName)
    {
        for (int i = 0; i < EntityList.Count; i++)
        {
            if (EntityList[i].Name == entityName)
                return (i + 1).ToString();
        }
        return string.Empty;
    }

    public int EntityIdNext(int currEntityId)
    {
        int numEntities = KoreSimFactory.Instance.EntityManager.NumEntities();
        int minId    = (numEntities > 0) ? 1 : 0;
        int maxId    = (numEntities > 0) ? numEntities: 0;

        if (numEntities == 0)      return 0;     // return 0 if no Entities
        if (currEntityId < minId)  return minId; // Move up to min if below it
        if (currEntityId >= maxId) return minId; // wrap around to min if at or above max
        return currEntityId + 1;                 // Move up one if mid-range
    }

    public int EntityIdPrev(int currEntityId)
    {
        int numEntities = KoreSimFactory.Instance.EntityManager.NumEntities();
        int minId    = (numEntities > 0) ? 1 : 0;
        int maxId    = (numEntities > 0) ? numEntities: 0;

        if (numEntities == 0)      return 0;     // return 0 if no Entities
        if (currEntityId <= minId) return maxId; // wrap around to max if at or below min
        if (currEntityId > maxId)  return maxId; // Move down to max if above it
        return currEntityId - 1;                 // Move down one if mid-range
    }

    public bool DoesEntityExist(string entityName)
    {
        return EntityForName(entityName) != null;
    }

    public List<string> EntityNameList()
    {
        List<string> entityNames = new();
        foreach (KoreEntity currEntity in EntityList)
        {
            entityNames.Add(currEntity.Name);
        }
        return entityNames;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elements
    // --------------------------------------------------------------------------------------------

    public bool DoesElementExist(string entityName, string elemname)
    {
        KoreEntity? entity = EntityForName(entityName);
        if (entity == null)
            return false;

        return entity.DoesElementExist(elemname);
    }

    public bool AddElement(string entityName, KoreEntityElement newElem)
    {
        // Return false if we don't find the Entity
        KoreEntity? entity = EntityForName(entityName);
        if (entity == null)
            return false;

        entity.AddElement(newElem);
        return true;
    }

    public void DeleteElement(string entityName, string elemname)
    {
        KoreEntity? entity = EntityForName(entityName);
        if (entity == null)
            return;

        entity.DeleteElement(elemname);
    }

    public KoreEntityElement? ElementForName(string entityName, string elemname)
    {
        KoreEntity? entity = EntityForName(entityName);
        if (entity == null)
            return null;

        return entity.ElementForName(elemname);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reports
    // --------------------------------------------------------------------------------------------

    public string EntityPositionsReport()
    {
        string report = "Entity Positions Report\n";

        foreach (KoreEntity currEntity in EntityList)
        {
            if (currEntity.Kinetics != null)
            {
                KoreLLAPoint currPos = currEntity.Kinetics.CurrPosition;
                report += $"Entity:{currEntity.Name} at LatDegs:{currPos.LatDegs:0.0000}, LonDegs:{currPos.LonDegs:0.0000}, AltMslM:{currPos.AltMslM:0.0000}, RadiusM:{currPos.RadiusM}\n";
            }
        }

        return report;
    }

    public string EntityElementsReport()
    {
        string report = "Entity Elements Report\n";

        foreach (KoreEntity currEntity in EntityList)
        {
            report += $"Entity: {currEntity.Name}\n";

            foreach (KoreEntityElement currElem in currEntity.ElementsList)
            {
                report += $"- Element: {currElem.Name} Type: {currElem.Type}\n";
            }
        }

        return report;
    }

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Entity Manager Report");
        sb.AppendLine("=======================");
        sb.AppendLine();

        sb.AppendLine("Entity List");
        sb.AppendLine("-------------");
        foreach (KoreEntity currEntity in EntityList)
        {
            sb.AppendLine($"Entity: {currEntity.Name}");
            sb.AppendLine($"- CurrPosition: {currEntity.Kinetics?.CurrPosition}");
            sb.AppendLine($"- CurrAttitude: {currEntity.Kinetics?.CurrAttitude}");
            sb.AppendLine($"- CurrCourse: {currEntity.Kinetics?.CurrCourse}");
            sb.AppendLine($"- CurrCourseDelta: {currEntity.Kinetics?.CurrCourseDelta}");
        }

        sb.AppendLine();
        sb.AppendLine("Entity Elements");
        sb.AppendLine("-----------------");
        foreach (KoreEntity currEntity in EntityList)
        {
            sb.AppendLine($"Entity: {currEntity.Name}");
            foreach (KoreEntityElement currElem in currEntity.ElementsList)
            {
                sb.AppendLine($"- Element: {currElem.Report()}");
            }
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reset
    // --------------------------------------------------------------------------------------------

    // Reset (or set) all the current Entity positions

    public void Reset()
    {
        foreach (KoreEntity currEntity in EntityList)
        {
            if (currEntity.Kinetics != null)
                currEntity.Kinetics.ResetPosition();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    public void UpdateKinetics()
    {
        double elapsedSeconds = KoreSimFactory.Instance.SimClock.ElapsedTimeSinceMark();
        KoreSimFactory.Instance.SimClock.MarkTime();

        foreach (KoreEntity currEntity in EntityList)
        {
            if (currEntity.Kinetics != null)
                currEntity.Kinetics.UpdateForDuration((float)elapsedSeconds);
        }

        // Log the LLA of the first Entity, to demonstrate some movement.
        if (EntityList.Count > 0)
        {
            KoreEntity firstEntity = EntityList[0];
            KoreLLAPoint currPos = firstEntity.Kinetics.CurrPosition;

            string LLAStr = $"Lat: {currPos.LatDegs:0.0000}, Lon: {currPos.LonDegs:0.0000}, Alt: {currPos.AltMslM:0.0000}";

            KoreCentralLog.AddEntry($"Entity: {firstEntity.Name} at {LLAStr}");
        }
    }

    // --------------------------------------------------------------------------------------------

}


