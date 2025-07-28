using System;

#nullable enable

using KoreCommon;

namespace KoreSim;

// KoreEntityElementOperations: Utility class to factor out all of the element searching and management operations, and avoid cluttering the Entity class

public static class KoreEntityElementOps
{
    public static KoreEntityElement? CreateEntityElement(string entityName, string elemName, string entityElemType)
    {
        KoreEntityElement? newElem = null;

        KoreEntity? entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);
        if (entity == null)
            return newElem;

        if (entity.DoesElementExist(elemName))
            return newElem;

        switch (entityElemType)
        {

            default:
                break;
        }

        if (newElem != null)
            entity.AddElement(newElem);

        return newElem;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Access Operations
    // ---------------------------------------------------------------------------------------------

    // public static KoreEntityElement? ElementForName(KoreEntity entity, string elemname)
    // {
    //     foreach (var element in entity.Elements)
    //     {
    //         if (element.Name == elemname)
    //             return element;
    //     }
    //     return null;
    // }

    // public static bool DoesElementExist(KoreEntity entity, string elemname)
    // {
    //     foreach (var element in entity.Elements)
    //     {
    //         if (element.Name == elemname)
    //             return true;
    //     }
    //     return false;
    // }

    // public static void DeleteElement(KoreEntity entity, string elemname)
    // {
    //     foreach (var element in entity.Elements)
    //     {
    //         if (element.Name == elemname)
    //         {
    //             Entity.Elements.Remove(element);
    //             return;
    //         }
    //     }
    // }

}