namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World;

public abstract class World3DObjects(string path)
{
    public readonly List<ModelData> ModelDatas = ModelDataLoader.LoadFromToml(path);
    public abstract void Draw3DModels();
    public abstract void Unload3DModels();
}