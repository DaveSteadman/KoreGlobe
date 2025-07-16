using Godot;

public static class KoreGodotMaterialFactory
{
    // --------------------------------------------------------------------------------------------
    // MARK: Mature / fixed functions
    // --------------------------------------------------------------------------------------------

    // Function to create a simple colored material
    public static StandardMaterial3D SimpleColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Function to create a transparent colored material
    // Color Alpha value sets the extent of transparency
    public static StandardMaterial3D TransparentColoredMaterial(Color color)
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = color;
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;

        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: ShaderMaterial vertexColorMaterial = KoreGodotMaterialFactory.VertexColorMaterial();
    public static ShaderMaterial VertexColorMaterial()
    {
        // Create a 3D spatial shader inline - no file dependency
        string shaderCode = @"
shader_type spatial;

varying vec4 vertex_color;

void vertex() {
    vertex_color = COLOR;
}

void fragment() {
    ALBEDO = vertex_color.rgb;
    ALPHA = vertex_color.a;
}
";

        Shader shader = new Shader();
        shader.Code = shaderCode;

        ShaderMaterial material = new ShaderMaterial();
        material.Shader = shader;

        return material;
    }

    // --------------------------------------------------------------------------------------------

    // Function to create a vertex-colored transparent material
    // Uses vertex colors for both color and transparency
    // Usage: StandardMaterial3D material = KoreGodotMaterialFactory.VertexColorTransparentMaterial();
    public static StandardMaterial3D VertexColorTransparentMaterial()
    {
        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = new Color(1, 1, 1, 1); // White base color
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        material.VertexColorUseAsAlbedo = true; // Use vertex colors as the albedo
        material.NoDepthTest = false; // Keep depth testing for proper sorting
        material.CullMode = BaseMaterial3D.CullModeEnum.Back; // Standard back-face culling

        return material;
    }
}





