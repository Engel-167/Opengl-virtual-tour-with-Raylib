using System.Numerics;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.Hitboxes;
using Opengl_virtual_tour_with_Raylib.Modules._3D_World.SkyBox;
using Opengl_virtual_tour_with_Raylib.Modules.Camera;
using Opengl_virtual_tour_with_Raylib.Modules.Core.Globals;
using Opengl_virtual_tour_with_Raylib.Modules.Lighting;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace Opengl_virtual_tour_with_Raylib.Modules.Scenes;

public class MainScene (byte id, string windowTitle): SceneObject(id, windowTitle)
{
    private HitboxLoader? _hitboxLoader;
    private HitboxLoader? _groundLoader;
    
    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    public static List<World3DObjects>? WorldObjects;
    private bool _hitboxEnabled;
    
    private bool _playAnimation;

    private static readonly Vector3 HitboxSize = new(CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize);

    private SkyBox? _skyBox;

    private readonly Watter _watter = new(new Vector3(1, -0.5f, 3.5f),19f,13f);
    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _hitboxLoader= new HitboxLoader("ConfigurationFiles/DATA/HitboxesDATA.toml");
        _groundLoader = new HitboxLoader("ConfigurationFiles/DATA/GroundDATA.toml");
        
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
                Variables.IsSettingsMenuEnabled = !Variables.IsSettingsMenuEnabled;

                if (Variables.IsSettingsMenuEnabled)
                {
                    _cameraControlEnabled = false;
                    EnableCursor();    
                }
            }
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left) && !Variables.IsSettingsMenuEnabled)
            {
                _cameraControlEnabled = true;
                DisableCursor();
            }
            
            // Change the camera mode only if the camera is already activated
            if (_cameraControlEnabled)
            {
                if (_hitboxLoader != null)
                    CharacterCamera3D.UpdateMyCamera(_hitboxLoader, _groundLoader,_camMode);
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
            
            // Play animation when P is held down
            if (IsKeyPressed(KeyboardKey.P))
            {
                _playAnimation = true;
            }

            if (_playAnimation)
            {
                unsafe
                {
                    Animations.animFrameCounter++;

                    if (Animations.animFrameCounter >= Animations.anims[0].FrameCount - 1)
                    {
                        Animations.animFrameCounter = Animations.anims[0].FrameCount - 1;
                        _playAnimation = false;
                    }

                    if (Variables.Buildings != null)
                    {
                        UpdateModelAnimation(Variables.Buildings.GateModel, Animations.anims[0], Animations.animFrameCounter);
                        Console.WriteLine($"Frame count: {Animations.anims[0].FrameCount}");
                    }
                }
            }
            
            BeginDrawing();
        
                ClearBackground(Color.SkyBlue);
                
                BeginMode3D(CharacterCamera3D.Camera);
            
                _skyBox?.Draw();
                
                if (_hitboxEnabled)
                {
                    _hitboxLoader?.DrawBoundingBoxes(Color.Blue);
                    _groundLoader?.DrawBoundingBoxes(Color.Red);
                }
                
                if (WorldObjects != null) Render3DModels(WorldObjects);
                
                _watter.Update();

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
        _skyBox?.Destroy();
        _watter.Kill();
        ShadowMap.UnloadShadowmapRenderTexture();
        
        Initialized = false;
    }
}