using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
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
        static void Main()
        {
            // Initialize the window
            SetConfigFlags(ConfigFlags.Msaa4xHint);
            InitWindow(800, 600, "3D virtual tour");
            ToggleFullscreen();
            SetTargetFPS(60);
            
            Buildings buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
            
            Roads roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
            
            Props props = new Props("ConfigurationFiles/DATA/PropsDATA.toml");
            
            CameraMode camMode = CameraMode.Custom;

            Shader shader = LoadShader("Assets/Shaders/lighting.vert", "Assets/Shaders/lighting.frag");
            
            // Get some required shader loactions
            unsafe
            {
                shader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shader, "viewPos");
            }

            // ambient light level
            int ambientLoc = GetShaderLocation(shader, "ambient");
            float[] ambient = [0.1f, 0.1f, 0.1f, 1.0f];
            Raylib.SetShaderValue(shader, ambientLoc, ambient, ShaderUniformDataType.Vec4);
            
            foreach (ModelData building in buildings._buildingsData)
            {
                unsafe
                {
                    for (int i = 0; i < building.Model.MaterialCount; i++)
                    {
                        building.Model.Materials[i].Shader = shader;
                    }

                }
            }

            foreach (ModelData road in roads._roadData)
            {
                unsafe
                {
                    for (int i = 0; i < road.Model.MaterialCount; i++)
                    {
                        road.Model.Materials[i].Shader = shader;
                    }
                }
            }

            foreach (ModelData propData in props._propsData)
            {
                unsafe
                {
                    for (int i = 0; i < propData.Model.MaterialCount; i++)
                    {
                        propData.Model.Materials[i].Shader = shader;
                    }
                        
                }
            }

            Light light = Rlights.CreateLight(
                0,
                LightType.Point,
                new Vector3(0, 5, 2),
                Vector3.Zero,
                Color.RayWhite,
                shader
            );

            
            
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
                
                Rlights.UpdateLightValues(shader, light);
                
                // Update the light shader with the camera view position
                unsafe
                {
                    Raylib.SetShaderValue(
                        shader,
                        shader.Locs[(int)ShaderLocationIndex.VectorView],
                        CharacterCamera3D.Camera.Position,
                        ShaderUniformDataType.Vec3
                    );    
                }
                
                if (IsKeyPressed(KeyboardKey.L))
                {
                    light.Enabled = !light.Enabled;
                }
                
                // if arrow up is pressed move the light to up
                if (IsKeyDown(KeyboardKey.Up))
                    light.Position += new Vector3(0, 0.1f, 0);
                
                // if arrow down is pressed move the light to down
                if (IsKeyDown(KeyboardKey.Down))
                    light.Position += new Vector3(0, -0.1f, 0);
                // if arrow right is pressed move the light to right
                if (IsKeyDown(KeyboardKey.Right))
                    light.Position += new Vector3(0.1f, 0, 0);
                // if arrow left is pressed move the light to left
                if (IsKeyDown(KeyboardKey.Left))
                    light.Position += new Vector3(-0.1f, 0, 0);
                
                
                // Start drawing
                BeginDrawing();
                ClearBackground(Blue);

                // Begin 3D mode
                BeginMode3D(CharacterCamera3D.Camera);
                buildings.Draw3DModels();
                roads.Draw3DModels();
                props.Draw3DModels();
                
                DrawSphereEx(light.Position, 0.2f, 8, 8, Color.Orange);
                
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