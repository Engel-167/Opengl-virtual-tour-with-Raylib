using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;

public class Props (string path) : World3DObjects(path)
{
    //public readonly List<ModelData> ModelDatas = ModelDataLoader.LoadFromToml(path);
    
    public override void Draw3DModels()
    {
        foreach (ModelData data in ModelDatas)
        {
            Raylib.DrawModelEx(data.Model, data.Position, data.Axis, data.Angle, data.Scale,Color.White);
        }
    }

    public override void Unload3DModels()
    {
        foreach (ModelData data in ModelDatas)
        {
            Raylib.UnloadModel(data.Model);
        }
    }
}