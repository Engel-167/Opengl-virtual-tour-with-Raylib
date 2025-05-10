using Opengl_virtual_tour_with_Raylib.Modules.Scenes;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Core;

enum Scene
{
    Logo = 0,
    Home = 1,
    Main = 2,
    Credits = 3
}

public class SceneManager(int screenWidth, int screenHeight)
{
    private int ScreenWidth { get; set; } = screenWidth;
    private int ScreenHeight { get; set; } = screenHeight;

    public int Start()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        
        InitWindow(ScreenWidth, ScreenHeight, "OpenGL-tour-with-Raylib");
        ToggleFullscreen();
        SetConfigFlags(ConfigFlags.VSyncHint);
        SetConfigFlags(ConfigFlags.Msaa4xHint);
        
        SetExitKey(KeyboardKey.Null);

        MainScene mainScene = new(1, "Main Scene");
        HomeScene homeScene = new(2, "Home Scene");
        
        Scene currentScreen = Scene.Logo;

        // TODO: Initialize all required variables and load all required data here!

        // Useful to count frames
        int framesCounter = 0;

        SetTargetFPS(60); // Set desired framerate (frames-per-second)
        //--------------------------------------------------------------------------------------
        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            switch (currentScreen)
            {
                case Scene.Logo:
                {
                    // TODO: Update LOGO screen variables here!

                    // Count frames
                    framesCounter++;

                    // Wait for 2 seconds (120 frames) before jumping to TITLE screen
                    if (framesCounter > 120)
                    {
                        currentScreen = Scene.Home;
                        homeScene.InitScene();
                    }
                }
                    break;
                case Scene.Home:
                {
                    // TODO: Update TITLE screen variables here!

                    // Press enter to change to GAMEPLAY screen
                    if (IsKeyPressed(KeyboardKey.Enter))
                    {
                        currentScreen = Scene.Main;
                        mainScene.InitScene();
                        homeScene.KillScene();
                    }
                }
                    break;
                case Scene.Main:
                {
                    // TODO: Update GAMEPLAY screen variables here!

                    // Press enter to change to ENDING screen
                    if (IsKeyPressed(KeyboardKey.Escape))
                    {
                        currentScreen = Scene.Credits;
                        mainScene.KillScene();
                    }
                }
                    break;
                case Scene.Credits:
                {
                    // TODO: Update ENDING screen variables here!
                    
                    // Press Escape to return to exit the program
                    SetExitKey(KeyboardKey.Escape);
                }
                    break;
            }
            
            BeginDrawing();

            ClearBackground(Color.RayWhite);

            switch (currentScreen)
            {
                case Scene.Logo:
                {
                    // TODO: Draw LOGO screen here!
                    DrawText("LOGO SCREEN", (1920/2) - 150, (1080/2), 40, Color.LightGray);
                    DrawText("WAIT for 2 SECONDS...", 290, 220, 20, Color.Gray);

                }
                    break;
                case Scene.Home:
                {
                    if (homeScene.UpdateScene() == 1)
                    {
                        currentScreen = Scene.Main;
                        mainScene.InitScene();
                        homeScene.KillScene();
                    }
                }
                    break;
                case Scene.Main:
                {
                    mainScene.UpdateScene();
                }
                    break;
                case Scene.Credits:
                {
                    // TODO: Draw ENDING screen here!
                    DrawRectangle(0, 0, ScreenWidth, ScreenHeight, Color.Blue);
                    DrawText("CREDITS SCREEN", 20, 20, 40, Color.DarkBlue);
                    DrawText("PRESS ENTER or TAP to RETURN to TITLE SCREEN", 120, 220, 20, Color.DarkBlue);

                }
                    break;
            }
            EndDrawing();
        }
        return 0;
    }
}    