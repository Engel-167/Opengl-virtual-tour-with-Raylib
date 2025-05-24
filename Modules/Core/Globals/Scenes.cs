namespace Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

public static class Scenes
{
    public enum Scene
    {
        Logo = 0,
        Home = 1,
        Main = 2,
        Credits = 3
    }

    public static Scene CurrentScene = Scene.Logo;
}