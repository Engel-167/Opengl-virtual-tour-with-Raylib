using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;

public class Roads(string path) : World3DObjects(path)
{
    public override void Draw3DModels()
    {
        foreach (ModelData model in ModelDataList)
        {
            if (!ModelsPath.Contains(model.ModelPath))
            {
                ModelsPath.Add(model.ModelPath);
                Models.Add(Raylib.LoadModel(model.ModelPath));
            }
            
            int index = ModelsPath.IndexOf(model.ModelPath);
            Raylib.DrawModelEx(Models[index], model.Position, Vector3.UnitY, model.Angle, model.Scale, Color.White);    
        }
    }
}