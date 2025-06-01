using System.Numerics;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;

public class Buildings(string path) : World3DObjects(path)
{
    public void DrawHitBoxes()
    {

        foreach (ModelData data in ModelDataList)
        {
            int index = ModelsPath.IndexOf(data.ModelPath);
            
            BoundingBox baseBox = Raylib.GetModelBoundingBox(Models[index]);
            BoundingBox boundingBox = new BoundingBox
            {
                Min = Vector3.Multiply(baseBox.Min, data.Scale) + data.Position,
                Max = Vector3.Multiply(baseBox.Max, data.Scale) + data.Position
            };
            
            Raylib.DrawBoundingBox(boundingBox, Color.Red);
        }
        
    }
}