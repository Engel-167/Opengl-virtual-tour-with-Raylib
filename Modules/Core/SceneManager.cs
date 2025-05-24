using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Scenes;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core;

public class SceneManager(int screenWidth, int screenHeight)
{
    private int ScreenWidth { get; set; } = screenWidth;
    private int ScreenHeight { get; set; } = screenHeight;

    public int Start()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        
        SetConfigFlags(ConfigFlags.VSyncHint);
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        SetConfigFlags(ConfigFlags.ResizableWindow);
        SetConfigFlags(ConfigFlags.AlwaysRunWindow);
        SetConfigFlags(ConfigFlags.BorderlessWindowMode);
        //InitWindow(ScreenWidth, ScreenHeight, "OpenGL-tour-with-Raylib");
        InitWindow(ScreenWidth, ScreenHeight, "OpenGL-tour-with-Raylib");
        SetWindowMinSize(1000, 600);
        SetExitKey(KeyboardKey.Null);
        
        AudioManager audioManager = new();
        audioManager.Initialize();
        
        MainScene mainScene = new(1, "Main Scene");
        HomeScene homeScene = new(2, "Home Scene");
        CreditsScene creditsScene = new(3, "Credits Scene");

        Variables.SettingsMenu = new SettingsUi();
        // Useful to count frames
        int framesCounter = 0;
        
        Thread.Sleep(500);
        ToggleFullscreen();
        
        //--------------------------------------------------------------------------------------
        // Main game loop
        while (!WindowShouldClose())
        {
            Variables.IsWindowResized = IsWindowResized();
            
            MouseCatcher.UpdateMouseCatcher();
            // Update
            //----------------------------------------------------------------------------------

            switch (Globals.Scenes.CurrentScene)
            {
                case Globals.Scenes.Scene.Logo:
                {
                    BeginDrawing();
                    
                        ClearBackground(Color.RayWhite);
                        DrawText("LOGO SCREEN", (1920/2) - 150, (1080/2), 40, Color.LightGray);
                        DrawText("WAIT for 2 SECONDS...", 290, 220, 20, Color.Gray);
                        
                    EndDrawing();
                    // Count frames
                    framesCounter++;

                    // Wait for 2 seconds (120 frames) before jumping to TITLE screen
                    if (framesCounter > 1)
                    {
                        Globals.Scenes.CurrentScene = Globals.Scenes.Scene.Home;
                    }

                }
                    break;
                case Globals.Scenes.Scene.Home:
                {
                    if (!homeScene.Initialized)
                    {
                        homeScene.InitScene();    
                    }

                    if (creditsScene.Initialized)
                    {
                        creditsScene.KillScene();
                    }
                    
                    homeScene.UpdateScene();
                }
                    break;
                case Globals.Scenes.Scene.Main:
                {
                    if (!mainScene.Initialized)
                    {
                        mainScene.InitScene();
                    }
                    
                    if (homeScene.Initialized)
                    {
                        homeScene.KillScene();
                    }
                    
                    mainScene.UpdateScene();
                }
                    break;
                case Globals.Scenes.Scene.Credits:
                {
                    if (!creditsScene.Initialized)
                    {
                        creditsScene.InitScene();
                    }
                    if (homeScene.Initialized)
                    {
                        homeScene.KillScene();
                    }
                    creditsScene.UpdateScene();
                }
                    break;
            }
        }
        // De-Initialization
        audioManager.Kill();
        return 0;
    }
}    