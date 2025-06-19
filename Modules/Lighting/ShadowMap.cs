using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Lighting;

public static unsafe class ShadowMap
{
    private static Shader _shadowShader;
    private const int ShadowmapResolution = 1080; // Reduced resolution for better performance
    
    // Location cache variables
    private static int _lightDirLoc = -1;
    private static int _viewPosLoc = -1;
    private static int _lightVpLoc = -1;
    private static int _shadowMapLoc = -1;
    
    private static Vector3 _lightDir;
    public static Camera3D LightCam; 
    private static RenderTexture2D _shadowMap;

    public static bool Enabled = true;
    
    public static Vector3 GetLightDirection()
    {
        return _lightDir;
    }
    
    public static void Init(List<World3DObjects> worldObjects)
    {
        // Load shader with error checking
        _shadowShader = LoadShader("Assets/Shaders/shadowmap.vert", "Assets/Shaders/shadowmap.frag");
        
        if (!IsShaderValid(_shadowShader))
        {
            TraceLog(TraceLogLevel.Error, "SHADER: Failed to load shadow shader!");
            Enabled = false;
            return;
        }

        // Get shader locations safely
        _viewPosLoc = GetShaderLocation(_shadowShader, "viewPos");
        _lightDirLoc = GetShaderLocation(_shadowShader, "lightDir");
        int lightColLoc = GetShaderLocation(_shadowShader, "lightColor");
        int ambientLoc = GetShaderLocation(_shadowShader, "ambient");
        _lightVpLoc = GetShaderLocation(_shadowShader, "lightVP");
        _shadowMapLoc = GetShaderLocation(_shadowShader, "shadowMap");
        int shadowResLoc = GetShaderLocation(_shadowShader, "shadowMapResolution");

        if (_viewPosLoc == -1 || _lightDirLoc == -1)
        {
            TraceLog(TraceLogLevel.Warning, "SHADER: Some uniform locations not found!");
        }

        foreach (var obj in worldObjects)
        {
            BindShader(obj.Models);
        }
        
        // Light setup
        _lightDir = Raymath.Vector3Normalize(new Vector3(0.35f, -1.0f, -0.35f));
        Color lightColor = new Color(255, 197, 143, 255);
        Vector4 lightColorNormalized = ColorNormalize(lightColor);
        
        // Set shader values only if locations are valid
        if (_lightDirLoc != -1)
            SetShaderValue(_shadowShader, _lightDirLoc, _lightDir, ShaderUniformDataType.Vec3);
        if (lightColLoc != -1)
            SetShaderValue(_shadowShader, lightColLoc, lightColorNormalized, ShaderUniformDataType.Vec4);
        
        // Ambient lighting (brighter for toon look)
        float[] ambient = [0.7f, 0.7f, 0.7f, 1.0f];
        if (ambientLoc != -1)
            SetShaderValue(_shadowShader, ambientLoc, ambient, ShaderUniformDataType.Vec4);
        
        // Shadow map setup
        int shadowMapResolution = ShadowmapResolution;
        if (shadowResLoc != -1)
            SetShaderValue(_shadowShader, shadowResLoc, shadowMapResolution, ShaderUniformDataType.Int);
        
        _shadowMap = LoadShadowmapRenderTexture(shadowMapResolution, shadowMapResolution);
        
        LightCam = new Camera3D
        {
            Position = Raymath.Vector3Scale(_lightDir, -35.0f),
            Target = Vector3.Zero,//new Vector3(5.0f, 0.0f, 10.0f),
            Projection = CameraProjection.Orthographic,
            Up = new Vector3(0.0f, 1.0f, 0.0f),
            FovY = 40.0f
        };
    }

    public static void Update(List<World3DObjects>? worldObjects)
    {
        if (!Enabled || !IsShaderValid(_shadowShader))
        {
        
            UnloadShadowmapRenderTexture();
            return;
        }
        
        
        try
        {
            Vector3 cameraPos = CharacterCamera3D.Camera.Position;
            if (_viewPosLoc != -1)
                SetShaderValue(_shadowShader, _viewPosLoc, cameraPos, ShaderUniformDataType.Vec3);
            
            _lightDir = Vector3.Normalize(_lightDir);
            
            if (_lightDirLoc != -1)
                SetShaderValue(_shadowShader, _lightDirLoc, _lightDir, ShaderUniformDataType.Vec3);
            
            Rlgl.EnableBackfaceCulling();
                        
            BeginTextureMode(_shadowMap);
            //Rlgl.SetCullFace(0);
            ClearBackground(Color.White);
            BeginMode3D(LightCam);

                Matrix4x4 lightView = Rlgl.GetMatrixModelview();
                Matrix4x4 lightProj = Rlgl.GetMatrixProjection();

                if (worldObjects != null)
                {
                    worldObjects[0].Draw3DModels();
                    worldObjects[2].Draw3DModels();
                }
                
            EndMode3D();
            
            EndTextureMode();
            Matrix4x4 lightViewProj = Raymath.MatrixMultiply(lightView, lightProj);
            
            if (_lightVpLoc != -1)
                SetShaderValueMatrix(_shadowShader, _lightVpLoc, lightViewProj);
            
            Rlgl.EnableShader(_shadowShader.Id);

            if (_shadowMapLoc != -1)
            {
                int slot = 10;
                Rlgl.ActiveTextureSlot(10);
                Rlgl.EnableTexture(_shadowMap.Depth.Id);
                Rlgl.SetUniform(_shadowMapLoc, &slot, (int)ShaderUniformDataType.Int, 1);
            }

            //Rlgl.SetCullFace(1);
        }
        catch (Exception ex)
        {
            TraceLog(TraceLogLevel.Error, $"SHADOWMAP ERROR: {ex.Message}");
            Enabled = false;
        }
    }

    public static void BindShader(List<Model> models)
    {
        foreach (var model in models)
        {
            for (int i = 0; i < model.MaterialCount; i++)
            {
                model.Materials[i].Shader = _shadowShader;
                //SetMaterialTexture(ref model.Materials[i], MaterialMapIndex.Albedo, Texture);
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
            if (Rlgl.FramebufferComplete(target.Id)) TraceLog(TraceLogLevel.Info, $"FBO: {target.Id} Framebuffer object created successfully");

            Rlgl.DisableFramebuffer();
        }
        else TraceLog(TraceLogLevel.Warning, "FBO: Framebuffer object can not be created");

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
        return LightCam.Position;
    }
}