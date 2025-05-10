using Opengl_virtual_tour_with_Raylib.Modules._3D_World;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public abstract class SceneObject (byte id, string windowTitle)
{
    public byte? Id = id;
    public string? WindowTitle = windowTitle;

    public abstract void InitScene();
    public abstract int UpdateScene();
    public abstract void KillScene();

    protected void Render3DModels(List<World3DObjects> worldObjects)
    {
        foreach (var currentObject in worldObjects)
        {
            currentObject.Draw3DModels();
        }
    }

    protected void Clear3DModels(List<World3DObjects> worldObjects)
    {
        foreach (var currentObject in worldObjects)
        {
            currentObject.Unload3DModels();
        }
    }
}