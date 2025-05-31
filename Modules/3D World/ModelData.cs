using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World
{
    
    public class ModelData
    {
        public String Model { get; set; }
        
        private Raylib_cs.Model _loadedModel;
        public Vector3 Position { get; set; }
        public Vector3 Axis { get; set; }
        public float Angle { get; set; }
        public Vector3 Scale { get; set; }
        public BoundingBox BoundingBox { get; private set; }
        public string AssetName { get; set; }

        public ModelData(RawModelData data)
        {
            Model = data.Path;
            _loadedModel = Raylib.LoadModel(Model);
            Position = new Vector3(data.Position[0], data.Position[1], data.Position[2]);
            Axis = new Vector3(data.Axis[0], data.Axis[1], data.Axis[2]);
            Angle = data.Angle;
            Scale = new Vector3(data.Scale[0], data.Scale[1], data.Scale[2]);
            AssetName = Path.GetFileNameWithoutExtension(data.Path);

            
            BoundingBox baseBox = Raylib.GetModelBoundingBox(_loadedModel);
            BoundingBox = new BoundingBox
            {
                Min = Vector3.Multiply(baseBox.Min, Scale) + Position,
                Max = Vector3.Multiply(baseBox.Max, Scale) + Position
            };
        }

        public void Draw()
        {
            //Raylib.DrawModelEx(Model, Position, Axis, Angle, Scale, Color.White);
        }
    }

}