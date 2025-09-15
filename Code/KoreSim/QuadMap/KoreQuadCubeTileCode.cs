

using System.Collections.Generic;
using KoreCommon;

namespace KoreSim;

// Define the tilecode for a quad cube map tile
// - The initial face is defined as Top, bottom, Left, Right, Front, Back
//  - A 0,0 lat long runs through the center of the Front face
// - Each face is divided into 4, equal quadrants, numbered 0-3
//      0 | 1
//      -----
//      2 | 3
// An overall tilecode string could read "Frt012" for Front face, quadrants 0 then 1 then 2

public struct KoreQuadCubeTileCode
{
    public KoreQuadFace.CubeFace Face { get; set; } = KoreQuadFace.CubeFace.Front;
    public List<int> Quadrants { get; set; } = new();

    public KoreQuadCubeTileCode() { }
    
    public static KoreQuadCubeTileCode Zero => new KoreQuadCubeTileCode();

    // --------------------------------------------------------------------------------------------

    public string CodeToString()
    {
        string faceStr = KoreQuadFace.CubeFaceNames[Face];
        string quadStr = string.Join("", Quadrants);

        return faceStr + quadStr;
    }

    public static (bool, KoreQuadCubeTileCode) CodeFromString(string code)
    {
        bool success = false;
        KoreQuadCubeTileCode newCode = new();

        // if the string is less than three characters, it's plainly invalid
        if (code.Length < 3) return (false, newCode);
        
        // Extract the face part (first three characters)
        string facePart = code.Substring(0, 3);
        string quadPart = code.Substring(3);

        // Determine the face
        foreach (var kvp in KoreQuadFace.CubeFaceNames)
        {
            if (kvp.Value == facePart)
            {
                newCode.Face = kvp.Key;
                success = true;
                break;
            }
        }

        // Determine the quadrants
        if (success && !string.IsNullOrEmpty(quadPart))
        {
            foreach (char c in quadPart)
            {
                // if the character is not 0-3, it's invalid and we fail the whole operation
                if (c < '0' || c > '3')
                {
                    return (false, newCode);
                }

                if (int.TryParse(c.ToString(), out int quad))
                {
                    newCode.Quadrants.Add(quad);
                }
            }
        }

        return (success, newCode);
    }
    

}