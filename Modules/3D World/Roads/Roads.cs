using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;

public class Roads(string path) : IModelsLoading
{
    private readonly List<ModelData> _roadData = ModelDataLoader.LoadFromToml(path);
    
    public void Draw3DModels()
    {
        foreach (ModelData model in _roadData)
        {
            Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale, Color.White);
        }
    }

    public void Unload3DModels()
    {
        foreach (ModelData model in _roadData)
        {
            Raylib.UnloadModel(model.Model);
        }
    }
}