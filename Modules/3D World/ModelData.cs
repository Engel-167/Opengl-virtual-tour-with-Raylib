using System.Numerics;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World
{
    
    public class ModelData(RawModelData data)
    {
        public String ModelPath { get; set; } = data.Path;
        public Vector3 Position { get; set; } = new(data.Position[0], data.Position[1], data.Position[2]);
        public Vector3 Axis { get; set; } = new(data.Axis[0], data.Axis[1], data.Axis[2]);
        public float Angle { get; set; } = data.Angle;
        public Vector3 Scale { get; set; } = new(data.Scale[0], data.Scale[1], data.Scale[2]);
        public string AssetName { get; set; } = Path.GetFileNameWithoutExtension(data.Path);
    }

}