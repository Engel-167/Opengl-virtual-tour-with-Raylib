using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;

public class Props (string path) : IModelsLoading
{
    public List<ModelData> _propsData = ModelDataLoader.LoadFromToml(path);
    
    public void Draw3DModels()
    {
        foreach (ModelData data in _propsData)
        {
            Raylib.DrawModelEx(data.Model, data.Position, data.Axis, data.Angle, data.Scale,Color.White);
        }
    }

    public void Unload3DModels()
    {
        foreach (ModelData data in _propsData)
        {
            Raylib.UnloadModel(data.Model);
        }
    }
}