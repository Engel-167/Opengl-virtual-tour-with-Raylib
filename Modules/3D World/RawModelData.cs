namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World;

public class RawModelData
{
    public RawModelData(string path, float[] position, float[] axis, float angle, float[] scale)
    {
        Path = path;
        Position = position;
        Axis = axis;
        Angle = angle;
        Scale = scale;
    }
    
    public RawModelData()
    {
        Path = string.Empty;
        Position = new float[3];
        Axis = new float[3];
        Angle = 0f;
        Scale = new float[3];
    }

    public string Path { get; set; }
    public float[] Position { get; set; }
    public float[] Axis { get; set; }
    public float Angle { get; set; }
    public float[] Scale { get; set; }
}