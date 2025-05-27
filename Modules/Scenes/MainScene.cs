using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Opengl_virtual_tour_with_Raylib.Modules.Audio;

using Raylib_cs;
using static Raylib_cs.Raylib;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class MainScene (byte id, string windowTitle): SceneObject(id, windowTitle)
{
    private Buildings? _buildings;
    private Roads? _roads;
    private Props? _props;
    //<Temporal>
    private HitboxLoader? _hitboxLoader;
    //</Temporal>
    
    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    public static List<World3DObjects>? WorldObjects;
    private bool _hitboxEnabled;
    private Shader _waterShader;
    /*private int    _uTimeLoc;
    private int    _viewPosLoc;
    private int    _lightDirLoc;*/
    //private float  _timeAccumulator;
    private Model _waterModel;
    private FootstepManager? _footstepManager;

    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
        _roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
        _props = new Props("ConfigurationFiles/DATA/PropsDATA.toml");
        
        //<Temporal>
        _hitboxLoader= new HitboxLoader("ConfigurationFiles/DATA/HitboxesDATA.toml");
        //</Temporal>
        
        //InitializeWorld();
        WorldObjects = new List<World3DObjects>();
        WorldObjects = new List<World3DObjects>();
        WorldObjects.AddRange(_buildings);
        WorldObjects.AddRange(_roads);
        WorldObjects.AddRange(_props);
            
        ShadowMap.Init(WorldObjects);
        
        _hitboxEnabled = true;
        
        // Inicializar FootstepManager con los modelos que tienen suelo tipo "tile"
        var modelos = _roads?.ModelDatas ?? new List<ModelData>();
        _footstepManager = new FootstepManager(modelos);
        
        // Load our water shader
        _waterShader   = LoadShader("Assets/Shaders/water.vert", "Assets/Shaders/water.frag");
        /*_uTimeLoc      = GetShaderLocation(_waterShader, "uTime");
        _viewPosLoc    = GetShaderLocation(_waterShader, "viewPos");
        _lightDirLoc   = GetShaderLocation(_waterShader, "lightDir");*/

        //_timeAccumulator = 0f;

        // … your existing init …

        // Create a plane mesh: width=2, length=2, 100×100 subdivisions
        Mesh waterMesh = GenMeshPlane(2f, 2f, 100, 100);
        _waterModel = LoadModelFromMesh(waterMesh);

        // Attach your water shader to the model's material
        unsafe
        {
            _waterModel.Materials[0].Shader = _waterShader;
        }
        
        Initialized = true;
    }

    public override void UpdateScene()
    {
            // Dibujar el hitbox de la cámara
            //DrawBoundingBox(CharacterCamera3D.HitBox, Color.Blue);
                
            // Change the camera Target when the middle mouse button and the F key is pressed
            if (IsMouseButtonDown(MouseButton.Middle)||IsKeyDown(KeyboardKey.F))
            {
                CharacterCamera3D.Camera.Target = new Vector3(0,0,0); // Make the camera look at the cube
            }
                
            // Start capturing the mouse
            if (IsMouseButtonDown(MouseButton.Left) && !Variables.IsSettingsMenuEnabled)
            {
                _camMode = CameraMode.Free;
                DisableCursor();
            }

            if (IsKeyPressed(KeyboardKey.Escape))
            {
                Variables.IsSettingsMenuEnabled = true;
                _cameraControlEnabled = false;
                EnableCursor();
            }
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left) && !Variables.IsSettingsMenuEnabled)
            {
                _cameraControlEnabled = true;
                DisableCursor(); // Captura del mouse
            }
            
            // Permitir cambio de modo solo si ya se activó la cámara
            if (_cameraControlEnabled)
            {
                
                //Choose a mode to use the camera 
                //By default is in Tourist mode
                if (IsKeyPressed(KeyboardKey.One)) 
                    CharacterCamera3D.Mode = CameraModeType.Tourist;// Movimiento del modo Turista

                if (IsKeyPressed(KeyboardKey.Two)) 
                    CharacterCamera3D.Mode = CameraModeType.Free;// Movimiento del modo Libre

                // Movimiento según el modo
                if (CharacterCamera3D.Mode == CameraModeType.Tourist)
                {
                    if (_buildings != null && _hitboxLoader?.Cajas !=null)
                        CharacterCamera3D.HandleTouristModeInput(_buildings.ModelDatas, _hitboxLoader.Cajas);
                }
                else
                {
                    // Update CharacterCamera3D position and hitbox
                    UpdateCamera(ref CharacterCamera3D.Camera, _camMode);
                }

                // Actualizar posición y restricciones
                CharacterCamera3D.UpdateHitBox();
                CharacterCamera3D.ApplyCameraConstraints();
            }

            // if key M is preced then stop updating the shadowmap and if is pressed again then enable the shadowmap update
            if (IsKeyPressed(KeyboardKey.M))
            {
                ShadowMap.Enabled = !ShadowMap.Enabled;

                if (ShadowMap.Enabled)
                {
                    if (WorldObjects != null) ShadowMap.Init(WorldObjects);
                }
            }   
            
            if (IsKeyPressed(KeyboardKey.B))
            {
                _hitboxEnabled = !_hitboxEnabled;
            }
            
            ShadowMap.Update();
            
            // Actualizar sonidos de pasos
            if (_footstepManager != null)
            {
                Vector3 playerPos = CharacterCamera3D.Camera.Position;
                bool isRunning = IsKeyDown(KeyboardKey.LeftShift); // correr con shift
                _footstepManager.Update(playerPos, isRunning);
            }
            
            // Advance time
            //_timeAccumulator += GetFrameTime();

            // Update shader uniforms
            /*SetShaderValue(_waterShader, _uTimeLoc,
                          new[] { _timeAccumulator }, ShaderUniformDataType.Float);
            // pass camera position
            Vector3 camPos = CharacterCamera3D.Camera.Position;
            SetShaderValue(_waterShader, _viewPosLoc,
                          new[] { camPos.X, camPos.Y, camPos.Z },
                          ShaderUniformDataType.Vec3);
            // simple directional light coming from above/front
            var light = Vector3.Normalize(new Vector3( 0.5f, -1.0f, 0.3f ));
            SetShaderValue(_waterShader, _lightDirLoc,
                          new[] { light.X, light.Y, light.Z },
                          ShaderUniformDataType.Vec3);*/
            //<Temporal>
            /*Vector3 center = new Vector3(4, 1, 3);
            Vector3 size = new Vector3(2, 2, 2);
            Vector3 halfExtents = size / 2f;

                // Rotación: 45 grados sobre el eje Y
            float angleY = 45.0f * MathF.PI / 180.0f;
            Quaternion rotY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angleY);
            
            float angleX = 20.0f * MathF.PI / 180.0f;
            Quaternion rotX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, angleX);

                // Combinación (primero X, luego Y)
            Quaternion rotation = Quaternion.Normalize(rotY * rotX);
            
            Obb prueba = new Obb(center, halfExtents, rotation);*/
            //</Temporal>
            
            BeginDrawing();
        
                ClearBackground(Color.SkyBlue);
                
                BeginMode3D(CharacterCamera3D.Camera);
            
                if (_hitboxEnabled)
                {
                    _buildings?.DrawHitBoxes();
                    //<Temporal>
                    _hitboxLoader?.DrawBoundingBoxes();
                    //<Temporal>
                }
                // 1) Draw your opaque world
                if (WorldObjects != null) Render3DModels(WorldObjects);

                // 2) Transparent water pass
                //BeginBlendMode(BlendMode.Alpha);
                //Rlgl.DisableDepthMask();  // don't write to depth

                // Update shader uniforms as before…
                /*_timeAccumulator += GetFrameTime();
                SetShaderValue(_waterShader, _uTimeLoc,
                    new[] { _timeAccumulator }, ShaderUniformDataType.Float);
                camPos = CharacterCamera3D.Camera.Position;
                SetShaderValue(_waterShader, _viewPosLoc,
                    new[]{camPos.X,camPos.Y,camPos.Z}, ShaderUniformDataType.Vec3);
                light = Vector3.Normalize(new Vector3(0.5f, -1f, 0.3f));
                SetShaderValue(_waterShader, _lightDirLoc,
                    new[]{light.X,light.Y,light.Z}, ShaderUniformDataType.Vec3);

                // Draw the subdivided plane at y=0, centered on your cube’s footprint
                // (rotate so it faces up)
                DrawModel(_waterModel,
                    new Vector3(0, 0, -3),    // position
                    1f,                       // uniform scale
                    Color.White               // color is ignored by shader
                );*/

                //Rlgl.EnableDepthMask();
                //EndBlendMode();

                EndMode3D();
                
                DrawText("Collision False", 28, 10, 20, Color.Black);
            
                DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps                
                Camera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.HitBox.Min} MAX-{CharacterCamera3D.HitBox.Max}
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
        _footstepManager?.Unload();

        Initialized = false;
    }
}