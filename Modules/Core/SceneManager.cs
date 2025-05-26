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
        
        LogoScene logoScene = new LogoScene(0,"Logo Scene");
        MainScene mainScene = new(1, "Main Scene");
        HomeScene homeScene = new(2, "Home Scene");
        CreditsScene creditsScene = new(3, "Credits Scene");

        Variables.SettingsMenu = new SettingsUi();
        // Useful to count frames
        
        Thread.Sleep(500);
        ToggleFullscreen();
        
        //--------------------------------------------------------------------------------------
        // Main game loop
        while (!WindowShouldClose())
        {
            MouseCatcher.UpdateMouseCatcher();
            // Update
            //----------------------------------------------------------------------------------

            switch (Globals.Scenes.CurrentScene)
            {
                case Globals.Scenes.Scene.Logo:
                {
                    if (!logoScene.Initialized)
                    {
                        logoScene.InitScene();
                    }
                    
                    logoScene.UpdateScene();
                }
                    break;
                case Globals.Scenes.Scene.Home:
                {
                    if (!homeScene.Initialized)
                    {
                        homeScene.InitScene();
                        Variables.CurrentBgMusic = 1;
                    }

                    if (logoScene.Initialized)
                    {
                        logoScene.KillScene();
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
                        Variables.CurrentBgMusic = 0;   
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