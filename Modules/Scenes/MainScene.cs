using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Buildings;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Props;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Roads;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class MainScene (byte id, string windowTitle): SceneObject(id, windowTitle)
{
    private Buildings? _buildings;
    private Roads? _roads;
    private Props? _props;

    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    private List<World3DObjects>? _worldObjects;
    private bool _hitboxEnabled;
    private Shader _waterShader;
    private int    _uTimeLoc;
    private int    _viewPosLoc;
    private int    _lightDirLoc;
    private float  _timeAccumulator;
    private Model _waterModel;

    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _buildings = new Buildings("ConfigurationFiles/DATA/BuildingsDATA.toml");
        _roads = new Roads("ConfigurationFiles/DATA/RoadsDATA.toml");
        _props = new Props("ConfigurationFiles/DATA/PropsDATA.toml");
        
        //InitializeWorld();
        _worldObjects = new List<World3DObjects>();
        _worldObjects.AddRange(_buildings);
        _worldObjects.AddRange(_roads);
        _worldObjects.AddRange(_props);
            
        ShadowMap.Init(_worldObjects);
        
        _hitboxEnabled = true;

        // Load our water shader
        _waterShader   = LoadShader("Assets/Shaders/water.vert", "Assets/Shaders/water.frag");
        _uTimeLoc      = GetShaderLocation(_waterShader, "uTime");
        _viewPosLoc    = GetShaderLocation(_waterShader, "viewPos");
        _lightDirLoc   = GetShaderLocation(_waterShader, "lightDir");

        _timeAccumulator = 0f;

        // … your existing init …

        // Create a plane mesh: width=2, length=2, 100×100 subdivisions
        Mesh waterMesh = GenMeshPlane(2f, 2f, 100, 100);
        _waterModel = LoadModelFromMesh(waterMesh);

        // Attach your water shader to the model's material
        unsafe
        {
            _waterModel.Materials[0].Shader = _waterShader;
        }
    }

    public override int UpdateScene()
    {
            //BeginDrawing();
            ClearBackground(Color.SkyBlue);
                
            // Dibujar el hitbox de la cámara
            //DrawBoundingBox(CharacterCamera3D.HitBox, Color.Blue);
                
            // Change the camera Target when the middle mouse button and the F key is pressed
            if (IsMouseButtonDown(MouseButton.Middle)||IsKeyDown(KeyboardKey.F))
            {
                CharacterCamera3D.Camera.Target = new Vector3(0,0,0); // Make the camera look at the cube
            }
                
            // Start capturing the mouse
            if (IsMouseButtonDown(MouseButton.Left))
            {
                _camMode = CameraMode.Free;
                DisableCursor();
            }

            if (IsKeyPressed(KeyboardKey.Y))
            {
                    //ClearConfigFlags(ConfigFlags.Msaa4xHint);
                    ClearWindowState(ConfigFlags.Msaa4xHint);
                    CloseWindow();
            }
                
            if (IsKeyPressed(KeyboardKey.X))
                SetConfigFlags(ConfigFlags.Msaa4xHint);
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left))
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
                    if (_buildings != null)
                        CharacterCamera3D.HandleTouristModeInput(_buildings.ModelDatas);
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
                    if (_worldObjects != null) ShadowMap.Init(_worldObjects);
                }
            }   
            
            ShadowMap.Update();
            
            // Advance time
            _timeAccumulator += GetFrameTime();

            // Update shader uniforms
            SetShaderValue(_waterShader, _uTimeLoc,
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
                          ShaderUniformDataType.Vec3);

            // Begin 3D once...
            BeginMode3D(CharacterCamera3D.Camera);

        // 1) Draw your opaque world
        if (_worldObjects != null) Render3DModels(_worldObjects);

        // 2) Transparent water pass
        Raylib.BeginBlendMode(BlendMode.Alpha);
        Rlgl.DisableDepthMask();  // don't write to depth

        // Update shader uniforms as before…
        _timeAccumulator += Raylib.GetFrameTime();
        Raylib.SetShaderValue(_waterShader, _uTimeLoc,
            new[] { _timeAccumulator }, ShaderUniformDataType.Float);
        camPos = CharacterCamera3D.Camera.Position;
        Raylib.SetShaderValue(_waterShader, _viewPosLoc,
            new[]{camPos.X,camPos.Y,camPos.Z}, ShaderUniformDataType.Vec3);
        light = Vector3.Normalize(new Vector3(0.5f, -1f, 0.3f));
        Raylib.SetShaderValue(_waterShader, _lightDirLoc,
            new[]{light.X,light.Y,light.Z}, ShaderUniformDataType.Vec3);

        // Draw the subdivided plane at y=0, centered on your cube’s footprint
        // (rotate so it faces up)
        Raylib.DrawModel(_waterModel,
            new Vector3(0, 0, -3),    // position
            1f,                       // uniform scale
            Color.White               // color is ignored by shader
        );

        Rlgl.EnableDepthMask();
        Raylib.EndBlendMode();

        EndMode3D();

            // Draw UI
            DrawText("Collision False", 28, 10, 20, Color.Black);
            
            DrawText($@"
                Raylib GLTF 3D model Loading
                {GetFPS()} fps                
                Camera Pos: {CharacterCamera3D.Camera.Position}
                CameraBox: MIN-{CharacterCamera3D.HitBox.Min} MAX-{CharacterCamera3D.HitBox.Max}
                Hitbox Enabled = {((_hitboxEnabled)?"Yes":"No")} (Press B to toggle)",-100,10,20,Color.Black);
                
            DrawText($@"Current Mode < {CharacterCamera3D.Mode} >", 200, 10, 20, Color.Black);
            DrawText($"Enable shadows: {ShadowMap.Enabled} (Press M to toggle)", 200, 50, 20, Color.Red);
            // End drawing
            //EndDrawing();
        return 0;   
    }

    public override void KillScene()
    {
        Raylib.UnloadShader(_waterShader);
        // … your other cleanup …
    }
}