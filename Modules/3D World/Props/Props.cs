using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;

public class Props (string path) : World3DObjects(path)
{
    //public readonly List<ModelData> ModelDatas = ModelDataLoader.LoadFromToml(path);
    
    private List<string> modelsPath = new List<string>();
    
    private List<Model> models = new List<Model>();
    
    public override void Draw3DModels()
    {
        foreach (ModelData model in ModelDatas)
        {
            if (!modelsPath.Contains(model.Model))
            {
                modelsPath.Add(model.Model);
                models.Add(Raylib.LoadModel(model.Model));
            }

            if (modelsPath.Contains(model.Model))
            {
                int index = modelsPath.IndexOf(model.Model);
                Raylib.DrawModelEx(models[index], model.Position, model.Axis, model.Angle, model.Scale, Color.White);    
            }
            
            /*// Check if the model's OBB collides with the camera's view OBB
            if (Obb.CheckCollisionBoundingBoxVsObb(model.BoundingBox, cameraViewObb))
            {
                Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale, Color.White);
            }*/
        }
    }

    public override void Unload3DModels()
    {
        foreach (Model model in models)
        {
            Raylib.UnloadModel(model);
        }
    }
}