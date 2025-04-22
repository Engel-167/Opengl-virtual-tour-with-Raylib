using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;

namespace Opengl_virtual_tour_with_Raylib
{
    static class Program
    {
        static void Main()
        {
            // Initialize the window
            InitWindow(800, 600, "3D virtual tour");
            ToggleFullscreen();
            SetTargetFPS(60);
            
            Buildings buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
            
            Roads roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
            
            CameraMode camMode = CameraMode.Custom;

            while (!WindowShouldClose())
            {
                // Change the camera Target when the middle mouse button and the F key is pressed
                if (IsMouseButtonDown(MouseButton.Middle)||IsKeyDown(KeyboardKey.F))
                {
                    CharacterCamera3D.Camera.Target = new Vector3(0,0,0); // Make the camera look at the cube
                }
                
                // Start capturing the mouse
                if (IsMouseButtonDown(MouseButton.Left))
                {
                    camMode = CameraMode.Free;
                    DisableCursor();
                }
                
                // Update CharacterCamera3D position and hitbox
                UpdateCamera(ref CharacterCamera3D.Camera, camMode);
                CharacterCamera3D.UpdateHitBox();
                
                // Start drawing
                BeginDrawing();
                ClearBackground(Blue);

                // Begin 3D mode
                BeginMode3D(CharacterCamera3D.Camera);
                buildings.Draw3DModels();
                roads.Draw3DModels();
                // End 3D mode
                EndMode3D();

                // Draw UI
                DrawText("Colision False", 28, 10, 20, Black);
            
                DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps
                Campera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.HitBox.Min} MAX-{CharacterCamera3D.HitBox.Max}",-100,10,20,Color.Black);
                // End drawing
                EndDrawing();
            }

            CloseWindow();
        }
    }
}