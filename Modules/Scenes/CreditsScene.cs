using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class CreditsScene(byte id, string windowTitle) : SceneObject(id, windowTitle)
{

    public override void InitScene()
    {
        Console.WriteLine("Credits Scene Initialized");
        Initialized = true;
    }

    public override void UpdateScene()
    {
        if (IsKeyPressed(KeyboardKey.Escape))
        {
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Home;
        }
        BeginDrawing();
            DrawRectangle(0, 0, GetScreenWidth(), GetScreenHeight(), Color.White);
            DrawText("CREDITS SCREEN", 20, 20, 40, Color.DarkBlue);
            DrawText("PRESS ESCAPE to RETURN to HOME SCREEN", 120, 220, 20, Color.DarkBlue);
        EndDrawing();
    }

    public override void KillScene()
    {
        Console.WriteLine("Credits Scene Unloaded");
        Initialized = false;
    }
}