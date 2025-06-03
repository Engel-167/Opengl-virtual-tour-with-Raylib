using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Opengl_virtual_tour_with_Raylib.Modules.Audio;

using Raylib_cs;
using static Raylib_cs.Raylib;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.SkyBox;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class MainScene (byte id, string windowTitle): SceneObject(id, windowTitle)
{
    //<Temporal>
    private HitboxLoader? _hitboxLoader;
    //</Temporal>
    
    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    public static List<World3DObjects>? WorldObjects;
    private bool _hitboxEnabled;
    private Shader _waterShader;
    private int    _uTimeLoc;
    private int    _viewPosLoc;
    private int    _lightDirLoc;
    private float  _timeAccumulator;
    private Model _waterModel;
    private FootstepManager? _footstepManager;

    private static readonly Vector3 HitboxSize = new Vector3(CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize);

    private SkyBox? _skyBox;
    
    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _hitboxLoader= new HitboxLoader("ConfigurationFiles/DATA/HitboxesDATA.toml");
        
        //InitializeWorld();
        WorldObjects = new List<World3DObjects>();
        WorldObjects = new List<World3DObjects>();
        
        if (Variables.Buildings != null) WorldObjects.AddRange(Variables.Buildings);
        
        if (Variables.Roads != null) WorldObjects.AddRange(Variables.Roads);
        
        if (Variables.Props != null) WorldObjects.AddRange(Variables.Props);

        Render3DModels(WorldObjects);

        Console.WriteLine($"ShadowMap enabled 1 : {ShadowMap.Enabled}");
        ShadowMap.Init(WorldObjects);
        Console.WriteLine($"ShadowMap enabled 1 : {ShadowMap.Enabled}");
        
        _hitboxEnabled = true;
        
        // Inicializar FootstepManager con los modelos que tienen suelo tipo "tile"
        var modelos = Variables.Roads?.ModelDataList;
        if (modelos != null) _footstepManager = new FootstepManager(modelos);

        // Load our water shader
        _waterShader   = LoadShader("Assets/Shaders/water.vert", "Assets/Shaders/water.frag");
        _uTimeLoc      = GetShaderLocation(_waterShader, "uTime");
        _viewPosLoc    = GetShaderLocation(_waterShader, "viewPos");
        _lightDirLoc   = GetShaderLocation(_waterShader, "lightDir");

        _timeAccumulator = 0f;

        // … your existing init …

        // Create a plane mesh: width=2, length=2, 100×100 subdivisions
        Mesh waterMesh = GenMeshPlane(19f, 3f, 100, 100);
        _waterModel = LoadModelFromMesh(waterMesh);

        // Attach your water shader to the model's material
        unsafe
        {
            _waterModel.Materials[0].Shader = _waterShader;
        }
        
        _skyBox = new SkyBox();
        
        Initialized = true;
    }

    public override void UpdateScene()
    {
            // Start capturing the mouse
            if (IsMouseButtonDown(MouseButton.Left) && !Variables.IsSettingsMenuEnabled)
            {
                _camMode = CameraMode.Free;
                DisableCursor();
            }

            //Show Settings Ui
            if (IsKeyPressed(KeyboardKey.Escape))
            {
                Variables.IsSettingsMenuEnabled = true;
                _cameraControlEnabled = false;
                EnableCursor();
            }
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left) && !Variables.IsSettingsMenuEnabled)
            {
                _cameraControlEnabled = true;
                DisableCursor();
            }
            
            // Change the camera mode only if the camera is already activated
            if (_cameraControlEnabled)
            {
                
                //Choose a mode to use the camera 
                //By default is in Tourist mode
                if (IsKeyPressed(KeyboardKey.One)) 
                    CharacterCamera3D.Mode = CameraModeType.Tourist;// Movimiento del modo Turista

                if (IsKeyPressed(KeyboardKey.Two)) 
                    CharacterCamera3D.Mode = CameraModeType.Free;// Movimiento del modo Libre

                // Movement modes
                if (CharacterCamera3D.Mode == CameraModeType.Tourist)
                {
                    if (_hitboxLoader?.Cajas !=null)
                        if (Variables.Buildings != null)
                            CharacterCamera3D.HandleTouristModeInput(Variables.Buildings.ModelDataList, _hitboxLoader.Cajas);
                }
                else
                {
                    // Update CharacterCamera3D position and hitbox
                    UpdateCamera(ref CharacterCamera3D.Camera, _camMode);
                }

                // Update Position and Restrictions
                CharacterCamera3D.ApplyCameraConstraints();
            }

            // if key M is pressed then stop updating the shadow map and if is pressed again then enable the shadow map update
            if (IsKeyPressed(KeyboardKey.M))
            {
                ShadowMap.Enabled = !ShadowMap.Enabled;

                if (ShadowMap.Enabled)
                {
                    if (WorldObjects != null) ShadowMap.Init(WorldObjects);
                }
            }   
            
            // Enable the hitboxes drawing
            if (IsKeyPressed(KeyboardKey.B))
            {
                _hitboxEnabled = !_hitboxEnabled;
            }
            
            //Rlgl.DisableBackfaceCulling();
            //Rlgl.SetCullFace(0);
            ShadowMap.Update(WorldObjects);
            
            //Rlgl.EnableBackfaceCulling();
            
            
            
            // Actualizar sonidos de pasos
            /*if (_footstepManager != null)
            {
                Vector3 playerPos = CharacterCamera3D.Camera.Position;
                bool isRunning = IsKeyDown(KeyboardKey.LeftShift); // correr con shift
                _footstepManager.Update(playerPos, isRunning);
            }*/
            
            // Advance time
            _timeAccumulator += GetFrameTime();

            // Update shader uniforms
            SetShaderValue(_waterShader, _uTimeLoc, new[] { _timeAccumulator }, ShaderUniformDataType.Float);
            // pass camera position
            Vector3 camPos = CharacterCamera3D.Camera.Position;
            SetShaderValue(_waterShader, _viewPosLoc, new[] { camPos.X, camPos.Y, camPos.Z }, ShaderUniformDataType.Vec3);
            // simple directional light coming from above/front
            var light = Vector3.Normalize(new Vector3( 0.5f, -1.0f, 0.3f ));
            SetShaderValue(_waterShader, _lightDirLoc, new[] { light.X, light.Y, light.Z }, ShaderUniformDataType.Vec3);
            
            BeginDrawing();
        
                ClearBackground(Color.SkyBlue);
                
                BeginMode3D(CharacterCamera3D.Camera);
            
                _skyBox?.Draw();
                
                if (_hitboxEnabled)
                {
                    _hitboxLoader?.DrawBoundingBoxes();
                }
                // 1) Draw your opaque world
                //Rlgl.SetCullFace(1);
                if (WorldObjects != null) Render3DModels(WorldObjects);

                // 2) Transparent water pass
                BeginBlendMode(BlendMode.Alpha);
                Rlgl.DisableDepthMask();  // don't write to depth

                    // Update shader uniforms as before…
                    _timeAccumulator += GetFrameTime();
                    SetShaderValue(_waterShader, _uTimeLoc, new[] { _timeAccumulator }, ShaderUniformDataType.Float);
                    camPos = CharacterCamera3D.Camera.Position;
                    SetShaderValue(_waterShader, _viewPosLoc, new[]{camPos.X,camPos.Y,camPos.Z}, ShaderUniformDataType.Vec3);
                    light = Vector3.Normalize(new Vector3(0.5f, -1f, 0.3f));
                    SetShaderValue(_waterShader, _lightDirLoc, new[]{light.X,light.Y,light.Z}, ShaderUniformDataType.Vec3);

                    // Draw the subdivided plane at y=0, centered on your cube’s footprint
                    // (rotate so it faces up)
                    DrawModel(_waterModel,
                        new Vector3(1, -0.5f, 3.5f),    // position
                        1f,                       // uniform scale
                        Color.White               // color is ignored by shader
                    );

                Rlgl.EnableDepthMask();
                EndBlendMode();

                EndMode3D();
                
                DrawText("Collision False", 28, 10, 20, Color.Black);
            
                DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps                
                Camera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.Camera.Position-HitboxSize}
                 MAX-{CharacterCamera3D.Camera.Position+HitboxSize}
                Hitbox Enabled = {((_hitboxEnabled)?"Yes":"No")} (Press B to toggle)",-100,10,20,Color.Black);
                
                DrawText($@"Current Mode < {CharacterCamera3D.Mode} >", 200, 10, 20, Color.Black);
                DrawText($"Enable shadows: {ShadowMap.Enabled} (Press M to toggle)", 200, 50, 20, Color.Red);
                
                if (Variables.IsSettingsMenuEnabled)
                {
                    Variables.SettingsMenu?.Draw();
                }

                if (IsWindowResized())
                {
                    Variables.SettingsMenu?.UpdateLayout();
                }
                
            EndDrawing();
    }

    public override void KillScene()
    {
        UnloadShader(_waterShader);

        _skyBox?.Destroy();
        
        ShadowMap.UnloadShadowmapRenderTexture();
        
        Initialized = false;
    }
}