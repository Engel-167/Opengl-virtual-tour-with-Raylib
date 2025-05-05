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
            
            var buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
            
            while (!WindowShouldClose())
            {
                BeginDrawing();
                ClearBackground(Color.RayWhite);
                
                // Dibujar el hitbox de la cámara
                Raylib.DrawBoundingBox(CharacterCamera3D.HitBox, Color.Blue);
                
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
                
                //Choose a mode to use the camera 
                //By default is in Tourist mode
                if (Raylib.IsKeyPressed(KeyboardKey.One)) 
                    CharacterCamera3D.Mode = CameraModeType.Tourist;
    
                if (Raylib.IsKeyPressed(KeyboardKey.Two)) 
                    CharacterCamera3D.Mode = CameraModeType.Free;
                
                if (CharacterCamera3D.Mode == CameraModeType.Tourist)
                {
                    CharacterCamera3D.HandleTouristModeInput(buildings.ModelDatas); // Movimiento del modo Turista
                }
                else
                {
                    UpdateCamera(ref CharacterCamera3D.Camera, camMode); // Movimiento del modo Libre
                }
                
                // Update CharacterCamera3D position and hitbox
                CharacterCamera3D.UpdateHitBox();
                CharacterCamera3D.ApplyCameraConstraints();
                
                ShadowMap.Update();
                
                // Begin 3D mode
                BeginMode3D(CharacterCamera3D.Camera);
                
                Draw3DModels();
                //DrawSphere(ShadowMap.GetLightCamPosition(), 1.0f, Yellow);
                // End 3D mode
                EndMode3D();

                // Draw UI
                DrawText("Colision False", 28, 10, 20, Black);
            
                DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps
                Camera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.HitBox.Min} MAX-{CharacterCamera3D.HitBox.Max}",-100,10,20,Color.Black);
                
                DrawText($@"Current Mode < {CharacterCamera3D.Mode} >", 200, 10, 20, Black);
                // End drawing
                EndDrawing();
            }
            
            ShadowMap.UnloadShadowmapRenderTexture();
            buildings.Unload3DModels();
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