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
    private HitboxLoader? _doorLoader;
    
    private CameraMode _camMode;
    private bool _cameraControlEnabled;

    public static List<World3DObjects>? WorldObjects;
    private bool _hitboxEnabled;
    
    private static readonly Vector3 HitboxSize = new(CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize, CharacterCamera3D.HitBoxSize);
    
    private SkyBox? _skyBox;

    private Water _water = null!;
    public override void InitScene()
    {
        _camMode = CameraMode.Custom;
        
        _hitboxLoader= new HitboxLoader("ConfigurationFiles/DATA/HitboxesDATA.toml");
        _groundLoader = new HitboxLoader("ConfigurationFiles/DATA/GroundDATA.toml");
        _doorLoader = new HitboxLoader("ConfigurationFiles/DATA/DoorsDATA.toml");
        
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
        
        _water = new Water(new Vector3(10.5f, -0.5f, 2.0f),38f,7f);
        
        Initialized = true;
    }
    
    private void UpdateLightFollow()
    {
        // Center the light camera on the player's XZ position to make shadows follow the player.
        Vector3 playerPos = CharacterCamera3D.Camera.Position;
        
        // The light camera will target the player's position on the ground plane.
        ShadowMap.LightCam.Target = playerPos with { Y = 0.0f };

        // The light camera is positioned away from the target, along the light's direction.
        // This ensures the shadow angle remains constant as the player moves.
        const float lightDistance = 35.0f; // Controls how far the light source is.
        ShadowMap.LightCam.Position = ShadowMap.LightCam.Target - (ShadowMap.GetLightDirection() * lightDistance);
    } 

    public override void UpdateScene()
    {
            // Start capturing the mouse
            if (IsMouseButtonDown(MouseButton.Left) && !Variables.IsSettingsMenuEnabled && !Variables.IsDialogBoxEnabled)
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
            
            if (IsKeyPressed(KeyboardKey.T) && !Variables.IsSettingsMenuEnabled)
            {
                Variables.IsDialogBoxEnabled = !Variables.IsDialogBoxEnabled;
                    
                if (Variables.IsDialogBoxEnabled)
                {
                    _cameraControlEnabled = false;
                    EnableCursor();    
                }
            }
                
            if (!_cameraControlEnabled && IsMouseButtonPressed(MouseButton.Left) && !Variables.IsSettingsMenuEnabled && !Variables.IsDialogBoxEnabled)
            {
                _cameraControlEnabled = true;
                DisableCursor();
            }
            
            // Change the camera mode only if the camera is already activated
            if (_cameraControlEnabled)
            {
                if (_hitboxLoader != null)
                    CharacterCamera3D.UpdateMyCamera(_hitboxLoader, _groundLoader,_doorLoader,_camMode);
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
            
            UpdateLightFollow();
            //Rlgl.DisableBackfaceCulling();
            //Rlgl.SetCullFace(0);
            ShadowMap.Update(WorldObjects);
            
            BeginDrawing();
        
                ClearBackground(Color.SkyBlue);
                
                BeginMode3D(CharacterCamera3D.Camera);
            
                _skyBox?.Draw();
                
                if (_hitboxEnabled)
                {
                    _hitboxLoader?.DrawBoundingBoxes(Color.Blue);
                    _groundLoader?.DrawBoundingBoxes(Color.Red);
                    _doorLoader?.DrawBoundingBoxes(Color.DarkGreen);
                }
                
                if (WorldObjects != null) Render3DModels(WorldObjects);
                
                _water.Update();

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
                
                
                
                if (Variables.IsSettingsMenuEnabled && !Variables.IsDialogBoxEnabled)
                {
                    Variables.SettingsMenu?.Draw();
                }
                else
                {
                    Variables.IsSettingsMenuEnabled = false;
                }

                if (Variables.IsDialogBoxEnabled && !Variables.IsSettingsMenuEnabled)
                {
                    Variables.DialogBox?.Draw();
                }
                else
                {
                    Variables.IsDialogBoxEnabled = false;
                }

                if (IsWindowResized())
                {
                    Variables.SettingsMenu?.UpdateLayout();
                    Variables.DialogBox?.UpdateLayout();
                }
                
            EndDrawing();
    }

    public override void KillScene()
    {
        _skyBox?.Destroy();
        _water.Kill();
        ShadowMap.UnloadShadowmapRenderTexture();
        
        Initialized = false;
    }
}