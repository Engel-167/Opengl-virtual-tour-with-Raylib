using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class LogoScene(byte id, string windowTitle) : SceneObject(id, windowTitle)
{
    private int _framesCounter;
    
    public override void InitScene()
    {
        Console.WriteLine("Logo scene Initialized");
        _framesCounter = 0;
        Initialized = true;
    }

    public override void UpdateScene()
    {
        // Count frames
        _framesCounter++;
        
        BeginDrawing();
        
            ClearBackground(Color.Black);
            
            Texture2D logoTexture = LoadTexture("Assets/UI-UX/UNI.png");

            Rectangle destRectangle = new Rectangle(GetScreenWidth()/2.0f - logoTexture.Width/2.0f, GetScreenHeight()/2.0f - logoTexture.Height/2.0f, logoTexture.Width, logoTexture.Height);
            
            NPatchInfo patchInfo = new NPatchInfo
            {
                Source = new Rectangle(0,0, logoTexture.Width, logoTexture.Height),
                Left = 1,
                Right = 1,
                Top = 1,
                Bottom = 1,
                Layout = NPatchLayout.NinePatch
            };

            DrawTextureNPatch(logoTexture, patchInfo, destRectangle, Vector2.Zero, 0.0f, Color.White);
        
        EndDrawing();
        
        // Wait for 2 seconds (120 frames) before jumping to TITLE screen
        if (_framesCounter > 1)
        {
            UnloadTexture(logoTexture);
            Core.Globals.Scenes.CurrentScene = Core.Globals.Scenes.Scene.Home;
        }
    }

    public override void KillScene()
    {
        Console.WriteLine("Logo scene Killed");
        Initialized = false;
    }
}