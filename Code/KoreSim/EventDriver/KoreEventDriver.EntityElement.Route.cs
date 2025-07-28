
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
    // MARK: Route
    // ---------------------------------------------------------------------------------------------

    public static void EntitySetRoute(string entityName, List<KoreLLAPoint> points)
    {
        string elemName = $"{entityName}_Route";
        KoreEntityElementRoute? route = GetElement(entityName, elemName) as KoreEntityElementRoute;

        if (route == null)
        {
            route = new KoreEntityElementRoute() { Name = elemName };
            route.AddPoints(points);

            KoreEntity? Entity = EntityForName(entityName);
            if (Entity == null)
            {
                KoreCentralLog.AddEntry($"EC0-0017: EntitySetRoute: Entity {entityName} not found.");
                return;
            }

            Entity.AddElement(route);
        }
        else
        {
            route.Clear();
            route.AddPoints(points);
        }
    }

    public static List<KoreLLAPoint> EntityGetRoutePoints(string entityName)
    {
        string elemName = $"{entityName}_Route";
        KoreEntityElementRoute? route = GetElement(entityName, elemName) as KoreEntityElementRoute;

        if (route == null)
        {
            KoreCentralLog.AddEntry($"EC0-0018: EntityGetRoutePoints: Route {elemName} not found.");
            return new List<KoreLLAPoint>();
        }

        // Return a new list copying the points (so the caller can't modify the route)
        return new List<KoreLLAPoint>(route.Points);
    }

    public static void EntityClearRoute(string entityName)
    {
        string elemName = $"{entityName}_Route";
        KoreEntityElementRoute? route = GetElement(entityName, elemName) as KoreEntityElementRoute;

        if (route == null)
        {
            KoreCentralLog.AddEntry($"EC0-0019: EntityClearRoute: Route {elemName} not found.");
            return;
        }

        // Clear the route
        route.Clear();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Route QuickAdd
    // ---------------------------------------------------------------------------------------------

    // Clar any existing route data and setup a new route start point

    public static void EntitySetNewRouteStart(string entName, string routeElementName, KoreLLAPoint startPos)
    {
        KoreEntity? ent = EntityForName(entName);
        KoreEntityElement? element = GetElement(entName, routeElementName);


        EntityClearRoute(entName);

        // create a new route element
        var route = new KoreEntityElementRoute() { Name = routeElementName };
        route.AddPoint(startPos);

        AddEntityElement(entName, routeElementName, route);
    }


    // ---------------------------------------------------------------------------------------------
    // MARK: Helper Routines
    // ---------------------------------------------------------------------------------------------

    private static KoreEntityElementRoute? GetRouteElement(string entityName, string elemName)
    {
        KoreEntityElement? element = GetElement(entityName, elemName);
        if (element == null)
        {
            KoreCentralLog.AddEntry($"EC0-0020: GetRouteElement: Element {elemName} not found.");
            return null;
        }

        if (element is KoreEntityElementRoute)
        {
            return element as KoreEntityElementRoute;
        }
        else
        {
            KoreCentralLog.AddEntry($"EC0-0021: GetRouteElement: Element {elemName} is not a route.");
            return null;
        }
    }



}
