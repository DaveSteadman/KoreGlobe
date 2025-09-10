

using System.Collections.Generic;

using KoreCommon;

public class KoreWorldObject
{
    public string Name;
    public KoreMiniMesh Mesh;

    // --------------------------------------------------------------------------------------------
    
}




// KoreWorldObjectManager: Loads meshes and creates an instance that can then be used in the world

public class KoreWorldObjectManager
{
    public List<KoreWorldObject> WorldObjectList = new();

    public List<KoreWorldObject> InstanceList = new();

    public void LoadObject(string name, KoreMiniMesh mesh)
    {
        KoreWorldObject worldObject = new()
        {
            Name = name,
            Mesh = mesh
        };
        WorldObjectList.Add(worldObject);
    }
    
    


}


