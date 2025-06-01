using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Lighting;

public static unsafe class ShadowMap
{
    private static readonly Shader ShadowShader = LoadShader("Assets/Shaders/shadowmap.vert", "Assets/Shaders/shadowmap.frag");
    
    private const int ShadowmapResolution = 1080;//4096

    private static Vector3 _lightDir;
    private static int _lightDirLoc;
    private static Camera3D _lightCam; 
    private static RenderTexture2D _shadowMap;
    private static int _lightVpLoc;
    private static int _shadowMapLoc;

    public static bool Enabled = true;
    
    public static void Init(List<World3DObjects> worldObjects)
    {
        
        foreach (var obj in worldObjects)
        {
            BindShader(obj.Models);
        }

        // ShadowMap.BindShader(materials);
        
        ShadowShader.Locs[(int)ShaderLocationIndex.VectorView] = GetShaderLocation(ShadowShader, "viewPos");
        
        _lightDir = Raymath.Vector3Normalize(new Vector3(0.35f, -1.0f, -0.35f)); //Vector3Normalize((Vector3){ 0.35f, -1.0f, -0.35f });
        Color lightColor = new Color(255, 197, 143, 255); //40 W Tungsten: new Color(255,197,143,255); High Pressure Sodium: new Color(255, 183, 76,255)
        Vector4 lightColorNormalized = ColorNormalize(lightColor);
        _lightDirLoc = GetShaderLocation(ShadowShader, "lightDir");
        int lightColLoc = GetShaderLocation(ShadowShader, "lightColor");
        SetShaderValue(ShadowShader, _lightDirLoc, _lightDir, ShaderUniformDataType.Vec3);
        SetShaderValue(ShadowShader, lightColLoc, lightColorNormalized, ShaderUniformDataType.Vec4);
        
        // Set shader light values
        int ambientLoc = GetShaderLocation(ShadowShader, "ambient");
        float[] ambient = [0.5f, 0.5f, 0.5f, 1.0f];
        SetShaderValue(ShadowShader, ambientLoc, ambient, ShaderUniformDataType.Vec4);
        _lightVpLoc = GetShaderLocation(ShadowShader, "lightVP");
        _shadowMapLoc = GetShaderLocation(ShadowShader, "shadowMap");
        int shadowMapResolution = ShadowmapResolution;
        SetShaderValue(ShadowShader, GetShaderLocation(ShadowShader, "shadowMapResolution"), shadowMapResolution, ShaderUniformDataType.Int);
        
        //LoadShadowmapRenderTexture(SHADOWMAP_RESOLUTION, SHADOWMAP_RESOLUTION);
        _shadowMap = LoadShadowmapRenderTexture(shadowMapResolution, shadowMapResolution);
        // For the shadow mapping algorithm, we will be rendering everything from the light's point of view
        _lightCam = new Camera3D
        {
            Position = Raymath.Vector3Scale(_lightDir, -15.0f), //Vector3Scale(lightDir, -15.0f);
            Target = Vector3.Zero,
            // Use an orthographic projection for directional lights
            Projection = CameraProjection.Orthographic,
            Up = new Vector3(0.0f, 1.0f, 0.0f), //(Vector3){ 0.0f, 1.0f, 0.0f };
            FovY = 20.0f
        };
    }

    public static void Update(List<World3DObjects>? worldObjects)
    {
        if (Enabled)
        {
            Vector3 cameraPos = CharacterCamera3D.Camera.Position;
            SetShaderValue(ShadowShader, ShadowShader.Locs[(int)ShaderLocationIndex.VectorView], cameraPos, ShaderUniformDataType.Vec3);    
        
            _lightDir = Vector3.Normalize(_lightDir);
            _lightCam.Position = Raymath.Vector3Scale(_lightDir, -15.0f);
                
            SetShaderValue(ShadowShader, _lightDirLoc, _lightDir, ShaderUniformDataType.Vec3);
            
                BeginTextureMode(_shadowMap);
                ClearBackground(Color.White);
                BeginMode3D(_lightCam);

                    Matrix4x4 lightView = Rlgl.GetMatrixModelview();
                    Matrix4x4 lightProj = Rlgl.GetMatrixProjection();

                    //Draw 3D Models
                    Rlgl.EnableBackfaceCulling();
                    if (worldObjects != null)
                    {
                        foreach (var obj in worldObjects)
                        {
                            obj.Draw3DModels();
                        }   
                    }
                    
                EndMode3D();
                EndTextureMode();
                
            Matrix4x4 lightViewProj = Raymath.MatrixMultiply(lightView, lightProj); //MatrixMultiply(lightView, lightProj);
                
            SetShaderValueMatrix(ShadowShader, _lightVpLoc, lightViewProj);
                
            Rlgl.EnableShader(ShadowShader.Id);

            int slot = 10;
                
            Rlgl.ActiveTextureSlot(10);
            Rlgl.EnableTexture(_shadowMap.Depth.Id);
            Rlgl.SetUniform(_shadowMapLoc, &slot, (int)ShaderUniformDataType.Int, 1);
        }
        else
        {
            UnloadShadowmapRenderTexture();
        }
    }

    public static void BindShader(List<Model> models)
    {
        foreach (var model in models)
        {
            for (int i = 0; i < model.MaterialCount; i++)
            {
                model.Materials[i].Shader = ShadowShader;
            }
        }
    }

    private static RenderTexture2D LoadShadowmapRenderTexture(int width, int height)
    {
        RenderTexture2D target = new RenderTexture2D
        {
            Id = Rlgl.LoadFramebuffer() // Load an empty framebuffer
        };

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
    
    public static void UnloadShadowmapRenderTexture()
    {
        if (_shadowMap.Id > 0)
        {
            // NOTE: Depth texture/renderbuffer is automatically
            // queried and deleted before deleting framebuffer
            Rlgl.UnloadFramebuffer(_shadowMap.Id);
        }
    }

    public static Vector3 GetLightCamPosition()
    {
        return _lightCam.Position;
    }
}