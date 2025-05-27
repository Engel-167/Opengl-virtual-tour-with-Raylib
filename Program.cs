using Opengl_virtual_tour_with_Raylib.Modules.Core;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;

namespace Opengl_virtual_tour_with_Raylib;

static class Program
{
    static void Main()
    {
        SceneManager sceneManager = new();
        sceneManager.Start();
    }
}