using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#nullable enable

namespace KoreCommon;

// OBJ and MTL file format export/import for KoreMeshData
// OBJ format is a simple text-based 3D geometry format widely supported by 3D applications
// MTL format defines materials referenced by OBJ files

public static partial class KoreMeshDataIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: OBJ Export
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Export KoreMeshData to OBJ format string
    /// </summary>
    /// <param name="mesh">The mesh data to export</param>
    /// <param name="objectName">Name for the object in the OBJ file</param>
    /// <param name="mtlFileName">Name of the MTL file (without extension)</param>
    /// <returns>OBJ file content as string</returns>
    public static string ToObj(KoreMeshData mesh, string objectName = "KoreMesh", string? mtlFileName = null)
    {
        var sb = new StringBuilder();
        
        // OBJ Header
        sb.AppendLine("# OBJ file exported from KoreMeshData");
        sb.AppendLine($"# Object: {objectName}");
        sb.AppendLine();
        
        // Reference MTL file if materials exist and MTL filename provided
        if (mesh.Materials.Count > 0 && !string.IsNullOrEmpty(mtlFileName))
        {
            sb.AppendLine($"mtllib {mtlFileName}.mtl");
            sb.AppendLine();
        }
        
        sb.AppendLine($"o {objectName}");
        sb.AppendLine();
        
        // Export vertices (v x y z)
        // Create a mapping from internal vertex IDs to OBJ vertex indices (1-based)
        var vertexIdToObjIndex = new Dictionary<int, int>();
        int objVertexIndex = 1;
        
        foreach (var kvp in mesh.Vertices.OrderBy(x => x.Key))
        {
            int vertexId = kvp.Key;
            KoreXYZVector pos = kvp.Value;
            
            vertexIdToObjIndex[vertexId] = objVertexIndex++;
            sb.AppendLine($"v {pos.X.ToString("F6", CultureInfo.InvariantCulture)} {pos.Y.ToString("F6", CultureInfo.InvariantCulture)} {pos.Z.ToString("F6", CultureInfo.InvariantCulture)}");
        }
        sb.AppendLine();
        
        // Export normals if they exist (vn x y z)
        var hasNormals = mesh.Normals.Count > 0;
        if (hasNormals)
        {
            foreach (var kvp in mesh.Vertices.OrderBy(x => x.Key))
            {
                int vertexId = kvp.Key;
                if (mesh.Normals.TryGetValue(vertexId, out KoreXYZVector normal))
                {
                    sb.AppendLine($"vn {normal.X.ToString("F6", CultureInfo.InvariantCulture)} {normal.Y.ToString("F6", CultureInfo.InvariantCulture)} {normal.Z.ToString("F6", CultureInfo.InvariantCulture)}");
                }
                else
                {
                    // Default normal if missing
                    sb.AppendLine("vn 0.0 1.0 0.0");
                }
            }
            sb.AppendLine();
        }
        
        // Export texture coordinates if they exist (vt u v)
        var hasUVs = mesh.UVs.Count > 0;
        if (hasUVs)
        {
            foreach (var kvp in mesh.Vertices.OrderBy(x => x.Key))
            {
                int vertexId = kvp.Key;
                if (mesh.UVs.TryGetValue(vertexId, out KoreXYVector uv))
                {
                    sb.AppendLine($"vt {uv.X.ToString("F6", CultureInfo.InvariantCulture)} {uv.Y.ToString("F6", CultureInfo.InvariantCulture)}");
                }
                else
                {
                    // Default UV if missing
                    sb.AppendLine("vt 0.0 0.0");
                }
            }
            sb.AppendLine();
        }
        
        // Export triangles grouped by material
        if (mesh.Triangles.Count > 0)
        {
            ExportTrianglesByMaterial(mesh, sb, vertexIdToObjIndex, hasNormals, hasUVs);
        }
        
        // Export lines as line elements (l v1 v2)
        if (mesh.Lines.Count > 0)
        {
            sb.AppendLine("# Lines");
            foreach (var kvp in mesh.Lines.OrderBy(x => x.Key))
            {
                KoreMeshLine line = kvp.Value;
                if (vertexIdToObjIndex.TryGetValue(line.A, out int objIndexA) && 
                    vertexIdToObjIndex.TryGetValue(line.B, out int objIndexB))
                {
                    sb.AppendLine($"l {objIndexA} {objIndexB}");
                }
            }
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
    
    private static void ExportTrianglesByMaterial(KoreMeshData mesh, StringBuilder sb, Dictionary<int, int> vertexIdToObjIndex, bool hasNormals, bool hasUVs)
    {
        // Group triangles by material
        var trianglesByMaterial = new Dictionary<string, List<KoreMeshTriangle>>();
        
        // First, try to use named triangle groups which have material assignments
        foreach (var groupKvp in mesh.NamedTriangleGroups)
        {
            string groupName = groupKvp.Key;
            KoreMeshTriangleGroup group = groupKvp.Value;
            
            // Get material name
            string materialName = "default";
            if (mesh.Materials.TryGetValue(group.MaterialId, out KoreMeshMaterial material))
            {
                materialName = material.Name;
            }
            
            if (!trianglesByMaterial.ContainsKey(materialName))
                trianglesByMaterial[materialName] = new List<KoreMeshTriangle>();
            
            // Add triangles from this group
            foreach (int triangleId in group.TriangleIds)
            {
                if (mesh.Triangles.TryGetValue(triangleId, out KoreMeshTriangle triangle))
                {
                    trianglesByMaterial[materialName].Add(triangle);
                }
            }
        }
        
        // Add any remaining triangles not in named groups
        var usedTriangleIds = new HashSet<int>();
        foreach (var group in mesh.NamedTriangleGroups.Values)
        {
            foreach (int triangleId in group.TriangleIds)
                usedTriangleIds.Add(triangleId);
        }
        
        var ungroupedTriangles = mesh.Triangles.Where(kvp => !usedTriangleIds.Contains(kvp.Key)).Select(kvp => kvp.Value).ToList();
        if (ungroupedTriangles.Count > 0)
        {
            trianglesByMaterial["default"] = ungroupedTriangles;
        }
        
        // Export triangles grouped by material
        foreach (var materialGroup in trianglesByMaterial)
        {
            string materialName = materialGroup.Key;
            List<KoreMeshTriangle> triangles = materialGroup.Value;
            
            if (triangles.Count == 0) continue;
            
            sb.AppendLine($"# Material: {materialName}");
            if (materialName != "default")
            {
                sb.AppendLine($"usemtl {materialName}");
            }
            
            foreach (var triangle in triangles)
            {
                if (vertexIdToObjIndex.TryGetValue(triangle.A, out int objIndexA) &&
                    vertexIdToObjIndex.TryGetValue(triangle.B, out int objIndexB) &&
                    vertexIdToObjIndex.TryGetValue(triangle.C, out int objIndexC))
                {
                    // Format: f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3
                    if (hasNormals && hasUVs)
                    {
                        sb.AppendLine($"f {objIndexA}/{objIndexA}/{objIndexA} {objIndexB}/{objIndexB}/{objIndexB} {objIndexC}/{objIndexC}/{objIndexC}");
                    }
                    else if (hasNormals)
                    {
                        sb.AppendLine($"f {objIndexA}//{objIndexA} {objIndexB}//{objIndexB} {objIndexC}//{objIndexC}");
                    }
                    else if (hasUVs)
                    {
                        sb.AppendLine($"f {objIndexA}/{objIndexA} {objIndexB}/{objIndexB} {objIndexC}/{objIndexC}");
                    }
                    else
                    {
                        sb.AppendLine($"f {objIndexA} {objIndexB} {objIndexC}");
                    }
                }
            }
            sb.AppendLine();
        }
    }
    
    // --------------------------------------------------------------------------------------------
    // MARK: MTL Export
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Export materials to MTL format string
    /// </summary>
    /// <param name="mesh">The mesh data containing materials</param>
    /// <returns>MTL file content as string</returns>
    public static string ToMtl(KoreMeshData mesh)
    {
        var sb = new StringBuilder();
        
        // MTL Header
        sb.AppendLine("# MTL file exported from KoreMeshData");
        sb.AppendLine();
        
        foreach (var kvp in mesh.Materials.OrderBy(x => x.Key))
        {
            KoreMeshMaterial material = kvp.Value;
            
            sb.AppendLine($"newmtl {material.Name}");
            
            // Ambient color (usually same as diffuse)
            float r = material.BaseColor.R / 255.0f;
            float g = material.BaseColor.G / 255.0f;
            float b = material.BaseColor.B / 255.0f;
            
            sb.AppendLine($"Ka {r.ToString("F6", CultureInfo.InvariantCulture)} {g.ToString("F6", CultureInfo.InvariantCulture)} {b.ToString("F6", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Kd {r.ToString("F6", CultureInfo.InvariantCulture)} {g.ToString("F6", CultureInfo.InvariantCulture)} {b.ToString("F6", CultureInfo.InvariantCulture)}");
            
            // Specular color (white for metals, darker for non-metals)
            float specular = material.Metallic;
            sb.AppendLine($"Ks {specular.ToString("F6", CultureInfo.InvariantCulture)} {specular.ToString("F6", CultureInfo.InvariantCulture)} {specular.ToString("F6", CultureInfo.InvariantCulture)}");
            
            // Specular exponent (inversely related to roughness)
            float shininess = 1.0f + (1.0f - material.Roughness) * 199.0f; // Linear interpolation from 1 to 200
            sb.AppendLine($"Ns {shininess.ToString("F2", CultureInfo.InvariantCulture)}");
            
            // Transparency (from alpha channel)
            float alpha = material.BaseColor.A / 255.0f;
            if (alpha < 1.0f)
            {
                sb.AppendLine($"d {alpha.ToString("F6", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"Tr {(1.0f - alpha).ToString("F6", CultureInfo.InvariantCulture)}");
            }
            
            // Illumination model (2 = color on and ambient on and highlight on)
            sb.AppendLine("illum 2");
            
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
    
    // --------------------------------------------------------------------------------------------
    // MARK: Convenience Methods
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Export mesh to both OBJ and MTL format strings
    /// </summary>
    /// <param name="mesh">The mesh data to export</param>
    /// <param name="objectName">Name for the object</param>
    /// <param name="mtlFileName">Name of the MTL file (without extension)</param>
    /// <returns>Tuple containing (objContent, mtlContent)</returns>
    public static (string objContent, string mtlContent) ToObjMtl(KoreMeshData mesh, string objectName = "KoreMesh", string mtlFileName = "materials")
    {
        string objContent = ToObj(mesh, objectName, mtlFileName);
        string mtlContent = ToMtl(mesh);
        return (objContent, mtlContent);
    }
}
