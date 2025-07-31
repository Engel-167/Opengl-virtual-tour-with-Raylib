using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;

public class Buildings : World3DObjects
{
    public Model GateModel { get; private set; }
    public Model OutlineGateModel { get; private set; }
    public Buildings(string path) : base(path)
    {
        string iqmPath = "Assets/gltf/KitMedieval_v1/wall_straigth_gate_opened.iqm";
        if (ModelsPath.Contains(iqmPath))
        {
            int iqmIndex = ModelsPath.IndexOf(iqmPath);
            Texture2D texture = Raylib.LoadTexture("Assets/gltf/KitMedieval_v1/hexagons_medieval.png");
            for (int i = 0; i < Models[iqmIndex].MaterialCount; i++)
            {
                Model model = Models[iqmIndex];
                Model outlineModel = OutlineModels[iqmIndex];
                
                float radians = -MathF.PI / 2.0f;
                Matrix4x4 firstRotation = Raymath.MatrixRotate(Vector3.UnitX, radians);
                Matrix4x4 transform = Raymath.MatrixMultiply(Raymath.MatrixScale(1.0f,1.0f,1.0f), firstRotation);
                model.Transform = transform;
                outlineModel.Transform = transform;
                
                GateModel = model;
                OutlineGateModel = outlineModel;
                
                unsafe
                {
                    Animations.anims = Raylib.LoadModelAnimations(iqmPath, ref Animations.animsCount);    
                }
                OutlineModels[iqmIndex] = outlineModel;
                Raylib.SetMaterialTexture(ref model, i, MaterialMapIndex.Albedo, ref texture);
                Models[iqmIndex] = model;
            }
        }
    }

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