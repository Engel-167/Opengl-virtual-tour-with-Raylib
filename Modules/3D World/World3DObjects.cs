using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World;

public abstract class World3DObjects
{
    public readonly List<ModelData> ModelDataList;

    protected readonly List<string> ModelsPath = new();

    public readonly List<Model> Models = new();

    protected World3DObjects(string path)
    {
        ModelDataList = ModelDataLoader.LoadFromToml(path);

        foreach (var model in ModelDataList.Where(model => !ModelsPath.Contains(model.ModelPath)))
        {
            ModelsPath.Add(model.ModelPath);
            Models.Add(Raylib.LoadModel(model.ModelPath));
        }
    }

    public virtual void Draw3DModels()
    {
        foreach (var model in ModelDataList)
        {
            if (!ModelsPath.Contains(model.ModelPath))
            {
                ModelsPath.Add(model.ModelPath);
                Models.Add(Raylib.LoadModel(model.ModelPath));
            }
            
            var index = ModelsPath.IndexOf(model.ModelPath);
            Raylib.DrawModelEx(Models[index], model.Position, model.Axis, model.Angle, model.Scale, Color.White);    
            
        }
    }

    public void Unload3DModels()
    {
        foreach (Model model in Models)
        {
            Raylib.UnloadModel(model);
        }
        
        Models.Clear();
        ModelDataList.Clear();
    }
}