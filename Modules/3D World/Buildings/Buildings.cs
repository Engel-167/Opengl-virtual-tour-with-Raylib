using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;

public class Buildings(string path) : World3DObjects(path)
{
    //public readonly List<ModelData> ModelDatas = ModelDataLoader.LoadFromToml(path);
    
    public override void Draw3DModels()
    {
        foreach (ModelData model in ModelDatas)
        {
            Raylib.DrawModelEx(model.Model, model.Position, model.Axis, model.Angle, model.Scale,Color.White);
            
            // Dibujar la hitbox del modelo para depuración
            Raylib.DrawBoundingBox(model.BoundingBox, Color.Red);
        }
    }

    public override void Unload3DModels()
    {
        foreach (ModelData model in ModelDatas)
        {
            Raylib.UnloadModel(model.Model);
        }
    }
}