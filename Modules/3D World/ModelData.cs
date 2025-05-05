using System.Numerics;
using Raylib_cs;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World
{
    
    public class ModelData
    {
        public Model Model { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Axis { get; set; }
        public float Angle { get; set; }
        public Vector3 Scale { get; set; }
        public BoundingBox BoundingBox { get; private set; }

        public ModelData(RawModelData data)
        {
            Model = Raylib.LoadModel(data.Path);
            Position = new Vector3(data.Position[0], data.Position[1], data.Position[2]);
            Axis = new Vector3(data.Axis[0], data.Axis[1], data.Axis[2]);
            Angle = data.Angle;
            Scale = new Vector3(data.Scale[0], data.Scale[1], data.Scale[2]);

            BoundingBox baseBox = Raylib.GetModelBoundingBox(Model);
            BoundingBox = new BoundingBox
            {
                Min = Vector3.Multiply(baseBox.Min, Scale) + Position,
                Max = Vector3.Multiply(baseBox.Max, Scale) + Position
            };
        }

        public void Draw()
        {
            Raylib.DrawModelEx(Model, Position, Axis, Angle, Scale, Color.White);
        }
    }

}