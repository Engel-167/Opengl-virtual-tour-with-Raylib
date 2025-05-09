using Opengl_virtual_tour_with_Raylib.Modules.Core;
using Raylib_cs;

namespace Opengl_virtual_tour_with_Raylib
{
    static class Program
    {
        static void Main()
        {
            SceneManager sceneManager = new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            sceneManager.Start();
        }
    }
}