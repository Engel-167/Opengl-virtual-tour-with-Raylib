using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;

namespace Opengl_virtual_tour_with_Raylib
{
    static class Program
    {
        private const int ShadowmapResolution = 1024;
        
        static void Main()
        {
            // Initialize the window
            SetConfigFlags(ConfigFlags.Msaa4xHint);
            InitWindow(800, 600, "3D virtual tour");
            ToggleFullscreen();
            SetTargetFPS(60);
            
            Shader shadowShader = LoadShader("Assets/Shaders/shadowmap.vert", "Assets/Shaders/shadowmap.frag");

            unsafe
            {
                shadowShader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(shadowShader, "viewPos");
            }

            Vector3 lightDir = Vector3.Normalize(new Vector3(0.35f, -1.0f, -0.35f)); //Vector3Normalize((Vector3){ 0.35f, -1.0f, -0.35f });
            Color lightColor = RayWhite;
            Vector4 lightColorNormalized = ColorNormalize(lightColor);
            int lightDirLoc = GetShaderLocation(shadowShader, "lightDir");
            int lightColLoc = GetShaderLocation(shadowShader, "lightColor");
            Raylib.SetShaderValue(shadowShader, lightDirLoc, lightDir, ShaderUniformDataType.Vec3);
            Raylib.SetShaderValue(shadowShader, lightColLoc, lightColorNormalized, ShaderUniformDataType.Vec4);
            int ambientLoc = GetShaderLocation(shadowShader, "ambient");
            float[] ambient = new[] { 0.1f, 0.1f, 0.1f, 1.0f };
            Raylib.SetShaderValue(shadowShader, ambientLoc, ambient, ShaderUniformDataType.Vec4);
            int lightVpLoc = GetShaderLocation(shadowShader, "lightVP");
            int shadowMapLoc = GetShaderLocation(shadowShader, "shadowMap");
            int shadowMapResolution = ShadowmapResolution;
            Raylib.SetShaderValue(shadowShader, GetShaderLocation(shadowShader, "shadowMapResolution"), shadowMapResolution, ShaderUniformDataType.Int);
            
            Buildings buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
            
            Roads roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
            
            Props props = new Props("ConfigurationFiles/DATA/PropsDATA.toml");

            CameraMode camMode = CameraMode.Custom;

            foreach (ModelData building in buildings._buildingsData)
            {
                unsafe
                {
                    for (int i = 0; i < building.Model.MaterialCount; i++)
                    {
                        building.Model.Materials[i].Shader = shadowShader;
                    }

                }
            }

            foreach (ModelData road in roads._roadData)
            {
                unsafe
                {
                    for (int i = 0; i < road.Model.MaterialCount; i++)
                    {
                        road.Model.Materials[i].Shader = shadowShader;
                    }
                }
            }

            foreach (ModelData propData in props._propsData)
            {
                unsafe
                {
                    for (int i = 0; i < propData.Model.MaterialCount; i++)
                    {
                        propData.Model.Materials[i].Shader = shadowShader;
                    }
                        
                }
            }
            
            //LoadShadowmapRenderTexture(SHADOWMAP_RESOLUTION, SHADOWMAP_RESOLUTION);
            RenderTexture2D shadowMap = LoadShadowmapRenderTexture(shadowMapResolution, shadowMapResolution);
            // For the shadowmapping algorithm, we will be rendering everything from the light's point of view
            Camera3D lightCam = new Camera3D();
            lightCam.Position = Raymath.Vector3Scale(lightDir, -15.0f);//Vector3Scale(lightDir, -15.0f);
            lightCam.Target = Vector3.Zero;
            // Use an orthographic projection for directional lights
            lightCam.Projection = CameraProjection.Orthographic;
            lightCam.Up = new Vector3(0.0f, 1.0f, 0.0f); //(Vector3){ 0.0f, 1.0f, 0.0f };
            lightCam.FovY = 20.0f;
            
            while (!WindowShouldClose())
            {
                
                float dt = GetFrameTime();
                
                Vector3 cameraPos = CharacterCamera3D.Camera.Position;
                unsafe
                {
                    Raylib.SetShaderValue(shadowShader, shadowShader.Locs[(int)ShaderLocationIndex.VectorView], cameraPos, ShaderUniformDataType.Vec3);    
                }
                
                lightDir = Vector3.Normalize(lightDir);
                lightCam.Position = Raymath.Vector3Scale(lightDir, -15.0f);
                
                SetShaderValue(shadowShader, lightDirLoc, lightDir, ShaderUniformDataType.Vec3);
                
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
                
                Matrix4x4 lightView;
                Matrix4x4 lightProj;
                
                BeginTextureMode(shadowMap);
                ClearBackground(White);
                BeginMode3D(lightCam);

                    lightView = Rlgl.GetMatrixModelview();
                    lightProj = Rlgl.GetMatrixProjection();
                    
                    // Draw 3D models
                    buildings.Draw3DModels();
                    roads.Draw3DModels();
                    props.Draw3DModels();
                
                EndMode3D();
                EndTextureMode();

                Matrix4x4 lightViewProj = Raymath.MatrixMultiply(lightView, lightProj); //MatrixMultiply(lightView, lightProj);
                
                ClearBackground(RayWhite);
                
                SetShaderValueMatrix(shadowShader, lightVpLoc, lightViewProj);
                
                Rlgl.EnableShader(shadowShader.Id);

                int slot = 10;
                
                Rlgl.ActiveTextureSlot(10);
                Rlgl.EnableTexture(shadowMap.Depth.Id);

                unsafe
                {
                    Rlgl.SetUniform(shadowMapLoc, &slot, (int)ShaderUniformDataType.Int, 1);    
                }
                // Begin 3D mode
                BeginMode3D(CharacterCamera3D.Camera);
                
                    buildings.Draw3DModels();
                    roads.Draw3DModels();
                    props.Draw3DModels();
                
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
            
            UnloadShadowmapRenderTexture(shadowMap);
            CloseWindow();
        }

        static RenderTexture2D LoadShadowmapRenderTexture(int width, int height)
        {
            RenderTexture2D target = new RenderTexture2D();
            
            target.Id = Rlgl.LoadFramebuffer(); // Load an empty framebuffer
            target.Texture.Width = width;
            target.Texture.Height = height;

            if (target.Id > 0)
            {
                Rlgl.EnableFramebuffer(target.Id);

                // Create depth texture
                // We don't need a color texture for the shadowmap
                target.Depth.Id = Rlgl.LoadTextureDepth(width, height, false);
                target.Depth.Width = width;
                target.Depth.Height = height;
                target.Depth.Format = (PixelFormat)19;       //DEPTH_COMPONENT_24BIT?
                target.Depth.Mipmaps = 1;

                // Attach depth texture to FBO
                Rlgl.FramebufferAttach(target.Id, target.Depth.Id, FramebufferAttachType.Depth, FramebufferAttachTextureType.Texture2D, 0);

                // Check if fbo is complete with attachments (valid)
                if (Rlgl.FramebufferComplete(target.Id)) Raylib.TraceLog(TraceLogLevel.Info, $"FBO: {target.Id} Framebuffer object created successfully");

                Rlgl.DisableFramebuffer();
            }
            else Raylib.TraceLog(TraceLogLevel.Warning, "FBO: Framebuffer object can not be created");

            return target;
        }
        
        // Unload shadowmap render texture from GPU memory (VRAM)
        static void UnloadShadowmapRenderTexture(RenderTexture2D target)
        {
            if (target.Id > 0)
            {
                // NOTE: Depth texture/renderbuffer is automatically
                // queried and deleted before deleting framebuffer
                Rlgl.UnloadFramebuffer(target.Id);
            }
        }
    }
}