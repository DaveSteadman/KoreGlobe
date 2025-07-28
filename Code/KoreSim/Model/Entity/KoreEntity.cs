using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#nullable enable

using KoreCommon;

namespace KoreSim;

public class KoreEntity
{
    public string Name { get; set; } = "Unknown-Name";

    // Kinetics object defines the initial and current position of the Entity. Has to exist in all cases.
    public KoreEntityKinetics Kinetics { get; set; } = new KoreEntityKinetics();

    private List<KoreEntityElement>? Elements { get; set; } = null;

    // String dictionary of additional properties that can be set by the user or the system on an entity without affecting its lowest level behavior.
    private KoreStringDictionary Properties { get; set; } = new KoreStringDictionary();

    // --------------------------------------------------------------------------------------------

    // Accessors constructors

    // public KoreEntityRoute RouteObject
    // {
    //     get { Route ??= new KoreEntityRoute(); return Route; }
    // }

    public List<KoreEntityElement> ElementsList
    {
        get { Elements ??= new List<KoreEntityElement>(); return Elements; }
    }

    public List<string> ElementNames()
    {
        List<string> names = new List<string>();

        foreach (KoreEntityElement element in ElementsList)
            names.Add(element.Name);

        return names;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Property
    // --------------------------------------------------------------------------------------------

    public bool HasProperty(string key) => Properties.Has(key);
    public void SetProperty(string key, string value) => Properties.Set(key, value);

    public string GetProperty(string key)
    {
        if (!Properties.Has(key))
            throw new KeyNotFoundException($"Property '{key}' not found on entity '{Name}'");

        return Properties.Get(key);
    }

    // Return list of all the property keys
    public List<string> PropertyKeys()
    {
        return Properties.KeysList();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Element
    // --------------------------------------------------------------------------------------------

    public bool DoesElementExist(string elementName) => ElementsList.Any(element => element.Name == elementName);
    public void AddElement(KoreEntityElement element) => ElementsList.Add(element);
    public void DeleteElement(KoreEntityElement element) => ElementsList.Remove(element);

    // Loops in reverse to avoid issues with removing elements from a list while iterating over it
    public void DeleteElement(string name)
    {
        for (int i = ElementsList.Count - 1; i >= 0; i--)
        {
            if (ElementsList[i].Name == name)
            {
                ElementsList.RemoveAt(i);
            }
        }
    }

    public KoreEntityElement? ElementForName(string name)
    {
        foreach (KoreEntityElement element in ElementsList)
        {
            if (element.Name == name)
                return element;
        }
        return null;
    }

    // --------------------------------------------------------------------------------------------

    public string PositionReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Entity: {Name}");
        sb.AppendLine($"- CurrPosition: {Kinetics.CurrPosition}");
        sb.AppendLine($"- CurrAttitude: {Kinetics.CurrAttitude}");
        sb.AppendLine($"- CurrCourse: {Kinetics.CurrCourse}");
        sb.AppendLine($"- CurrCourseDelta: {Kinetics.CurrCourseDelta}");
        sb.AppendLine($"- CurrAttitudeDelta: {Kinetics.CurrAttitudeDelta}");

        return sb.ToString();
    }

    public string ElementReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Entity: {Name}");
        sb.AppendLine($"- Elements: {ElementsList.Count}");

        foreach (KoreEntityElement element in ElementsList)
        {
            sb.AppendLine($"- Element: {element.Name}");
        }

        return sb.ToString();
    }
}
