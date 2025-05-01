using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;

namespace Opengl_virtual_tour_with_Raylib
{
    static class Program
    {
        private static readonly Buildings Buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
            
        private static readonly Roads Roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
            
        private static readonly Props Props = new Props("ConfigurationFiles/DATA/PropsDATA.toml");
        
        static void Main()
        {
            // Initialize the window
            SetConfigFlags(ConfigFlags.Msaa4xHint);
            InitWindow(800, 600, "3D virtual tour");
            ToggleFullscreen();
            SetTargetFPS(60);

            CameraMode camMode = CameraMode.Custom;

            ShadowMap.BindShader(Buildings.ModelDatas);

            ShadowMap.BindShader(Roads.ModelDatas);

            ShadowMap.BindShader(Props.ModelDatas);
            
            ShadowMap.Init();
            
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
                
                ShadowMap.Update();
                // Begin 3D mode
                BeginMode3D(CharacterCamera3D.Camera);
                
                Draw3DModels();
                
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
            
            ShadowMap.UnloadShadowmapRenderTexture();
            CloseWindow();
        }

        public static void Draw3DModels()
        {
            Buildings.Draw3DModels();
            Roads.Draw3DModels();
            Props.Draw3DModels();
        }
    }
}