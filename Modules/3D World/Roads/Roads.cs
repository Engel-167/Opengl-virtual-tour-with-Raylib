using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;

public class Roads(string path) : IModelsLoading
{
    public readonly List<ModelData> ModelDatas = ModelDataLoader.LoadFromToml(path);
    
    public void Draw3DModels()
    {
        foreach (ModelData model in ModelDatas)
        {
            Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale, Color.White);
        }
    }

    public void Unload3DModels()
    {
        foreach (ModelData model in ModelDatas)
        {
            Raylib.UnloadModel(model.Model);
        }
    }
}