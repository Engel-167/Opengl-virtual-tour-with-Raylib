using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib.Modules._3D_World;

public abstract class World3DObjects
{
    protected readonly List<ModelData> ModelDataList;

    protected readonly List<string> ModelsPath = new();

    public readonly List<Model> Models = new();
    
    public readonly List<Model> OutlineModels = new();
    protected float ScaleFactor = 0.01f; // Adjust this value to change the outline thickness

    protected World3DObjects(string path)
    {
        ModelDataList = ModelDataLoader.LoadFromToml(path);

        foreach (var model in ModelDataList.Where(model => !ModelsPath.Contains(model.ModelPath)))
        {
            ModelsPath.Add(model.ModelPath);
            Models.Add(Raylib.LoadModel(model.ModelPath));
            OutlineModels.Add(Raylib.LoadModel(model.ModelPath));
        }
        
        // Load all Models from list Models in to the list OutlineModels and change the color to black for each model
        foreach (var model in OutlineModels)
        {
            unsafe
            {
                model.Materials[0].Maps[0].Color = Color.Black; // Change the color of the model to black
            }
        }
    }

    public virtual void Draw3DModels()
    {
        foreach (var model in ModelDataList)
        {
            /*if (!ModelsPath.Contains(model.ModelPath))
            {
                ModelsPath.Add(model.ModelPath);
                Models.Add(Raylib.LoadModel(model.ModelPath));
            }*/
            
            var index = ModelsPath.IndexOf(model.ModelPath);
            Raylib.DrawModelEx(Models[index], model.Position, model.Axis, model.Angle, model.Scale, Color.White);    
            
        }

        if (!Variables.AppSettings.OutlineEnabled)
        {
            return;
        }
        
        // Draw the outline of the models
        foreach (var model in ModelDataList)
        {
            if (!ModelsPath.Contains(model.ModelPath))
            {
                ModelsPath.Add(model.ModelPath);
                Models.Add(Raylib.LoadModel(model.ModelPath));
            }
            
            var index = ModelsPath.IndexOf(model.ModelPath);
            
            Rlgl.SetCullFace(0); // Disable culling to draw outlines correctly
            Raylib.DrawModelEx(OutlineModels[index], model.Position, model.Axis, model.Angle, model.Scale + new Vector3(ScaleFactor), Color.Black);
            Rlgl.SetCullFace(1); // Re-enable culling
        }
        
        /*List<Matrix4x4> transformations = new List<Matrix4x4>();
        
        foreach (var target in ModelsPath)
        {
            foreach (var obj in ModelDataList)
            {
                if (obj.ModelPath == target)
                {
                    transformations.Add(Matrix4x4.CreateScale(obj.Scale) * Matrix4x4.CreateFromAxisAngle(obj.Axis, obj.Angle) * Matrix4x4.CreateTranslation(obj.Position));
                }
            }

            unsafe
            {
                Mesh mesh = Models[ModelsPath.IndexOf(target)].Meshes[0];
                Material material = Models[ModelsPath.IndexOf(target)].Materials[0];
                Matrix4x4[] transformationArray = transformations.ToArray();
                
                Raylib.DrawMeshInstanced(mesh, material, transformationArray, transformations.Count);
            }
        }*/
        
    }


    public void Unload3DModels()
    {
        foreach (Model model in Models)
        {
            Raylib.UnloadModel(model);
        }

        foreach (Model model in OutlineModels)
        {
            Raylib.UnloadModel(model);
        }
        
        Models.Clear();
        OutlineModels.Clear();
        ModelDataList.Clear();
    }
}