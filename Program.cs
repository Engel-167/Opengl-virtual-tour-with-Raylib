using Raylib_cs;

using Opengl_virtual_tour_with_Raylib.Modules.Scenes;

namespace Opengl_virtual_tour_with_Raylib
{
    static class Program
    {
        static void Main()
        {
            MainScene mainScene = new(1, "Main Scene");
            
            mainScene.InitScene();
            
            while (!Raylib.WindowShouldClose())
            {
                mainScene.UpdateScene();    
            }
            
            mainScene.KillScene();
        }
    }
}