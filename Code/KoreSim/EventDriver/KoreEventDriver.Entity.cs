
using System;
using System.Collections.Generic;

#nullable enable

using KoreCommon;

namespace KoreSim;

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class KoreEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add/Del
    // ---------------------------------------------------------------------------------------------

    public static void AddEntity(string entName)
    {
        // Create a new entity
        // if (KoreSimFactory.Instance.EntityManager == null)
        //     KoreCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Entity Manager not found in KoreSimFactory.Instance");

        KoreEntity? newEnt = KoreSimFactory.Instance.EntityManager.Add(entName);
        if (newEnt == null)
        {
            KoreCentralLog.AddEntry($"EC0-0026: Entity {entName} not created, already exists.");
            return;
        }
    }

    public static void AddEntity(string entName, string entType)
    {
        // Create a new entity
        // if (KoreSimFactory.Instance.EntityManager == null)
        //     KoreCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Entity Manager not found in KoreSimFactory.Instance");

        KoreEntity? newEnt = KoreSimFactory.Instance.EntityManager.Add(entName);
        if (newEnt == null)
        {
            KoreCentralLog.AddEntry($"EC0-0026: Entity {entName} not created, already exists.");
            return;
        }


        //DefaultEntityDetails(entName);
    }

    public static bool HasEntity(string entName) => KoreSimFactory.Instance.EntityManager.DoesEntityExist(entName);


    public static bool DoesEntityExist(string entName) => KoreSimFactory.Instance.EntityManager.DoesEntityExist(entName);
    public static void DeleteEntity(string entName) => KoreSimFactory.Instance.EntityManager.Delete(entName);
    public static void DeleteAllEntities() => KoreSimFactory.Instance.EntityManager.DeleteAllEntities();
    public static int NumEntities() => KoreSimFactory.Instance.EntityManager.NumEntities();

    // ---------------------------------------------------------------------------------------------
    // MARK: Details
    // ---------------------------------------------------------------------------------------------

    public static void DefaultEntityDetails(string entityName)
    {
        KoreLLAPoint startPos = new KoreLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        KoreLLAPoint currPos = new KoreLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        KoreAttitude att = new KoreAttitude() { PitchUpDegs = 0.0, RollClockwiseDegs = 0.0, YawClockwiseDegs = 0.0 };
        KoreCourse course = new KoreCourse() { SpeedKph = 0.0, HeadingDegs = 0.0, ClimbRateMps = 0.0 };
        KoreCourseDelta courseDelta = new KoreCourseDelta() { SpeedChangeMpMps = 0.0, HeadingChangeClockwiseDegsSec = 0.0 };

        SetEntityStartDetails(entityName, startPos, att, course);
        SetEntityCurrDetails(entityName, currPos, att, course, courseDelta);
    }

    public static void SetEntityStartDetails(string entityName, KoreLLAPoint startPos, KoreAttitude startAtt, KoreCourse startCourse)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0002: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's details
        Entity.Kinetics.CurrPosition = startPos;
        Entity.Kinetics.CurrAttitude = startAtt;
        Entity.Kinetics.CurrCourse = startCourse;
    }

    public static void SetEntityCurrDetails(string entityName, KoreLLAPoint currPos, KoreAttitude currAtt, KoreCourse course, KoreCourseDelta courseDelta)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0002: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's details
        Entity.Kinetics.CurrPosition = currPos;
        Entity.Kinetics.CurrAttitude = currAtt;
        Entity.Kinetics.CurrCourse = course;
        Entity.Kinetics.CurrCourseDelta = courseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Start
    // ---------------------------------------------------------------------------------------------

    public static void SetEntityStart(string entityName, KoreLLAPoint newpos, KoreCourse newcourse, KoreAttitude newAtt)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0005: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's start location
        Entity.Kinetics.CurrPosition = newpos;
        Entity.Kinetics.CurrCourse   = newcourse;
        Entity.Kinetics.CurrAttitude = newAtt;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Position
    // ---------------------------------------------------------------------------------------------

    public static void SetEntityStartLLA(string entityName, KoreLLAPoint newpos)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0005: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's start location
        Entity.Kinetics.CurrPosition = newpos;
    }

    public static KoreLLAPoint? EntityStartLLA(string entityName)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return null;

        return Entity.Kinetics.CurrPosition;
    }

    // ---------------------------------------------------------------------------------------------

    public static void SetEntityPosition(string entityName, KoreLLAPoint newpos)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0006: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's position
        Entity.Kinetics.CurrPosition = newpos;
    }

    public static KoreLLAPoint GetEntityPosition(string entityName)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return KoreLLAPoint.Zero;

        return Entity.Kinetics.CurrPosition;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Attitude
    // ---------------------------------------------------------------------------------------------

    public static KoreAttitude GetEntityAttitude(string entityName)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return KoreAttitude.Zero;

        return Entity.Kinetics.CurrAttitude;
    }

    public static void SetEntityAttitude(string entityName, KoreAttitude newatt)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0008: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's attitude
        Entity.Kinetics.CurrAttitude = newatt;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Course
    // ---------------------------------------------------------------------------------------------

    public static void SetEntityCourse(string entityName, KoreCourse course)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0009: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's course
        Entity.Kinetics.CurrCourse = course;
    }

    public static KoreCourse GetEntityCourse(string entityName)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0010: Entity {entityName} not found.");
            return KoreCourse.Zero;
        }

        return Entity.Kinetics.CurrCourse;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Course Delta
    // ---------------------------------------------------------------------------------------------

    public static void SetEntityCourseDelta(string entityName, KoreCourseDelta courseDelta)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            KoreCentralLog.AddEntry($"EC0-0010: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's course delta
        Entity.Kinetics.CurrCourseDelta = courseDelta;
    }

    public static KoreCourseDelta? EntityCurrCourseDelta(string entityName)
    {
        // Get the Entity
        KoreEntity? Entity = KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return null;

        return Entity.Kinetics.CurrCourseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Report
    // ---------------------------------------------------------------------------------------------

    public static string EntityPositionsReport() => KoreSimFactory.Instance.EntityManager.EntityPositionsReport();
    public static string EntityElementsReport() => KoreSimFactory.Instance.EntityManager.EntityElementsReport();

    // ---------------------------------------------------------------------------------------------
    // MARK: Names
    // ---------------------------------------------------------------------------------------------

    public static string EntityNameForIndex(int index) => KoreSimFactory.Instance.EntityManager.EntityNameForIndex(index);
    public static KoreEntity? EntityForIndex(int index) => KoreSimFactory.Instance.EntityManager.EntityForIndex(index);
    public static KoreEntity? EntityForName(string entityName) => KoreSimFactory.Instance.EntityManager.EntityForName(entityName);

    // Id being the 1-based user presented index

    public static string EntityIdForName(string entityName) => KoreSimFactory.Instance.EntityManager.EntityIdForName(entityName);
    public static string EntityNameForId(int entityId) => KoreSimFactory.Instance.EntityManager.EntityNameForId(entityId);

    public static int EntityIdNext(int currEntityId) => KoreSimFactory.Instance.EntityManager.EntityIdNext(currEntityId);
    public static int EntityIdPrev(int currEntityId) => KoreSimFactory.Instance.EntityManager.EntityIdPrev(currEntityId);

    public static List<string> EntityNameList() => KoreSimFactory.Instance.EntityManager.EntityNameList();


}


