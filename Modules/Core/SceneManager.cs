using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Settings;
using Opengl_virtual_tour_with_Raylib.Modules.Scenes;
using Opengl_virtual_tour_with_Raylib.Modules.UI_UX;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core;

public class SceneManager
{

    public int Start()
    {
        // Initialization
        //Config Flags
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        SetConfigFlags(ConfigFlags.ResizableWindow);
        SetConfigFlags(ConfigFlags.AlwaysRunWindow);
        
        //Window initialization
        InitWindow(Variables.AppSettings.ScreenWidth, Variables.AppSettings.ScreenHeight, "OpenGL-tour-with-Raylib");
        SetWindowMinSize(1000, 600);
        SetExitKey(KeyboardKey.Null);
        SetTargetFPS(60);
        
        //Audio initialization
        AudioManager audioManager = new();
        audioManager.Initialize();

        //Scenes initialization
        LogoScene logoScene = new LogoScene(0,"Logo Scene");
        MainScene mainScene = new(1, "Main Scene");
        HomeScene homeScene = new(2, "Home Scene");
        CreditsScene creditsScene = new(3, "Credits Scene");
        
        //Ui initialization
        Variables.SettingsMenu = new SettingsUi();
        Variables.HomeUi = new HomeUi();
        Variables.DialogBox = new DialogBox();

        CheckAppFirstTimeSetUp();
        
        Thread.Sleep(500);

        CheckSavedFullScreenState();
        
        Variables.AppSettings = SettingsLoader.LoadSettings(Variables.SettingsFilePath);
        
        int framesCounter = 0;
        //--------------------------------------------------------------------------------------
        // Main game loop
        while (!WindowShouldClose())
        {
            MouseCatcher.UpdateMouseCatcher();
            // Update
            //----------------------------------------------------------------------------------
            if (Variables.UpdateText)
            {
                framesCounter++;

                // 2 seconds span for drawing functions to update the Ui text
                if (framesCounter > 240)
                {
                    Variables.UpdateText = false;
                    framesCounter = 0;
                }
            }
            
            if (Variables.CanInteract && !Variables.IsSettingsMenuEnabled)
            {
                framesCounter++;

                // 2 seconds span for drawing functions to update the Ui text
                if (framesCounter > 120)
                {
                    Variables.CanInteract = false;
                    framesCounter = 0;
                }
            }
            
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
                    }

                    if (logoScene.Initialized)
                    {
                        logoScene.KillScene();
                    }
                    
                    if (creditsScene.Initialized)
                    {
                        creditsScene.KillScene();
                    }

                    if (mainScene.Initialized)
                    {
                        mainScene.KillScene();
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

    private void CheckSavedFullScreenState()
    {
        if (Variables.AppSettings.Fullscreen)
        {
            int monitor = GetCurrentMonitor();
            SetWindowSize(GetMonitorWidth(monitor), GetMonitorHeight(monitor));
            Variables.SettingsMenu?.UpdateLayout();
            Variables.HomeUi?.UpdateLayout();
            ToggleFullscreen();
            Variables.AppSettings.Fullscreen = true;
            SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
        }
    }

    private void CheckAppFirstTimeSetUp()
    {
        if (Variables.AppSettings.ScreenWidth == 0 && Variables.AppSettings.ScreenHeight == 0)
        {
            Variables.FirstTime = true;
            var currentMonitor = GetCurrentMonitor();
            Variables.AppSettings.ScreenWidth = GetMonitorWidth(currentMonitor); 
            Variables.AppSettings.ScreenHeight = GetMonitorHeight(currentMonitor);
            
            SettingsLoader.SaveSettings(Variables.SettingsFilePath, Variables.AppSettings);
            
            Console.WriteLine($"Variables width and height updated: {Variables.AppSettings.ScreenWidth}x{Variables.AppSettings.ScreenHeight}");
        }
    }
}    