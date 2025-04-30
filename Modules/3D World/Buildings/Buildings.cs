using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;

public class Buildings(string path) : IModelsLoading
{
    public List<ModelData> _buildingsData = ModelDataLoader.LoadFromToml(path);
    
    public void Draw3DModels()
    {
        foreach (ModelData model in _buildingsData)
        {
            Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale,Color.White);
        }
    }

    public void Unload3DModels()
    {
        foreach (ModelData model in _buildingsData)
        {
            Raylib.UnloadModel(model.Model);
        }
    }
}