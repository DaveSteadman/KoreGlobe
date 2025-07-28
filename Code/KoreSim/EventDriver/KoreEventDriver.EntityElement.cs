
using System;
using System.Collections.Generic;

using KoreCommon;
namespace KoreSim;


#nullable enable

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class KoreEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Basic Element Management
    // ---------------------------------------------------------------------------------------------

    public static void AddEntityElement(string entityName, string elemName, string entityElemType)
    {
        // Create a new Entity
        KoreEntityElementOps.CreateEntityElement(entityName, elemName, entityElemType);
    }

    public static void AddEntityElement(string entityName, string elemName, KoreEntityElement element)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return;

        // Add the element to the Entity
        Entity.AddElement(element);
    }

    public static void DeleteEntityElement(string entityName, string elemName)
    {
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return;

        Entity.DeleteElement(elemName);
    }

    public static List<string> EntityElementNames(string entityName)
    {
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return new List<string>();

        return Entity.ElementNames();
    }

    // ---------------------------------------------------------------------------------------------

    public static void EntityAddSizerBox(string entityName, string entityType)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return;

        // fixed elemName for the box
        string elemName = "SizerBox";

        // Get the element
        KoreEntityElement? element = Entity.ElementForName(elemName);

        if (element == null)
            return;

        // Set the element's size
        return;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public static KoreEntityElement? GetElement(string entityName, string elemName)
    {
        if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(elemName))
            return null;

        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return null;

        return Entity.ElementForName(elemName);
    }



}
