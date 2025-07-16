using System;
using System.Collections.Generic;
using System.Linq;

namespace KoreCommon;


public static class KoreStringListOps
{
    // --------------------------------------------------------------------------------------------
    // MARK: Prefix Suffix
    // --------------------------------------------------------------------------------------------

    public static List<string> RemovePrefix(List<string> list, string prefix) => list.Select(item => item.StartsWith(prefix) ? item.Substring(prefix.Length) : item).ToList();
    public static List<string> RemoveSuffix(List<string> list, string suffix) => list.Select(item => item.EndsWith(suffix)   ? item.Substring(0, item.Length - suffix.Length) : item).ToList();

    // Remove content after a specified string (such as a filename extension or just a ".")
    public static List<string> RemoveAfter(List<string> list, string after)     => list.Select(item => item.Contains(after)  ? item.Substring(0, item.IndexOf(after)) : item).ToList();
    public static List<string> RemoveAfterLast(List<string> list, string after) => list.Select(item => item.Contains(after)  ? item.Substring(0, item.LastIndexOf(after)) : item).ToList();
    public static List<string> RemoveBefore(List<string> list, string before)   => list.Select(item => item.Contains(before) ? item.Substring(item.IndexOf(before) + before.Length) : item).ToList();

    // Extract content after or before the last occurrence of a specified string (e.g. getting file extensions with "." or filenames from paths with "/" or "\\")
    public static List<string> StringAfterLast(List<string> list, string after) => list.Select(item => item.Contains(after) ? item.Substring(item.LastIndexOf(after) + after.Length) : item).ToList();
    public static List<string> StringBeforeLast(List<string> list, string before) => list.Select(item => item.Contains(before) ? item.Substring(0, item.LastIndexOf(before)) : item).ToList();

    // --------------------------------------------------------------------------------------------
    // MARK: Replace
    // --------------------------------------------------------------------------------------------

    // Replace a substring with another substring in each string in the list
    // Usage: List<string> replacedList = KoreStringListOps.Replace(list, "old", "new");
    public static List<string> Replace(List<string> list, string oldSubstring, string newSubstring) => list.Select(item => item.Replace(oldSubstring, newSubstring)).ToList();

    // --------------------------------------------------------------------------------------------
    // MARK: List Order
    // --------------------------------------------------------------------------------------------

    public static List<string> OrderAlphabetically(List<string> list) => list.OrderBy(item => item).ToList();

    // --------------------------------------------------------------------------------------------
    // MARK: String Filter
    // --------------------------------------------------------------------------------------------

    // Filters the list to include only those strings that contain the specified substring
    // Usage: List<string> filteredList = KoreStringListOps.FilterIn(list, "substring");
    public static List<string> FilterIn(List<string> list, string substring) => list.Where(item => item.Contains(substring)).ToList();

    // Filters the list to exclude those strings that contain the specified substring
    public static List<string> FilterOut(List<string> list, string substring) => list.Where(item => !item.Contains(substring)).ToList();

    // Filters to return a list containing only items with stated prefix/suffix
    public static List<string> FilterSuffix(List<string> list, string suffixsubstring) => list.Where(item => item.EndsWith(suffixsubstring)).ToList();
    public static List<string> FilterPrefix(List<string> list, string prefixsubstring) => list.Where(item => item.StartsWith(prefixsubstring)).ToList();

    // --------------------------------------------------------------------------------------------
    // MARK: List Filter
    // --------------------------------------------------------------------------------------------

    // Take two lists of strings, and return a list that are not in the second list
    // Usage: List<string> omittedInSecond = KoreStringListOps.ListOmittedInSecond(list1, list2);
    public static List<string> ListOmittedInSecond(List<string> list1, List<string> list2) => list1.Except(list2).ToList();

    // Return the list of common items between both lists
    public static List<string> ListIntersection(List<string> list1, List<string> list2) => list1.Intersect(list2).ToList();

    // Return a combination of boths lists, with no duplicates
    public static List<string> ListCombined(List<string> list1, List<string> list2) => list1.Union(list2).ToList();

}
